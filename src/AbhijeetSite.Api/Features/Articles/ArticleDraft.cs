using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Durable owner-authored article draft.
/// </summary>
public sealed class ArticleDraft
{
    /// <summary>
    /// Maximum title length.
    /// </summary>
    public const int TitleMaximumLength = 200;

    /// <summary>
    /// Maximum summary length.
    /// </summary>
    public const int SummaryMaximumLength = 500;

    private ArticleDraft()
    {
        Title = string.Empty;
        Summary = string.Empty;
        MdxSource = string.Empty;
    }

    private ArticleDraft(
        ArticleDraftId id,
        ArticleSlug slug,
        string title,
        string summary,
        string mdxSource,
        DateTimeOffset createdAt)
    {
        Id = id;
        Slug = slug;
        Title = title.Trim();
        Summary = summary.Trim();
        MdxSource = mdxSource;
        Status = ArticleDraftStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Gets the article draft identifier.
    /// </summary>
    public ArticleDraftId Id { get; private set; }

    /// <summary>
    /// Gets the URL-safe draft slug.
    /// </summary>
    public ArticleSlug Slug { get; private set; }

    /// <summary>
    /// Gets the draft title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the public draft summary.
    /// </summary>
    public string Summary { get; private set; }

    /// <summary>
    /// Gets the owner-authored MDX source.
    /// </summary>
    public string MdxSource { get; private set; }

    /// <summary>
    /// Gets the draft lifecycle status.
    /// </summary>
    public ArticleDraftStatus Status { get; private set; }

    /// <summary>
    /// Gets when the draft was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets when the draft was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new draft in the initial draft state.
    /// </summary>
    public static Result<ArticleDraft> Create(
        ArticleDraftId id,
        ArticleSlug slug,
        string title,
        string summary,
        string mdxSource,
        DateTimeOffset createdAt)
    {
        Error? validationError = Validate(title, summary, mdxSource);
        return validationError is null
            ? Result<ArticleDraft>.Success(new ArticleDraft(id, slug, title, summary, mdxSource, createdAt))
            : Result<ArticleDraft>.Failure(validationError);
    }

    private static Error? Validate(string title, string summary, string mdxSource)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return ArticlesErrors.TitleRequired();
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            return ArticlesErrors.SummaryRequired();
        }

        return string.IsNullOrWhiteSpace(mdxSource) ? ArticlesErrors.MdxSourceRequired() : null;
    }
}
