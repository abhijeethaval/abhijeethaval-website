using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Lists admin article drafts.
/// </summary>
public sealed class GetArticleDraftsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetArticleDraftsHandler> _logger;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public GetArticleDraftsHandler(AppDbContext dbContext, ILogger<GetArticleDraftsHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Lists article drafts.
    /// </summary>
    public async Task<Result<IReadOnlyList<ArticleDraftSummaryResponse>>> HandleAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            List<ArticleDraft> drafts = await LoadDraftsAsync(cancellationToken);
            HashSet<ArticleDraftId> publishedDraftIds = await LoadPublishedDraftIdsAsync(cancellationToken);
            IReadOnlyList<ArticleDraftSummaryResponse> response = drafts
                .Select(draft => ToSummary(draft, publishedDraftIds))
                .ToList();
            return Result<IReadOnlyList<ArticleDraftSummaryResponse>>.Success(response);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Loading article drafts failed.");
            return Result<IReadOnlyList<ArticleDraftSummaryResponse>>.Failure(ArticlesErrors.ReadFailure());
        }
    }

    private async Task<List<ArticleDraft>> LoadDraftsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ArticleDrafts
            .OrderByDescending(draft => draft.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    private async Task<HashSet<ArticleDraftId>> LoadPublishedDraftIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.PublishedArticles
            .Select(article => article.DraftId)
            .ToHashSetAsync(cancellationToken);
    }

    private static ArticleDraftSummaryResponse ToSummary(
        ArticleDraft draft,
        HashSet<ArticleDraftId> publishedDraftIds)
    {
        return new ArticleDraftSummaryResponse(
            draft.Id.Value.ToString(),
            draft.Slug.Value,
            draft.Title,
            draft.Status.ToString(),
            draft.UpdatedAt,
            draft.Version,
            publishedDraftIds.Contains(draft.Id));
    }
}
