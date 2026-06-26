using AbhijeetSite.Api.Features.Articles.GetPublishedArticle;
using AbhijeetSite.Api.Features.Articles.GetPublishedArticles;

namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Registers public article endpoints.
/// </summary>
public static class ArticleEndpointsExtension
{
    /// <summary>
    /// Maps public article read routes.
    /// </summary>
    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/articles", GetPublishedArticlesEndpoint.HandleAsync)
            .WithName("GetPublishedArticles");
        app.MapGet("/api/articles/{slug}", GetPublishedArticleEndpoint.HandleAsync)
            .WithName("GetPublishedArticle");

        return app;
    }
}
