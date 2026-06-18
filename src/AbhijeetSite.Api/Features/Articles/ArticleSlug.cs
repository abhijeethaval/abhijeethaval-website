using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// URL-safe article slug.
/// </summary>
/// <param name="Value">Normalized slug value.</param>
public readonly record struct ArticleSlug(string Value)
{
    /// <summary>
    /// Maximum stored slug length.
    /// </summary>
    public const int MaximumLength = 160;

    private const char Separator = '-';

    /// <summary>
    /// Creates a validated article slug.
    /// </summary>
    public static Result<ArticleSlug> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ArticleSlug>.Failure(ArticlesErrors.InvalidSlug("Article slug is required."));
        }

        string normalizedValue = value.Trim().ToLowerInvariant();
        Error? validationError = Validate(normalizedValue);
        return validationError is null
            ? Result<ArticleSlug>.Success(new ArticleSlug(normalizedValue))
            : Result<ArticleSlug>.Failure(validationError);
    }

    private static Error? Validate(string value)
    {
        if (value.Length > MaximumLength)
        {
            return ArticlesErrors.InvalidSlug($"Article slug cannot exceed {MaximumLength} characters.");
        }

        if (value[0] == Separator || value[^1] == Separator)
        {
            return ArticlesErrors.InvalidSlug("Article slug cannot start or end with a hyphen.");
        }

        return IsUrlSafe(value) ? null : ArticlesErrors.InvalidSlug("Article slug must be URL safe.");
    }

    private static bool IsUrlSafe(string value)
    {
        for (int index = 0; index < value.Length; index++)
        {
            if (!IsAllowed(value[index], index, value))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAllowed(char character, int index, string value)
    {
        if (char.IsAsciiLetterLower(character) || char.IsAsciiDigit(character))
        {
            return true;
        }

        return character == Separator && value[index - 1] != Separator;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
}
