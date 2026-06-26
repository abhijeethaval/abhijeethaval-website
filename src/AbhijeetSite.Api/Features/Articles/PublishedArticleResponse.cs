namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Render-ready public article.
/// </summary>
/// <param name="Slug">Stable public article slug.</param>
/// <param name="Title">Public article title.</param>
/// <param name="Summary">Short article summary.</param>
/// <param name="RenderedHtml">Sanitized HTML produced for public rendering.</param>
/// <param name="PublishedAt">Initial publication timestamp.</param>
/// <param name="UpdatedAt">Latest publication timestamp.</param>
/// <param name="Revision">Published revision number.</param>
public sealed record PublishedArticleResponse(
    string Slug,
    string Title,
    string Summary,
    string RenderedHtml,
    DateTimeOffset PublishedAt,
    DateTimeOffset UpdatedAt,
    int Revision);
