namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Admin detail view of an article draft.
/// </summary>
/// <param name="Id">Draft identifier.</param>
/// <param name="Slug">Draft slug.</param>
/// <param name="Title">Draft title.</param>
/// <param name="Summary">Draft summary.</param>
/// <param name="MdxSource">Draft MDX source.</param>
/// <param name="Status">Draft lifecycle status.</param>
/// <param name="CreatedAt">Created timestamp.</param>
/// <param name="UpdatedAt">Last update timestamp.</param>
/// <param name="Version">Optimistic concurrency version.</param>
/// <param name="IsPublished">Whether the draft has a public article row.</param>
public sealed record ArticleDraftResponse(
    string Id,
    string Slug,
    string Title,
    string Summary,
    string MdxSource,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int Version,
    bool IsPublished)
{
    /// <summary>
    /// Creates a response from a draft.
    /// </summary>
    public static ArticleDraftResponse FromDraft(
        ArticleDraft draft,
        ArticleDraftPublicationState publicationState)
    {
        return new ArticleDraftResponse(
            draft.Id.Value.ToString(),
            draft.Slug.Value,
            draft.Title,
            draft.Summary,
            draft.MdxSource,
            draft.Status.ToString(),
            draft.CreatedAt,
            draft.UpdatedAt,
            draft.Version,
            publicationState == ArticleDraftPublicationState.Published);
    }
}
