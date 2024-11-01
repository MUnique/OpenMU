// <copyright file="CharacterSelectPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for character select packets (0xF3, 0x03 identifier).
/// </summary>
[PlugIn("Character - Select", "Packet handler for character select packets (0xF3, 0x03 identifier).")]
[Guid("82638A51-6C8E-46DF-9B4D-BF976D49A4A6")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterSelectPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly SelectCharacterAction _characterSelectAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 3;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        SelectCharacter message = packet;
        await this._characterSelectAction.SelectCharacterAsync(player, message.Name).ConfigureAwait(false);
    }
}