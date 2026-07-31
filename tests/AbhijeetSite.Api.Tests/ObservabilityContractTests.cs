using System.Diagnostics;
using System.Collections.Concurrent;
using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Articles.CreateArticleDraft;
using AbhijeetSite.Api.Features.Identity;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenTelemetry;

namespace AbhijeetSite.Api.Tests;

public sealed class ObservabilityContractTests
{
    private const string InvalidSlug = "private invalid slug";
    private const string PrivateEmail = "private@example.com";
    private const string QueryTextAttribute = "db.query.text";
    private const string ResultCategoryAttribute = "app.result.category";
    private const int ExportFlushTimeoutMilliseconds = 5_000;

    [Fact]
    public async Task HandleAsync_ValidationFailure_EmitsPrivacySafeUseCaseActivity()
    {
        Activity? completedActivity = null;
        using ActivityListener listener = CreateListener(
            ArticlesTelemetry.SourceName,
            activity => completedActivity = activity);
        ActivitySource.AddActivityListener(listener);
        using Activity parent = new Activity("browser.fetch").SetIdFormat(ActivityIdFormat.W3C).Start();
        await using AppDbContext dbContext = CreateDbContext();
        CreateArticleDraftHandler handler = CreateHandler(dbContext);

        Result<CreateArticleDraftResult> result = await handler.HandleAsync(
            new CreateArticleDraftCommand("Title", InvalidSlug, "Summary", "# Content"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ArticlesTelemetry.CreateDraftActivityName, completedActivity?.DisplayName);
        Assert.Equal(parent.TraceId, completedActivity?.TraceId);
        Assert.Equal("validation", completedActivity?.GetTagItem(ResultCategoryAttribute));
        Assert.DoesNotContain(completedActivity?.TagObjects ?? [], tag => Equals(tag.Value, InvalidSlug));
    }

    [Fact]
    public async Task HandleAsync_UnverifiedIdentity_DoesNotExportEmail()
    {
        Activity? completedActivity = null;
        using ActivityListener listener = CreateListener(
            IdentityTelemetry.SourceName,
            activity => completedActivity = activity);
        ActivitySource.AddActivityListener(listener);
        await using AppDbContext dbContext = CreateDbContext();
        ExternalLoginUpsertHandler handler = CreateIdentityHandler(dbContext);
        ExternalLoginClaims claims = CreateUnverifiedClaims();

        Result<SignInUserResult> result = await handler.HandleAsync(claims, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityTelemetry.ExternalLoginUpsertActivityName, completedActivity?.DisplayName);
        Assert.DoesNotContain(completedActivity?.TagObjects ?? [], tag => Equals(tag.Value, PrivateEmail));
    }

    [Fact]
    public void OnEnd_SensitiveDatabaseActivity_RemovesQueryText()
    {
        using Activity activity = new("postgresql.command");
        activity.Start();
        activity.SetTag(QueryTextAttribute, "select private_content from articles");
        TelemetryDataProcessor processor = new();

        processor.OnEnd(activity);

        Assert.Null(activity.GetTagItem(QueryTextAttribute));
    }

    [Theory]
    [InlineData(200, false)]
    [InlineData(500, true)]
    public void OnEnd_HealthRequest_ExportsOnlyFailedTrace(int statusCode, bool isExported)
    {
        using Activity activity = new("GET /health");
        activity.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        activity.Start();
        activity.SetTag("http.route", "/health");
        activity.SetTag("http.response.status_code", statusCode);
        ConcurrentQueue<Activity> exportedActivities = new();
        CollectingActivityExporter exporter = new(exportedActivities);
        using TelemetryBatchExportProcessor processor = new(exporter);

        processor.OnEnd(activity);
        Assert.True(processor.ForceFlush(ExportFlushTimeoutMilliseconds));

        Assert.Equal(isExported, !exportedActivities.IsEmpty);
    }

    private static ActivityListener CreateListener(string sourceName, Action<Activity> onStopped)
    {
        return new ActivityListener
        {
            ShouldListenTo = source => source.Name == sourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = onStopped
        };
    }

    private static AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>().Options;
        return new AppDbContext(options);
    }

    private static CreateArticleDraftHandler CreateHandler(AppDbContext dbContext)
    {
        ManualApplicationClock clock = new(DateTimeOffset.UnixEpoch);
        return new CreateArticleDraftHandler(
            dbContext,
            clock,
            NullLogger<CreateArticleDraftHandler>.Instance);
    }

    private static ExternalLoginUpsertHandler CreateIdentityHandler(AppDbContext dbContext)
    {
        IdentityAuthenticationOptions options = new();
        ManualApplicationClock clock = new(DateTimeOffset.UnixEpoch);
        return new ExternalLoginUpsertHandler(
            dbContext,
            clock,
            Options.Create(options),
            NullLogger<ExternalLoginUpsertHandler>.Instance);
    }

    private static ExternalLoginClaims CreateUnverifiedClaims()
    {
        return ExternalLoginClaims.Create(
            ExternalLoginProvider.Google,
            "provider-subject",
            "Private Name",
            PrivateEmail,
            false,
            null).Value;
    }

    private sealed class CollectingActivityExporter : BaseExporter<Activity>
    {
        private readonly ConcurrentQueue<Activity> _exportedActivities;

        internal CollectingActivityExporter(ConcurrentQueue<Activity> exportedActivities)
        {
            _exportedActivities = exportedActivities;
        }

        public override ExportResult Export(in Batch<Activity> batch)
        {
            foreach (Activity activity in batch)
            {
                _exportedActivities.Enqueue(activity);
            }

            return ExportResult.Success;
        }
    }
}
