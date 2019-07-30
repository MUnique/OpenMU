// <copyright file="CharacterDeletePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for character delete packets (0xF3, 0x02 identifier).
    /// </summary>
    [PlugIn("Character - Delete", "Packet handler for character delete packets (0xF3, 0x02 identifier).")]
    [Guid("5391D003-E244-42E8-AF30-33CB0654B66A")]
    [BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterDeletePacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly DeleteCharacterAction deleteCharacterAction = new DeleteCharacterAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 2;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            string characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            string securityCode = packet.ExtractString(14, packet.Length - 14, Encoding.UTF8);
            this.deleteCharacterAction.DeleteCharacter(player, characterName, securityCode);
        }
    }
}