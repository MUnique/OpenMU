// <copyright file="ApiHost.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi;

using System.Threading;
using apache.log4net.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Hosts the public API server.
/// </summary>
public class ApiHost : IHostedService, IDisposable
{
    private readonly ICollection<IGameServer> _gameServers;
    private readonly IEnumerable<IConnectServer> _connectServers;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ApiHost> _logger;
    private IHost? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiHost" /> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="connectServers">The connect servers.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>The async task.</returns>
    public ApiHost(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, ILoggerFactory loggerFactory)
    {
        this._gameServers = gameServers;
        this._connectServers = connectServers;
        this._loggerFactory = loggerFactory;
        this._logger = this._loggerFactory.CreateLogger<ApiHost>();
    }

    /// <summary>
    /// Creates the Host instance.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="connectServers">The connect servers.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="loggingConfigurationPath">The logging configuration path.</param>
    /// <returns>
    /// The created host.
    /// </returns>
    public static IHost BuildHost(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, ILoggerFactory loggerFactory, string? loggingConfigurationPath)
    {
        var builder = Host.CreateDefaultBuilder();
        if (!string.IsNullOrEmpty(loggingConfigurationPath))
        {
            builder.ConfigureLogging(configureLogging =>
            {
                configureLogging.ClearProviders();
                var settings = new Log4NetSettings { ConfigFile = loggingConfigurationPath, Watch = true };

                configureLogging.AddLog4Net(settings);
            });
        }

        return builder
            .ConfigureServices(s =>
            {
                s.AddSingleton(loggerFactory);
                s.AddSingleton(gameServers);
                s.AddSingleton(connectServers);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseUrls("http://*:80", "https://*:443");
            })
            .Build();
    }

    /// <summary>
    /// Creates and runs a host instance.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="connectServers">The connect servers.</param>
    /// <param name="loggingConfigurationPath">The path to the logging configuration.</param>
    public static Task RunAsync(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, string? loggingConfigurationPath)
    {
        return BuildHost(gameServers, connectServers, new NullLoggerFactory(), loggingConfigurationPath).StartAsync();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            this._host = BuildHost(this._gameServers, this._connectServers, this._loggerFactory, null);
            this._logger.LogInformation("Starting API...");
            await this._host.StartAsync(cancellationToken);
            this._logger.LogInformation("Started API");
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't start ApiHost.");
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (this._host is { } host)
        {
            this._logger.LogInformation("Stopping API...");
            return host.StopAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._host?.Dispose();
    }
}