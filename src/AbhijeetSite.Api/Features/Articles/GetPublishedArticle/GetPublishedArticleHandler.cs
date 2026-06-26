using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Features.Articles.GetPublishedArticle;

/// <summary>
/// Loads one render-ready public article.
/// </summary>
public sealed class GetPublishedArticleHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetPublishedArticleHandler> _logger;

    /// <summary>
    /// Creates the public article detail handler.
    /// </summary>
    public GetPublishedArticleHandler(
        AppDbContext dbContext,
        ILogger<GetPublishedArticleHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Loads a published article by its stable slug.
    /// </summary>
    public async Task<Result<PublishedArticleResponse?>> HandleAsync(
        ArticleSlug slug,
        CancellationToken cancellationToken)
    {
        try
        {
            PublishedArticleResponse? article = await _dbContext.PublishedArticles
                .AsNoTracking()
                .Where(item => item.Status == PublishedArticleStatus.Published && item.Slug == slug)
                .Select(item => new PublishedArticleResponse(
                    item.Slug.Value,
                    item.Title,
                    item.Summary,
                    item.RenderedHtml,
                    item.PublishedAt,
                    item.UpdatedAt,
                    item.Revision))
                .SingleOrDefaultAsync(cancellationToken);

            return Result<PublishedArticleResponse?>.Success(article);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Loading published article {ArticleSlug} failed.", slug.Value);
            return Result<PublishedArticleResponse?>.Failure(ArticlesErrors.ReadFailure());
        }
    }
}
