// <copyright file="AttackableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions for <see cref="IAttackable"/>s. Contains the damage calculation.
/// </summary>
public static class AttackableExtensions
{
    private static readonly IDictionary<AttributeDefinition, AttributeDefinition> ReductionModifiers =
        new Dictionary<AttributeDefinition, AttributeDefinition>
        {
            { Stats.CurrentMana, Stats.ManaUsageReduction },
            { Stats.CurrentAbility, Stats.AbilityUsageReduction },
        };

    /// <summary>
    /// Calculates the damage, using a skill.
    /// </summary>
    /// <param name="attacker">The object which is attacking.</param>
    /// <param name="defender">The object which is defending.</param>
    /// <param name="skill">The skill which is used.</param>
    /// <param name="isCombo">If set to <c>true</c>, the damage gets increased by a combo bonus.</param>
    /// <param name="damageFactor">The damage factor.</param>
    /// <returns>
    /// The hit information.
    /// </returns>
    public static async ValueTask<HitInfo> CalculateDamageAsync(this IAttacker attacker, IAttackable defender, SkillEntry? skill, bool isCombo, double damageFactor = 1.0)
    {
        if (!attacker.IsAttackSuccessfulTo(defender))
        {
            return new HitInfo(0, 0, DamageAttributes.Undefined);
        }

        DamageAttributes attributes = DamageAttributes.Undefined;
        bool isCriticalHit = Rand.NextRandomBool(attacker.Attributes[Stats.CriticalDamageChance]);
        bool isExcellentHit = Rand.NextRandomBool(attacker.Attributes[Stats.ExcellentDamageChance]);
        bool isIgnoringDefense = Rand.NextRandomBool(attacker.Attributes[Stats.DefenseIgnoreChance]);
        attacker.GetBaseDmg(skill, out int baseMinDamage, out int baseMaxDamage);
        int dmg;
        if (isExcellentHit)
        {
            dmg = (int)(baseMaxDamage * 1.2);
            attributes |= DamageAttributes.Excellent;
        }
        else if (isCriticalHit)
        {
            dmg = baseMaxDamage;
            attributes |= DamageAttributes.Critical;
        }
        else if (baseMaxDamage <= baseMinDamage)
        {
            dmg = baseMinDamage;
        }
        else
        {
            dmg = Rand.NextInt(baseMinDamage, baseMaxDamage);
        }

        if (!isIgnoringDefense)
        {
            var defenseAttribute = defender.GetDefenseAttribute(attacker);
            var defense = (int)defender.Attributes[defenseAttribute];
            dmg -= defense;
        }
        else
        {
            attributes |= DamageAttributes.IgnoreDefense;
        }

        dmg = Math.Max(dmg, 0);

        dmg = (int)(dmg * defender.Attributes[Stats.DamageReceiveDecrement]);
        dmg = (int)(dmg * attacker.Attributes[Stats.AttackDamageIncrease]);

        if (skill != null)
        {
            dmg += (int)attacker.Attributes[Stats.SkillDamageBonus];
            dmg = (int)(dmg * attacker.Attributes[Stats.SkillMultiplier]);
        }

        if (attacker.Attributes[Stats.IsTwoHandedWeaponEquipped] > 0)
        {
            dmg = (int)(dmg * attacker.Attributes[Stats.TwoHandedWeaponDamageIncrease]);
        }

        if (attacker is Player && defender is Player)
        {
            dmg += (int)attacker.Attributes[Stats.FinalDamageIncreasePvp];
        }

        if (isCombo)
        {
            dmg += (int)attacker.Attributes[Stats.ComboBonus];
            attributes |= DamageAttributes.Combo;
        }

        bool isDoubleDamage = Rand.NextRandomBool(attacker.Attributes[Stats.DoubleDamageChance]);
        if (isDoubleDamage)
        {
            dmg *= 2;
            attributes |= DamageAttributes.Double;
        }

        dmg = (int)(dmg * damageFactor);

        var minimumDamage = attacker.Attributes[Stats.Level] / 10;
        return defender.GetHitInfo(Math.Max((uint)dmg, (uint)minimumDamage), attributes, attacker);
    }

