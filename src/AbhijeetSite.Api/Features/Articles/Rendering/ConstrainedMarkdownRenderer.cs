using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles.Rendering;

/// <summary>
/// Renders the allowed first-slice Markdown subset into escaped HTML.
/// </summary>
public sealed class ConstrainedMarkdownRenderer
{
    private const int MaximumLineLength = 4_000;
    private const int RegexTimeoutSeconds = 1;
    private const string CodeFence = "```";

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(RegexTimeoutSeconds);
    private static readonly Regex LinkPattern = new(
        @"\[([^\]]+)\]\((https?://[^)\s]+)\)",
        RegexOptions.Compiled,
        RegexTimeout);
    private static readonly Regex InlineCodePattern = new(
        "`([^`]+)`",
        RegexOptions.Compiled,
        RegexTimeout);
    private static readonly Regex StrongPattern = new(
        @"\*\*([^*]+)\*\*",
        RegexOptions.Compiled,
        RegexTimeout);
    private static readonly Regex EmphasisPattern = new(
        @"\*([^*]+)\*",
        RegexOptions.Compiled,
        RegexTimeout);

    /// <summary>
    /// Renders source Markdown into safe public HTML.
    /// </summary>
    public Result<string> Render(string source)
    {
        Result validation = ValidateSource(source);
        if (validation.IsFailure)
        {
            return ToRenderFailure(validation);
        }

        MarkdownBlockWriter writer = new();
        foreach (string line in GetLines(source))
        {
            writer.WriteLine(line);
        }

        writer.Finish();
        return Result<string>.Success(writer.Html);
    }

    private static Result<string> ToRenderFailure(Result validation)
    {
        if (validation.Error is Error error)
        {
            return Result<string>.Failure(error);
        }

        throw new InvalidOperationException("A failed validation result must include an error.");
    }

    private static Result ValidateSource(string source)
    {
        bool isInCodeBlock = false;
        int lineNumber = 0;
        foreach (string line in GetLines(source))
        {
            lineNumber++;
            Result lineResult = ValidateLine(line, lineNumber, ref isInCodeBlock);
            if (lineResult.IsFailure)
            {
                return lineResult;
            }
        }

        return isInCodeBlock ? Invalid("Code fence is not closed.") : Result.Success();
    }

    private static Result ValidateLine(string line, int lineNumber, ref bool isInCodeBlock)
    {
        if (line.Length > MaximumLineLength)
        {
            return Invalid($"Line {lineNumber} exceeds {MaximumLineLength} characters.");
        }

        string trimmed = line.TrimStart();
        if (trimmed.StartsWith(CodeFence, StringComparison.Ordinal))
        {
            isInCodeBlock = !isInCodeBlock;
            return Result.Success();
        }

        return isInCodeBlock ? Result.Success() : ValidateNonCodeLine(trimmed, lineNumber);
    }

    private static Result ValidateNonCodeLine(string trimmed, int lineNumber)
    {
        if (trimmed.StartsWith("import ", StringComparison.Ordinal)
            || trimmed.StartsWith("export ", StringComparison.Ordinal))
        {
            return Invalid($"Line {lineNumber} uses imports or exports, which are not allowed.");
        }

        if (trimmed.Contains('<', StringComparison.Ordinal)
            || trimmed.Contains('>', StringComparison.Ordinal))
        {
            return Invalid($"Line {lineNumber} uses raw HTML or JSX, which is not allowed.");
        }

        return HasExpressionSyntax(trimmed)
            ? Invalid($"Line {lineNumber} uses MDX expression syntax, which is not allowed.")
            : Result.Success();
    }

    private static bool HasExpressionSyntax(string line)
    {
        return line.Contains('{', StringComparison.Ordinal) || line.Contains('}', StringComparison.Ordinal);
    }

    private static Result Invalid(string message)
    {
        return Result.Failure(ArticlesErrors.InvalidMdxSource(message));
    }

    private static string[] GetLines(string source)
    {
        return source.ReplaceLineEndings("\n").Split('\n');
    }

    private static string RenderInline(string text)
    {
        string escaped = WebUtility.HtmlEncode(text);
        string withLinks = LinkPattern.Replace(escaped, RenderLink);
        string withCode = InlineCodePattern.Replace(withLinks, "<code>$1</code>");
        string withStrong = StrongPattern.Replace(withCode, "<strong>$1</strong>");
        return EmphasisPattern.Replace(withStrong, "<em>$1</em>");
    }

