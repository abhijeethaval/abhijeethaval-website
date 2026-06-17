using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Identity;

namespace AbhijeetSite.Api.Features.Comments;

/// <summary>
/// Authenticated user comment on a published article.
/// </summary>
public sealed class Comment
{
    private Comment()
    {
        Body = string.Empty;
    }

    private Comment(
        CommentId id,
        PublishedArticleId publishedArticleId,
        UserId userId,
        string body,
        DateTimeOffset createdAt)
    {
        Id = id;
        PublishedArticleId = publishedArticleId;
        UserId = userId;
        Body = body;
        Status = CommentStatus.Pending;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the comment identifier.
    /// </summary>
    public CommentId Id { get; private set; }

    /// <summary>
    /// Gets the commented published article identifier.
    /// </summary>
    public PublishedArticleId PublishedArticleId { get; private set; }

    /// <summary>
    /// Gets the author user identifier.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the plain-text comment body.
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    /// Gets the comment moderation status.
    /// </summary>
    public CommentStatus Status { get; private set; }

    /// <summary>
    /// Gets when the comment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets when the comment was moderated.
    /// </summary>
    public DateTimeOffset? ModeratedAt { get; private set; }

    /// <summary>
    /// Gets the administrator who moderated the comment.
    /// </summary>
    public UserId? ModeratedByUserId { get; private set; }

    /// <summary>
    /// Creates a pending comment.
    /// </summary>
    public static Comment Create(
        CommentId id,
        PublishedArticleId publishedArticleId,
        UserId userId,
        string body,
        DateTimeOffset createdAt)
    {
        return new Comment(id, publishedArticleId, userId, body, createdAt);
    }
}