    /// <summary>
    /// Gets the hit information, calculates which part of the damage damages the shield and which the health.
    /// </summary>
    /// <param name="defender">The defender.</param>
    /// <param name="damage">The damage.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="attacker">The attacker.</param>
    /// <returns>The calculated hit info.</returns>
    public static HitInfo GetHitInfo(this IAttackable defender, uint damage, DamageAttributes attributes, IAttacker attacker)
    {
        var shieldBypass = Rand.NextRandomBool(attacker.Attributes[Stats.ShieldBypassChance]);
        if (shieldBypass || defender.Attributes[Stats.CurrentShield] < 1)
        {
            return new HitInfo(damage, 0, attributes);
        }

        var shieldRatio = 0.90;
        shieldRatio -= attacker.Attributes[Stats.ShieldDecreaseRateIncrease];
        shieldRatio += defender.Attributes[Stats.ShieldRateIncrease];
        shieldRatio = Math.Max(0, shieldRatio);
        shieldRatio = Math.Min(1, shieldRatio);
        return new HitInfo((uint)(damage * (1 - shieldRatio)), (uint)(damage * shieldRatio), attributes);
    }

    /// <summary>
    /// Applies the magic effect of the players skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="player">The player.</param>
    /// <param name="skillEntry">The skill entry.</param>
    public static async ValueTask ApplyMagicEffectAsync(this IAttackable target, IAttacker attacker, SkillEntry skillEntry)
    {
        if (skillEntry.PowerUps is null && attacker is Player player)
        {
            player.CreateMagicEffectPowerUp(skillEntry);
        }

        await target.ApplyMagicEffectAsync(attacker, skillEntry.Skill!.MagicEffectDef!, skillEntry.PowerUpDuration!, skillEntry.PowerUps!).ConfigureAwait(false);
    }

