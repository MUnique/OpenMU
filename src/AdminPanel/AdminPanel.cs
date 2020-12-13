// <copyright file="AdminPanel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using SixLabors.ImageSharp;
    using SixLabors.Memory;

    /// <summary>
    /// The admin panel host class which provides a web server over ASP.NET Core Kestrel.
    /// </summary>
    public sealed class AdminPanel : IHostedService, IDisposable
    {
        private readonly IList<IManageableServer> servers;
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly IServerConfigurationChangeListener changeListener;
        private readonly ILoggerFactory loggerFactory;
        private readonly AdminPanelSettings settings;
        private readonly ILogger<AdminPanel> logger;
        private IHost? host;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPanel" /> class.
        /// </summary>
        /// <param name="servers">All manageable servers, including game servers, connect servers etc.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="changeListener">The change listener.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="settings">The admin panel settings.</param>
        public AdminPanel(IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, IServerConfigurationChangeListener changeListener, ILoggerFactory loggerFactory, AdminPanelSettings settings)
        {
            this.servers = servers;
            this.persistenceContextProvider = persistenceContextProvider;
            this.changeListener = changeListener;
            this.loggerFactory = loggerFactory;
            this.settings = settings;
            this.logger = this.loggerFactory.CreateLogger<AdminPanel>();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"Start initializing admin panel for port {this.settings.Port}.");
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();

            // you might need to allow it first with netsh:
            // netsh http add urlacl http://+:1234/ user=[Username]
            this.host = Host.CreateDefaultBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton(this.servers);
                    serviceCollection.AddSingleton(this.persistenceContextProvider);
                    serviceCollection.AddSingleton(this.changeListener);
                    serviceCollection.AddSingleton(this.loggerFactory);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseStaticWebAssets();

                    // For testing purposes, we use http. Later we need to switch to https.
                    // The self-signed certificate would otherwise cause a lot of warnings in the browser.
                    webBuilder.UseUrls($"http://*:{this.settings.Port}");
                })
                .Build();
            await this.host!.StartAsync(cancellationToken);
            this.logger.LogInformation($"Admin panel initialized, port {this.settings.Port}.");
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Stopping admin panel");
            return this.host?.StopAsync(cancellationToken) ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.host?.Dispose();
        }
    }
}
