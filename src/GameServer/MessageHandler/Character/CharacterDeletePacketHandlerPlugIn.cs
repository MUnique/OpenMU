// <copyright file="CharacterDeletePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for character delete packets (0xF3, 0x02 identifier).
/// </summary>
[PlugIn("Character - Delete", "Packet handler for character delete packets (0xF3, 0x02 identifier).")]
[Guid("5391D003-E244-42E8-AF30-33CB0654B66A")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterDeletePacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DeleteCharacterAction _deleteCharacterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 2;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        DeleteCharacter message = packet;
        await this._deleteCharacterAction.DeleteCharacterAsync(player, message.Name, message.SecurityCode).ConfigureAwait(false);
    }
}