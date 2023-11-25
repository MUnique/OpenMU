// <copyright file="DaprService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Helper class to create an <see cref="WebApplicationBuilder"/> which predefined common
/// services for all of our service applications.
/// </summary>
public static class DaprService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebApplicationBuilder" /> class with preconfigured defaults.
    /// </summary>
    /// <param name="serviceName">Name of the service, used for the OpenAPI and OpenTelemetry tracing.</param>
    /// <param name="args">Command line arguments.</param>
    /// <returns>
    /// The <see cref="WebApplicationBuilder" />.
    /// </returns>
    public static WebApplicationBuilder CreateBuilder(string serviceName, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls($"http://*:8080");

        var services = builder.Services;
        services.AddControllers();
        services.AddDaprClient();

        services.AddOpenTelemetry()
            .WithTracing(t =>
            {
                t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddZipkinExporter(o => o.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"));
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = serviceName, Version = "v1" });
        });

        services.AddSingleton<IDatabaseConnectionSettingProvider, SecretStoreDatabaseConnectionSettingsProvider>();

        // Logging:
        builder.UseLoki(serviceName);
        builder.Logging.AddOpenTelemetry(options => options.AddOtlpExporter());

        return builder;
    }
}