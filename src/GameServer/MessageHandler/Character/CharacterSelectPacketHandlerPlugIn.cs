// <copyright file="CharacterSelectPacketHandlerPlugIn.cs" company="MUnique">
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
    /// Packet handler for character select packets (0xF3, 0x03 identifier).
    /// </summary>
    [PlugIn("Character - Select", "Packet handler for character select packets (0xF3, 0x03 identifier).")]
    [Guid("82638A51-6C8E-46DF-9B4D-BF976D49A4A6")]
    [BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterSelectPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly SelectCharacterAction characterSelectAction = new SelectCharacterAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 3;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            string characterName = packet.ExtractString(4, 10, Encoding.UTF8);
            this.characterSelectAction.SelectCharacter(player, characterName);
        }
    }
}