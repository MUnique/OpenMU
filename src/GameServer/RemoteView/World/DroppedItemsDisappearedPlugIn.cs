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
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, 5 + (2 * count)))
            {
                var data = writer.Span;
                data[3] = 0x21;
                data[4] = (byte)count;
                int i = 0;
                foreach (var dropId in disappearedItemIds)
                {
                    data[5 + (i * 2)] = (byte)((dropId >> 8) & 0xFF);
                    data[6 + (i * 2)] = (byte)(dropId & 0xFF);
                    i++;
                }

                writer.Commit();
            }
        }
    }
}