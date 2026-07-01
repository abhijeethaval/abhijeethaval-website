namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Admin request for updating an article draft.
/// </summary>
/// <param name="Title">Draft title.</param>
/// <param name="Slug">Draft slug.</param>
/// <param name="Summary">Draft summary.</param>
/// <param name="MdxSource">Draft MDX source.</param>
/// <param name="ExpectedVersion">Version read by the admin before editing.</param>
public sealed record UpdateArticleDraftRequest(
    string Title,
    string Slug,
    string Summary,
    string MdxSource,
    int ExpectedVersion);
