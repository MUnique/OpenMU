// <copyright file="RequestedItemConsumptionFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameServer.RemoteView.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IRequestedItemConsumptionFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("RequestedItemConsumptionFailedPlugIn", "The default implementation of the IRequestedItemConsumptionFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("c3a03a1c-71c7-4581-a244-0b1b31497f05")]
    public class RequestedItemConsumptionFailedPlugIn : IRequestedItemConsumptionFailedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestedItemConsumptionFailedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public RequestedItemConsumptionFailedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        /// <remarks>The server sends the current health/shield to the client, with <see cref="UpdateType.Failed"/>.</remarks>
        public void RequestedItemConsumptionFailed()
        {
            var hp = (ushort)Math.Max(this.player.Attributes[Stats.CurrentHealth], 0f);
            var sd = (ushort)Math.Max(this.player.Attributes[Stats.CurrentShield], 0f);

            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x09))
            {
                var packet = writer.Span;
                packet[2] = 0x26;
                packet[3] = (byte)UpdateType.Failed;
                packet[4] = hp.GetHighByte();
                packet[5] = hp.GetLowByte();
                packet[7] = sd.GetHighByte();
                packet[8] = sd.GetLowByte();
                writer.Commit();
            }
        }
    }
}