using System.Diagnostics;
using OpenTelemetry;

namespace Microsoft.Extensions.Hosting;

internal sealed class TelemetryBatchExportProcessor : BatchExportProcessor<Activity>
{
    internal TelemetryBatchExportProcessor(BaseExporter<Activity> exporter)
        : base(exporter)
    {
    }

    public override void OnEnd(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        if (TelemetryDataProcessor.ApplyExportPolicy(activity))
        {
            base.OnEnd(activity);
        }
    }
}
