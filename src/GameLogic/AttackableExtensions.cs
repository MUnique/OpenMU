﻿// <copyright file="AttackableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
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
        bool isPvp = attacker is Player && defender is Player;
        bool isClassicPvp = isPvp && defender.Attributes[Stats.MaximumShield] > 0;
        bool isClassicPvpDuel = isClassicPvp && ((Player)attacker).DuelRoom?.Opponent == defender;
        bool isCriticalHit = Rand.NextRandomBool(attacker.Attributes[Stats.CriticalDamageChance]);
        bool isExcellentHit = Rand.NextRandomBool(attacker.Attributes[Stats.ExcellentDamageChance]);
        bool isIgnoringDefense = Rand.NextRandomBool(attacker.Attributes[Stats.DefenseIgnoreChance]);
        attacker.GetBaseDmg(skill, out int baseMinDamage, out int baseMaxDamage, out DamageType damageType);
        int dmg;
        if (damageType == DamageType.Physical)
        {
            if (isExcellentHit)
            {
                dmg = (int)((baseMaxDamage * 1.2) + attacker.Attributes[Stats.ExcellentDamageBonus]);
                dmg += (int)(attacker.Attributes[Stats.CriticalDamageBonus] + attacker.Attributes[Stats.BerserkerMaxPhysDmgBonus]);
                attributes |= DamageAttributes.Excellent;
            }
            else if (isCriticalHit)
            {
                dmg = baseMaxDamage + (int)(attacker.Attributes[Stats.CriticalDamageBonus] + attacker.Attributes[Stats.BerserkerMaxPhysDmgBonus]);
                attributes |= DamageAttributes.Critical;
            }
            else
            {
                baseMinDamage += (int)attacker.Attributes[Stats.BerserkerMinPhysDmgBonus];
                baseMaxDamage += (int)attacker.Attributes[Stats.BerserkerMaxPhysDmgBonus];

                if (baseMaxDamage <= baseMinDamage)
                {
                    dmg = baseMinDamage;
                }
                else
                {
                    dmg = Rand.NextInt(baseMinDamage, baseMaxDamage);
                }
            }

            if (attacker.Attributes[Stats.DoubleWieldWeaponCount] == 2)
            {
                // double wield => 110% dmg (55% + 55%)
                dmg += dmg;
            }

            if (isClassicPvpDuel)
            {
                dmg = (int)(dmg * 0.6);
            }

            dmg += GetMasterSkillTreePhysicalPassiveDamageBonus(attacker, true);

            if (attacker.Attributes[Stats.IsTwoHandedWeaponEquipped] > 0)
            {
                dmg += (int)(dmg * attacker.Attributes[Stats.TwoHandedWeaponDamageIncrease]);
            }
        }
        else
        {
            // Wizardry, Curse & Fenrir
            if (isExcellentHit)
            {
                dmg = (int)((baseMaxDamage * (isClassicPvpDuel ? 0.8 : 1.2)) + attacker.Attributes[Stats.ExcellentDamageBonus]);
                attributes |= DamageAttributes.Excellent;
            }
            else if (isCriticalHit)
            {
                dmg = (int)((baseMaxDamage * (isClassicPvpDuel ? 0.6 : 1)) + attacker.Attributes[Stats.CriticalDamageBonus]);
                attributes |= DamageAttributes.Critical;
            }
            else
            {
                if (baseMaxDamage <= baseMinDamage)
                {
                    dmg = baseMinDamage;
                }
                else
                {
                    dmg = Rand.NextInt(baseMinDamage, baseMaxDamage);
                }

                dmg = (int)(dmg * (isClassicPvpDuel ? 0.6 : 1));
            }
        }

        if (isIgnoringDefense)
        {
            attributes |= DamageAttributes.IgnoreDefense;
        }
        else
        {
            var defenseAttribute = defender.GetDefenseAttribute(attacker);
            var defense = (int)defender.Attributes[defenseAttribute];
            dmg -= defense;
        }

        dmg += (int)attacker.Attributes[Stats.GreaterDamageBonus];

        /*Scroll of Wrath/Wizardry and Remedy of Love go here (but for now they don't exist)*/

        if (damageType == DamageType.Wizardry || damageType == DamageType.Curse)
        {
            // Beyond the base wiz dmg the berserker wiz multiplier also applies here
            dmg += (int)(dmg * attacker.Attributes[Stats.BerserkerWizardryMultiplier]);
        }
        else if (damageType == DamageType.Physical)
        {
            dmg -= (int)(dmg * attacker.Attributes[Stats.WeaknessPhysDmgDecrement]);
            dmg += GetMasterSkillTreePhysicalPassiveDamageBonus(attacker, false);
        }
        else
        {
            // nothing to do
        }

        /*after this it's post the different dmg types methods */
        /*Scroll of Battle (crit dmg)/Strengthener (exc dmg) go here (but for now they don't exist)*/

        if (!isPvp && defender.Overrates(attacker))
        {
            dmg = (int)(dmg * 0.3);
        }

        dmg -= (int)(dmg * defender.Attributes[Stats.ArmorDamageDecrease]);

        var minLevelDmg = (int)Math.Max(1, attacker.Attributes[Stats.TotalLevel] / 10);
        if (dmg < minLevelDmg)
        {
            dmg = minLevelDmg;
        }

        dmg -= (int)(dmg * defender.Attributes[Stats.ShieldSkillReceiveDecrement]);

        // dmg to defender's pet goes here (SpriteDamage(lpTargetObj, AttackDamage))
        // self dmg effects from imp go here
        dmg += (int)(dmg * attacker.Attributes[Stats.ImpAttackDamageIncrease]);
        dmg -= (int)(dmg * defender.Attributes[Stats.GuardianReceiveDecrement]);

        dmg += (int)attacker.Attributes[Stats.PandaRingDamageBonus];

        dmg = (int)(dmg * defender.Attributes[Stats.DamageReceiveDecrement]);
        dmg = (int)(dmg * attacker.Attributes[Stats.AttackDamageIncrease]);

        if (skill != null)
        {
            ApplySkillMultipliersOrLateStageBonus(attacker, defender, skill, ref dmg, damageFactor);
        }
        else if (attacker.Attributes[Stats.IsDinorantEquipped] > 0)
        {
            dmg = (int)(dmg * 1.3);
        }

        var soulBarrierManaToll = defender.Attributes[Stats.SoulBarrierManaTollPerHit];
        if (soulBarrierManaToll > 0 && defender.Attributes[Stats.CurrentMana] > soulBarrierManaToll)
        {
            defender.Attributes[Stats.CurrentMana] -= soulBarrierManaToll;
            dmg -= (int)(dmg * defender.Attributes[Stats.SoulBarrierReceiveDecrement]);
        }

        dmg += (int)attacker.Attributes[Stats.FinalDamageBonus];

        if (isPvp)
        {
            dmg += (int)attacker.Attributes[Stats.FinalDamageIncreasePvp];

            if (((Player)attacker).CurrentMiniGame?.Definition.Type == MiniGameType.ChaosCastle)
            {
                // In Chaos Castle damage is halved
                dmg /= 2;
            }
        }

        /* CS Castle Gates and Satus PvM Damage & item durability reductions go here*/

        dmg += (int)(dmg * attacker.Attributes[Stats.InfinityArrowStrMultiplier]);

        dmg += (int)(dmg * attacker.Attributes[Stats.FenrirAttackDamageIncrease]);
        dmg -= (int)(dmg * defender.Attributes[Stats.FenrirDamageReceiveDecrement]);

        if (dmg < 0)
        {
            dmg = 0;
        }

        /* If is Fenrir skill attack, there is a strong reduction of the defender's item durability here */

        if (isPvp)
        {
            dmg += GetMasterSkillTreeMasteryPvpDamageBonus(attacker);
        }

        if (dmg > 0)
        {
            // Ice arrow magic effect duration is reduce by 1s for every successful hit (dmg >0)
            // Sleep magic effect is canceled for successful hit (dmg > 0)
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

            if (isClassicPvp && ((Player)attacker).CurrentMiniGame?.Definition.Type == MiniGameType.ChaosCastle)
            {
                // Further halve damage in Chaos Castle for classic PvP
                dmg /= 2;
            }
        }

        return defender.GetHitInfo((uint)dmg, attributes, attacker);
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
    /// Applies the magic effect of the player's skill to the target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="attacker">The attacker.</param>
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
    /// Applies the regeneration of the player's skill to the target.
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
            }
            else
            {
                player.Logger.LogWarning(
                    $"Regeneration skill {skill.Name} is configured to regenerate a non-regeneration-able target attribute {powerUpDefinition.TargetAttribute}.");
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

    private static bool Overrates(this IAttackable defender, IAttacker attacker)
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

        return defenseRate > attackRate;
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
    /// Returns the base damage of the attacker, using a specific skill.
    /// </summary>
    /// <remarks>Should exclude: elf's attack dmg buff, wing dmg increase, imp, RH weapon % rises.
    /// Should include: some item options (normal, exc, JoH base dmg, socket), ancient set base dmg and wiz rise options, Master Skill's base/passive dmg bonuses; demon, pet skeleton, Gold Fenrir, arrows/bolts, special rings' increases.</remarks>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">Skill which is used.</param>
    /// <param name="minimumBaseDamage">Minimum base damage.</param>
    /// <param name="maximumBaseDamage">Maximum base damage.</param>
    /// <param name="damageType">The damage type.</param>
    private static void GetBaseDmg(this IAttacker attacker, SkillEntry? skill, out int minimumBaseDamage, out int maximumBaseDamage, out DamageType damageType)
    {
        var skillMinimumDamage = 0;
        var skillMaximumDamage = 0;
        var attackerStats = attacker.Attributes;
        bool isSummoner = attackerStats[Stats.MinimumCurseBaseDmg] > 0;
        bool doubleWield = attackerStats[Stats.DoubleWieldWeaponCount] == 2;

        damageType = DamageType.Physical;
        if (skill?.Skill != null)
        {
            damageType = skill.Skill.DamageType;

            var skillDamage = skill.GetDamage();
            skillMinimumDamage += skillDamage;
            skillMaximumDamage += skillDamage + (skillDamage / 2);

            if (skill.Skill.SkillType == SkillType.Nova)
            {
                var novaDamage = (int)(attackerStats[Stats.NovaBonusDamage] + attackerStats[Stats.NovaStageDamage]);

                skillMinimumDamage += novaDamage;
                skillMaximumDamage += novaDamage;
            }

            if (!isSummoner && damageType != DamageType.Fenrir)
            {
                // For Summoner skill bonus gets added last
                skillMinimumDamage += (int)attackerStats[Stats.SkillDamageBonus];
                skillMaximumDamage += (int)attackerStats[Stats.SkillDamageBonus];
            }

            switch (damageType)
            {
                // Dark Lord skills specific damage types
                case DamageType.ElectricSpike:
                    var nearbyPartyMembers = attacker.Party?.PartyList.Where(p => p == attacker || p.Observers.Contains((IWorldObserver)attacker)).Count() ?? 0;
                    skillMinimumDamage += (int)attackerStats[Stats.ElectricSpikeBonusDmg] + (nearbyPartyMembers * 50);
                    skillMaximumDamage += (int)attackerStats[Stats.ElectricSpikeBonusDmg] + (nearbyPartyMembers * 50);
                    damageType = DamageType.Physical;
                    break;
                case DamageType.Earthshake:
                    skillMinimumDamage += (int)attackerStats[Stats.EarthshakeBonusDmg];
                    skillMaximumDamage += (int)attackerStats[Stats.EarthshakeBonusDmg];
                    damageType = DamageType.Physical;
                    break;
                case DamageType.ChaoticDiseier:
                    skillMinimumDamage += (int)attackerStats[Stats.ChaoticDiseierBonusDmg];
                    skillMaximumDamage += (int)attackerStats[Stats.ChaoticDiseierBonusDmg];
                    damageType = DamageType.Physical;
                    break;
                case DamageType.GenericDarkLordSkill:
                    skillMinimumDamage += (int)attackerStats[Stats.DarkLordGenericSkillBonusDmg];
                    skillMaximumDamage += (int)attackerStats[Stats.DarkLordGenericSkillBonusDmg];
                    damageType = DamageType.Physical;
                    break;

                // Elf skills specific damage types
                case DamageType.MultiShot:
                    skillMinimumDamage *= (int)0.8;
                    skillMaximumDamage *= (int)0.8;
                    damageType = DamageType.Physical;
                    break;

                default:
                    break;
            }
        }

        minimumBaseDamage = (int)(attackerStats[Stats.BaseDamageBonus] + attackerStats[Stats.BaseMinDamageBonus]);
        maximumBaseDamage = (int)(attackerStats[Stats.BaseDamageBonus] + attackerStats[Stats.BaseMaxDamageBonus]);

        switch (damageType)
        {
            // Common damage types
            case DamageType.Wizardry:
                minimumBaseDamage = (int)((minimumBaseDamage + attackerStats[Stats.MinimumWizBaseDmg] + skillMinimumDamage) * attackerStats[Stats.WizardryAttackDamageIncrease]);
                maximumBaseDamage = (int)((maximumBaseDamage + attackerStats[Stats.MaximumWizBaseDmg] + skillMaximumDamage) * attackerStats[Stats.WizardryAttackDamageIncrease]);
                break;
            case DamageType.Curse:
                minimumBaseDamage = (int)((minimumBaseDamage + attackerStats[Stats.MinimumCurseBaseDmg] + skillMinimumDamage) * attackerStats[Stats.CurseAttackDamageIncrease]);
                maximumBaseDamage = (int)((maximumBaseDamage + attackerStats[Stats.MaximumCurseBaseDmg] + skillMaximumDamage) * attackerStats[Stats.CurseAttackDamageIncrease]);
                break;
            case DamageType.Physical:
                // Because double wield damage will be doubled later, we only take half of the skill dmg here (only the RH skill counts)
                minimumBaseDamage = (int)(((minimumBaseDamage + attackerStats[Stats.MinimumPhysBaseDmg]) * (doubleWield ? 0.55 : 1)) + (skillMinimumDamage / (doubleWield ? 2 : 1)));
                maximumBaseDamage = (int)(((maximumBaseDamage + attackerStats[Stats.MaximumPhysBaseDmg]) * (doubleWield ? 0.55 : 1)) + (skillMaximumDamage / (doubleWield ? 2 : 1)));
                break;
            case DamageType.Fenrir:
                // For Fenrir damage only its base and skill damage apply
                minimumBaseDamage = (int)attackerStats[Stats.FenrirBaseDmg] + skillMinimumDamage;
                maximumBaseDamage = (int)attackerStats[Stats.FenrirBaseDmg] + skillMaximumDamage;
                break;
            default:
                // the skill has some other damage type defined which is not applicable to this calculation
                break;
        }

        if (isSummoner && skill != null)
        {
            minimumBaseDamage += (int)attackerStats[Stats.SkillDamageBonus];
            maximumBaseDamage += (int)attackerStats[Stats.SkillDamageBonus];
        }
    }

    /// <summary>
    /// Applies the magic effect of the attacker's skill to the target.
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
            && magicEffectDefinition.PowerUpDefinitions.Any(e => e.TargetAttribute == Stats.IsFrozen || e.TargetAttribute == Stats.IsStunned))
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

    private static int GetMasterSkillTreePhysicalPassiveDamageBonus(IAttacker attacker, bool preBuffsStage)
    {
        if (preBuffsStage)
        {
            if (attacker.Attributes[Stats.IsBowEquipped] > 0)
            {
                return (int)attacker.Attributes[Stats.BowStrBonusDamage];
            }
            else if (attacker.Attributes[Stats.IsCrossBowEquipped] > 0)
            {
                return (int)attacker.Attributes[Stats.CrossBowStrBonusDamage];
            }
            else if (attacker.Attributes[Stats.IsTwoHandedSwordEquipped] > 0)
            {
                return (int)attacker.Attributes[Stats.TwoHandedSwordStrBonusDamage];
            }
            else
            {
                return 0;
            }
        }
        else
        {
            int bonusDamage = 0;

            if (attacker.Attributes[Stats.IsSpearEquipped] > 0) // always two-handed
            {
                bonusDamage = (int)attacker.Attributes[Stats.SpearBonusDamage];
            }
            else if (attacker.Attributes[Stats.IsScepterEquipped] > 0) // impossible to double wield
            {
                bonusDamage = (int)attacker.Attributes[Stats.ScepterStrBonusDamage];
            }
            else if (attacker.Attributes[Stats.IsGloveWeaponEquipped] > 0) // impossible to double wield
            {
                bonusDamage = (int)attacker.Attributes[Stats.GloveWeaponBonusDamage];
            }
            else
            {
                if (attacker.Attributes[Stats.IsOneHandedSwordEquipped] > 0)
                {
                    bonusDamage = (int)attacker.Attributes[Stats.OneHandedSwordBonusDamage];
                }

                if (attacker.Attributes[Stats.IsMaceEquipped] > 0)
                {
                    // In case of a double wield with different possible bonuses, a 50-50 weight split is applied
                    bonusDamage = (int)(bonusDamage == 0
                        ? attacker.Attributes[Stats.MaceBonusDamage]
                        : (bonusDamage * 0.5) + (attacker.Attributes[Stats.MaceBonusDamage] * 0.5));
                }
            }

            return bonusDamage + (int)attacker.Attributes[Stats.MasterSkillPhysBonusDmg];
        }
    }

    private static int GetMasterSkillTreeMasteryPvpDamageBonus(IAttacker attacker)
    {
        if (attacker.Attributes[Stats.IsTwoHandedSwordEquipped] > 0)
        {
            return (int)attacker.Attributes[Stats.TwoHandedSwordMasteryBonusDamage];
        }
        else if (attacker.Attributes[Stats.IsTwoHandedStaffEquipped] > 0)
        {
            return (int)attacker.Attributes[Stats.TwoHandedStaffMasteryBonusDamage];
        }
        else if (attacker.Attributes[Stats.IsCrossBowEquipped] > 0)
        {
            return (int)attacker.Attributes[Stats.CrossBowMasteryBonusDamage];
        }
        else if (attacker.Attributes[Stats.IsStickEquipped] > 0)
        {
            return (int)attacker.Attributes[Stats.StickMasteryBonusDamage];
        }
        else if (attacker.Attributes[Stats.IsScepterEquipped] > 0)
        {
            return (int)attacker.Attributes[Stats.ScepterMasteryBonusDamage];
        }
        else
        {
            return 0;
        }
    }

    private static int ApplySkillMultipliersOrLateStageBonus(IAttacker attacker, IAttackable defender, SkillEntry skill, ref int dmg, double damageFactor)
    {
        switch (skill.Skill?.Number)
        {
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 41:
            case 42:
            case 43:
            case 44:
            case 49:
            case 55:
            case 57:
            case 47:
            case 56:
        // Dl Group
            case 60:
            case 61:
            case 62:
            case 65:
            case 74:
            case 78:
            case 232: // Strike of Destruction (DK)
            case 236: // Flamestrike (MG)
        // MST
            case 330:
            case 332:
            case 481:
            case 326:
            case 479:
            case 327:
            case 328:
            case 329:
            case 337:
            case 340:
            case 343:
        // DL
            case 516: // Earthshake
            case 514:
            case 509:
            case 512:
            case 508:
        // DK
            case 336:
            case 339:
            case 342:
            case 331:
            case 333:
        // MG
            case 490:
            case 493:
            case 492:
            case 494:
            case 482:
        // DL
            case 518:
            case 520:
                // DL, MG, RF, DK
                dmg = (int)(dmg * attacker.Attributes[Stats.SkillMultiplier]);
                break;
            case 46:
            case 51:
            case 52:
            // MST
            case 416:
            case 424:
            case 427:
            case 434:
                // Elf
                dmg *= 2;
                break;
            case 76: // fenrir
                var levelBonus = Math.Max(0, attacker.Attributes[Stats.TotalLevel] - 300) / 5;
                dmg = (int)(dmg * ((levelBonus / 100) + 2));
                break;
            case 215: // chain lightning
            case 455: // chain lightning strengthener
                dmg = (int)(dmg * damageFactor);
                break;
            case 238: // Chaotic Diseier (DL)
            // MST
            case 523: // Chaotic Diseier Strengthener (DL)
                dmg = (int)(dmg * attacker.Attributes[Stats.SkillMultiplier] / 1.25f);
                break;
        // mob skills
            case 250:
                dmg *= 2;
                break;
            case 251:
                dmg = (int)(dmg * 2.2f);
                break;
            case 252:
                dmg = (int)(dmg * 2.3f);
                break;
            case 253:
                dmg = (int)(dmg * 2.5f);
                break;
        // RF
            case 260:
            case 261:
            case 262:
            case 269:
            // MST
            case 551:
            case 554:
            case 552:
            case 555:
            case 558:
            case 562:
                dmg = (int)(dmg * (0.5 + (attacker.Attributes[Stats.TotalVitality] / 1000)));
                break;
            case 270:
                dmg = (int)(dmg * (2 + (attacker.Attributes[Stats.TotalVitality] / 1000)));
                break;
            case 263:
            // MST
            case 559:
            case 563:
                dmg = (int)(dmg * (1 + (attacker.Attributes[Stats.TotalAgility] / 800) + (attacker.Attributes[Stats.TotalEnergy] / 1000)));
                break;
            case 264:
            // MST
            case 560:
            case 561:
                dmg = (int)(dmg * (0.5 + (attacker.Attributes[Stats.TotalEnergy] / 1000)));
                break;
            case 265:
            // MST
            case 564:
            case 565: // probably bug, but doesn't exist in S6
                if (defender is Player)
                {
                    dmg = (int)(dmg * (0.5 + (attacker.Attributes[Stats.TotalEnergy] / 1000)));
                }
                else
                {
                    dmg = (int)((dmg * (0.5 + (attacker.Attributes[Stats.TotalEnergy] / 1000)) * 3) + 300);
                }

                break;
        // DK, MST
            case 344:
            case 346:
                dmg = (int)(dmg * (2 + (attacker.Attributes[Stats.TotalEnergy] / 1000)));
                break;
        // Summoner late stage bonuses
            case 223: // explosion (samut)
                dmg += (int)attacker.Attributes[Stats.ExplosionBonusDmg];
                break;
            case 224: // requiem (neil)
                dmg += (int)attacker.Attributes[Stats.RequiemBonusDmg];
                break;
            case 225: // pollution (lagle)
                dmg += (int)attacker.Attributes[Stats.PollutionBonusDmg];
                break;
            default:
                break;
        }

        return dmg;
    }
}