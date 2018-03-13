// <copyright file="IGameServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;

    /// <summary>
    /// Interface for a game server listener.
    /// </summary>
    /// <remarks>
    /// The idea is, that there can be different listeners or interfaces through which the player could connect.
    /// E.g. the gameserver could be available under several ports, not just one. Apart of that, connecting
    /// through different types of connections could be possible, e.g. coming from a browser by WebSockets or SignalR.
    /// </remarks>
    public interface IGameServerListener
    {
        /// <summary>
        /// Occurs when a player has connected to the game successfully.
        /// </summary>
        event EventHandler<PlayerConnectedEventArgs> PlayerConnected;

        /// <summary>
        /// Starts this listener.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this listener.
        /// </summary>
        void Stop();
    }
}