namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Render-ready public article read model.
/// </summary>
public sealed class PublishedArticle
{
    private const int InitialRevision = 1;

    private PublishedArticle()
    {
        Title = string.Empty;
        Summary = string.Empty;
        RenderedHtml = string.Empty;
    }

    private PublishedArticle(
        PublishedArticleId id,
        ArticleDraft draft,
        string renderedHtml,
        DateTimeOffset publishedAt)
    {
        Id = id;
        DraftId = draft.Id;
        Slug = draft.Slug;
        Title = draft.Title;
        Summary = draft.Summary;
        RenderedHtml = renderedHtml;
        Status = PublishedArticleStatus.Published;
        PublishedAt = publishedAt;
        UpdatedAt = publishedAt;
        Revision = InitialRevision;
    }

    /// <summary>
    /// Gets the published article identifier.
    /// </summary>
    public PublishedArticleId Id { get; private set; }

    /// <summary>
    /// Gets the source draft identifier.
    /// </summary>
    public ArticleDraftId DraftId { get; private set; }

    /// <summary>
    /// Gets the stable public slug.
    /// </summary>
    public ArticleSlug Slug { get; private set; }

    /// <summary>
    /// Gets the public article title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the public article summary.
    /// </summary>
    public string Summary { get; private set; }

    /// <summary>
    /// Gets the render-ready HTML content.
    /// </summary>
    public string RenderedHtml { get; private set; }

    /// <summary>
    /// Gets the article visibility status.
    /// </summary>
    public PublishedArticleStatus Status { get; private set; }

    /// <summary>
    /// Gets when the article was first published.
    /// </summary>
    public DateTimeOffset PublishedAt { get; private set; }

    /// <summary>
    /// Gets when the article was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the published revision number.
    /// </summary>
    public int Revision { get; private set; }

    /// <summary>
    /// Creates the first public article row from a draft.
    /// </summary>
    public static PublishedArticle PublishFirstRevision(
        PublishedArticleId id,
        ArticleDraft draft,
        string renderedHtml,
        DateTimeOffset publishedAt)
    {
        return new PublishedArticle(id, draft, renderedHtml, publishedAt);
    }

    /// <summary>
    /// Updates the current public article row from a draft.
    /// </summary>
    public void PublishNextRevision(ArticleDraft draft, string renderedHtml, DateTimeOffset updatedAt)
    {
        Title = draft.Title;
        Summary = draft.Summary;
        RenderedHtml = renderedHtml;
        Status = PublishedArticleStatus.Published;
        UpdatedAt = updatedAt;
        Revision++;
    }
}
