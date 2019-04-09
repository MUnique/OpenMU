// <copyright file="CharacterAddMasterPointPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Sub packet handler for master skill point add requests.
    /// </summary>
    [PlugIn("Character - Add Master Skill Points", "Packet handler for character master skill point adding (0xF3, 0x52 identifier).")]
    [Guid("9F39C1FB-26F7-460F-A21B-C4DFEA234E66")]
    [BelongsToGroupAttribute(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterAddMasterPointPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly AddMasterPointAction addMasterPointAction = new AddMasterPointAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 0x52;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // LO HI
            // C1 08 F3 52 A6 01 00 00
            var skillId = NumberConversionExtensions.MakeWord(packet[4], packet[5]);
            this.addMasterPointAction.AddMasterPoint(player, skillId);
        }
    }
}