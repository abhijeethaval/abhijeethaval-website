using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Comments;
using AbhijeetSite.Api.Features.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbhijeetSite.Api.Infrastructure.Persistence;

/// <summary>
/// EF Core unit of work for the modular monolith.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Creates the application database context.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets local users.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets external login mappings.
    /// </summary>
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();

    /// <summary>
    /// Gets article drafts.
    /// </summary>
    public DbSet<ArticleDraft> ArticleDrafts => Set<ArticleDraft>();

    /// <summary>
    /// Gets published article read models.
    /// </summary>
    public DbSet<PublishedArticle> PublishedArticles => Set<PublishedArticle>();

    /// <summary>
    /// Gets user comments.
    /// </summary>
    public DbSet<Comment> Comments => Set<Comment>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ConfigureCommentRelationships(modelBuilder);
    }

    private static void ConfigureCommentRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(comment => comment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(comment => comment.ModeratedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne<PublishedArticle>()
            .WithMany()
            .HasForeignKey(comment => comment.PublishedArticleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
