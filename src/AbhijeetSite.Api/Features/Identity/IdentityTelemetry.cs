using System.Diagnostics;
using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Identity;

internal static class IdentityTelemetry
{
    internal const string ExternalLoginUpsertActivityName = "identity.external_login_upsert";
    internal const string SourceName = "AbhijeetSite.Api.Identity";

    private const string ErrorTypeAttribute = "error.type";
    private const string ResultCategoryAttribute = "app.result.category";
    private const string UserIdAttribute = "app.user.id";

    private static readonly ActivitySource Source = new(SourceName);

    internal static Activity? Start(string activityName)
    {
        return Source.StartActivity(activityName, ActivityKind.Internal);
    }

    internal static Result<TValue> Complete<TValue>(Activity? activity, Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            activity?.SetTag(ResultCategoryAttribute, "success");
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }

        Error error = result.Error
            ?? throw new InvalidOperationException("A failed result must include an error.");
        activity?.SetTag(ResultCategoryAttribute, error.Category.ToString().ToLowerInvariant());
        activity?.SetTag(ErrorTypeAttribute, error.Code);
        if (error.Category == ErrorCategory.Infrastructure)
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.Code);
        }

        return result;
    }

    internal static void SetUserId(Activity? activity, UserId userId)
    {
        activity?.SetTag(UserIdAttribute, userId.Value.ToString());
    }

    internal static void RecordException(Exception exception)
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
}
