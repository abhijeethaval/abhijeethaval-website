using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbhijeetSite.Api.Features.Comments.Persistence;

/// <summary>
/// EF Core mapping for comments.
/// </summary>
public sealed class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
{
    private const int BodyMaximumLength = 4000;
    private const int StatusMaximumLength = 32;
    private const string SchemaName = "comments";
    private const string TableName = "Comments";
    private const string StatusCheckName = "CK_Comments_Status";
    private const string StatusCheckConstraint = "\"Status\" IN ('Pending', 'Approved', 'Rejected', 'Deleted')";
    private const string TimestampWithTimeZone = "timestamp with time zone";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(TableName, SchemaName, table => table.HasCheckConstraint(StatusCheckName, StatusCheckConstraint));
        builder.HasKey(comment => comment.Id);
        builder.Property(comment => comment.Id).HasConversion(id => id.Value, value => new CommentId(value));
        builder.Property(comment => comment.Id).ValueGeneratedNever();
        builder.Property(comment => comment.PublishedArticleId).HasConversion(id => id.Value, value => new PublishedArticleId(value));
        builder.Property(comment => comment.UserId).HasConversion(id => id.Value, value => new UserId(value));
        builder.Property(comment => comment.ModeratedByUserId).HasConversion(id => ToGuid(id), value => ToUserId(value));
        builder.Property(comment => comment.Body).HasMaxLength(BodyMaximumLength).IsRequired();
        builder.Property(comment => comment.Status).HasConversion<string>().HasMaxLength(StatusMaximumLength);
        builder.Property(comment => comment.CreatedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.Property(comment => comment.ModeratedAt).HasColumnType(TimestampWithTimeZone);
    }

    private static Guid? ToGuid(UserId? id)
    {
        return id?.Value;
    }

    private static UserId? ToUserId(Guid? value)
    {
        return value.HasValue ? new UserId(value.Value) : null;
    }
}
