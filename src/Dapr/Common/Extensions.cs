// <copyright file="Extensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration.Memory;
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
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
/// Common extensions for the building of daprized services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the <see cref="PersistenceContextProvider"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="publishConfigChanges">If set to <c>true</c>, configuration changes are published to other Dapr services.</param>
    /// <returns>The modified service collection.</returns>
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
    /// <param name="services">The service collection.</param>
    /// <param name="plugInConfigurations">The plug in configurations.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddPlugInManager(this IServiceCollection services, ICollection<PlugInConfiguration> plugInConfigurations)
    {
        return services
            .AddSingleton(plugInConfigurations)
            .AddSingleton<PlugInManager>()
            .AddTransient<ReferenceHandler, ByDataSourceReferenceHandler>(provider =>
            {
                var persistenceContextProvider = provider.GetService<IPersistenceContextProvider>();
                var dataSource = new GameConfigurationDataSource(
                    provider.GetService<ILogger<GameConfigurationDataSource>>()!,
                    persistenceContextProvider!);
                var configId = persistenceContextProvider!.CreateNewConfigurationContext().GetDefaultGameConfigurationIdAsync(default).AsTask().WaitAndUnwrapException();
                dataSource.GetOwnerAsync(configId!.Value).AsTask().WaitAndUnwrapException();
                var referenceHandler = new ByDataSourceReferenceHandler(dataSource);
                return referenceHandler;
            });
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
    /// <param name="services">The service collection.</param>
    /// <param name="predicate">The predicate to select actual object.</param>
    /// <returns>The modified service collection.</returns>
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
    /// <param name="services">The service collection.</param>
    /// <param name="predicate">The predicate to select the actual object.</param>
    /// <returns>The modified service collection.</returns>
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
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
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
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection PublishManageableServer<TServer>(this IServiceCollection services)
        where TServer : IManageableServer
    {
        services.AddSingleton<IManageableServer>(s => s.GetService<TServer>()!)
            .AddHostedService<ManagableServerStatePublisher>()
            .AddControllers().AddApplicationPart(typeof(ManageableServerController).Assembly);

        return services;
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

        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
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
    /// Waits for the database secrets to be loaded and then for the database to be up-to-date.
    /// This is intended to be called from <see cref="Microsoft.Extensions.Hosting.IHostedLifecycleService.StartedAsync"/>
    /// which is called after the web server has already started, breaking the circular dependency where:
    /// the Dapr sidecar needs the app HTTP API to be up before it initializes its secret store,
    /// but the app needs Dapr secrets to connect to the database.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task WaitForDatabaseInitializationAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var dbConnectionProvider = serviceProvider.GetRequiredService<IDatabaseConnectionSettingProvider>();
        if (dbConnectionProvider.Initialization is { } initTask)
        {
            await initTask.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        await serviceProvider.GetRequiredService<PersistenceContextProvider>()
            .WaitForUpdatedDatabaseAsync(cancellationToken)
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
                serviceProvider.GetService<ILogger<IIpAddressResolver>>()?.LogError(ex, "Unexpected error when trying to load the system configuration during ip resolver creation.");
            }

            return IpAddressResolverFactory.CreateIpResolver(args, settings, serviceProvider.GetService<ILoggerFactory>()!);
        });
    }
}