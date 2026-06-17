namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Strongly typed identity for an article draft.
/// </summary>
/// <param name="Value">Underlying UUID value.</param>
public readonly record struct ArticleDraftId(Guid Value)
{
    /// <summary>
    /// Creates a new article draft identifier.
    /// </summary>
    public static ArticleDraftId New()
    {
        return new ArticleDraftId(Guid.CreateVersion7());
    }
}
