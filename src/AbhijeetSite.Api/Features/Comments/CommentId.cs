namespace AbhijeetSite.Api.Features.Comments;

/// <summary>
/// Strongly typed identity for a user comment.
/// </summary>
/// <param name="Value">Underlying UUID value.</param>
public readonly record struct CommentId(Guid Value)
{
    /// <summary>
    /// Creates a new comment identifier.
    /// </summary>
    public static CommentId New()
    {
        return new CommentId(Guid.CreateVersion7());
    }
}
