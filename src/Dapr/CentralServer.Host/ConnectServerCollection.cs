// <copyright file="ConnectServerCollection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using System.Collections;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx.Synchronous;
using ConnectServer = MUnique.OpenMU.ConnectServer.ConnectServer;

/// <summary>
/// The connect servers of the central server, one for each <see cref="ConnectServerDefinition"/>,
/// i.e. one for each supported client version.
/// </summary>
public sealed class ConnectServerCollection : IReadOnlyList<ConnectServer>
{
    private readonly IReadOnlyList<ConnectServer> _connectServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerCollection"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConnectServerCollection(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
    {
        using var context = persistenceContextProvider.CreateNewConfigurationContext();
        var definitions = context.GetAsync<ConnectServerDefinition>().AsTask().WaitAndUnwrapException().ToList();
        if (definitions.Count == 0)
        {
            // The database is not initialized yet. The container doesn't keep the instance when it throws,
            // so it's created again when it's requested again.
            throw new InvalidOperationException("No connect server definitions found. The database is probably not initialized yet.");
        }

        this._connectServers = definitions
            .OrderBy(definition => definition.ClientListenerPort)
            .Select(definition => new ConnectServer(definition, loggerFactory))
            .ToList();
    }

    /// <inheritdoc />
    public int Count => this._connectServers.Count;

    /// <inheritdoc />
    public ConnectServer this[int index] => this._connectServers[index];

    /// <summary>
    /// Determines whether the end point is meant for the clients of the specified connect server.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="endPoint">The end point of a game server.</param>
    /// <returns><c>true</c>, if the end point is meant for the clients of the connect server; otherwise, <c>false</c>.</returns>
    public static bool IsFor(ConnectServer connectServer, GameServerEndPointInfo endPoint)
    {
        return connectServer.Settings is ConnectServerDefinition { Client: { } client }
               && client.Season == endPoint.Season
               && client.Episode == endPoint.Episode
               && (byte)client.Language == endPoint.Language;
    }

    /// <inheritdoc />
    public IEnumerator<ConnectServer> GetEnumerator() => this._connectServers.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
