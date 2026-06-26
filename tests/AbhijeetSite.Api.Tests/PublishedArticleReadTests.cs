using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Articles.GetPublishedArticle;
using AbhijeetSite.Api.Features.Articles.GetPublishedArticles;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace AbhijeetSite.Api.Tests;

public sealed class PublishedArticleReadTests : IClassFixture<PostgreSqlDatabaseFixture>
{
    private const string ExistingSlug = "building-this-site-as-a-modular-monolith";
    private const string ExistingTitle = "Building This Site as a Modular Monolith";
    private const string MissingSlug = "missing-article";

    private readonly PostgreSqlDatabaseFixture _fixture;

    public PublishedArticleReadTests(PostgreSqlDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerRequiredFact]
    public async Task HandleAsync_PublishedArticle_ReturnsArticleInPublicList()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        GetPublishedArticlesHandler handler = new(
            dbContext,
            NullLogger<GetPublishedArticlesHandler>.Instance);

        Result<IReadOnlyList<PublishedArticleSummaryResponse>> result =
            await handler.HandleAsync(CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        PublishedArticleSummaryResponse article = Assert.Single(result.Value);
        Assert.Equal(ExistingSlug, article.Slug);
        Assert.Equal(ExistingTitle, article.Title);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_ExistingSlug_ReturnsRenderedPublishedArticle()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        GetPublishedArticleHandler handler = CreateDetailHandler(dbContext);
        ArticleSlug slug = ArticleSlug.Create(ExistingSlug).Value;

        Result<PublishedArticleResponse?> result =
            await handler.HandleAsync(slug, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.NotNull(result.Value);
        Assert.Contains("<h2>", result.Value.RenderedHtml);
        Assert.Equal(ExistingTitle, result.Value.Title);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_MissingSlug_ReturnsNoArticle()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        GetPublishedArticleHandler handler = CreateDetailHandler(dbContext);
        ArticleSlug slug = ArticleSlug.Create(MissingSlug).Value;

        Result<PublishedArticleResponse?> result =
            await handler.HandleAsync(slug, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Null(result.Value);
    }

    private static GetPublishedArticleHandler CreateDetailHandler(AppDbContext dbContext)
    {
        return new GetPublishedArticleHandler(
            dbContext,
            NullLogger<GetPublishedArticleHandler>.Instance);
    }
}
