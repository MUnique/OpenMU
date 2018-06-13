// <copyright file="AdminPanel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Owin.Extensions;
    using Microsoft.Owin.Host.HttpListener;
    using Microsoft.Owin.Hosting;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using Nancy.Bootstrapper;
    using Owin;

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
            var startOptions = new StartOptions($"http://+:{port}")
            {
                ServerFactory = typeof(OwinHttpListener).Namespace,
                AppStartup = typeof(Startup).AssemblyQualifiedName
            };
            WorldObserverHub.Servers = servers;
            ServerListHub.Servers = servers;

            // you might need to allow it first with netsh:
            // netsh http add urlacl http://+:1234/ user=[Username]
            this.webApp = WebApp.Start<Startup>(startOptions);
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

            /// <summary>
            /// Configurations the specified web application.
            /// </summary>
            /// <param name="app">The application.</param>
            public void Configuration(IAppBuilder app)
            {
                app.MapSignalR();
                app.UseNancy(config => config.Bootstrapper = Bootstrapper);
                app.UseStageMarker(PipelineStage.MapHandler);
            }
        }
    }
}
