// <copyright file="Extensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Net;
using System.Reflection;

using Serilog.Debugging;
using Serilog.Filters;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.PlugIns;
using OpenTelemetry.Metrics;

/// <summary>
/// Common extensions for the building of daprized services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the <see cref="PersistenceContextProvider"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="publishConfigChanges">If set to <c>true</c>, configuration changes are published to other Dapr services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddPeristenceProvider(this IServiceCollection services, bool publishConfigChanges = false)
    {
        if (publishConfigChanges)
        {
            services.AddSingleton<IConfigurationChangePublisher, ConfigurationChangePublisher>();
        }
        else
        {
            services.AddSingleton(e => IConfigurationChangePublisher.None);
        }

        return services
            .AddSingleton<IMigratableDatabaseContextProvider, PersistenceContextProvider>()
            .AddSingleton(s => (PersistenceContextProvider)s.GetService<IMigratableDatabaseContextProvider>()!)
            .AddSingleton(s => (IPersistenceContextProvider)s.GetService<IMigratableDatabaseContextProvider>()!);
    }

    /// <summary>
    /// Adds the plug in manager.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddPlugInManager(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICollection<PlugInConfiguration>>(s => s.GetService<IPersistenceContextProvider>()?.CreateNewTypedContext<PlugInConfiguration>().Get<PlugInConfiguration>().ToList() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."))
            .AddSingleton<PlugInManager>();
    }

    /// <summary>
    /// Adds the <see cref="CustomIpResolver"/> with the specified address.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="address">The address.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddCustomIpResover(this IServiceCollection services, IPAddress address)
    {
        return services.AddSingleton<IIpAddressResolver>(s => new CustomIpResolver(address));
    }

    /// <summary>
    /// Adds a persistent object as singleton to the services.
    /// </summary>
    /// <typeparam name="T">The base type of the persistent object.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="predicate">The predicate to select actual object.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddPersistentSingleton<T>(this IServiceCollection services, Func<T, bool>? predicate = null)
        where T : class
    {
        return services.AddPersistentSingleton<T, T>(predicate);
    }

    /// <summary>
    /// Adds the persistent object as singleton to the services.
    /// </summary>
    /// <typeparam name="TTarget">The target, exposed type of the persistent object, usually an interface.</typeparam>
    /// <typeparam name="TActual">The actual base type of the persistent object.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddPersistentSingleton<TTarget, TActual>(this IServiceCollection services, Func<TActual, bool>? predicate = null)
        where TActual : class, TTarget
        where TTarget : class
    {
        return services.AddSingleton(s =>
            (TTarget)s.GetService<IPersistenceContextProvider>()?.CreateNewConfigurationContext().Get<TActual>().First(predicate ?? (_ => true))!);
    }

    /// <summary>
    /// Adds the <see cref="ManagableServerRegistry"/> to the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddManageableServerRegistry(this IServiceCollection services)
    {
        services.AddSingleton<ManagableServerRegistry>()
            .AddSingleton<IServerProvider>(s => s.GetService<ManagableServerRegistry>()!);
        return services;
    }

    /// <summary>
    /// Publishes the server to other daprized services by registering a <see cref="ManagableServerStatePublisher"/>.
    /// </summary>
    /// <typeparam name="TServer">The type of the server.</typeparam>
    /// <param name="services">The services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection PublishManageableServer<TServer>(this IServiceCollection services)
        where TServer : IManageableServer
    {
        services.AddSingleton<IManageableServer>(s => s.GetService<TServer>()!)
            .AddHostedService<ManagableServerStatePublisher>()
            .AddControllers().AddApplicationPart(typeof(ManageableServerController).Assembly);

        return services;
    }

    /// <summary>
    /// Configures the usage of logging to loki.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder UseLoki(this WebApplicationBuilder builder, string serviceName)
    {
        // We just want to transmit some static labels, as suggested in the best practice in the Loki documentation
        var includeLabels = new[] { "Account", "Character", "Connection", "ServiceName", "SourceContext" };

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.FromLogContext()
            .WriteTo
            .GrafanaLoki(
                uri: "http://loki:3100",
                filtrationLabels: includeLabels,
                filtrationMode: LokiLabelFiltrationMode.Include)
            .Filter.ByExcluding(Matching.FromSource("Microsoft")) // We don't want all of the ASP.NET logging, because that really keeps loki pretty busy
            .CreateLogger();

        SelfLog.Enable(Console.Error);

        builder.Host.ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders());
        builder.Host.UseSerilog(logger);
        builder.Host.ConfigureLogging((_, loggingBuilder) => loggingBuilder.AddConsole());
        return builder;
    }

    /// <summary>
    /// Adds the open telemetry metrics.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="registry">The registry.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder AddOpenTelemetryMetrics(this WebApplicationBuilder builder, MetricsRegistry registry)
    {
        var meters = registry.Meters.ToArray();
        if (meters.Length == 0)
        {
            return builder;
        }

        builder.Services.AddOpenTelemetryMetrics(opt =>
            opt
                .AddMeter(meters)
                .AddPrometheusExporter(p =>
                {
                    p.StartHttpListener = true;

                    // Workaround, see https://github.com/open-telemetry/opentelemetry-dotnet/issues/2840#issuecomment-1072977042
                    p.GetType()
                        ?.GetField("httpListenerPrefixes", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.SetValue(p, new[] { "http://*:9464" });
                }));

        return builder;
    }

    /// <summary>
    /// Builds and configures the web application.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="addBlazor">If set to <c>true</c>, it configures the application to provide a blazor server app.</param>
    /// <returns>The built and configured web application.</returns>
    public static WebApplication BuildAndConfigure(this WebApplicationBuilder builder, bool addBlazor = false)
    {
        var pathBase = Environment.GetEnvironmentVariable("PATH_BASE");
        var useReverseProxy = !string.IsNullOrWhiteSpace(pathBase);

        var app = builder.Build();

        if (useReverseProxy)
        {
            app.UsePathBase(pathBase!.TrimEnd('/'));
            app.UseForwardedHeaders();
        }

        app.ConfigureDaprService(addBlazor);

        return app;
    }

    /// <summary>
    /// Configures the web application as dapr service.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="addBlazor">If set to <c>true</c>, it configures the application to provide a blazor server app.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication ConfigureDaprService(this WebApplication app, bool addBlazor = false)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (addBlazor)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
        }

        app.UseCloudEvents();
        app.MapControllers();
        app.MapSubscribeHandler();

        ConnectionConfigurator.Initialize(app.Services.GetService<IDatabaseConnectionSettingProvider>() ?? new ConfigFileDatabaseConnectionStringProvider());

        return app;
    }

    /// <summary>
    /// Waits for the completion of outstanding database updates.
    /// </summary>
    /// <param name="app">The application.</param>
    public static Task WaitForUpdatedDatabase(this WebApplication app)
    {
        return app.Services.GetService<PersistenceContextProvider>()!.WaitForUpdatedDatabase();
    }
}