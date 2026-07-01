namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Admin request for creating an article draft.
/// </summary>
/// <param name="Title">Draft title.</param>
/// <param name="Slug">Draft slug.</param>
/// <param name="Summary">Draft summary.</param>
/// <param name="MdxSource">Draft MDX source.</param>
public sealed record CreateArticleDraftRequest(
    string Title,
    string Slug,
    string Summary,
    string MdxSource);
