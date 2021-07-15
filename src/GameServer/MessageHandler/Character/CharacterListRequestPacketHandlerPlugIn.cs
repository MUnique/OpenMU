// <copyright file="CharacterListRequestPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for character list request packets (0xF3, 0x00 identifier).
    /// </summary>
    [PlugIn("Character - Request List", "Packet handler for character list request packets (0xF3, 0x00 identifier).")]
    [Guid("EBB70865-EF6B-4178-A84A-F94015900812")]
    [BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
    internal class CharacterListRequestPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly RequestCharacterListAction requestCharacterListAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => 0;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.requestCharacterListAction.RequestCharacterList(player);
        }
    }
}