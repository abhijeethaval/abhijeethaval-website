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

    private const string InvalidSlugCode = "articles.slug.invalid";
    private const string TitleRequiredCode = "articles.title.required";
    private const string SummaryRequiredCode = "articles.summary.required";
    private const string MdxSourceRequiredCode = "articles.mdxSource.required";
    private const string PersistenceFailureCode = "articles.persistence.failure";

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
    /// Gets the summary-required validation error.
    /// </summary>
    public static Error SummaryRequired()
    {
        return new Error(SummaryRequiredCode, "Article summary is required.", ErrorCategory.Validation);
    }

    /// <summary>
    /// Gets the MDX source-required validation error.
    /// </summary>
    public static Error MdxSourceRequired()
    {
        return new Error(MdxSourceRequiredCode, "Article MDX source is required.", ErrorCategory.Validation);
    }

    /// <summary>
    /// Creates a persistence failure error.
    /// </summary>
    public static Error PersistenceFailure(string message)
    {
        return new Error(PersistenceFailureCode, message, ErrorCategory.Infrastructure);
    }
}
