namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Public summary of a published article.
/// </summary>
/// <param name="Slug">Stable public article slug.</param>
/// <param name="Title">Public article title.</param>
/// <param name="Summary">Short article summary.</param>
/// <param name="PublishedAt">Initial publication timestamp.</param>
/// <param name="UpdatedAt">Latest publication timestamp.</param>
public sealed record PublishedArticleSummaryResponse(
    string Slug,
    string Title,
    string Summary,
    DateTimeOffset PublishedAt,
    DateTimeOffset UpdatedAt);
