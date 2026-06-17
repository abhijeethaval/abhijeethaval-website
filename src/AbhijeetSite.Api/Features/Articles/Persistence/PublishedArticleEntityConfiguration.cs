using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbhijeetSite.Api.Features.Articles.Persistence;

/// <summary>
/// EF Core mapping for published article read models.
/// </summary>
public sealed class PublishedArticleEntityConfiguration : IEntityTypeConfiguration<PublishedArticle>
{
    private const int StatusMaximumLength = 32;
    private const string SchemaName = "articles";
    private const string TableName = "PublishedArticles";
    private const string StatusCheckName = "CK_PublishedArticles_Status";
    private const string StatusCheckConstraint = "\"Status\" IN ('Published', 'Unpublished')";
    private const string TimestampWithTimeZone = "timestamp with time zone";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PublishedArticle> builder)
    {
        builder.ToTable(TableName, SchemaName, table => table.HasCheckConstraint(StatusCheckName, StatusCheckConstraint));
        builder.HasKey(article => article.Id);
        builder.Property(article => article.Id).HasConversion(id => id.Value, value => new PublishedArticleId(value));
        builder.Property(article => article.Id).ValueGeneratedNever();
        builder.Property(article => article.DraftId).HasConversion(id => id.Value, value => new ArticleDraftId(value));
        builder.Property(article => article.Slug).HasConversion(slug => slug.Value, value => new ArticleSlug(value));
        builder.Property(article => article.Slug).HasMaxLength(ArticleSlug.MaximumLength).IsRequired();
        builder.Property(article => article.Title).HasMaxLength(ArticleDraft.TitleMaximumLength).IsRequired();
        builder.Property(article => article.Summary).HasMaxLength(ArticleDraft.SummaryMaximumLength).IsRequired();
        builder.Property(article => article.RenderedHtml).HasColumnType("text").IsRequired();
        builder.Property(article => article.Status).HasConversion<string>().HasMaxLength(StatusMaximumLength);
        builder.Property(article => article.PublishedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.Property(article => article.UpdatedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.HasIndex(article => article.Slug).IsUnique();
        builder.HasOne<ArticleDraft>().WithMany().HasForeignKey(article => article.DraftId).OnDelete(DeleteBehavior.Restrict);
    }
}
