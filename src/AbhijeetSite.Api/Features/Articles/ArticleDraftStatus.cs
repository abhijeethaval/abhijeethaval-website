namespace AbhijeetSite.Api.Features.Articles;

/// <summary>
/// Lifecycle state for an owner-authored article draft.
/// </summary>
public enum ArticleDraftStatus
{
    /// <summary>
    /// Draft is editable and not ready for publishing.
    /// </summary>
    Draft,

    /// <summary>
    /// Draft is ready for a later publish transition.
    /// </summary>
    ReadyToPublish,

    /// <summary>
    /// Draft is no longer active.
    /// </summary>
    Archived
}
