using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AbhijeetSite.Api.Infrastructure.Persistence;

/// <summary>
/// Creates the application database context for EF Core design-time commands.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DesignTimeConnectionString =
        "Host=localhost;Port=5432;Database=abhijeetsite;Username=postgres;Password=postgres";

    /// <inheritdoc />
    public AppDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(DesignTimeConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}
