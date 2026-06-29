using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbhijeetSite.Api.Features.Identity.Persistence;

/// <summary>
/// EF Core mapping for local users.
/// </summary>
public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    private const int DisplayNameMaximumLength = 200;
    private const int EmailMaximumLength = 320;
    private const int AvatarUrlMaximumLength = 2048;
    private const string SchemaName = "identity";
    private const string TableName = "Users";
    private const string TimestampWithTimeZone = "timestamp with time zone";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableName, SchemaName);
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasConversion(id => id.Value, value => new UserId(value));
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.DisplayName).HasMaxLength(DisplayNameMaximumLength).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(EmailMaximumLength).IsRequired();
        builder.Property(user => user.AvatarUrl).HasMaxLength(AvatarUrlMaximumLength);
        builder.Property(user => user.CreatedAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.Property(user => user.LastSignedInAt).HasColumnType(TimestampWithTimeZone).IsRequired();
        builder.Property(user => user.IsAdmin).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique();
    }
}
