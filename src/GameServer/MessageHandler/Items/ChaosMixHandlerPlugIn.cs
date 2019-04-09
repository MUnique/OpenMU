// <copyright file="ChaosMixHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for chaos mix packets.
    /// </summary>
    [PlugIn("ChaosMixHandlerPlugIn", "Handler for chaos mix packets.")]
    [Guid("0693e102-0adc-41e4-b0d4-ce22687b6dbb")]
    internal class ChaosMixHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ItemCraftAction mixAction = new ItemCraftAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.ChaosMachineMix;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 4)
            {
                return;
            }

            if (packet[2] != 0x86)
            {
                return;
            }

            byte mixId = packet[2];
            this.mixAction.MixItems(player, mixId);
        }
    }
}
