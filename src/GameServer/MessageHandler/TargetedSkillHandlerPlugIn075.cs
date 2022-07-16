// <copyright file="TargetedSkillHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for targeted skill packets of version 0.75.
/// </summary>
[PlugIn(nameof(TargetedSkillHandlerPlugIn075), "Handler for targeted skill packets for version 0.75")]
[Guid("83D2ABC0-6491-409A-8525-941794C54660")]
internal class TargetedSkillHandlerPlugIn075 : TargetedSkillHandlerPlugIn
{
    /// <inheritdoc />
    public override bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    /// <remarks>
    /// In early versions, the index of the skill is used as identifier. Later it was the skill id.
    /// </remarks>
    public override async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TargetedSkill075 message = packet;
        var skill = player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillIndex);
        if (skill is null)
        {
            return;
        }

        await this.HandleAsync(player, (ushort)skill.Number, message.TargetId).ConfigureAwait(false);
    }
}