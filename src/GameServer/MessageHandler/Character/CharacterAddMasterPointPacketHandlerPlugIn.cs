// <copyright file="CharacterAddMasterPointPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sub packet handler for master skill point add requests.
/// </summary>
[PlugIn("Character - Add Master Skill Points", "Packet handler for character master skill point adding (0xF3, 0x52 identifier).")]
[Guid("9F39C1FB-26F7-460F-A21B-C4DFEA234E66")]
[BelongsToGroup(CharacterGroupHandlerPlugIn.GroupKey)]
internal class CharacterAddMasterPointPacketHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly AddMasterPointAction _addMasterPointAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 0x52;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        AddMasterSkillPoint message = packet;
        await this._addMasterPointAction.AddMasterPointAsync(player, message.SkillId).ConfigureAwait(false);
    }
}