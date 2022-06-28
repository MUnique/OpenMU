// <copyright file="AreaSkillHitHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill hit packets.
/// </summary>
/// <remarks>
/// TODO: It's usually required to perform a <see cref="AreaSkillAttackAction"/> before, so this check has to be implemented.
/// </remarks>
[PlugIn(nameof(AreaSkillHitHandlerPlugIn075), "Handler for area skill hit packets.")]
[Guid("D08CA02F-C413-4527-B79C-87F3C4641B60")]
internal class AreaSkillHitHandlerPlugIn075 : AreaSkillHitHandlerMultiTargetPlugInBase, IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AreaSkillHit075.Code;

    /// <inheritdoc/>
    public void HandlePacket(Player player, Span<byte> packet)
    {
        if (packet.Length < 7)
        {
            return;
        }

        AreaSkillHit075 message = packet;

        if (packet.Length < AreaSkillHit075.GetRequiredSize(message.TargetCount)
            || !this.TryGetSkillEntry(player, message.SkillIndex, out var skillEntry))
        {
            return;
        }

        for (var i = 0; i < message.TargetCount; i++)
        {
            this.AttackTarget(player, skillEntry, message[i].TargetId);
        }
    }
}