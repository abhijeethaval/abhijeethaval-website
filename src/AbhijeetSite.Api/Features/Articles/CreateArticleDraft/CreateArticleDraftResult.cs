namespace AbhijeetSite.Api.Features.Articles.CreateArticleDraft;

/// <summary>
/// Successful article draft creation result.
/// </summary>
/// <param name="Id">Created draft identifier.</param>
/// <param name="Slug">Created draft slug.</param>
/// <param name="Title">Created draft title.</param>
/// <param name="Summary">Created draft summary.</param>
/// <param name="MdxSource">Created draft MDX source.</param>
/// <param name="Status">Created draft lifecycle status.</param>
/// <param name="CreatedAt">Created timestamp.</param>
/// <param name="UpdatedAt">Last update timestamp.</param>
/// <param name="Version">Optimistic concurrency version.</param>
public sealed record CreateArticleDraftResult(
    ArticleDraftId Id,
    ArticleSlug Slug,
    string Title,
    string Summary,
    string MdxSource,
    ArticleDraftStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int Version)
{
    /// <summary>
    /// Creates the result from a persisted draft.
    /// </summary>
    public static CreateArticleDraftResult FromDraft(ArticleDraft draft)
    {
        return new CreateArticleDraftResult(
            draft.Id,
            draft.Slug,
            draft.Title,
            draft.Summary,
            draft.MdxSource,
            draft.Status,
            draft.CreatedAt,
            draft.UpdatedAt,
            draft.Version);
    }
}
