// <copyright file="InfinityArrowSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;
/*
/// <summary>
/// The Infinity Arrow skill action.
/// </summary>
[PlugIn(nameof(InfinityArrowSkillAction), "Handles the infinity arrow skill of the muse elf.")]
[Guid("a41b5a5f-a9f6-4ae3-8c7c-7fc3acaa30b1")]
public class InfinityArrowSkillAction : TargetedSkillDefaultPlugin
{
    private const short InifiniteArrowMagicEffectNumber = 6;
    private readonly float[] extraManaLossPerAmmoLevel = [0, 2, 5]; // +0/+1/+2 arrows/bolts

    /// <inheritdoc/>
    public override short Key => 77;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        var skillEntry = player.SkillList!.GetSkill(skillId);
        var skill = skillEntry?.Skill;
        if (skill is null)
        {
            return;
        }

        var miniGame = player.CurrentMiniGame;
        var inMiniGame = miniGame is { };
        var isBuff = skill.SkillType is SkillType.Buff or SkillType.Regeneration;
        if (player.IsAtSafezone() && !(inMiniGame && isBuff))
        {
            return;
        }

        if (inMiniGame && !miniGame!.IsSkillAllowed(skill, player, target))
        {
            return;
        }

        if (!target.IsActive())
        {
            return;
        }

        if (!target.CheckSkillTargetRestrictions(player, skill))
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        skillEntry!.PowerUps = null;
        player.CreateMagicEffectPowerUp(skillEntry);

        var ammoItemLevel = player.Inventory?.EquippedAmmunitionItem?.Level ?? 0;
        float extraManaLoss;
        if (ammoItemLevel < this.extraManaLossPerAmmoLevel.Length)
        {
            extraManaLoss = this.extraManaLossPerAmmoLevel[ammoItemLevel];
        }
        else
        {
            extraManaLoss = this.extraManaLossPerAmmoLevel.Last();
        }

        if (extraManaLoss > 0)
        {
            skillEntry.PowerUps =
            [
                .. skillEntry.PowerUps!,
                (Stats.ManaLossAfterHit, new ConstantElement(extraManaLoss, AggregateType.AddRaw)),
            ];
        }

        await AttackableExtensions.ApplyMagicEffectAsync(target, player, skillEntry).ConfigureAwait(false);
        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, target, skill, true), true).ConfigureAwait(false);
    }
}*/