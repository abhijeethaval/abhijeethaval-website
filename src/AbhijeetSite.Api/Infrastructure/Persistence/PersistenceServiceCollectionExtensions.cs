using Microsoft.EntityFrameworkCore;
using AbhijeetSite.Api.Features.Articles.Admin;
using AbhijeetSite.Api.Features.Articles.CreateArticleDraft;
using AbhijeetSite.Api.Features.Articles.GetPublishedArticle;
using AbhijeetSite.Api.Features.Articles.GetPublishedArticles;
using AbhijeetSite.Api.Features.Articles.Rendering;

namespace AbhijeetSite.Api.Infrastructure.Persistence;

/// <summary>
/// Registers persistence infrastructure.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    private const string DatabaseCommandSpanName = "postgresql.command";

    /// <summary>
    /// Adds EF Core persistence services when a database connection string is configured.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        string? connectionString = configuration.GetConnectionString(PersistenceConnectionNames.ApplicationDatabase);
        EnsureProductionConnectionStringIsConfigured(connectionString, environment);

        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString, npgsql => npgsql.ConfigureDataSource(dataSource =>
                    dataSource.ConfigureTracing(tracing =>
                        tracing.ConfigureCommandSpanNameProvider(_ => DatabaseCommandSpanName))));
            }
        });
        services.AddSingleton<ConstrainedMarkdownRenderer>();
        services.AddScoped<CreateArticleDraftHandler>();
        services.AddScoped<GetPublishedArticleHandler>();
        services.AddScoped<GetPublishedArticlesHandler>();
        services.AddScoped<GetArticleDraftHandler>();
        services.AddScoped<GetArticleDraftsHandler>();
        services.AddScoped<PublishArticleDraftHandler>();
        services.AddScoped<UpdateArticleDraftHandler>();

        return services;
    }

    private static void EnsureProductionConnectionStringIsConfigured(
        string? connectionString,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment() && string.IsNullOrWhiteSpace(connectionString))
        {
            const string message =
                "ConnectionStrings__abhijeetsite-db must be configured outside Development.";
            throw new InvalidOperationException(message);
        }
    }
}
