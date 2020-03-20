// <copyright file="AdminPanel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using apache.log4net.Extensions.Logging;
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
    public sealed class AdminPanel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPanel" /> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="servers">All manageable servers, including game servers, connect servers etc.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="changeListener">The change listener.</param>
        /// <param name="loggingConfigurationPath">The logging configuration file path.</param>
        public AdminPanel(ushort port, IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider, IServerConfigurationChangeListener changeListener, string loggingConfigurationPath)
        {
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();

            // you might need to allow it first with netsh:
            // netsh http add urlacl http://+:1234/ user=[Username]
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(configureLogging =>
                {
                    configureLogging.ClearProviders();
                    var settings = new Log4NetSettings { ConfigFile = loggingConfigurationPath, Watch = true };

                    configureLogging.AddLog4Net(settings);
                })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton(servers);
                    serviceCollection.AddSingleton(persistenceContextProvider);
                    serviceCollection.AddSingleton(changeListener);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseStaticWebAssets();

                    // For testing purposes, we use http. Later we need to switch to https.
                    // The self-signed certificate would otherwise cause a lot of warnings in the browser.
                    webBuilder.UseUrls($"http://*:{port}");
                })
                .Build();
            host.Start();
        }
    }
}
