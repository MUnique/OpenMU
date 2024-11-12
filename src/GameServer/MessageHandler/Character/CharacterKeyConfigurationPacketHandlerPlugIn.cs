// <copyright file="CharacterKeyConfigurationPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for character key configuration packets (0xF3, 0x30 identifier).
/// </summary>
[PlugIn("Character - Key Configuration", "Packet handler for character key configuration packets (0xF3, 0x30 identifier).")]
[Guid("91A6A9A8-5885-498E-A6BE-625F55A811A4")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterKeyConfigurationPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly SaveKeyConfigurationAction _saveKeyConfigurationAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 0x30;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        SaveKeyConfiguration message = packet;
        this._saveKeyConfigurationAction.SaveKeyConfiguration(player, message.Configuration.ToArray());
    }
}