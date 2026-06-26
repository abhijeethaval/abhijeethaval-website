using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Infrastructure.Persistence;

/// <summary>
/// Initializes configured persistence dependencies.
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Applies pending database migrations when the application database is configured.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        string? connectionString = app.Configuration.GetConnectionString(
            PersistenceConnectionNames.ApplicationDatabase);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ILogger<AppDbContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await dbContext.Database.MigrateAsync(app.Lifetime.ApplicationStopping);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database migration failed for {DatabaseName}.",
                PersistenceConnectionNames.ApplicationDatabase);
            throw new InvalidOperationException(
                "Application startup stopped because PostgreSQL migrations could not be applied.",
                exception);
        }
    }
}
