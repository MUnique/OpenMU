// <copyright file="CharacterClientReadyPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Sub packet handler for 'client ready' packets which are sent by the client after map changes.
    /// </summary>
    [PlugIn("Character - Client ready", "Packet handler for 'client ready' packets (0xF3, 0x12 identifier) which are sent after map changes.")]
    [Guid("8FB0AD6B-B3A6-4BF7-865B-EB4DF3C2A52F")]
    [BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterClientReadyPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 0x12;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            player.ClientReadyAfterMapChange();
        }
    }
}