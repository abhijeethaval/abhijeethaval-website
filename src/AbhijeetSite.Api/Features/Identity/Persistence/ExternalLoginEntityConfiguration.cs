using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbhijeetSite.Api.Features.Identity.Persistence;

/// <summary>
/// EF Core mapping for external login provider links.
/// </summary>
public sealed class ExternalLoginEntityConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    private const int ProviderMaximumLength = 32;
    private const int ProviderSubjectMaximumLength = 256;
    private const int EmailMaximumLength = 320;
    private const string ProviderCheckConstraint = "\"Provider\" IN ('Google', 'LinkedIn')";
    private const string ProviderCheckName = "CK_ExternalLogins_Provider";
    private const string SchemaName = "identity";
    private const string TableName = "ExternalLogins";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable(TableName, SchemaName, table => table.HasCheckConstraint(ProviderCheckName, ProviderCheckConstraint));
        builder.HasKey(login => login.Id);
        builder.Property(login => login.Id).HasConversion(id => id.Value, value => new ExternalLoginId(value));
        builder.Property(login => login.Id).ValueGeneratedNever();
        builder.Property(login => login.UserId).HasConversion(id => id.Value, value => new UserId(value));
        builder.Property(login => login.Provider).HasConversion<string>().HasMaxLength(ProviderMaximumLength);
        builder.Property(login => login.ProviderSubject).HasMaxLength(ProviderSubjectMaximumLength).IsRequired();
        builder.Property(login => login.EmailAtLogin).HasMaxLength(EmailMaximumLength).IsRequired();
        builder.HasIndex(login => new { login.Provider, login.ProviderSubject }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(login => login.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
