namespace AbhijeetSite.Api.Features.Articles;

internal static partial class ArticlesLog
{
    private const int CreateDraftFailureEventId = 3101;
    private const int UpdateDraftConflictEventId = 3102;
    private const int UpdateDraftFailureEventId = 3103;
    private const int PublishDraftConflictEventId = 3104;
    private const int PublishDraftFailureEventId = 3105;
    private const int GetDraftFailureEventId = 3106;
    private const int GetDraftsFailureEventId = 3107;
    private const int GetPublishedArticleFailureEventId = 3108;
    private const int GetPublishedArticlesFailureEventId = 3109;

    [LoggerMessage(
        EventId = CreateDraftFailureEventId,
        EventName = "Articles.CreateDraftPersistenceFailed",
        Level = LogLevel.Error,
        Message = "Creating article draft {ArticleDraftId} failed with {ExceptionType}.")]
    internal static partial void CreateDraftPersistenceFailed(
        this ILogger logger,
        Guid articleDraftId,
        string exceptionType);

    [LoggerMessage(
        EventId = UpdateDraftConflictEventId,
        EventName = "Articles.UpdateDraftVersionConflict",
        Level = LogLevel.Warning,
        Message = "Article draft {ArticleDraftId} version conflict.")]
    internal static partial void UpdateDraftVersionConflict(this ILogger logger, Guid articleDraftId);

    [LoggerMessage(
        EventId = UpdateDraftFailureEventId,
        EventName = "Articles.UpdateDraftPersistenceFailed",
        Level = LogLevel.Error,
        Message = "Saving article draft {ArticleDraftId} failed with {ExceptionType}.")]
    internal static partial void UpdateDraftPersistenceFailed(
        this ILogger logger,
        Guid articleDraftId,
        string exceptionType);

    [LoggerMessage(
        EventId = PublishDraftConflictEventId,
        EventName = "Articles.PublishDraftVersionConflict",
        Level = LogLevel.Warning,
        Message = "Article draft {ArticleDraftId} publish version conflict.")]
    internal static partial void PublishDraftVersionConflict(this ILogger logger, Guid articleDraftId);

    [LoggerMessage(
        EventId = PublishDraftFailureEventId,
        EventName = "Articles.PublishDraftPersistenceFailed",
        Level = LogLevel.Error,
        Message = "Publishing article draft {ArticleDraftId} failed with {ExceptionType}.")]
    internal static partial void PublishDraftPersistenceFailed(
        this ILogger logger,
        Guid articleDraftId,
        string exceptionType);

    [LoggerMessage(
        EventId = GetDraftFailureEventId,
        EventName = "Articles.GetDraftFailed",
        Level = LogLevel.Error,
        Message = "Loading article draft {ArticleDraftId} failed with {ExceptionType}.")]
    internal static partial void GetDraftFailed(
        this ILogger logger,
        Guid articleDraftId,
        string exceptionType);

    [LoggerMessage(
        EventId = GetDraftsFailureEventId,
        EventName = "Articles.GetDraftsFailed",
        Level = LogLevel.Error,
        Message = "Loading article drafts failed with {ExceptionType}.")]
    internal static partial void GetDraftsFailed(this ILogger logger, string exceptionType);

    [LoggerMessage(
        EventId = GetPublishedArticleFailureEventId,
        EventName = "Articles.GetPublishedArticleFailed",
        Level = LogLevel.Error,
        Message = "Loading published article failed with {ExceptionType}.")]
    internal static partial void GetPublishedArticleFailed(this ILogger logger, string exceptionType);

    [LoggerMessage(
        EventId = GetPublishedArticlesFailureEventId,
        EventName = "Articles.GetPublishedArticlesFailed",
        Level = LogLevel.Error,
        Message = "Loading published article summaries failed with {ExceptionType}.")]
    internal static partial void GetPublishedArticlesFailed(this ILogger logger, string exceptionType);
}
