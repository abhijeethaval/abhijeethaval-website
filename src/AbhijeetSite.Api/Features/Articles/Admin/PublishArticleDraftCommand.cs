namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Input for publishing an article draft.
/// </summary>
/// <param name="Id">Draft identifier.</param>
/// <param name="ExpectedVersion">Version read by the admin before publishing.</param>
public sealed record PublishArticleDraftCommand(ArticleDraftId Id, int ExpectedVersion);
