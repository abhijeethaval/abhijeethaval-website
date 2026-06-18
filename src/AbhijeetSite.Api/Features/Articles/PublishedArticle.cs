namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Render-ready public article read model.
/// </summary>
public sealed class PublishedArticle
{
    private PublishedArticle()
    {
        Title = string.Empty;
        Summary = string.Empty;
        RenderedHtml = string.Empty;
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
}
