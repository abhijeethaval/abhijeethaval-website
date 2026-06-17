using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbhijeetSite.Api.Features.Articles.Persistence;

/// <summary>
/// EF Core mapping for article drafts.
/// </summary>
public sealed class ArticleDraftEntityConfiguration : IEntityTypeConfiguration<ArticleDraft>
{
    private const int StatusMaximumLength = 32;
    private const string SchemaName = "articles";
    private const string TableName = "ArticleDrafts";
    private const string StatusCheckName = "CK_ArticleDrafts_Status";
    private const string StatusCheckConstraint = "\"Status\" IN ('Draft', 'ReadyToPublish', 'Archived')";
    private const string TimestampWithTimeZone = "timestamp with time zone";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ArticleDraft> builder)
    {
        builder.ToTable(TableName, SchemaName, table => table.HasCheckConstraint(StatusCheckName, StatusCheckConstraint));
        builder.HasKey(draft => draft.Id);
        builder.Property(draft => draft.Id).HasConversion(id => id.Value, value => new ArticleDraftId(value));
        builder.Property(draft => draft.Id).ValueGeneratedNever();
        builder.Property(draft => draft.Slug).HasConversion(slug => slug.Value, value => new ArticleSlug(value));
        builder.Property(draft => draft.Slug).HasMaxLength(ArticleSlug.MaximumLength).IsRequired();
        builder.Property(draft => draft.Title).HasMaxLength(ArticleDraft.TitleMaximumLength).IsRequired();
        builder.Property(draft => draft.Summary).HasMaxLength(ArticleDraft.SummaryMaximumLength).IsRequired();
        builder.Property(draft => draft.MdxSource).HasColumnType("text").IsRequired();
        builder.Property(draft => draft.Status).HasConversion<string>().HasMaxLength(StatusMaximumLength);
        builder.Property(draft => draft.CreatedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.Property(draft => draft.UpdatedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.HasIndex(draft => draft.Slug).IsUnique();
    }
}
