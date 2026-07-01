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
    /// <summary>
    /// Adds EF Core persistence services when a database connection string is configured.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(PersistenceConnectionNames.ApplicationDatabase);

        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
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
}
