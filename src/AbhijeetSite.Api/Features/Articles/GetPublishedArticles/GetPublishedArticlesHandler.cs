using System.Diagnostics;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Features.Articles.GetPublishedArticles;

/// <summary>
/// Loads public summaries from the published article read model.
/// </summary>
public sealed class GetPublishedArticlesHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetPublishedArticlesHandler> _logger;

    /// <summary>
    /// Creates the public article list handler.
    /// </summary>
    public GetPublishedArticlesHandler(
        AppDbContext dbContext,
        ILogger<GetPublishedArticlesHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Loads published articles in reverse chronological order.
    /// </summary>
    public async Task<Result<IReadOnlyList<PublishedArticleSummaryResponse>>> HandleAsync(
        CancellationToken cancellationToken)
    {
        using Activity? activity = ArticlesTelemetry.Start(
            ArticlesTelemetry.GetPublishedArticlesActivityName);
        Result<IReadOnlyList<PublishedArticleSummaryResponse>> result = await ExecuteAsync(cancellationToken);
        return ArticlesTelemetry.Complete(activity, result);
    }

    private async Task<Result<IReadOnlyList<PublishedArticleSummaryResponse>>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            List<PublishedArticleSummaryResponse> articles = await _dbContext.PublishedArticles
                .AsNoTracking()
                .Where(article => article.Status == PublishedArticleStatus.Published)
                .OrderByDescending(article => article.PublishedAt)
                .Select(article => new PublishedArticleSummaryResponse(
                    article.Slug.Value,
                    article.Title,
                    article.Summary,
                    article.PublishedAt,
                    article.UpdatedAt))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<PublishedArticleSummaryResponse>>.Success(articles);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.GetPublishedArticlesFailed(exception.GetType().Name);
            ArticlesTelemetry.RecordException(exception);
            return Result<IReadOnlyList<PublishedArticleSummaryResponse>>.Failure(
                ArticlesErrors.ReadFailure());
        }
    }
}
