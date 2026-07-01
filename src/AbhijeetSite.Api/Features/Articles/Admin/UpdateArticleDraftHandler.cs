using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Updates an admin article draft.
/// </summary>
public sealed class UpdateArticleDraftHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationClock _clock;
    private readonly ILogger<UpdateArticleDraftHandler> _logger;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public UpdateArticleDraftHandler(
        AppDbContext dbContext,
        IApplicationClock clock,
        ILogger<UpdateArticleDraftHandler> logger)
    {
        _dbContext = dbContext;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// Updates a draft if the expected version is current.
    /// </summary>
    public async Task<Result<ArticleDraftResponse>> HandleAsync(
        UpdateArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        Result<ArticleSlug> slugResult = ArticleSlug.Create(command.Slug);
        if (slugResult.IsFailure)
        {
            return ToFailure(slugResult.Error);
        }

        ArticleDraft? draft = await FindDraftAsync(command.Id, cancellationToken);
        if (draft is null)
        {
            return Result<ArticleDraftResponse>.Failure(ArticlesErrors.DraftNotFound(command.Id));
        }

        return await UpdateDraftAsync(draft, slugResult.Value, command, cancellationToken);
    }

    private async Task<Result<ArticleDraftResponse>> UpdateDraftAsync(
        ArticleDraft draft,
        ArticleSlug slug,
        UpdateArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        bool isPublished = await IsPublishedAsync(draft.Id, cancellationToken);
        Result transition = isPublished
            ? UpdatePublishedDraft(draft, slug, command)
            : await UpdateUnpublishedDraftAsync(draft, slug, command, cancellationToken);
        return transition.IsFailure
            ? ToFailure(transition.Error)
            : await PersistAsync(draft, isPublished, cancellationToken);
    }

    private async Task<Result> UpdateUnpublishedDraftAsync(
        ArticleDraft draft,
        ArticleSlug slug,
        UpdateArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        bool isDuplicateSlug = await IsDuplicateSlugAsync(draft.Id, slug, cancellationToken);
        if (isDuplicateSlug)
        {
            return Result.Failure(ArticlesErrors.DuplicateSlug(slug));
        }

        return draft.UpdateBeforeFirstPublish(
            slug,
            command.Title,
            command.Summary,
            command.MdxSource,
            command.ExpectedVersion,
            _clock.UtcNow);
    }

    private Result UpdatePublishedDraft(
        ArticleDraft draft,
        ArticleSlug slug,
        UpdateArticleDraftCommand command)
    {
        if (slug != draft.Slug)
        {
            return Result.Failure(ArticlesErrors.SlugLockedAfterPublish(draft.Slug));
        }

        return draft.UpdateAfterFirstPublish(
            command.Title,
            command.Summary,
            command.MdxSource,
            command.ExpectedVersion,
            _clock.UtcNow);
    }

    private async Task<Result<ArticleDraftResponse>> PersistAsync(
        ArticleDraft draft,
        bool isPublished,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<ArticleDraftResponse>.Success(ToResponse(draft, isPublished));
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Article draft {ArticleDraftId} version conflict.", draft.Id.Value);
            return Result<ArticleDraftResponse>.Failure(ArticlesErrors.DraftVersionConflict());
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result<ArticleDraftResponse>.Failure(ArticlesErrors.DuplicateSlug(draft.Slug));
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Saving article draft {ArticleDraftId} failed.", draft.Id.Value);
            return Result<ArticleDraftResponse>.Failure(ArticlesErrors.PersistenceFailure(
                "Article draft could not be saved. Verify PostgreSQL connectivity and retry."));
        }
    }

    private async Task<ArticleDraft?> FindDraftAsync(ArticleDraftId id, CancellationToken cancellationToken)
    {
        return await _dbContext.ArticleDrafts.SingleOrDefaultAsync(
            draft => draft.Id == id,
            cancellationToken);
    }

    private async Task<bool> IsPublishedAsync(ArticleDraftId id, CancellationToken cancellationToken)
    {
        return await _dbContext.PublishedArticles
            .AnyAsync(article => article.DraftId == id, cancellationToken);
    }

    private async Task<bool> IsDuplicateSlugAsync(
        ArticleDraftId id,
        ArticleSlug slug,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ArticleDrafts
            .AnyAsync(draft => draft.Id != id && draft.Slug == slug, cancellationToken);
    }

    private static ArticleDraftResponse ToResponse(ArticleDraft draft, bool isPublished)
    {
        ArticleDraftPublicationState state = isPublished
            ? ArticleDraftPublicationState.Published
            : ArticleDraftPublicationState.Unpublished;
        return ArticleDraftResponse.FromDraft(draft, state);
    }

    private static Result<ArticleDraftResponse> ToFailure(Error? error)
    {
        if (error is not null)
        {
            return Result<ArticleDraftResponse>.Failure(error);
        }

        throw new InvalidOperationException("A failed result must include an error.");
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.GetBaseException() is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
