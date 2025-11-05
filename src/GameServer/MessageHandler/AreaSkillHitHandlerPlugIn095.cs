// <copyright file="AreaSkillHitHandlerPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill hit packets.
/// </summary>
[PlugIn(nameof(AreaSkillHitHandlerPlugIn095), "Handler for area skill hit packets.")]
[Guid("71C2E116-D1B2-4F07-9A3B-41CEA1975108")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
internal class AreaSkillHitHandlerPlugIn095 : AreaSkillHitHandlerMultiTargetPlugInBase, IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AreaSkillHit095.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < 8)
        {
            return;
        }

        AreaSkillHit095 message = packet;

        if (packet.Length < AreaSkillHit095.GetRequiredSize(message.TargetCount)
            || !this.TryGetSkillEntry(player, message.SkillIndex, out var skillEntry))
        {
            return;
        }

        // Validate that the skill was performed before allowing hits
        if (skillEntry.Skill?.SkillType == SkillType.AreaSkillExplicitHits)
        {
            // For explicit hit skills, validate using the hit counter
            // Using animation counter 0 and hit counter from the message
            var (isValid, increaseCounter) = player.SkillHitValidator.IsHitValid((ushort)skillEntry.Skill.Number, 0, message.Counter);
            if (!isValid)
            {
                return;
            }

            try
            {
                for (var i = 0; i < message.TargetCount; i++)
                {
                    await this.AttackTargetAsync(player, skillEntry, message[i].TargetId).ConfigureAwait(false);
                }
            }
            finally
            {
                if (increaseCounter)
                {
                    player.SkillHitValidator.IncreaseCounterAfterHit();
                }
            }
        }
        else
        {
            // For non-explicit hit skills, just process normally
            for (var i = 0; i < message.TargetCount; i++)
            {
                await this.AttackTargetAsync(player, skillEntry, message[i].TargetId).ConfigureAwait(false);
            }
        }
    }
}