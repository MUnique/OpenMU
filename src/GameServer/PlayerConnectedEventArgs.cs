// <copyright file="PlayerConnectedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Event arguments for the <see cref="IGameServerListener.PlayerConnected"/> event.
    /// </summary>
    public class PlayerConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConnectedEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PlayerConnectedEventArgs(Player player)
        {
            this.ConntectedPlayer = player;
        }

        /// <summary>
        /// Gets the conntected player.
        /// </summary>
        public Player ConntectedPlayer { get; }
    }
}