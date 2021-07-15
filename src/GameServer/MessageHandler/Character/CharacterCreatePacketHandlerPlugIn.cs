// <copyright file="CharacterCreatePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for character creation packets (0xF3, 0x01 identifier).
    /// </summary>
    [PlugIn("Character - Create", "Packet handler for character creation packets (0xF3, 0x01 identifier).")]
    [Guid("A26831DE-4D67-44CD-9434-12BDC4B07F47")]
    [BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterCreatePacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly CreateCharacterAction createCharacterAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 1;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            CreateCharacter message = packet;
            this.createCharacterAction.CreateCharacter(player, message.Name, (byte)message.Class);
        }
    }
}