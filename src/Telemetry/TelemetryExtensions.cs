// <copyright file="TelemetryExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Telemetry;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
/// Extensions to export logs, metrics and traces with OpenTelemetry.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Determines whether an OTLP endpoint is configured, to which logs, metrics and traces are exported.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><c>true</c>, if the standard <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is configured.</returns>
    public static bool IsOtlpExportConfigured(this IConfiguration configuration)
    {
        return !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
    }

    /// <summary>
    /// Configures logging, tracing and the export of all telemetry signals over OTLP.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>The configured web application builder.</returns>
    /// <remarks>
    /// The export is only enabled when an OTLP endpoint is configured, e.g. by the standard
    /// environment variable <c>OTEL_EXPORTER_OTLP_ENDPOINT</c>. All other standard OTEL_* environment
    /// variables apply as well, e.g. <c>OTEL_EXPORTER_OTLP_PROTOCOL</c> or <c>OTEL_RESOURCE_ATTRIBUTES</c>.
    /// Log levels are configured as usual through the <c>Logging</c> configuration section,
    /// e.g. by the environment variable <c>Logging__LogLevel__Default</c>.
    /// </remarks>
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder, string serviceName)
    {
        // Defaults with the lowest precedence, so that every other configuration source can overwrite them.
        // We don't want all of the ASP.NET logging, because that really keeps the log storage and the console pretty busy.
        builder.Configuration.Sources.Insert(0, new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = nameof(LogLevel.Debug),
                ["Logging:LogLevel:Microsoft"] = nameof(LogLevel.Warning),
                ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = nameof(LogLevel.Information),
                ["Logging:Console:LogLevel:Default"] = nameof(LogLevel.Information),
                ["Logging:Console:LogLevel:Microsoft"] = nameof(LogLevel.Warning),
                ["Logging:Console:LogLevel:Microsoft.Hosting.Lifetime"] = nameof(LogLevel.Information),
            },
        });

        var openTelemetry = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithLogging(
                configureBuilder: null,
                configureOptions: options =>
                {
                    // The scopes contain the account, character and connection of a log entry.
                    options.IncludeScopes = true;
                    options.IncludeFormattedMessage = true;
                })
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddSource("System.Net.Http") // the outgoing calls to the dapr sidecar
                .AddSource("Npgsql"));

        if (builder.Configuration.IsOtlpExportConfigured())
        {
            openTelemetry.UseOtlpExporter();
        }

        return builder;
    }

    /// <summary>
    /// Adds the open telemetry metrics.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="registry">The registry.</param>
    /// <returns>The configured web application builder.</returns>
    public static WebApplicationBuilder AddOpenTelemetryMetrics(this WebApplicationBuilder builder, MetricsRegistry registry)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter(registry.Meters.ToArray())
                .AddAspNetCoreInstrumentation()
                .AddMeter("System.Runtime", "System.Net.Http", "Npgsql"));

        return builder;
    }
}
