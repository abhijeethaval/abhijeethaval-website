namespace AbhijeetSite.Api.Infrastructure.Persistence;

internal static partial class PersistenceLog
{
    private const int MigrationFailureEventId = 1101;

    [LoggerMessage(
        EventId = MigrationFailureEventId,
        EventName = "Persistence.DatabaseMigrationFailed",
        Level = LogLevel.Critical,
        Message = "Database migration failed for {DatabaseName} with {ExceptionType}.")]
    internal static partial void DatabaseMigrationFailed(
        this ILogger logger,
        string databaseName,
        string exceptionType);
}
