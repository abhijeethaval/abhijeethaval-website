using AbhijeetSite.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace AbhijeetSite.Api.Tests.Support;

public sealed class PostgreSqlDatabaseFixture : IAsyncLifetime
{
    private const string DatabaseName = "abhijeetsite";
    private const string Password = "postgres";
    private const string PostgresImage = "postgres:17-alpine";
    private const string Username = "postgres";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(PostgresImage)
        .WithDatabase(DatabaseName)
        .WithUsername(Username)
        .WithPassword(Password)
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using AppDbContext dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new AppDbContext(options);
    }
}
