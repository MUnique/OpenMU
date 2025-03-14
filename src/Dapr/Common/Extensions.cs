// <copyright file="Extensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx.Synchronous;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Prometheus;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;

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
        services.AddSingleton<IConfigurationChangeListener, ConfigurationChangeListener>();
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
            .AddSingleton(s => (IPersistenceContextProvider)s.GetService<IMigratableDatabaseContextProvider>()!)
            .AddSingleton(s => new Lazy<IPersistenceContextProvider>(s.GetRequiredService<IPersistenceContextProvider>));
    }

    /// <summary>
    /// Adds the plug in manager.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="plugInConfigurations">The plug in configurations.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddPlugInManager(this IServiceCollection services, ICollection<PlugInConfiguration> plugInConfigurations)
    {
        return services
            .AddSingleton(plugInConfigurations)
            .AddSingleton<PlugInManager>();
    }

    /// <summary>
    /// Tries to load the plug in configurations.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="plugInConfigurations">The list of plug in configurations, where the loaded configurations will be added.</param>
    public static async ValueTask TryLoadPlugInConfigurationsAsync(this IServiceProvider serviceProvider, List<PlugInConfiguration> plugInConfigurations)
    {
        if (serviceProvider.GetService<IMigratableDatabaseContextProvider>() is not { } persistenceContextProvider)
        {
            throw new Exception($"{nameof(IPersistenceContextProvider)} not registered.");
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            if (!await persistenceContextProvider.CanConnectToDatabaseAsync(cts.Token).ConfigureAwait(false)
                || !await persistenceContextProvider.DatabaseExistsAsync(cts.Token).ConfigureAwait(false))
            {
                return;
            }

            var configs = await persistenceContextProvider.CreateNewTypedContext(typeof(PlugInConfiguration), false).GetAsync<PlugInConfiguration>().ConfigureAwait(false);
            plugInConfigurations.AddRange(configs);
        }
        catch
        {
            // If we can't load it yet, because the database is not initialized, we just return
        }
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
        {
            if (s.GetService<IPersistenceContextProvider>() is not { } persistenceContextProvider)
            {
                throw new Exception($"{nameof(IPersistenceContextProvider)} not registered.");
            }

            var objects = persistenceContextProvider.CreateNewConfigurationContext().GetAsync<TActual>().AsTask().WaitAndUnwrapException();
            return (TTarget)objects.First(predicate ?? (_ => true))!;
        });
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
                propertiesAsLabels: includeLabels)
            .WriteTo
            .Console(LogEventLevel.Information)
            .Filter.ByExcluding(Matching.FromSource("Microsoft")) // We don't want all of the ASP.NET logging, because that really keeps loki and the console pretty busy
            .CreateLogger();

        SelfLog.Enable(Console.Error);

        builder.Host.ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders());
        builder.Host.UseSerilog(logger);
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
        builder.Services.AddOpenTelemetry()
            .WithMetrics(x =>
            {
                x.AddMeter(registry.Meters.ToArray());
                x.AddPrometheusExporter();
                x.AddOtlpExporter();
            });
        builder.Services.AddHealthChecks().ForwardToPrometheus();

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
        app.MapPrometheusScrapingEndpoint();

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
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

        return app;
    }

    /// <summary>
    /// Waits for the completion of outstanding database updates.
    /// </summary>
    /// <param name="app">The application.</param>
    public static async Task WaitForUpdatedDatabaseAsync(this WebApplication app)
    {
        await app.WaitForDatabaseConnectionInitializationAsync().ConfigureAwait(false);
        await app.Services.GetService<PersistenceContextProvider>()!
            .WaitForUpdatedDatabaseAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for database connection (settings) initialization.
    /// </summary>
    /// <param name="app">The application.</param>
    public static async Task WaitForDatabaseConnectionInitializationAsync(this WebApplication app)
    {
        await app.Services.GetService<IDatabaseConnectionSettingProvider>()!
            .InitializeAsync(default)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the ip resolver to the collection, depending on the command line arguments
    /// and the <see cref="SystemConfiguration"/> in the database.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The <paramref name="serviceCollection"/>.</returns>
    public static IServiceCollection AddIpResolver(this IServiceCollection serviceCollection, string[] args)
    {
        return serviceCollection.AddSingleton(serviceProvider =>
        {
            (IpResolverType IpResolver, string? IpResolverParameter)? settings = default;
            try
            {
                var persistenceContextProvider = serviceProvider.GetService<IPersistenceContextProvider>() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered.");
                using var context = persistenceContextProvider.CreateNewTypedContext(typeof(SystemConfiguration), false);

                // TODO: this may lead to a deadlock?
                var configuration = context.GetAsync<SystemConfiguration>().AsTask().WaitAndUnwrapException().FirstOrDefault();
                if (configuration is not null)
                {
                    settings = (configuration.IpResolver, configuration.IpResolverParameter);
                }
            }
            catch (Exception ex)
            {
                serviceProvider.GetService<ILogger<IIpAddressResolver>>()?.LogError(ex, "Unexpected error when trying to load the system configuration during ip resolver creation: {ex}", ex);
            }

            return IpAddressResolverFactory.CreateIpResolver(args, settings, serviceProvider.GetService<ILoggerFactory>()!);
        });
    }
}