using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbhijeetSite.Api.Features.Articles.CreateArticleDraft;

/// <summary>
/// Creates a durable article draft below the HTTP boundary.
/// </summary>
public sealed class CreateArticleDraftHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationClock _clock;
    private readonly ILogger<CreateArticleDraftHandler> _logger;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public CreateArticleDraftHandler(
        AppDbContext dbContext,
        IApplicationClock clock,
        ILogger<CreateArticleDraftHandler> logger)
    {
        _dbContext = dbContext;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    /// Creates an article draft.
    /// </summary>
    public async Task<Result<CreateArticleDraftResult>> HandleAsync(
        CreateArticleDraftCommand command,
        CancellationToken cancellationToken)
    {
        Result<ArticleSlug> slugResult = ArticleSlug.Create(command.Slug);
        if (slugResult.IsFailure)
        {
            return ToDraftResultFailure(slugResult.Error);
        }

        ArticleSlug slug = slugResult.Value;
        if (await SlugExistsAsync(slug, cancellationToken))
        {
            return Result<CreateArticleDraftResult>.Failure(ArticlesErrors.DuplicateSlug(slug));
        }

        Result<ArticleDraft> draftResult = CreateDraft(command, slug);
        return draftResult.IsFailure
            ? ToDraftResultFailure(draftResult.Error)
            : await PersistDraftAsync(draftResult.Value, cancellationToken);
    }

    private static Result<CreateArticleDraftResult> ToDraftResultFailure(Error? error)
    {
        if (error is null)
        {
            throw new InvalidOperationException("A failed result must include an error.");
        }

        return Result<CreateArticleDraftResult>.Failure(error);
    }

    private Result<ArticleDraft> CreateDraft(CreateArticleDraftCommand command, ArticleSlug slug)
    {
        return ArticleDraft.Create(
            ArticleDraftId.New(),
            slug,
            command.Title,
            command.Summary,
            command.MdxSource,
            _clock.UtcNow);
    }

    private async Task<bool> SlugExistsAsync(ArticleSlug slug, CancellationToken cancellationToken)
    {
        return await _dbContext.ArticleDrafts.AnyAsync(draft => draft.Slug == slug, cancellationToken);
    }

    private async Task<Result<CreateArticleDraftResult>> PersistDraftAsync(
        ArticleDraft draft,
        CancellationToken cancellationToken)
    {
        _dbContext.ArticleDrafts.Add(draft);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result<CreateArticleDraftResult>.Failure(ArticlesErrors.DuplicateSlug(draft.Slug));
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Creating article draft {ArticleSlug} failed.", draft.Slug.Value);
            return Result<CreateArticleDraftResult>.Failure(ArticlesErrors.PersistenceFailure(
                "Article draft could not be created. Verify PostgreSQL connectivity and retry."));
        }

        return Result<CreateArticleDraftResult>.Success(CreateArticleDraftResult.FromDraft(draft));
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.GetBaseException() is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