    private static string RenderLink(Match match)
    {
        string label = match.Groups[1].Value;
        string url = match.Groups[2].Value;
        return $"<a href=\"{url}\" rel=\"noopener noreferrer\">{label}</a>";
    }

    private enum ListKind
    {
        None,
        Ordered,
        Unordered
    }

    private sealed class MarkdownBlockWriter
    {
        private readonly StringBuilder _html = new();
        private readonly StringBuilder _paragraph = new();
        private ListKind _listKind;
        private bool _isInCodeBlock;

        public string Html => _html.ToString();

        public void WriteLine(string line)
        {
            string trimmed = line.TrimStart();
            if (trimmed.StartsWith(CodeFence, StringComparison.Ordinal))
            {
                ToggleCodeBlock();
                return;
            }

            if (_isInCodeBlock)
            {
                _html.Append(WebUtility.HtmlEncode(line)).Append('\n');
                return;
            }

            WriteContentLine(line, trimmed);
        }

        public void Finish()
        {
            CloseParagraph();
            CloseList();
        }

        private void WriteContentLine(string line, string trimmed)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                CloseParagraph();
                CloseList();
                return;
            }

            if (TryWriteHeading(trimmed) || TryWriteListItem(trimmed))
            {
                return;
            }

            AppendParagraph(trimmed);
        }

        private void ToggleCodeBlock()
        {
            CloseParagraph();
            CloseList();
            if (_isInCodeBlock)
            {
                _html.Append("</code></pre>");
                _isInCodeBlock = false;
                return;
            }

            _html.Append("<pre><code>");
            _isInCodeBlock = true;
        }

        private bool TryWriteHeading(string trimmed)
        {
            int level = GetHeadingLevel(trimmed);
            if (level == 0)
            {
                return false;
            }

            CloseParagraph();
            CloseList();
            string text = trimmed[level..].Trim();
            int htmlLevel = Math.Min(level + 1, 6);
            _html.Append("<h").Append(htmlLevel).Append('>')
                .Append(RenderInline(text))
                .Append("</h").Append(htmlLevel).Append('>');
            return true;
        }

        private static int GetHeadingLevel(string trimmed)
        {
            int level = 0;
            while (level < trimmed.Length && trimmed[level] == '#')
            {
                level++;
            }

            return HasHeadingSeparator(trimmed, level) ? level : 0;
        }

        private static bool HasHeadingSeparator(string trimmed, int level)
        {
            return level > 0 && level <= 6 && level < trimmed.Length && trimmed[level] == ' ';
        }

        private bool TryWriteListItem(string trimmed)
        {
            if (trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                WriteListItem(ListKind.Unordered, trimmed[2..]);
                return true;
            }

            int orderedPrefixLength = GetOrderedPrefixLength(trimmed);
            if (orderedPrefixLength == 0)
            {
                return false;
            }

            WriteListItem(ListKind.Ordered, trimmed[orderedPrefixLength..]);
            return true;
        }

        private static int GetOrderedPrefixLength(string trimmed)
        {
            int index = 0;
            while (index < trimmed.Length && char.IsAsciiDigit(trimmed[index]))
            {
                index++;
            }

            return HasOrderedMarker(trimmed, index) ? index + 2 : 0;
        }

        private static bool HasOrderedMarker(string trimmed, int index)
        {
            return index > 0 && index + 1 < trimmed.Length && trimmed[index] == '.'
                && trimmed[index + 1] == ' ';
        }

        private void WriteListItem(ListKind kind, string text)
        {
            CloseParagraph();
            OpenList(kind);
            _html.Append("<li>").Append(RenderInline(text.Trim())).Append("</li>");
        }

        private void OpenList(ListKind kind)
        {
            if (_listKind == kind)
            {
                return;
            }

            CloseList();
            _html.Append(kind == ListKind.Ordered ? "<ol>" : "<ul>");
            _listKind = kind;
        }

        private void AppendParagraph(string text)
        {
            CloseList();
            if (_paragraph.Length > 0)
            {
                _paragraph.Append(' ');
            }

            _paragraph.Append(text);
        }

        private void CloseParagraph()
        {
            if (_paragraph.Length == 0)
            {
                return;
            }

            _html.Append("<p>").Append(RenderInline(_paragraph.ToString())).Append("</p>");
            _paragraph.Clear();
        }

        private void CloseList()
        {
            if (_listKind == ListKind.None)
            {
                return;
            }

            _html.Append(_listKind == ListKind.Ordered ? "</ol>" : "</ul>");
            _listKind = ListKind.None;
        }
    }
}
