// <copyright file="TargetedSkillHandlerPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for targeted skill packets until season 2.
/// In these versions, the skill identifiers were only 1 byte big. After that, the master skill tree required more skills to be added than fit into it.
/// </summary>
[PlugIn(nameof(TargetedSkillHandlerPlugIn095), "Handler for targeted skill packets for version 0.95.")]
[Guid("B879F514-D8CF-4ADB-BC43-405B806A856A")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
internal class TargetedSkillHandlerPlugIn095 : TargetedSkillHandlerPlugIn
{
    /// <inheritdoc />
    public override bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    /// <remarks>
    /// In early versions, the index of the skill is used as identifier. Later it was the skill id.
    /// This may have changed earlier than season 2!.
    /// </remarks>
    public override async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TargetedSkill095 message = packet;
        var skill = player.ViewPlugIns.GetPlugIn<ISkillListViewPlugIn>()?.GetSkillByIndex(message.SkillIndex);
        if (skill is null)
        {
            return;
        }

        await this.HandleAsync(player, (ushort)skill.Number, message.TargetId).ConfigureAwait(false);
    }
}