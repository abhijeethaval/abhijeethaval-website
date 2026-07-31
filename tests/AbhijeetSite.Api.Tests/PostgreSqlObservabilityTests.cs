using System.Diagnostics;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace AbhijeetSite.Api.Tests;

public sealed class PostgreSqlObservabilityTests : IClassFixture<PostgreSqlDatabaseFixture>
{
    private const string DatabaseCommandSpanName = "postgresql.command";

    private readonly PostgreSqlDatabaseFixture _fixture;

    public PostgreSqlObservabilityTests(PostgreSqlDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerRequiredFact]
    public async Task QueryAsync_CompletedCommand_ExportsNoSqlOrParameters()
    {
        Activity? completedActivity = null;
        using ActivityListener listener = CreateListener(activity => completedActivity = activity);
        ActivitySource.AddActivityListener(listener);
        await using NpgsqlDataSource dataSource = CreateDataSource();
        await using AppDbContext dbContext = CreateDbContext(dataSource);

        await dbContext.PublishedArticles.AsNoTracking().CountAsync();

        Assert.NotNull(completedActivity);
        TelemetryDataProcessor processor = new();
        processor.OnEnd(completedActivity);
        Assert.Equal(DatabaseCommandSpanName, completedActivity.DisplayName);
        Assert.DoesNotContain(completedActivity.TagObjects, IsSqlAttribute);
    }

    private static ActivityListener CreateListener(Action<Activity> onStopped)
    {
        return new ActivityListener
        {
            ShouldListenTo = source => source.Name.Contains("Npgsql", StringComparison.Ordinal),
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = onStopped
        };
    }

    private NpgsqlDataSource CreateDataSource()
    {
        NpgsqlDataSourceBuilder builder = new(_fixture.ConnectionString);
        builder.ConfigureTracing(options => options.ConfigureCommandSpanNameProvider(
            _ => DatabaseCommandSpanName));
        return builder.Build();
    }

    private static AppDbContext CreateDbContext(NpgsqlDataSource dataSource)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(dataSource)
            .Options;
        return new AppDbContext(options);
    }

    private static bool IsSqlAttribute(KeyValuePair<string, object?> attribute)
    {
        return attribute.Key is "db.query.text" or "db.statement"
            || attribute.Value?.ToString()?.Contains("SELECT", StringComparison.OrdinalIgnoreCase) == true;
    }
}
