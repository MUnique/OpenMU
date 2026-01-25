// <copyright file="RageSkillAttackHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for rage fighter area skill attack packets (beast uppercut, chain drive, dark side).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RageSkillAttackHandlerPlugIn_Name), Description = nameof(PlugInResources.RageSkillAttackHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D09ED5FB-DB6F-4707-A740-144CF2BA5D92")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
internal class RageSkillAttackHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => RageAttackRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        RageAttackRequest message = packet;
        var skillId = message.SkillId;
        var targetId = message.TargetId;

        var target = player.GetObject(targetId) as IAttackable;
        if (player.SkillList is null || player.SkillList.GetSkill(message.SkillId) is not { } skill
                                     || (target is null || !target.IsInRange(player.Position, skill.Skill!.Range)))
        {
            return;
        }

        await player.ForEachWorldObserverAsync<IShowRageAttackPlugIn>(p => p.ShowAttackAsync(player, target, skillId), true).ConfigureAwait(false);
    }
}