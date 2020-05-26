// <copyright file="ApiHost.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi
{
    using System.Collections.Generic;
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
    public class ApiHost
    {
        private IHost host;
        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="connectServers">The connect servers.</param>
        /// <param name="loggingConfigurationPath">The path to the logging configuration.</param>
        /// <returns>The async task.</returns>

        public ApiHost(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, string? loggingConfigurationPath)
        {
            host = buildHost(gameServers, connectServers, loggingConfigurationPath);
        }

        /// <summary>
        /// Start Server
        /// </summary>
        public void Start()
        {
            host.StartAsync();
        }

        /// <summary>
        /// Stop Server
        /// </summary>
        public void Shutdown()
        {
            host.StopAsync();
        }

        /// <summary>
        /// Create the Host instance
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="connectServers">The connect servers.</param>
        /// <param name="loggingConfigurationPath">The path to the logging configuration.</param>
        /// <returns></returns>
        public static IHost buildHost(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, string? loggingConfigurationPath)
        {
            var builder = Host.CreateDefaultBuilder();
            if (!string.IsNullOrEmpty(loggingConfigurationPath))
            {
                builder = builder.ConfigureLogging(configureLogging =>
                {
                    configureLogging.ClearProviders();
                    var settings = new Log4NetSettings { ConfigFile = loggingConfigurationPath, Watch = true };

                    configureLogging.AddLog4Net(settings);
                });
            }

            return builder
                .ConfigureServices(s =>
                {
                    s.Add(new ServiceDescriptor(typeof(ICollection<IGameServer>), gameServers));
                    s.Add(new ServiceDescriptor(typeof(IEnumerable<IConnectServer>), connectServers));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls("http://*:80", "https://*:443");
                })
                .Build();
        }

        public static Task RunAsync(ICollection<IGameServer> gameServers, IEnumerable<IConnectServer> connectServers, string? loggingConfigurationPath)
        {
            return buildHost(gameServers, connectServers, loggingConfigurationPath)
                .RunAsync();
        }
    }
}