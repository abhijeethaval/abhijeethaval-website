namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Strongly typed identity for a published article revision.
/// </summary>
/// <param name="Value">Underlying UUID value.</param>
public readonly record struct PublishedArticleId(Guid Value)
{
    /// <summary>
    /// Creates a new published article identifier.
    /// </summary>
    public static PublishedArticleId New()
    {
        return new PublishedArticleId(Guid.CreateVersion7());
    }
}
