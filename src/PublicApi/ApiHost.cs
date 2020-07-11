// <copyright file="ApiHost.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Logging.Abstractions;

namespace MUnique.OpenMU.PublicApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using apache.log4net.Extensions.Logging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Hosts the public API server.
    /// </summary>
    public class ApiHost : IHostedService, IDisposable
    {
        private readonly ICollection<IGameServer> gameServers;
        private readonly IEnumerable<IConnectServer> connectServers;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<ApiHost> logger;
        private IHost? host;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHost" /> class.
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="connectServers">The connect servers.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>The async task.</returns>
        public ApiHost(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, ILoggerFactory loggerFactory)
        {
            this.gameServers = gameServers;
            this.connectServers = connectServers;
            this.loggerFactory = loggerFactory;
            this.logger = this.loggerFactory.CreateLogger<ApiHost>();
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
                    if (loggerFactory != null)
                    {
                        s.AddSingleton(loggerFactory);
                    }

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
            this.host = BuildHost(this.gameServers, this.connectServers, this.loggerFactory, null);
            this.logger.LogInformation("Starting API...");
            await this.host.StartAsync(cancellationToken);
            this.logger.LogInformation("Started API");
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Stopping API...");
            return this.host?.StopAsync(cancellationToken) ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.host?.Dispose();
        }
    }
}
