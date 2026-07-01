using AbhijeetSite.Api.Features.Articles.Rendering;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Publishes an article draft into the public article read model.
/// </summary>
public sealed class PublishArticleDraftHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationClock _clock;
    private readonly ConstrainedMarkdownRenderer _renderer;
    private readonly ILogger<PublishArticleDraftHandler> _logger;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public PublishArticleDraftHandler(
        AppDbContext dbContext,
        IApplicationClock clock,
        ConstrainedMarkdownRenderer renderer,
        ILogger<PublishArticleDraftHandler> logger)
    {
        _dbContext = dbContext;
        _clock = clock;
        _renderer = renderer;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a draft if the expected version is current.
    /// </summary>
    public async Task<Result<PublishedArticleResponse>> HandleAsync(
        PublishArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        ArticleDraft? draft = await FindDraftAsync(command.Id, cancellationToken);
        if (draft is null)
        {
            return Result<PublishedArticleResponse>.Failure(ArticlesErrors.DraftNotFound(command.Id));
        }

        Result<string> renderResult = _renderer.Render(draft.MdxSource);
        if (renderResult.IsFailure)
        {
            return ToFailure(renderResult.Error);
        }

        return await PublishRenderedDraftAsync(draft, renderResult.Value, command, cancellationToken);
    }

    private async Task<Result<PublishedArticleResponse>> PublishRenderedDraftAsync(
        ArticleDraft draft,
        string renderedHtml,
        PublishArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        PublishedArticle? article = await FindPublishedArticleAsync(draft.Id, cancellationToken);
        Result transition = draft.MarkReadyToPublish(command.ExpectedVersion, _clock.UtcNow);
        if (transition.IsFailure)
        {
            return ToFailure(transition.Error);
        }

        return article is null
            ? await PublishFirstRevisionAsync(draft, renderedHtml, cancellationToken)
            : await PublishNextRevisionAsync(article, draft, renderedHtml, cancellationToken);
    }

    private async Task<Result<PublishedArticleResponse>> PublishFirstRevisionAsync(
        ArticleDraft draft,
        string renderedHtml,
        CancellationToken cancellationToken)
    {
        bool isDuplicateSlug = await IsPublishedSlugTakenAsync(draft, cancellationToken);
        if (isDuplicateSlug)
        {
            return Result<PublishedArticleResponse>.Failure(ArticlesErrors.DuplicateSlug(draft.Slug));
        }

        PublishedArticle article = PublishedArticle.PublishFirstRevision(
            PublishedArticleId.New(),
            draft,
            renderedHtml,
            _clock.UtcNow);
        _dbContext.PublishedArticles.Add(article);
        return await PersistAsync(article, cancellationToken);
    }

    private async Task<Result<PublishedArticleResponse>> PublishNextRevisionAsync(
        PublishedArticle article,
        ArticleDraft draft,
        string renderedHtml,
        CancellationToken cancellationToken)
    {
        article.PublishNextRevision(draft, renderedHtml, _clock.UtcNow);
        return await PersistAsync(article, cancellationToken);
    }

    private async Task<Result<PublishedArticleResponse>> PersistAsync(
        PublishedArticle article,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<PublishedArticleResponse>.Success(ToResponse(article));
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Article draft publish hit a version conflict.");
            return Result<PublishedArticleResponse>.Failure(ArticlesErrors.DraftVersionConflict());
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result<PublishedArticleResponse>.Failure(ArticlesErrors.DuplicateSlug(article.Slug));
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Publishing article draft {ArticleDraftId} failed.", article.DraftId.Value);
            return Result<PublishedArticleResponse>.Failure(ArticlesErrors.PersistenceFailure(
                "Article draft could not be published. Verify PostgreSQL connectivity and retry."));
        }
    }

    private async Task<ArticleDraft?> FindDraftAsync(ArticleDraftId id, CancellationToken cancellationToken)
    {
        return await _dbContext.ArticleDrafts.SingleOrDefaultAsync(
            draft => draft.Id == id,
            cancellationToken);
    }

    private async Task<PublishedArticle?> FindPublishedArticleAsync(
        ArticleDraftId id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.PublishedArticles.SingleOrDefaultAsync(
            article => article.DraftId == id,
            cancellationToken);
    }

    private async Task<bool> IsPublishedSlugTakenAsync(
        ArticleDraft draft,
        CancellationToken cancellationToken)
    {
        return await _dbContext.PublishedArticles
            .AnyAsync(article => article.DraftId != draft.Id && article.Slug == draft.Slug, cancellationToken);
    }

    private static PublishedArticleResponse ToResponse(PublishedArticle article)
    {
        return new PublishedArticleResponse(
            article.Slug.Value,
            article.Title,
            article.Summary,
            article.RenderedHtml,
            article.PublishedAt,
            article.UpdatedAt,
            article.Revision);
    }

    private static Result<PublishedArticleResponse> ToFailure(Error? error)
    {
        if (error is not null)
        {
            return Result<PublishedArticleResponse>.Failure(error);
        }

        throw new InvalidOperationException("A failed result must include an error.");
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.GetBaseException() is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
