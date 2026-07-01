namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Admin request for publishing an article draft.
/// </summary>
/// <param name="ExpectedVersion">Version read by the admin before publishing.</param>
public sealed record PublishArticleDraftRequest(int ExpectedVersion);
