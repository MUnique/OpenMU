// <copyright file="AdminPanel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using Nancy.Bootstrapper;
    using Nancy.Owin;

    /// <summary>
    /// The admin panel host class which provides the web server over OWIN.
    /// </summary>
    public sealed class AdminPanel : IDisposable
    {
        /// <summary>
        /// The OWIN web application.
        /// </summary>
        private IDisposable webApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPanel"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="servers">All manageable servers, including game servers, connect servers etc.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public AdminPanel(ushort port, IList<IManageableServer> servers, IPersistenceContextProvider persistenceContextProvider)
        {
            Startup.Bootstrapper = new MyBootstrapper(servers, persistenceContextProvider);
            WorldObserverHub.Servers = servers;
            ServerListHub.Servers = servers;

            // you might need to allow it first with netsh:
            // netsh http add urlacl http://+:1234/ user=[Username]
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel(options =>
                {
                    // Default port
                    options.ListenAnyIP(port);
                })
                .UseStartup<Startup>()
                .Build();

            host.Start();
            SignalRGameServerStateObserver.Services = host.Services;
            SignalRMapStateObserver.Services = host.Services;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.webApp != null)
            {
                this.webApp.Dispose();
                this.webApp = null;
            }
        }

        /// <summary>
        /// The startup class for the OWIN web app.
        /// </summary>
        public class Startup
        {
            /// <summary>
            /// Gets or sets the nancy bootstrapper.
            /// </summary>
            public static INancyBootstrapper Bootstrapper { get; set; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSignalR();
            }

            /// <summary>
            /// Configures the specified web application.
            /// </summary>
            /// <param name="app">The application builder.</param>
            /// <param name="env">The environment.</param>
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseSignalR(opt =>
                {
                    opt.MapHub<ServerListHub>("/admin/signalr/hubs/serverListHub");
                    opt.MapHub<WorldObserverHub>("/admin/signalr/hubs/worldObserverHub");
                    opt.MapHub<SystemHub>("/admin/signalr/hubs/systemHub");
                });
                app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = Bootstrapper));
            }
        }
    }
}
