namespace AbhijeetSite.Api.Features.Articles.CreateArticleDraft;

/// <summary>
/// Successful article draft creation result.
/// </summary>
/// <param name="Id">Created draft identifier.</param>
/// <param name="Slug">Created draft slug.</param>
public sealed record CreateArticleDraftResult(ArticleDraftId Id, ArticleSlug Slug);
