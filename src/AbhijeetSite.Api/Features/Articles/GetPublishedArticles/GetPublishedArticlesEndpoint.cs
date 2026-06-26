using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Articles.GetPublishedArticles;

/// <summary>
/// HTTP boundary for the public article collection.
/// </summary>
public static class GetPublishedArticlesEndpoint
{
    /// <summary>
    /// Returns summaries for all currently published articles.
    /// </summary>
    public static async Task<IResult> HandleAsync(
        GetPublishedArticlesHandler handler,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<PublishedArticleSummaryResponse>> result =
            await handler.HandleAsync(cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error?.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}
