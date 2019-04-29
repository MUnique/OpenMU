// <copyright file="IConnectServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System.Net;

    /// <summary>
    /// The interface for a connect server.
    /// </summary>
    public interface IConnectServer : IManageableServer, IGameServerStateObserver
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        IConnectServerSettings Settings { get; }

    }

    /// <summary>
    /// An interface for an object which observes the state of game servers.
    /// </summary>
    public interface IGameServerStateObserver
    {
        /// <summary>
        /// Registers the game server, so that it can be accessed through the connect server.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        /// <param name="publicEndPoint">The public end point.</param>
        void RegisterGameServer(IGameServerInfo gameServer, IPEndPoint publicEndPoint);

        /// <summary>
        /// Unregisters the game server.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        void UnregisterGameServer(IGameServerInfo gameServer);
    }
}
