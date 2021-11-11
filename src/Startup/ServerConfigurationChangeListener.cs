// <copyright file="ServerConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

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
    private readonly IList<IManageableServer> _servers;
    private readonly ConnectServerFactory _connectServerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerConfigurationChangeListener" /> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <param name="connectServerFactory">The connect server factory.</param>
    public ServerConfigurationChangeListener(IList<IManageableServer> servers, ConnectServerFactory connectServerFactory)
    {
        this._servers = servers;
        this._connectServerFactory = connectServerFactory;
    }

    /// <summary>
    /// Adds a connection server after a new configuration has been added.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void ConnectionServerAdded(ConnectServerDefinition configuration)
    {
        var connectServer = this._connectServerFactory.CreateConnectServer(configuration, new ClientVersion(configuration.Client!.Season, configuration.Client.Episode, configuration.Client.Language), configuration.GetId());
        this._servers.Add(connectServer);
    }

    /// <summary>
    /// Restarts the connection server when its configuration has been changed and it's currently running.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void ConnectionServerChanged(ConnectServerDefinition configuration)
    {
        var server = this._servers.FirstOrDefault(s => s.Id == configuration.ServerId);
        if (server is null)
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