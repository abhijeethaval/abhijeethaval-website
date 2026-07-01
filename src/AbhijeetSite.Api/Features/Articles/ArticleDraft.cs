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

    /// <summary>
    /// Maximum MDX source length.
    /// </summary>
    public const int MdxSourceMaximumLength = 100_000;

    private const int InitialVersion = 1;

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
        Version = InitialVersion;
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
    /// Gets the optimistic concurrency version.
    /// </summary>
    public int Version { get; private set; }

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

    /// <summary>
    /// Updates editable fields before first publish.
    /// </summary>
    public Result UpdateBeforeFirstPublish(
        ArticleSlug slug,
        string title,
        string summary,
        string mdxSource,
        int expectedVersion,
        DateTimeOffset updatedAt)
    {
        Error? updateError = ValidateUpdate(title, summary, mdxSource, expectedVersion);
        if (updateError is not null)
        {
            return Result.Failure(updateError);
        }

        Slug = slug;
        ApplyContentUpdate(title, summary, mdxSource, updatedAt);
        return Result.Success();
    }

    /// <summary>
    /// Updates editable fields after first publish while preserving the public slug.
    /// </summary>
    public Result UpdateAfterFirstPublish(
        string title,
        string summary,
        string mdxSource,
        int expectedVersion,
        DateTimeOffset updatedAt)
    {
        Error? updateError = ValidateUpdate(title, summary, mdxSource, expectedVersion);
        if (updateError is not null)
        {
            return Result.Failure(updateError);
        }

        ApplyContentUpdate(title, summary, mdxSource, updatedAt);
        return Result.Success();
    }

    /// <summary>
    /// Marks the draft as ready after a successful publish transition.
    /// </summary>
    public Result MarkReadyToPublish(int expectedVersion, DateTimeOffset updatedAt)
    {
        Error? versionError = ValidateVersion(expectedVersion);
        if (versionError is not null)
        {
            return Result.Failure(versionError);
        }

        Status = ArticleDraftStatus.ReadyToPublish;
        UpdatedAt = updatedAt;
        Version++;
        return Result.Success();
    }

    private void ApplyContentUpdate(
        string title,
        string summary,
        string mdxSource,
        DateTimeOffset updatedAt)
    {
        Title = title.Trim();
        Summary = summary.Trim();
        MdxSource = mdxSource;
        Status = ArticleDraftStatus.Draft;
        UpdatedAt = updatedAt;
        Version++;
    }

    private Error? ValidateUpdate(
        string title,
        string summary,
        string mdxSource,
        int expectedVersion)
    {
        Error? versionError = ValidateVersion(expectedVersion);
        return versionError ?? Validate(title, summary, mdxSource);
    }

    private Error? ValidateVersion(int expectedVersion)
    {
        return expectedVersion == Version ? null : ArticlesErrors.DraftVersionConflict();
    }

    private static Error? Validate(string title, string summary, string mdxSource)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return ArticlesErrors.TitleRequired();
        }

        if (title.Length > TitleMaximumLength)
        {
            return ArticlesErrors.TitleTooLong();
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            return ArticlesErrors.SummaryRequired();
        }

        if (summary.Length > SummaryMaximumLength)
        {
            return ArticlesErrors.SummaryTooLong();
        }

        if (string.IsNullOrWhiteSpace(mdxSource))
        {
            return ArticlesErrors.MdxSourceRequired();
        }

        return mdxSource.Length > MdxSourceMaximumLength ? ArticlesErrors.MdxSourceTooLong() : null;
    }
}
