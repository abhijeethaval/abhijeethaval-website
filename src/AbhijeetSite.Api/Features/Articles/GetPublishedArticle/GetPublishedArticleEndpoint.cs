using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles.GetPublishedArticle;

/// <summary>
/// HTTP boundary for one public article.
/// </summary>
public static class GetPublishedArticleEndpoint
{
    /// <summary>
    /// Returns a published article by slug.
    /// </summary>
    public static async Task<IResult> HandleAsync(
        string slug,
        GetPublishedArticleHandler handler,
        CancellationToken cancellationToken)
    {
        Result<ArticleSlug> slugResult = ArticleSlug.Create(slug);
        if (slugResult.IsFailure)
        {
            return Results.NotFound();
        }

        Result<PublishedArticleResponse?> result =
            await handler.HandleAsync(slugResult.Value, cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                result.Error?.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return result.Value is null ? Results.NotFound() : Results.Ok(result.Value);
    }
}
