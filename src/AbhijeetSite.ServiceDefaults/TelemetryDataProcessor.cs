using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Enforces the production telemetry privacy and noise contract before export.
/// </summary>
public sealed class TelemetryDataProcessor : BaseProcessor<Activity>
{
    private const string AliveRoute = "/alive";
    private const string HealthRoute = "/health";

    private static readonly string[] ProhibitedAttributes =
    [
        "db.query.text",
        "db.statement",
        "http.request.header.authorization",
        "http.target",
        "http.url",
        "url.full",
        "url.path",
        "url.query"
    ];

    /// <inheritdoc />
    public override void OnEnd(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        ApplyExportPolicy(activity);
    }

    internal static bool ApplyExportPolicy(Activity activity)
    {
        RemoveProhibitedAttributes(activity);
        return !IsSuccessfulHealthRequest(activity);
    }

    private static void RemoveProhibitedAttributes(Activity activity)
    {
        foreach (string attribute in ProhibitedAttributes)
        {
            activity.SetTag(attribute, null);
        }
    }

    private static bool IsSuccessfulHealthRequest(Activity activity)
    {
        string? route = activity.GetTagItem("http.route") as string;
        bool isHealthRoute = route is HealthRoute or AliveRoute;
        return isHealthRoute && GetStatusCode(activity) < StatusCodes.Status500InternalServerError;
    }

    private static int GetStatusCode(Activity activity)
    {
        object? value = activity.GetTagItem("http.response.status_code");
        return value switch
        {
            int statusCode => statusCode,
            long statusCode => checked((int)statusCode),
            string statusCode when int.TryParse(statusCode, out int parsed) => parsed,
            _ => StatusCodes.Status200OK
        };
    }
}
