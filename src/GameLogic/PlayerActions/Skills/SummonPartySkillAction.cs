// <copyright file="SummonPartySkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The summon skill action for Dark Lord.
/// </summary>
public class SummonPartySkillAction : TargetedSkillActionBase
{
    private static readonly TimeSpan CompletionDelay = TimeSpan.FromMilliseconds(5000);

    /// <inheritdoc />
    public override ushort Key => 63;

    /// <inheritdoc />
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        var skillEntry = player.SkillList?.GetSkill(skillId);
        if (skillEntry?.Skill is null)
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skillEntry.Skill).ConfigureAwait(false))
        {
            return;
        }

        await this.SummonTargetsAsync(player, skillEntry).ConfigureAwait(false);
    }

    private async ValueTask SummonTargetsAsync(Player player, SkillEntry skillEntry)
    {
        if (!player.IsAlive || player.IsAtSafezone() || skillEntry.Skill is not { } skill)
        {
            return;
        }

        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, player, skill.Number, true), true).ConfigureAwait(false);
        var targets = player.Party?.PartyList ?? Enumerable.Empty<IPartyMember>();

        await Task.Delay(CompletionDelay, CancellationToken.None).ConfigureAwait(false);

        foreach (var targetMember in targets.OfType<Player>().Where(p => p != player))
        {
            _ = Task.Run(() => targetMember.TeleportAsync(player.Position, skill));
        }
    }
}
