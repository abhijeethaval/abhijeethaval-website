using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbhijeetSite.Api.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialPersistence : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "articles");
        migrationBuilder.EnsureSchema(name: "comments");
        migrationBuilder.EnsureSchema(name: "identity");

        CreateUsers(migrationBuilder);
        CreateArticleDrafts(migrationBuilder);
        CreateExternalLogins(migrationBuilder);
        CreatePublishedArticles(migrationBuilder);
        CreateComments(migrationBuilder);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Comments", schema: "comments");
        migrationBuilder.DropTable(name: "ExternalLogins", schema: "identity");
        migrationBuilder.DropTable(name: "PublishedArticles", schema: "articles");
        migrationBuilder.DropTable(name: "Users", schema: "identity");
        migrationBuilder.DropTable(name: "ArticleDrafts", schema: "articles");
        migrationBuilder.DropSchema(name: "comments");
        migrationBuilder.DropSchema(name: "identity");
        migrationBuilder.DropSchema(name: "articles");
    }

    private static void CreateUsers(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                AvatarUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                LastSignedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Users", user => user.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            schema: "identity",
            table: "Users",
            column: "Email",
            unique: true);
    }

    private static void CreateArticleDrafts(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ArticleDrafts",
            schema: "articles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                MdxSource = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArticleDrafts", draft => draft.Id);
                table.CheckConstraint(
                    "CK_ArticleDrafts_Status",
                    "\"Status\" IN ('Draft', 'ReadyToPublish', 'Archived')");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ArticleDrafts_Slug",
            schema: "articles",
            table: "ArticleDrafts",
            column: "Slug",
            unique: true);
    }

    private static void CreateExternalLogins(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ExternalLogins",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ProviderSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                EmailAtLogin = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExternalLogins", login => login.Id);
                table.CheckConstraint("CK_ExternalLogins_Provider", "\"Provider\" IN ('Google', 'LinkedIn')");
                table.ForeignKey(
                    name: "FK_ExternalLogins_Users_UserId",
                    column: login => login.UserId,
                    principalSchema: "identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ExternalLogins_Provider_ProviderSubject",
            schema: "identity",
            table: "ExternalLogins",
            columns: ["Provider", "ProviderSubject"],
            unique: true);
    }

    private static void CreatePublishedArticles(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PublishedArticles",
            schema: "articles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DraftId = table.Column<Guid>(type: "uuid", nullable: false),
                Slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                RenderedHtml = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Revision = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PublishedArticles", article => article.Id);
                table.CheckConstraint("CK_PublishedArticles_Status", "\"Status\" IN ('Published', 'Unpublished')");
                table.ForeignKey(
                    name: "FK_PublishedArticles_ArticleDrafts_DraftId",
                    column: article => article.DraftId,
                    principalSchema: "articles",
                    principalTable: "ArticleDrafts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PublishedArticles_DraftId",
            schema: "articles",
            table: "PublishedArticles",
            column: "DraftId");

        migrationBuilder.CreateIndex(
            name: "IX_PublishedArticles_Slug",
            schema: "articles",
            table: "PublishedArticles",
            column: "Slug",
            unique: true);
    }

    private static void CreateComments(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Comments",
            schema: "comments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PublishedArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModeratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ModeratedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", comment => comment.Id);
                table.CheckConstraint(
                    "CK_Comments_Status",
                    "\"Status\" IN ('Pending', 'Approved', 'Rejected', 'Deleted')");
                table.ForeignKey(
                    name: "FK_Comments_PublishedArticles_PublishedArticleId",
                    column: comment => comment.PublishedArticleId,
                    principalSchema: "articles",
                    principalTable: "PublishedArticles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Comments_Users_UserId",
                    column: comment => comment.UserId,
                    principalSchema: "identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Comments_Users_ModeratedByUserId",
                    column: comment => comment.ModeratedByUserId,
                    principalSchema: "identity",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        CreateCommentIndexes(migrationBuilder);
    }

    private static void CreateCommentIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Comments_ModeratedByUserId",
            schema: "comments",
            table: "Comments",
            column: "ModeratedByUserId");
        migrationBuilder.CreateIndex(
            name: "IX_Comments_PublishedArticleId",
            schema: "comments",
            table: "Comments",
            column: "PublishedArticleId");
        migrationBuilder.CreateIndex(
            name: "IX_Comments_UserId",
            schema: "comments",
            table: "Comments",
            column: "UserId");
    }
}
