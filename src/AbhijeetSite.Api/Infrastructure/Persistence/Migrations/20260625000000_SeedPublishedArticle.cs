using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbhijeetSite.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
[DbContext(typeof(AppDbContext))]
[Migration("20260625000000_SeedPublishedArticle")]
public sealed class SeedPublishedArticle : Migration
{
    private static readonly Guid DraftId = new("01979e51-2c00-7000-8000-000000000001");
    private static readonly Guid PublishedArticleId = new("01979e51-2c00-7000-8000-000000000002");
    private static readonly DateTimeOffset PublishedAt =
        new(2026, 06, 25, 08, 00, 00, TimeSpan.Zero);

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        InsertDraft(migrationBuilder);
        InsertPublishedArticle(migrationBuilder);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "articles",
            table: "PublishedArticles",
            keyColumn: "Id",
            keyValue: PublishedArticleId);
        migrationBuilder.DeleteData(
            schema: "articles",
            table: "ArticleDrafts",
            keyColumn: "Id",
            keyValue: DraftId);
    }

    private static void InsertDraft(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            schema: "articles",
            table: "ArticleDrafts",
            columns: ["Id", "Slug", "Title", "Summary", "MdxSource", "Status", "CreatedAt", "UpdatedAt"],
            columnTypes: ["uuid", "character varying(160)", "character varying(200)",
                "character varying(500)", "text", "character varying(32)",
                "timestamp with time zone", "timestamp with time zone"],
            values: new object[]
            {
                DraftId,
                "building-this-site-as-a-modular-monolith",
                "Building This Site as a Modular Monolith",
                "The architecture decisions behind a portfolio that doubles as a production publishing platform.",
                SeedArticleContent.MdxSource,
                "ReadyToPublish",
                PublishedAt,
                PublishedAt
            });
    }

    private static void InsertPublishedArticle(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            schema: "articles",
            table: "PublishedArticles",
            columns: ["Id", "DraftId", "Slug", "Title", "Summary", "RenderedHtml",
                "Status", "PublishedAt", "UpdatedAt", "Revision"],
            columnTypes: ["uuid", "uuid", "character varying(160)", "character varying(200)",
                "character varying(500)", "text", "character varying(32)",
                "timestamp with time zone", "timestamp with time zone", "integer"],
            values: new object[]
            {
                PublishedArticleId,
                DraftId,
                "building-this-site-as-a-modular-monolith",
                "Building This Site as a Modular Monolith",
                "The architecture decisions behind a portfolio that doubles as a production publishing platform.",
                SeedArticleContent.RenderedHtml,
                "Published",
                PublishedAt,
                PublishedAt,
                1
            });
    }
}
