using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Error catalog owned by the articles module.
/// </summary>
public static class ArticlesErrors
{
    /// <summary>
    /// Stable duplicate slug error code.
    /// </summary>
    public const string DuplicateSlugCode = "articles.slug.duplicate";

    /// <summary>
    /// Stable draft version conflict error code.
    /// </summary>
    public const string DraftVersionConflictCode = "articles.draft.versionConflict";

    /// <summary>
    /// Stable missing draft error code.
    /// </summary>
    public const string DraftNotFoundCode = "articles.draft.notFound";

    private const string InvalidSlugCode = "articles.slug.invalid";
    private const string TitleRequiredCode = "articles.title.required";
    private const string TitleTooLongCode = "articles.title.tooLong";
    private const string SummaryRequiredCode = "articles.summary.required";
    private const string SummaryTooLongCode = "articles.summary.tooLong";
    private const string MdxSourceRequiredCode = "articles.mdxSource.required";
    private const string MdxSourceTooLongCode = "articles.mdxSource.tooLong";
    private const string InvalidMdxSourceCode = "articles.mdxSource.invalid";
    private const string SlugLockedCode = "articles.slug.locked";
    private const string PersistenceFailureCode = "articles.persistence.failure";
    private const string ReadFailureCode = "articles.read.failure";

    /// <summary>
    /// Creates an invalid slug validation error.
    /// </summary>
    public static Error InvalidSlug(string message)
    {
        return new Error(InvalidSlugCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Creates a duplicate slug business error.
    /// </summary>
    public static Error DuplicateSlug(ArticleSlug slug)
    {
        string message = $"Article slug '{slug.Value}' already exists.";
        return new Error(DuplicateSlugCode, message, ErrorCategory.Business);
    }

    /// <summary>
    /// Gets the title-required validation error.
    /// </summary>
    public static Error TitleRequired()
    {
        return new Error(TitleRequiredCode, "Article title is required.", ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the title-too-long validation error.
    /// </summary>
    public static Error TitleTooLong()
    {
        string message = $"Article title cannot exceed {ArticleDraft.TitleMaximumLength} characters.";
        return new Error(TitleTooLongCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the summary-required validation error.
    /// </summary>
    public static Error SummaryRequired()
    {
        return new Error(SummaryRequiredCode, "Article summary is required.", ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the summary-too-long validation error.
    /// </summary>
    public static Error SummaryTooLong()
    {
        string message = $"Article summary cannot exceed {ArticleDraft.SummaryMaximumLength} characters.";
        return new Error(SummaryTooLongCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the MDX source-required validation error.
    /// </summary>
    public static Error MdxSourceRequired()
    {
        return new Error(MdxSourceRequiredCode, "Article MDX source is required.", ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the MDX source-too-long validation error.
    /// </summary>
    public static Error MdxSourceTooLong()
    {
        string message = $"Article MDX source cannot exceed {ArticleDraft.MdxSourceMaximumLength} characters.";
        return new Error(MdxSourceTooLongCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Creates an invalid MDX source validation error.
    /// </summary>
    public static Error InvalidMdxSource(string message)
    {
        return new Error(InvalidMdxSourceCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Creates a missing draft business error.
    /// </summary>
    public static Error DraftNotFound(ArticleDraftId id)
    {
        string message = $"Article draft '{id.Value}' was not found. Refresh the draft list and retry.";
        return new Error(DraftNotFoundCode, message, ErrorCategory.Business);
    }

    /// <summary>
    /// Creates a stale draft version business error.
    /// </summary>
    public static Error DraftVersionConflict()
    {
        const string Message = "Article draft was changed by another request. Reload it before saving.";
        return new Error(DraftVersionConflictCode, Message, ErrorCategory.Business);
    }

    /// <summary>
    /// Creates a locked slug business error.
    /// </summary>
    public static Error SlugLockedAfterPublish(ArticleSlug slug)
    {
        string message = $"Article slug '{slug.Value}' is locked after first publish.";
        return new Error(SlugLockedCode, message, ErrorCategory.Business);
    }

    /// <summary>
    /// Creates a persistence failure error.
    /// </summary>
    public static Error PersistenceFailure(string message)
    {
        return new Error(PersistenceFailureCode, message, ErrorCategory.Infrastructure);
    }

    /// <summary>
    /// Creates a public article read failure error.
    /// </summary>
    public static Error ReadFailure()
    {
        return new Error(
            ReadFailureCode,
            "Published articles could not be loaded. Verify the PostgreSQL connection and retry.",
            ErrorCategory.Infrastructure);
    }
}
