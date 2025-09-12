// <copyright file="AttackableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Pet;
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

        Player? attackerPlayer = attacker is AttackerSurrogate surrogate ? surrogate.Owner : (attacker is Player player ? player : null);
        bool isPvp = attackerPlayer is not null && defender is Player;
        bool isClassicPvp = isPvp && defender.Attributes[Stats.MaximumShield] == 0;
        bool isClassicPvpDuel = isClassicPvp && attackerPlayer!.DuelRoom?.Opponent == defender;
        var duelDmgDec = isClassicPvpDuel ? 0.6 : 1;

        DamageAttributes attributes = DamageAttributes.Undefined;
        bool isCriticalHit = Rand.NextRandomBool(attacker.Attributes[Stats.CriticalDamageChance]);
        bool isExcellentHit = Rand.NextRandomBool(attacker.Attributes[Stats.ExcellentDamageChance]);
        bool isIgnoringDefense = Rand.NextRandomBool(attacker.Attributes[Stats.DefenseIgnoreChance]);

        var defense = 0;
        if (isIgnoringDefense)
        {
            attributes |= DamageAttributes.IgnoreDefense;
        }
        else
        {
            var defenseAttribute = defender.GetDefenseAttribute(attacker);
            defense = (int)defender.Attributes[defenseAttribute];
        }

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

            if (attacker.Attributes[Stats.HasDoubleWield] > 0)
            {
                // double wield => 110% dmg (55% + 55%)
                dmg += dmg;
            }

            dmg = (int)((dmg * duelDmgDec) - defense);
            dmg += GetMasterSkillTreePhysicalPassiveDamageBonus(attacker, true);

            if (attacker.Attributes[Stats.IsTwoHandedWeaponEquipped] > 0)
            {
                dmg += (int)(dmg * attacker.Attributes[Stats.TwoHandedWeaponDamageIncrease]);
            }

            if (attacker is AttackerSurrogate)
            {
                dmg += (int)attacker.Attributes[Stats.RavenBonusDamage];

                if ((attributes & (DamageAttributes.Excellent | DamageAttributes.Critical)) == DamageAttributes.Undefined)
                {
                    dmg = (int)(dmg / 1.5);
                }
            }
        }
        else
        {
            // Wizardry, Curse, and Fenrir
            if (isExcellentHit)
            {
                dmg = (int)((baseMaxDamage * duelDmgDec) - defense);
                dmg = (int)((dmg * 1.2) + attacker.Attributes[Stats.ExcellentDamageBonus]);
                attributes |= DamageAttributes.Excellent;
            }
            else if (isCriticalHit)
            {
                dmg = (int)((baseMaxDamage * duelDmgDec) - defense);
                dmg += (int)attacker.Attributes[Stats.CriticalDamageBonus];
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

                dmg = (int)((dmg * duelDmgDec) - defense);
            }
        }

        dmg += (int)attacker.Attributes[Stats.GreaterDamageBonus];

        /*Scroll of Wrath/Wizardry and Remedy of Love go here (but for now they don't exist)*/

        if (damageType == DamageType.Wizardry || damageType == DamageType.Curse)
        {
            dmg += (int)(dmg * attacker.Attributes[Stats.BerserkerProficiencyMultiplier]);
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

        /*Scroll of Battle (crit dmg)/Strengthener (exc dmg) go here (but for now they don't exist)*/

        if (!isPvp && defender.Overrates(attacker))
        {
            dmg = (int)(dmg * 0.3);
        }

        dmg -= (int)(dmg * defender.Attributes[Stats.ArmorDamageDecrease]);

        var attackerLevel = attacker is Player ? attacker.Attributes[Stats.TotalLevel] :
            attacker is AttackerSurrogate ? attackerPlayer!.Attributes![Stats.Level] : attacker.Attributes[Stats.Level];
        var minLevelDmg = Math.Max(1, (int)attackerLevel / 10);
        if (dmg < minLevelDmg)
        {
            dmg = minLevelDmg;
        }

        dmg = (int)(dmg * attacker.Attributes[Stats.AttackDamageIncrease]);
        if (dmg > 1)
        {
            dmg = (int)(dmg * defender.Attributes[Stats.DamageReceiveDecrement]);
        }

        if (skill != null)
        {
            dmg = (int)(dmg * attacker.Attributes[Stats.SkillMultiplier]);
        }
        else if (attacker.Attributes[Stats.IsDinorantEquipped] > 0)
        {
            dmg = (int)(dmg * 1.3);
        }
        else
        {
            // Nothing to do
        }

        float manaToll = 0;
        var soulBarrierManaToll = defender.Attributes[Stats.SoulBarrierManaTollPerHit];
        if (soulBarrierManaToll > 0 && defender.Attributes[Stats.CurrentMana] > soulBarrierManaToll)
        {
            manaToll = soulBarrierManaToll;
            dmg -= (int)(dmg * defender.Attributes[Stats.SoulBarrierReceiveDecrement]);
        }

        dmg += (int)attacker.Attributes[Stats.FinalDamageBonus];

        if (isPvp && attacker is not AttackerSurrogate)
        {
            dmg += (int)attacker.Attributes[Stats.FinalDamageIncreasePvp];
            dmg += GetMasterSkillTreeMasteryPvpDamageBonus(attacker);

            if (attackerPlayer!.CurrentMiniGame?.Definition.Type == MiniGameType.ChaosCastle)
            {
                // In Chaos Castle PvP damage is halved (except for raven)
                dmg /= 2;
            }
        }

        if (dmg > 0)
        {
            // To-do: Ice arrow magic effect duration is reduced by 1s for every successful hit (dmg >0)
            //        Sleep magic effect is canceled for successful hit (dmg > 0)
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

            if (isClassicPvp && attackerPlayer!.CurrentMiniGame?.Definition.Type == MiniGameType.ChaosCastle)
            {
                // Further halve damage in Chaos Castle for classic PvP
                dmg /= 2;
            }
        }

        return defender.GetHitInfo((uint)dmg, attributes, attacker, (uint)manaToll);
    }

    /// <summary>
    /// Gets the hit information, calculates which part of the damage damages the shield and which the health.
    /// </summary>
    /// <param name="defender">The defender.</param>
    /// <param name="damage">The damage.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="manaToll">The mana reduction amount that is traded for damage (e.g. from Soul Barrier).</param>
    /// <returns>The calculated hit info.</returns>
    public static HitInfo GetHitInfo(this IAttackable defender, uint damage, DamageAttributes attributes, IAttacker attacker, uint manaToll = 0)
    {
        var shieldBypass = Rand.NextRandomBool(attacker.Attributes[Stats.ShieldBypassChance]);
        if (shieldBypass || defender.Attributes[Stats.CurrentShield] < 1)
        {
            return new HitInfo(damage, 0, attributes, manaToll);
        }

        var shieldRatio = 0.90;
        shieldRatio -= attacker.Attributes[Stats.ShieldDecreaseRateIncrease];
        shieldRatio += defender.Attributes[Stats.ShieldRateIncrease];
        shieldRatio = Math.Max(0, shieldRatio);
        shieldRatio = Math.Min(1, shieldRatio);
        return new HitInfo((uint)(damage * (1 - shieldRatio)), (uint)(damage * shieldRatio), attributes, manaToll);
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
    /// Applies the elemental effects of a player's skill to the target.
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
        if (resistance >= 255 || !Rand.NextRandomBool(1 / (resistance + 1)))
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
        if (resistance >= 255 || !Rand.NextRandomBool(1 / (resistance + 1)))
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
    /// <param name="addExtraManaCost">If set to <c>true</c>, <see cref="Stats.SkillExtraManaCost"/> applies.</param>
    /// <returns>The required value.</returns>
    public static int GetRequiredValue(this IAttacker attacker, AttributeRequirement attributeRequirement, bool addExtraManaCost = false)
    {
        var modifier = 1.0f;
        if (ReductionModifiers.TryGetValue(
                attributeRequirement.Attribute ?? throw Error.NotInitializedProperty(attributeRequirement, nameof(attributeRequirement.Attribute)),
                out var reductionAttribute))
        {
            modifier -= attacker.Attributes[reductionAttribute];
        }

        var extraCost = 0;
        if (addExtraManaCost && attributeRequirement.Attribute == Stats.CurrentMana)
        {
            extraCost = (int)attacker.Attributes[Stats.SkillExtraManaCost];
        }

        return (int)((attributeRequirement.MinimumValue + extraCost) * modifier);
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
        if (attacker is Player or AttackerSurrogate && defender is Player)
        {
            return Stats.DefensePvp;
        }

        return Stats.DefensePvm;
    }

    private static bool Overrates(this IAttackable defender, IAttacker attacker)
    {
        return defender.Attributes[Stats.DefenseRatePvm] > attacker.Attributes[Stats.AttackRatePvm];
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

    private static IAttributeSystem? EnsureSkillAttributes(this SkillEntry skillEntry, IAttributeSystem attackerSystem)
    {
        var skillAttributes = skillEntry.Attributes;
        if (skillAttributes is not null)
        {
            return skillAttributes;
        }

        if (skillEntry.Skill is not { } skill)
        {
            return skillAttributes;
        }

        var baseSkills = new List<Skill> { skill };
        baseSkills.AddRange(skillEntry.Skill.GetBaseSkills());

        if (baseSkills.All(s => s.AttributeRelationships.Count == 0))
        {
            return skillAttributes;
        }

        skillAttributes = skillEntry.Attributes = new AttributeSystem([], [], []);
        var levelElement = new SimpleElement(skillEntry.Level, AggregateType.AddRaw);
        skillEntry.PropertyChanged += (_, _) => levelElement.Value = skillEntry.Level;
        skillAttributes.AddElement(levelElement, Stats.SkillLevel);
        foreach (var relationship in baseSkills.SelectMany(s => s.AttributeRelationships))
        {
            skillAttributes.AddAttributeRelationship(relationship, attackerSystem, relationship.AggregateType);
        }

        return skillAttributes;
    }

    private static void GetSkillDmg(this IAttacker attacker, SkillEntry? skillEntry, out int skillMinimumDamage, out int skillMaximumDamage, out DamageType damageType, out bool isSummonerSkill)
    {
        skillMinimumDamage = 0;
        skillMaximumDamage = 0;
        var attackerStats = attacker.Attributes;
        damageType = DamageType.Physical;
        isSummonerSkill = attackerStats[Stats.MinimumCurseBaseDmg] > 0 && damageType != DamageType.Fenrir;

        if (skillEntry?.Skill is not { } skill)
        {
            return;
        }

        damageType = skill.DamageType;
        var skillDamage = skillEntry.GetDamage();
        skillMinimumDamage += skillDamage;
        skillMaximumDamage += skillDamage + (skillDamage / 2);

        if (skill.ElementalModifierTarget is { } resistance
            && Stats.ElementResistanceToDamageBonus.TryGetValue(resistance, out var dmgBonus))
        {
            skillMinimumDamage += (int)attackerStats[dmgBonus];
            skillMaximumDamage += (int)attackerStats[dmgBonus];
        }

        if (!isSummonerSkill && damageType != DamageType.Fenrir)
        {
            // For Summoner, the skill bonus gets added last
            skillMinimumDamage += (int)attackerStats[Stats.SkillDamageBonus];
            skillMaximumDamage += (int)attackerStats[Stats.SkillDamageBonus];
        }

        if (skillEntry.EnsureSkillAttributes(attackerStats) is { } skillAttributes)
        {
            skillMinimumDamage += (int)skillAttributes[Stats.SkillDamageBonus];
            skillMaximumDamage += (int)skillAttributes[Stats.SkillDamageBonus];

            var multiplier = skillAttributes[Stats.SkillMultiplier];
            if (multiplier > 0)
            {
                skillMinimumDamage = (int)(skillMinimumDamage * multiplier);
                skillMaximumDamage = (int)(skillMaximumDamage * multiplier);
            }
        }

        if (damageType == DamageType.Physical && attackerStats[Stats.HasDoubleWield] > 0)
        {
            // Because double wield dmg will be doubled later, we only take half of the skill dmg here (the skill is from a single weapon)
            skillMinimumDamage /= 2;
            skillMaximumDamage /= 2;
        }

        if (isSummonerSkill)
        {
            skillMinimumDamage += (int)(attackerStats[Stats.WizardryAndCurseBaseDmgBonus] + attackerStats[Stats.MinWizardryAndCurseDmgBonus]);
            skillMaximumDamage += (int)attackerStats[Stats.WizardryAndCurseBaseDmgBonus];

            if (damageType == DamageType.Wizardry)
            {
                skillMinimumDamage += (int)attackerStats[Stats.BerserkerMinWizDmgBonus];
                skillMaximumDamage += (int)attackerStats[Stats.BerserkerMaxWizDmgBonus];
            }
            else
            {
                skillMinimumDamage += (int)attackerStats[Stats.BerserkerMinCurseDmgBonus];
                skillMaximumDamage += (int)attackerStats[Stats.BerserkerMaxCurseDmgBonus];
            }
        }
    }

    /// <summary>
    /// Returns the base damage of the attacker, using a specific skill.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">Skill which is used.</param>
    /// <param name="minimumBaseDamage">Minimum base damage.</param>
    /// <param name="maximumBaseDamage">Maximum base damage.</param>
    /// <param name="damageType">The damage type.</param>
    private static void GetBaseDmg(this IAttacker attacker, SkillEntry? skill, out int minimumBaseDamage, out int maximumBaseDamage, out DamageType damageType)
    {
        minimumBaseDamage = 0;
        maximumBaseDamage = 0;
        var attackerStats = attacker.Attributes;
        GetSkillDmg(attacker, skill, out var skillMinimumDamage, out var skillMaximumDamage, out damageType, out var isSummonerSkill);

        switch (damageType)
        {
            // Common damage types
            case DamageType.Wizardry:
                minimumBaseDamage = (int)((attackerStats[Stats.MinimumWizBaseDmg] + skillMinimumDamage) * attackerStats[Stats.WizardryAttackDamageIncrease]);
                maximumBaseDamage = (int)((attackerStats[Stats.MaximumWizBaseDmg] + skillMaximumDamage) * attackerStats[Stats.WizardryAttackDamageIncrease]);
                break;
            case DamageType.Curse:
                minimumBaseDamage = (int)((attackerStats[Stats.MinimumCurseBaseDmg] + skillMinimumDamage) * attackerStats[Stats.CurseAttackDamageIncrease]);
                maximumBaseDamage = (int)((attackerStats[Stats.MaximumCurseBaseDmg] + skillMaximumDamage) * attackerStats[Stats.CurseAttackDamageIncrease]);
                break;
            case DamageType.Physical:
                minimumBaseDamage = (int)attackerStats[Stats.MinimumPhysBaseDmg] + skillMinimumDamage;
                maximumBaseDamage = (int)attackerStats[Stats.MaximumPhysBaseDmg] + skillMaximumDamage;
                break;
            case DamageType.Fenrir:
                minimumBaseDamage = (int)attackerStats[Stats.FenrirBaseDmg] + skillMinimumDamage;
                maximumBaseDamage = (int)attackerStats[Stats.FenrirBaseDmg] + skillMaximumDamage;
                break;
            default:
                // the skill has some other damage type defined which is not applicable to this calculation
                break;
        }

        if (isSummonerSkill)
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
                    // In case of a double wield with different possible bonuses, take the average
                    bonusDamage = (int)(bonusDamage == 0
                        ? attacker.Attributes[Stats.MaceBonusDamage]
                        : (bonusDamage + attacker.Attributes[Stats.MaceBonusDamage]) / 2);
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
}