namespace AbhijeetSite.Api.Features.Articles.Admin;

/// <summary>
/// Whether an article draft has a current public article row.
/// </summary>
public enum ArticleDraftPublicationState
{
    /// <summary>
    /// Draft has not been published.
    /// </summary>
    Unpublished,

    /// <summary>
    /// Draft has a public article row.
    /// </summary>
    Published
}
