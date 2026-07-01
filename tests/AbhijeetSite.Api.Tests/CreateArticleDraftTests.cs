using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Articles.CreateArticleDraft;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace AbhijeetSite.Api.Tests;

public sealed class CreateArticleDraftTests : IClassFixture<PostgreSqlDatabaseFixture>
{
    private const string DraftSummary = "Short summary for a persisted architecture draft.";
    private const string DraftTitle = "Architecture Baseline";
    private const string InvalidSlug = "invalid slug";
    private const string MdxSource = "# Architecture\n\nThis is intentionally not compiled in iteration 01.";
    private const string ValidSlug = "architecture-baseline";

    private static readonly DateTimeOffset CreatedAt = new(2026, 06, 17, 12, 00, 00, TimeSpan.Zero);

    private readonly PostgreSqlDatabaseFixture _fixture;

    public CreateArticleDraftTests(PostgreSqlDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerRequiredFact]
    public async Task HandleAsync_ValidCommand_PersistsDraft()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftHandler handler = CreateHandler(dbContext);

        Result<CreateArticleDraftResult> result = await handler.HandleAsync(
            CreateValidCommand(UniqueSlug()),
            CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        ArticleDraft draft = await dbContext.ArticleDrafts.SingleAsync(item => item.Id == result.Value.Id);
        Assert.Equal(DraftTitle, draft.Title);
        Assert.Equal(CreatedAt, draft.CreatedAt);
        Assert.Equal(ArticleDraftStatus.Draft, draft.Status);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_DuplicateSlug_ReturnsDuplicateSlugError()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftHandler handler = CreateHandler(dbContext);
        CreateArticleDraftCommand command = CreateValidCommand(UniqueSlug());

        await handler.HandleAsync(command, CancellationToken.None);
        Result<CreateArticleDraftResult> duplicateResult = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(duplicateResult.IsFailure);
        Assert.Equal(ArticlesErrors.DuplicateSlugCode, duplicateResult.Error?.Code);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_InvalidSlug_ReturnsValidationError()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftHandler handler = CreateHandler(dbContext);
        CreateArticleDraftCommand command = CreateValidCommand(InvalidSlug);

        Result<CreateArticleDraftResult> result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCategory.Validation, result.Error?.Category);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_InvalidMdxSyntax_PersistsDraftBecauseCompilationIsDeferred()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftHandler handler = CreateHandler(dbContext);
        CreateArticleDraftCommand command = CreateValidCommand(UniqueSlug(), "<BrokenMdx>");

        Result<CreateArticleDraftResult> result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    private static CreateArticleDraftHandler CreateHandler(AppDbContext dbContext)
    {
        ManualApplicationClock clock = new(CreatedAt);
        return new CreateArticleDraftHandler(
            dbContext,
            clock,
            NullLogger<CreateArticleDraftHandler>.Instance);
    }

    private static CreateArticleDraftCommand CreateValidCommand(string slug, string mdxSource = MdxSource)
    {
        return new CreateArticleDraftCommand(DraftTitle, slug, DraftSummary, mdxSource);
    }

    private static string UniqueSlug()
    {
        return $"{ValidSlug}-{Guid.CreateVersion7():N}";
    }
}
