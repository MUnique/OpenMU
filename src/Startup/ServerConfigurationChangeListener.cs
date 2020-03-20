// <copyright file="ServerConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AdminPanel;
    using MUnique.OpenMU.ConnectServer;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Implementation of a <see cref="IServerConfigurationChangeListener"/> which creates and updates server instances after
    /// configuration has been changed.
    /// </summary>
    internal class ServerConfigurationChangeListener : IServerConfigurationChangeListener
    {
        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfigurationChangeListener"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public ServerConfigurationChangeListener(IList<IManageableServer> servers)
        {
            this.servers = servers;
        }

        /// <summary>
        /// Adds a connection server after a new configuration has been added.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void ConnectionServerAdded(ConnectServerDefinition configuration)
        {
            var connectServer = ConnectServerFactory.CreateConnectServer(configuration, new ClientVersion(configuration.Client.Season, configuration.Client.Episode, configuration.Client.Language), configuration.GetId());
            this.servers.Add(connectServer);
        }

        /// <summary>
        /// Restarts the connection server when its configuration has been changed and it's currently running.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void ConnectionServerChanged(ConnectServerDefinition configuration)
        {
            var server = this.servers.FirstOrDefault(s => s.Id == configuration.ServerId);
            if (server == null)
            {
                return;
            }

            if (server.ServerState == ServerState.Started)
            {
                server.Shutdown();
                server.Start();
            }
        }
    }
}