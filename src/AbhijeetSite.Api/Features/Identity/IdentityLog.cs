namespace AbhijeetSite.Api.Features.Identity;

internal static partial class IdentityLog
{
    private const int PersistenceFailureEventId = 2101;
    private const int MissingLocalUserEventId = 2102;

    [LoggerMessage(
        EventId = PersistenceFailureEventId,
        EventName = "Identity.ExternalLoginPersistenceFailed",
        Level = LogLevel.Error,
        Message = "Persisting external login for user {UserId} failed with {ExceptionType}.")]
    internal static partial void ExternalLoginPersistenceFailed(
        this ILogger logger,
        Guid userId,
        string exceptionType);

    [LoggerMessage(
        EventId = MissingLocalUserEventId,
        EventName = "Identity.ExternalLoginMissingLocalUser",
        Level = LogLevel.Error,
        Message = "External login references missing local user {UserId}.")]
    internal static partial void ExternalLoginMissingLocalUser(this ILogger logger, Guid userId);
}
