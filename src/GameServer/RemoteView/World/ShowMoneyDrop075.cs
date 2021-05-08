﻿// <copyright file="ShowMoneyDrop075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowDroppedItemsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowMoneyDrop 0.75", "The default implementation of the IShowMoneyDropPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f89308c3-5fe7-46e2-adfc-85a56ba23232")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class ShowMoneyDrop075 : IShowMoneyDropPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMoneyDrop075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowMoneyDrop075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowMoney(ushort itemId, bool isFreshDrop, uint amount, Point point)
        {
            this.player.Connection.SendMoneyDropped075(itemId, isFreshDrop, point.X, point.Y, amount);
        }
    }
}