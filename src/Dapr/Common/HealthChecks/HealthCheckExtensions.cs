// <copyright file="HealthCheckExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common.HealthChecks;

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Extensions to add and map the health checks of a dapr service.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item><c>/health/live</c> is healthy as long as the process runs. It's meant for liveness probes,
///   which restart a process. It doesn't depend on e.g. the database, because restarting wouldn't fix that.</item>
///   <item><c>/health/ready</c> reports whether the process can do its work, e.g. if the database is installed.
///   It's meant for readiness probes, and the response contains the result of each check.</item>
/// </list>
/// </remarks>
public static class HealthCheckExtensions
{
    /// <summary>
    /// The tag of the health checks which are part of <c>/health/ready</c>.
    /// </summary>
    public const string ReadyTag = "ready";

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Adds the health checks which every dapr service has.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, to add more services.</returns>
    public static IServiceCollection AddDaprServiceHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ManageableServersHealthCheck>("servers", tags: [ReadyTag]);
        return services;
    }

    /// <summary>
    /// Adds the health check of the database, for services which can't work without it.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, to add more services.</returns>
    public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: [ReadyTag]);
        return services;
    }

    /// <summary>
    /// Maps the health check endpoints <c>/health/live</c> and <c>/health/ready</c>.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application, to configure it further.</returns>
    public static WebApplication MapDaprServiceHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadyTag),
            ResponseWriter = WriteResponseAsync,
        });
        return app;
    }

    private static Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    data = entry.Value.Data,
                }),
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(result, SerializerOptions));
    }
}
