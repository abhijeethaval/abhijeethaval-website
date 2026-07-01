using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Loads one admin article draft.
/// </summary>
public sealed class GetArticleDraftHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetArticleDraftHandler> _logger;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public GetArticleDraftHandler(AppDbContext dbContext, ILogger<GetArticleDraftHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Loads an article draft.
    /// </summary>
    public async Task<Result<ArticleDraftResponse?>> HandleAsync(
        ArticleDraftId id,
        CancellationToken cancellationToken)
    {
        try
        {
            ArticleDraft? draft = await _dbContext.ArticleDrafts
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (draft is null)
            {
                return Result<ArticleDraftResponse?>.Success(null);
            }

            bool isPublished = await IsPublishedAsync(id, cancellationToken);
            return Result<ArticleDraftResponse?>.Success(ToResponse(draft, isPublished));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Loading article draft {ArticleDraftId} failed.", id.Value);
            return Result<ArticleDraftResponse?>.Failure(ArticlesErrors.ReadFailure());
        }
    }

    private async Task<bool> IsPublishedAsync(ArticleDraftId id, CancellationToken cancellationToken)
    {
        return await _dbContext.PublishedArticles
            .AnyAsync(article => article.DraftId == id, cancellationToken);
    }

    private static ArticleDraftResponse ToResponse(ArticleDraft draft, bool isPublished)
    {
        ArticleDraftPublicationState state = isPublished
            ? ArticleDraftPublicationState.Published
            : ArticleDraftPublicationState.Unpublished;
        return ArticleDraftResponse.FromDraft(draft, state);
    }
}
