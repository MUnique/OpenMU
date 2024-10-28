// <copyright file="CharacterFocusPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for character focus packets (0xF3, 0x15 identifier).
/// </summary>
[PlugIn("Character - Focus", "Packet handler for character focus packets (0xF3, 0x15 identifier).")]
[Guid("8687C77F-E26C-4510-AD85-E5F51305DE2A")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterFocusPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly FocusCharacterAction _focusCharacterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 0x15;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        FocusCharacter message = packet;
        await this._focusCharacterAction.FocusCharacterAsync(player, message.Name).ConfigureAwait(false);
    }
}