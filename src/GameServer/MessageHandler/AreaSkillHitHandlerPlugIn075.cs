// <copyright file="AreaSkillHitHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill hit packets.
/// </summary>
[PlugIn(nameof(AreaSkillHitHandlerPlugIn075), "Handler for area skill hit packets.")]
[Guid("D08CA02F-C413-4527-B79C-87F3C4641B60")]
internal class AreaSkillHitHandlerPlugIn075 : AreaSkillHitHandlerMultiTargetPlugInBase, IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AreaSkillHit075.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
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

        // Validate that an area skill attack was performed for explicit hit skills
        if (skillEntry.Skill?.SkillType == SkillType.AreaSkillExplicitHits)
        {
            // For client 0.75, there's no counter field, so we can only verify that
            // the last registered skill matches the skill being used for hits
            if (player.SkillHitValidator.LastRegisteredSkillId != (ushort)skillEntry.Skill.Number)
            {
                player.Logger.LogWarning("Possible Hacker - Area skill hit without corresponding attack. Expected skill: {0}, Actual: {1}", player.SkillHitValidator.LastRegisteredSkillId, skillEntry.Skill.Number);
                return;
            }
        }

        for (var i = 0; i < message.TargetCount; i++)
        {
            await this.AttackTargetAsync(player, skillEntry, message[i].TargetId).ConfigureAwait(false);
        }
    }
}