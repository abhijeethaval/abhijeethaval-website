using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Articles.Admin;
using AbhijeetSite.Api.Features.Articles.CreateArticleDraft;
using AbhijeetSite.Api.Features.Articles.Rendering;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace AbhijeetSite.Api.Tests;

public sealed class AdminArticleDraftWorkflowTests : IClassFixture<PostgreSqlDatabaseFixture>
{
    private const string DraftSummary = "Short summary for an admin-authored article.";
    private const string DraftTitle = "Admin Authored Article";
    private const string InitialMdxSource = "# Initial heading\n\nInitial paragraph.";
    private const string UpdatedMdxSource = "# Updated heading\n\nUpdated paragraph.";
    private const string UpdatedTitle = "Updated Admin Article";

    private static readonly DateTimeOffset CreatedAt = new(2026, 06, 30, 08, 00, 00, TimeSpan.Zero);
    private static readonly DateTimeOffset UpdatedAt = new(2026, 06, 30, 09, 00, 00, TimeSpan.Zero);

    private readonly PostgreSqlDatabaseFixture _fixture;

    public AdminArticleDraftWorkflowTests(PostgreSqlDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerRequiredFact]
    public async Task HandleAsync_CurrentVersion_UpdatesDraftAndIncrementsVersion()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftResult draft = await CreateDraftAsync(dbContext);
        UpdateArticleDraftHandler handler = CreateUpdateHandler(dbContext);

        Result<ArticleDraftResponse> result = await handler.HandleAsync(
            CreateUpdateCommand(draft, draft.Slug.Value),
            CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(UpdatedTitle, result.Value.Title);
        Assert.Equal(draft.Version + 1, result.Value.Version);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_StaleVersion_ReturnsVersionConflict()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftResult draft = await CreateDraftAsync(dbContext);
        UpdateArticleDraftHandler handler = CreateUpdateHandler(dbContext);
        await handler.HandleAsync(CreateUpdateCommand(draft, draft.Slug.Value), CancellationToken.None);

        Result<ArticleDraftResponse> staleResult = await handler.HandleAsync(
            CreateUpdateCommand(draft, draft.Slug.Value),
            CancellationToken.None);

        Assert.True(staleResult.IsFailure);
        Assert.Equal(ArticlesErrors.DraftVersionConflictCode, staleResult.Error?.Code);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_FirstPublish_CreatesPublicArticle()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftResult draft = await CreateDraftAsync(dbContext);
        PublishArticleDraftHandler handler = CreatePublishHandler(dbContext);

        Result<PublishedArticleResponse> result = await handler.HandleAsync(
            new PublishArticleDraftCommand(draft.Id, draft.Version),
            CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(draft.Slug.Value, result.Value.Slug);
        Assert.Contains("<h2>Initial heading</h2>", result.Value.RenderedHtml);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_EditAfterPublish_DoesNotChangePublicArticleUntilRepublished()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftResult draft = await CreateDraftAsync(dbContext);
        PublishArticleDraftHandler publishHandler = CreatePublishHandler(dbContext);
        await publishHandler.HandleAsync(new PublishArticleDraftCommand(draft.Id, draft.Version), CancellationToken.None);
        UpdateArticleDraftHandler updateHandler = CreateUpdateHandler(dbContext);

        Result<ArticleDraftResponse> updateResult = await updateHandler.HandleAsync(
            CreateUpdateCommand(draft, draft.Slug.Value, expectedVersion: 2),
            CancellationToken.None);
        PublishedArticle beforeRepublish = await LoadPublishedArticleAsync(dbContext, draft.Slug);
        Assert.Equal(DraftTitle, beforeRepublish.Title);

        Result<PublishedArticleResponse> republishResult = await publishHandler.HandleAsync(
            new PublishArticleDraftCommand(draft.Id, updateResult.Value.Version),
            CancellationToken.None);

        Assert.True(republishResult.IsSuccess, republishResult.Error?.Message);
        Assert.Equal(UpdatedTitle, republishResult.Value.Title);
        Assert.Equal(2, republishResult.Value.Revision);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_PublishedDraftSlugChange_ReturnsSlugLockedError()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        CreateArticleDraftResult draft = await CreateDraftAsync(dbContext);
        PublishArticleDraftHandler publishHandler = CreatePublishHandler(dbContext);
        await publishHandler.HandleAsync(new PublishArticleDraftCommand(draft.Id, draft.Version), CancellationToken.None);
        UpdateArticleDraftHandler updateHandler = CreateUpdateHandler(dbContext);

        Result<ArticleDraftResponse> result = await updateHandler.HandleAsync(
            CreateUpdateCommand(draft, UniqueSlug(), expectedVersion: 2),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCategory.Business, result.Error?.Category);
    }

    [Fact]
    public void Render_RawHtml_ReturnsValidationError()
    {
        ConstrainedMarkdownRenderer renderer = new();

        Result<string> result = renderer.Render("# Title\n\n<script>alert(1)</script>");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCategory.Validation, result.Error?.Category);
    }

    private async Task<CreateArticleDraftResult> CreateDraftAsync(AppDbContext dbContext)
    {
        CreateArticleDraftHandler handler = new(
            dbContext,
            new ManualApplicationClock(CreatedAt),
            NullLogger<CreateArticleDraftHandler>.Instance);
        Result<CreateArticleDraftResult> result = await handler.HandleAsync(
            new CreateArticleDraftCommand(DraftTitle, UniqueSlug(), DraftSummary, InitialMdxSource),
            CancellationToken.None);
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private static UpdateArticleDraftHandler CreateUpdateHandler(AppDbContext dbContext)
    {
        return new UpdateArticleDraftHandler(
            dbContext,
            new ManualApplicationClock(UpdatedAt),
            NullLogger<UpdateArticleDraftHandler>.Instance);
    }

    private static PublishArticleDraftHandler CreatePublishHandler(AppDbContext dbContext)
    {
        return new PublishArticleDraftHandler(
            dbContext,
            new ManualApplicationClock(UpdatedAt),
            new ConstrainedMarkdownRenderer(),
            NullLogger<PublishArticleDraftHandler>.Instance);
    }

    private static UpdateArticleDraftCommand CreateUpdateCommand(
        CreateArticleDraftResult draft,
        string slug,
        int? expectedVersion = null)
    {
        return new UpdateArticleDraftCommand(
            draft.Id,
            UpdatedTitle,
            slug,
            DraftSummary,
            UpdatedMdxSource,
            expectedVersion ?? draft.Version);
    }

    private static async Task<PublishedArticle> LoadPublishedArticleAsync(
        AppDbContext dbContext,
        ArticleSlug slug)
    {
        return await dbContext.PublishedArticles.SingleAsync(article => article.Slug == slug);
    }

    private static string UniqueSlug()
    {
        return $"admin-authored-article-{Guid.CreateVersion7():N}";
    }
}
