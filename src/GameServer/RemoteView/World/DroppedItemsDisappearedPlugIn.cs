// <copyright file="DroppedItemsDisappearedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IDroppedItemsDisappearedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("DroppedItemsDisappearedPlugIn", "The default implementation of the IDroppedItemsDisappearedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ecd14e95-33be-44f7-bb9b-1429a57a7a94")]
    public class DroppedItemsDisappearedPlugIn : IDroppedItemsDisappearedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedItemsDisappearedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public DroppedItemsDisappearedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void DroppedItemsDisappeared(IEnumerable<ushort> disappearedItemIds)
        {
            ////C2 00 07 21 01 00 0C
            int count = disappearedItemIds.Count();
            using var writer = this.player.Connection.StartSafeWrite(ItemDropRemoved.HeaderType, ItemDropRemoved.GetRequiredSize(count));
            var message = new ItemDropRemoved(writer.Span)
            {
                ItemCount = (byte)count,
            };
            int i = 0;
            foreach (var dropId in disappearedItemIds)
            {
                var drop = message[i];
                drop.Id = dropId;
                i++;
            }

            writer.Commit();
        }
    }
}