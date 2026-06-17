namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Public visibility state for a published article.
/// </summary>
public enum PublishedArticleStatus
{
    /// <summary>
    /// Article is visible to public readers.
    /// </summary>
    Published,

    /// <summary>
    /// Article is not visible to public readers.
    /// </summary>
    Unpublished
}
