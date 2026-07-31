using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace AbhijeetSite.Api.Infrastructure.Observability;

internal sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private const int UnhandledExceptionEventId = 1201;

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        LogUnhandledException(traceId, exception.GetType().Name);
        RecordException(exception);
        IResult problem = Results.Problem(
            title: "request.unhandled",
            detail: "The request failed unexpectedly. Retry or use the trace ID for support.",
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?> { ["traceId"] = traceId });
        await problem.ExecuteAsync(httpContext);
        return true;
    }

    private static void RecordException(Exception exception)
    {
        Activity? activity = Activity.Current;
        activity?.AddEvent(new ActivityEvent(
            "exception",
            tags: new ActivityTagsCollection
            {
                ["exception.type"] = exception.GetType().FullName
            }));
        activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
    }

    [LoggerMessage(
        EventId = UnhandledExceptionEventId,
        EventName = "Api.UnhandledException",
        Level = LogLevel.Error,
        Message = "Unhandled request failure for trace {TraceId} with {ExceptionType}.")]
    private partial void LogUnhandledException(string traceId, string exceptionType);
}