    /// <summary>
    /// Applies the regeneration of the players skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="player">The player.</param>
    /// <param name="skillEntry">The skill entry.</param>
    public static async ValueTask ApplyRegenerationAsync(this IAttackable target, Player player, SkillEntry skillEntry)
    {
        if (player.Attributes is null)
        {
            return;
        }

        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        var isHealthUpdated = false;
        var isManaUpdated = false;
        var skill = skillEntry.Skill;
        foreach (var powerUpDefinition in skill.MagicEffectDef?.PowerUpDefinitions ?? Enumerable.Empty<PowerUpDefinition>())
        {
            var regeneration = Stats.IntervalRegenerationAttributes.FirstOrDefault(r => r.CurrentAttribute == powerUpDefinition.TargetAttribute);
            if (regeneration != null)
            {
                var regenerationValue = player.Attributes.CreateElement(powerUpDefinition);
                var value = skillEntry.Level == 0 ? regenerationValue.Value : regenerationValue.Value + skillEntry.CalculateValue();
                target.Attributes[regeneration.CurrentAttribute] = Math.Min(
                    target.Attributes[regeneration.CurrentAttribute] + value,
                    target.Attributes[regeneration.MaximumAttribute]);
                isHealthUpdated |= regeneration.CurrentAttribute == Stats.CurrentHealth || regeneration.CurrentAttribute == Stats.CurrentShield;
                isManaUpdated |= regeneration.CurrentAttribute == Stats.CurrentMana || regeneration.CurrentAttribute == Stats.CurrentAbility;
            }
            else
            {
                player.Logger.LogWarning(
                    $"Regeneration skill {skill.Name} is configured to regenerate a non-regeneration-able target attribute {powerUpDefinition.TargetAttribute}.");
            }
        }

        if (target is IWorldObserver observer)
        {
            if (isHealthUpdated)
            {
                await observer.InvokeViewPlugInAsync<IUpdateCurrentHealthPlugIn>(p => p.UpdateCurrentHealthAsync()).ConfigureAwait(false);
            }

            if (isManaUpdated)
            {
                await observer.InvokeViewPlugInAsync<IUpdateCurrentManaPlugIn>(p => p.UpdateCurrentManaAsync()).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Applies the elemental effects of a players skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skillEntry">The skill entry.</param>
    /// <returns>The success of the appliance.</returns>
    public static async ValueTask<bool> TryApplyElementalEffectsAsync(this IAttackable target, IAttacker attacker, SkillEntry skillEntry)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
        var modifier = skillEntry.Skill.ElementalModifierTarget;
        if (modifier is null)
        {
            return false;
        }

        var resistance = target.Attributes[modifier];
        if (resistance >= 1.0f || !Rand.NextRandomBool(1.0f - resistance))
        {
            return false;
        }

        var applied = false;

        if (skillEntry.Skill.MagicEffectDef is { } effectDefinition
            && !target.MagicEffectList.ActiveEffects.ContainsKey(effectDefinition.Number))
        {
            // power-up is the wrong term here... it's more like a power-down ;-)
            await target.ApplyMagicEffectAsync(attacker, skillEntry).ConfigureAwait(false);
            applied = true;
        }

        if (modifier == Stats.LightningResistance)
        {
            await target.MoveRandomlyAsync().ConfigureAwait(false);
            applied = true;
        }

        return applied;
    }

    /// <summary>
    /// Applies the elemental effects of a players skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="powerUp">The power up.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <returns>
    /// The success of the appliance.
    /// </returns>
    public static async ValueTask<bool> TryApplyElementalEffectsAsync(this IAttackable target, IAttacker attacker, Skill skill, IElement? powerUp, IElement? duration, AttributeDefinition? targetAttribute)
    {
        var modifier = skill.ElementalModifierTarget;
        if (modifier is null)
        {
            return false;
        }

        var resistance = target.Attributes[modifier];
        if (resistance >= 1.0f || !Rand.NextRandomBool(1.0f - resistance))
        {
            return false;
        }

        var applied = false;

        if (skill.MagicEffectDef is { } effectDefinition
            && !target.MagicEffectList.ActiveEffects.ContainsKey(effectDefinition.Number)
            && powerUp is not null
            && duration is not null
            && targetAttribute is not null)
        {
            // power-up is the wrong term here... it's more like a power-down ;-)
            await target.ApplyMagicEffectAsync(attacker, effectDefinition, duration, (targetAttribute, powerUp)).ConfigureAwait(false);
            applied = true;
        }

        if (modifier == Stats.LightningResistance)
        {
            await target.MoveRandomlyAsync().ConfigureAwait(false);
            applied = true;
        }

        return applied;
    }

    /// <summary>
    /// Applies the ammunition consumption.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="hitInfo">The hit information.</param>
    public static void ApplyAmmunitionConsumption(this IAttacker attacker, HitInfo hitInfo)
    {
        if (!hitInfo.Attributes.HasFlag(DamageAttributes.Reflected) && attacker.Attributes[Stats.AmmunitionConsumptionRate] > 0.0)
        {
            // Every hit needs ammo. Failed hits don't need ammo.
            if (attacker.Attributes[Stats.AmmunitionAmount] < attacker.Attributes[Stats.AmmunitionConsumptionRate])
            {
                return;
            }

            attacker.Attributes[Stats.AmmunitionAmount] -= attacker.Attributes[Stats.AmmunitionConsumptionRate];
        }
    }

    /// <summary>
    /// Checks if the target can be attacked by the player, based on the target restrictions of the skill.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="player">The player.</param>
    /// <param name="skill">The skill.</param>
    /// <returns><c>True</c>, if the target can be attacked; Otherwise, <c>false</c>.</returns>
    public static bool CheckSkillTargetRestrictions(this IAttackable target, Player player, Skill skill)
    {
        var result = true;
        if (skill.TargetRestriction == SkillTargetRestriction.Self && target != player)
        {
            player.Logger.LogWarning($"Player '{player.Name}' tried to perform Skill '{skill.Name}' on target '{target}', but the skill is restricted to himself.");
            result = false;
        }
        else if (skill.TargetRestriction == SkillTargetRestriction.Party && target != player && target is IPartyMember partyMember && (player.Party is null || !player.Party.PartyList.Contains(partyMember)))
        {
            player.Logger.LogWarning($"Player '{player.Name}' tried to perform Skill '{skill.Name}' on target '{target}', but the skill is restricted to his party.");
            result = false;
        }
        else if (skill.TargetRestriction == SkillTargetRestriction.Player && target is not Player && target is Monster { SummonedBy: null })
        {
            player.Logger.LogWarning($"Player '{player.Name}' tried to perform Skill '{skill.Name}' on target '{target}', but the skill is restricted to players.");
            result = false;
        }
        else
        {
            // everything fine
        }

        return result;
    }

    /// <summary>Moves the target randomly.</summary>
    /// <param name="target">The target.</param>
    public static async ValueTask MoveRandomlyAsync(this IAttackable target)
    {
        if (target is IMovable movable && target.CurrentMap is { } map)
        {
            var terrain = map.Terrain;
            var newX = target.Position.X + Rand.NextInt(-1, 2);
            var newY = target.Position.Y + Rand.NextInt(-1, 2);
            var isNewXAllowed = newX is >= byte.MinValue and <= byte.MaxValue;
            var isNewYAllowed = newY is >= byte.MinValue and <= byte.MaxValue;
            if (isNewXAllowed && isNewYAllowed && terrain.AIgrid[newX, newY] == 1)
            {
                await movable.MoveAsync(new Point((byte)newX, (byte)newY)).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Gets the required value of the <see cref="AttributeRequirement"/>.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="attributeRequirement">The attribute requirement, e.g. of a skill.</param>
    /// <returns>The required value.</returns>
    public static int GetRequiredValue(this IAttacker attacker, AttributeRequirement attributeRequirement)
    {
        var modifier = 1.0f;
        if (ReductionModifiers.TryGetValue(
                attributeRequirement.Attribute ?? throw Error.NotInitializedProperty(attributeRequirement, nameof(attributeRequirement.Attribute)),
                out var reductionAttribute))
        {
            modifier -= attacker.Attributes[reductionAttribute];
        }

        return (int)(attributeRequirement.MinimumValue * modifier);
    }

    /// <summary>
    /// Calculates the base experience for a killed target object.
    /// </summary>
    /// <param name="killedObject">The killed target object.</param>
    /// <param name="killerLevel">The level of the killer.</param>
    /// <returns>The calculated base experience.</returns>
    public static double CalculateBaseExperience(this IAttackable killedObject, float killerLevel)
    {
        var targetLevel = killedObject.Attributes[Stats.Level];
        var tempExperience = (targetLevel + 25) * targetLevel / 3.0;

        if (killerLevel > targetLevel + 10)
        {
            tempExperience *= (targetLevel + 10) / killerLevel;
        }

        if (killedObject.Attributes[Stats.Level] >= 65)
        {
            tempExperience += (targetLevel - 64) * (targetLevel / 4);
        }

        return Math.Max(tempExperience, 0) * 1.25;
    }

    private static bool IsAttackSuccessfulTo(this IAttacker attacker, IAttackable defender)
    {
        var hitChance = attacker.GetHitChanceTo(defender);
        return Rand.NextRandomBool(hitChance);
    }

    private static float GetHitChanceTo(this IAttacker attacker, IAttackable defender)
    {
        float defenseRate;
        float attackRate;
        if (defender is Player && attacker is Player)
        {
            defenseRate = defender.Attributes[Stats.DefenseRatePvp];
            attackRate = attacker.Attributes[Stats.AttackRatePvp];
        }
        else
        {
            defenseRate = defender.Attributes[Stats.DefenseRatePvm];
            attackRate = attacker.Attributes[Stats.AttackRatePvm];
        }

        float hitChance = 0.03f;
        if (defenseRate < attackRate)
        {
            hitChance = 1.0f - (defenseRate / attackRate);
        }

        return hitChance;
    }

    private static AttributeDefinition GetDefenseAttribute(this IAttackable defender, IAttacker attacker)
    {
        if (attacker is Player && defender is Player)
        {
            return Stats.DefensePvp;
        }

        return Stats.DefensePvm;
    }

    private static int GetDamage(this SkillEntry skill)
    {
        skill.ThrowNotInitializedProperty(skill.Skill is null, nameof(skill.Skill));

        var result = skill.Skill.AttackDamage;
        if (skill.Skill.MasterDefinition != null)
        {
            result += (int)skill.CalculateValue();
        }

        return result;
    }

    /// <summary>
    /// Returns the base damage if the attacker, using a specific skill.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">Skill which is used.</param>
    /// <param name="minimumBaseDamage">Minimum base damage.</param>
    /// <param name="maximumBaseDamage">Maximum base damage.</param>
    private static void GetBaseDmg(this IAttacker attacker, SkillEntry? skill, out int minimumBaseDamage, out int maximumBaseDamage)
    {
        var attackerStats = attacker.Attributes;
        minimumBaseDamage = (int)(attackerStats[Stats.BaseDamageBonus] + attackerStats[Stats.BaseMinDamageBonus]);
        maximumBaseDamage = (int)(attackerStats[Stats.BaseDamageBonus] + attackerStats[Stats.BaseMaxDamageBonus]);

        DamageType damageType = DamageType.Physical;
        if (skill?.Skill != null)
        {
            damageType = skill.Skill.DamageType;

            var skillDamage = skill.GetDamage();
            minimumBaseDamage += skillDamage;
            maximumBaseDamage += skillDamage + (skillDamage / 2);

            if (skill.Skill.SkillType == SkillType.Nova)
            {
                var novaDamage = (int)(attackerStats[Stats.NovaBonusDamage] + attackerStats[Stats.NovaStageDamage]);

                minimumBaseDamage += novaDamage;
                maximumBaseDamage += novaDamage;
            }
        }

        switch (damageType)
        {
            case DamageType.Wizardry:
                minimumBaseDamage = (int)((minimumBaseDamage + attackerStats[Stats.MinimumWizBaseDmg]) * attackerStats[Stats.WizardryAttackDamageIncrease]);
                maximumBaseDamage = (int)((maximumBaseDamage + attackerStats[Stats.MaximumWizBaseDmg]) * attackerStats[Stats.WizardryAttackDamageIncrease]);

                break;
            case DamageType.Curse:
                minimumBaseDamage += (int)((minimumBaseDamage + attackerStats[Stats.MinimumCurseBaseDmg]) * attackerStats[Stats.CurseAttackDamageIncrease]);
                maximumBaseDamage += (int)((maximumBaseDamage + attackerStats[Stats.MaximumCurseBaseDmg]) * attackerStats[Stats.CurseAttackDamageIncrease]);

                break;
            case DamageType.Physical:
                minimumBaseDamage += (int)attackerStats[Stats.MinimumPhysBaseDmg];
                maximumBaseDamage += (int)attackerStats[Stats.MaximumPhysBaseDmg];
                break;
            case DamageType.All:
                minimumBaseDamage += Math.Max(
                    Math.Max(
                        (int)(attackerStats[Stats.MinimumWizBaseDmg] * attackerStats[Stats.WizardryAttackDamageIncrease]),
                        (int)(attackerStats[Stats.MinimumCurseBaseDmg] * attackerStats[Stats.CurseAttackDamageIncrease])),
                    (int)attackerStats[Stats.MinimumPhysBaseDmg]);
                maximumBaseDamage += Math.Max(
                    Math.Max(
                        (int)(attackerStats[Stats.MaximumWizBaseDmg] * attackerStats[Stats.WizardryAttackDamageIncrease]),
                        (int)(attackerStats[Stats.MaximumCurseBaseDmg] * attackerStats[Stats.CurseAttackDamageIncrease])),
                    (int)attackerStats[Stats.MaximumPhysBaseDmg]);
                break;
            case DamageType.Fenrir:
                minimumBaseDamage += (int)attackerStats[Stats.FenrirBaseDmg];
                maximumBaseDamage += (int)attackerStats[Stats.FenrirBaseDmg];
                break;
            default:
                // the skill has some other damage type defined which is not applicable to this calculation
                break;
        }
    }

    /// <summary>
    /// Applies the magic effect of the attackers skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="magicEffectDefinition">The magic effect definition.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="powerUps">The power ups of the effect.</param>
    private static async ValueTask ApplyMagicEffectAsync(this IAttackable target, IAttacker attacker, MagicEffectDefinition magicEffectDefinition, IElement duration, params (AttributeDefinition Target, IElement Boost)[] powerUps)
    {
        var isPoisonEffect = magicEffectDefinition.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsPoisoned);
        var magicEffect = isPoisonEffect
            ? new PoisonMagicEffect(powerUps[0].Boost, magicEffectDefinition, TimeSpan.FromSeconds(duration.Value), attacker, target)
            : new MagicEffect(TimeSpan.FromSeconds(duration.Value), magicEffectDefinition, powerUps.Select(p => new MagicEffect.ElementWithTarget(p.Boost, p.Target)).ToArray());

        await target.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);
        if (target is ISupportWalk walkSupporter
            && walkSupporter.IsWalking
            && magicEffectDefinition.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsFrozen || e.TargetAttribute == Stats.IsStunned || e.TargetAttribute == Stats.IsAsleep))
        {
            await walkSupporter.StopWalkingAsync().ConfigureAwait(false);

            // Since the actual coordinates could be out of sync with the client
            // coordinates, we simply update the position on the client side.
            if (walkSupporter is IObservable observable)
            {
                await observable.ForEachWorldObserverAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(walkSupporter, MoveType.Instant), true).ConfigureAwait(false);
            }
        }
    }
}