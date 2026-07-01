namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Admin list summary of an article draft.
/// </summary>
/// <param name="Id">Draft identifier.</param>
/// <param name="Slug">Draft slug.</param>
/// <param name="Title">Draft title.</param>
/// <param name="Status">Draft lifecycle status.</param>
/// <param name="UpdatedAt">Last update timestamp.</param>
/// <param name="Version">Optimistic concurrency version.</param>
/// <param name="IsPublished">Whether the draft has a public article row.</param>
public sealed record ArticleDraftSummaryResponse(
    string Id,
    string Slug,
    string Title,
    string Status,
    DateTimeOffset UpdatedAt,
    int Version,
    bool IsPublished);
