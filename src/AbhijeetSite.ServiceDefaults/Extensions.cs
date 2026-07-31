using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Adds shared service discovery, resilience, health checks, and OpenTelemetry defaults.
/// </summary>
public static class Extensions
{
    private const string LoggingEndpointName = "CONTAINERAPP_OTEL_LOGGING_GRPC_ENDPOINT";
    private const string MetricsEndpointName = "CONTAINERAPP_OTEL_METRIC_GRPC_ENDPOINT";
    private const string OtlpEndpointName = "OTEL_EXPORTER_OTLP_ENDPOINT";
    private const string OtlpProtocolName = "OTEL_EXPORTER_OTLP_PROTOCOL";
    private const string TracingEndpointName = "CONTAINERAPP_OTEL_TRACING_GRPC_ENDPOINT";

    /// <summary>
    /// Adds the service defaults required by an application.
    /// </summary>
    public static TBuilder AddServiceDefaults<TBuilder>(
        this TBuilder builder,
        string serviceName,
        params string[] activitySourceNames)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        builder.ConfigureOpenTelemetry(serviceName, activitySourceNames);
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });
        return builder;
    }

    /// <summary>
    /// Configures vendor-neutral telemetry collection and OTLP export.
    /// </summary>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(
        this TBuilder builder,
        string serviceName,
        params string[] activitySourceNames)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(activitySourceNames);
        Sdk.SetDefaultTextMapPropagator(new TraceContextPropagator());
        ConfigureLogging(builder);
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => ConfigureResource(resource, serviceName))
            .WithMetrics(metrics => ConfigureMetrics(builder, metrics))
            .WithTracing(tracing => ConfigureTracing(builder, tracing, activitySourceNames));
        return builder;
    }

    /// <summary>
    /// Adds the default liveness health check.
    /// </summary>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        return builder;
    }

    /// <summary>
    /// Maps the default health endpoints.
    /// </summary>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("live")
        });

        return app;
    }

    private static void ConfigureLogging<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            AddLoggingExporter(builder, logging);
        });
    }

    private static void ConfigureResource(ResourceBuilder resource, string serviceName)
    {
        string? serviceVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
        resource.AddService(serviceName, serviceVersion: serviceVersion);
    }

    private static void ConfigureMetrics<TBuilder>(TBuilder builder, MeterProviderBuilder metrics)
        where TBuilder : IHostApplicationBuilder
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        if (!HasManagedAgent(builder) && HasEndpoint(builder, OtlpEndpointName))
        {
            metrics.AddOtlpExporter();
        }
    }

    private static void ConfigureTracing<TBuilder>(
        TBuilder builder,
        TracerProviderBuilder tracing,
        string[] activitySourceNames)
        where TBuilder : IHostApplicationBuilder
    {
        tracing.SetSampler(new ParentBasedSampler(new AlwaysOnSampler()))
            .AddSource(activitySourceNames)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddNpgsql();
        AddTracingExporter(builder, tracing);
    }

    private static void AddLoggingExporter<TBuilder>(
        TBuilder builder,
        OpenTelemetryLoggerOptions logging)
        where TBuilder : IHostApplicationBuilder
    {
        if (HasEndpoint(builder, OtlpEndpointName))
        {
            logging.AddOtlpExporter();
        }
        else
        {
            Uri? endpoint = GetEndpoint(builder, LoggingEndpointName);
            if (endpoint is not null)
            {
                logging.AddOtlpExporter(options => options.Endpoint = endpoint);
            }
        }
    }

    private static void AddTracingExporter<TBuilder>(
        TBuilder builder,
        TracerProviderBuilder tracing)
        where TBuilder : IHostApplicationBuilder
    {
        Uri? endpoint = GetEndpoint(builder, OtlpEndpointName)
            ?? GetEndpoint(builder, TracingEndpointName);
        if (endpoint is not null)
        {
            OtlpExporterOptions options = new()
            {
                Endpoint = endpoint,
                Protocol = GetOtlpProtocol(builder)
            };
            OtlpTraceExporter exporter = new(options);
            tracing.AddProcessor(new TelemetryBatchExportProcessor(exporter));
        }
    }

    private static bool HasManagedAgent<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        return HasEndpoint(builder, TracingEndpointName)
            || HasEndpoint(builder, LoggingEndpointName)
            || HasEndpoint(builder, MetricsEndpointName);
    }

    private static OtlpExportProtocol GetOtlpProtocol<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        string? protocol = builder.Configuration[OtlpProtocolName]?.Trim();
        return protocol?.ToLowerInvariant() switch
        {
            null or "" or "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            "grpc" => OtlpExportProtocol.Grpc,
            _ => throw new InvalidOperationException(
                $"Unsupported {OtlpProtocolName} value '{protocol}'. Use 'grpc' or 'http/protobuf'.")
        };
    }

    private static bool HasEndpoint<TBuilder>(TBuilder builder, string configurationName)
        where TBuilder : IHostApplicationBuilder
    {
        return GetEndpoint(builder, configurationName) is not null;
    }

    private static Uri? GetEndpoint<TBuilder>(TBuilder builder, string configurationName)
        where TBuilder : IHostApplicationBuilder
    {
        string? value = builder.Configuration[configurationName]?.Trim();
        return Uri.TryCreate(value, UriKind.Absolute, out Uri? endpoint) ? endpoint : null;
    }
}
