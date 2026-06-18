namespace AbhijeetSite.Api.Features.Comments;

/// <summary>
/// Moderation state for a user comment.
/// </summary>
public enum CommentStatus
{
    /// <summary>
    /// Comment is waiting for administrator review.
    /// </summary>
    Pending,

    /// <summary>
    /// Comment is approved for public display.
    /// </summary>
    Approved,

    /// <summary>
    /// Comment was rejected by moderation.
    /// </summary>
    Rejected,

    /// <summary>
    /// Comment was deleted.
    /// </summary>
    Deleted
}
