// <copyright file="HitAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Action to hit targets without a skill with pure melee damage.
/// </summary>
public class HitAction
{
    /// <summary>
    /// Hits the specified target by the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    /// <param name="attackAnimation">The attack animation.</param>
    /// <param name="lookingDirection">The looking direction.</param>
    public async ValueTask HitAsync(Player player, IAttackable target, byte attackAnimation, Direction lookingDirection)
    {
        if (target is IObservable targetAsObservable)
        {
            using (await targetAsObservable.ObserverLock.ReaderLockAsync())
            {
                if (!targetAsObservable.Observers.Contains(player))
                {
                    // Target out of range
                    return;
                }
            }
        }

        player.Rotation = lookingDirection;
        await target.AttackByAsync(player, null);
        if (player.Attributes?[Stats.TransformationSkin] is { } skin and not 0
            && await this.ApplySkinnedMonstersSkillAsync(player, target, (short)skin) is var (skill, effectApplied))
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, target, skill, effectApplied), true).ConfigureAwait(false);
            return;
        }

        await player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(p => p.ShowAnimationAsync(player, attackAnimation, target, lookingDirection), false).ConfigureAwait(false);
    }

    private async ValueTask<(Skill Skill, bool EffectApplied)?> ApplySkinnedMonstersSkillAsync(Player player, IAttackable target, short skin)
    {
        var effectApplied = false;
        if (player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == skin)?.AttackSkill
            is not { ElementalModifierTarget: not null } skill)
        {
            return null;
        }

        var modifier = skill.ElementalModifierTarget!;
        var resistance = target.Attributes[modifier];
        if (resistance >= 1.0f || !Rand.NextRandomBool(1.0f - resistance))
        {
            return (skill, effectApplied);
        }

        if (skill.MagicEffectDef is { } effectDefinition
            && !target.MagicEffectList.ActiveEffects.ContainsKey(effectDefinition.Number)
            && effectDefinition.PowerUpDefinition is { Boost: not null, Duration: not null } powerUpDef)
        {
            var powerUp = target.Attributes!.CreateElement(powerUpDef.Boost!);
            var powerUpDuration = target.Attributes!.CreateElement(powerUpDef.Duration!);
            var magicEffect = effectDefinition.PowerUpDefinition.TargetAttribute == Stats.IsPoisoned
                ? new PoisonMagicEffect(powerUp, effectDefinition, TimeSpan.FromSeconds(powerUpDuration.Value), player, target)
                : new MagicEffect(powerUp, effectDefinition, TimeSpan.FromSeconds(powerUpDuration.Value));
            await target.MagicEffectList.AddEffectAsync(magicEffect);
            effectApplied = true;
        }

        if (modifier == Stats.LightningResistance)
        {
            await target.MoveRandomlyAsync();
        }

        return (skill, effectApplied);
    }
}