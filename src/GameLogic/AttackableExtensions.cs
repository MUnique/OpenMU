// <copyright file="AttackableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
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
        /// <returns>The hit information.</returns>
        public static HitInfo CalculateDamage(this IAttacker attacker, IAttackable defender, SkillEntry? skill)
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
                if (defender.Attributes[Stats.IsShieldEquipped] > 0)
                {
                    defense += (int)(defense * defender.Attributes[Stats.DefenseIncreaseWithEquippedShield]);
                }

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

            bool isDoubleDamage = Rand.NextRandomBool(attacker.Attributes[Stats.DoubleDamageChance]);
            if (isDoubleDamage)
            {
                dmg *= 2;
                attributes |= DamageAttributes.Double;
            }

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
        public static HitInfo GetHitInfo(this IAttackable defender, uint damage, DamageAttributes attributes,  IAttacker attacker)
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
        public static void ApplyMagicEffect(this IAttackable target, Player player, SkillEntry skillEntry)
        {
            if (skillEntry.BuffPowerUp is null)
            {
                player.CreateMagicEffectPowerUp(skillEntry);
            }

            var magicEffect = skillEntry.Skill!.MagicEffectDef?.PowerUpDefinition!.TargetAttribute == Stats.IsPoisoned
                ? new PoisonMagicEffect(skillEntry.BuffPowerUp!, skillEntry.Skill!.MagicEffectDef!, TimeSpan.FromSeconds(skillEntry.PowerUpDuration!.Value), player, target)
                : new MagicEffect(skillEntry.BuffPowerUp!, skillEntry.Skill!.MagicEffectDef!, TimeSpan.FromSeconds(skillEntry.PowerUpDuration!.Value));

            target.MagicEffectList.AddEffect(magicEffect);
        }

        /// <summary>
        /// Applies the regeneration of the players skill to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="player">The player.</param>
        /// <param name="skillEntry">The skill entry.</param>
        public static void ApplyRegeneration(this IAttackable target, Player player, SkillEntry skillEntry)
        {
            if (player.Attributes is null)
            {
                return;
            }

            skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

            var skill = skillEntry.Skill;
            var regeneration = Stats.IntervalRegenerationAttributes.FirstOrDefault(r => r.CurrentAttribute == skill.MagicEffectDef?.PowerUpDefinition?.TargetAttribute);
            if (regeneration != null)
            {
                var powerUpDefinition = skill.MagicEffectDef!.PowerUpDefinition!;
                var regenerationValue = player.Attributes.CreateElement(powerUpDefinition.Boost ?? throw Error.NotInitializedProperty(powerUpDefinition, nameof(powerUpDefinition.Boost)));
                var value = skillEntry.Level == 0 ? regenerationValue.Value : regenerationValue.Value + skillEntry.CalculateValue();
                target.Attributes[regeneration.CurrentAttribute] = Math.Min(
                    target.Attributes[regeneration.CurrentAttribute] + value,
                    target.Attributes[regeneration.MaximumAttribute]);
            }
            else
            {
                player.Logger.LogWarning(
                    $"Regeneration skill {skill.Name} is configured to regenerate a non-regeneration-able target attribute {skill.MagicEffectDef?.PowerUpDefinition?.TargetAttribute}.");
            }
        }

        /// <summary>
        /// Applies the elemental effects of a players skill to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="player">The player.</param>
        /// <param name="skillEntry">The skill entry.</param>
        /// <returns>The success of the appliance.</returns>
        public static bool TryApplyElementalEffects(this IAttackable target, Player player, SkillEntry skillEntry)
        {
            skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
            var modifier = skillEntry.Skill.ElementalModifierTarget;
            if (modifier is null)
            {
                return false;
            }

            var resistance = target.Attributes[modifier];
            if (resistance >= 1.0f)
            {
               return false;
            }

            if (target.MagicEffectList.ActiveEffects.ContainsKey(skillEntry.Skill.MagicEffectDef!.Number))
            {
                return false;
            }

            if (Rand.NextRandomBool(1.0f - resistance))
            {
                // power-up is the wrong term here... it's more like a power-down ;-)
                if (skillEntry.Skill.MagicEffectDef != null)
                {
                    target.ApplyMagicEffect(player, skillEntry);
                }

                if (modifier == Stats.LightningResistance)
                {
                    target.MoveRandomly();
                }
            }

            return true;
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
        public static void MoveRandomly(this IAttackable target)
        {
            if (target is IMovable movable && target.CurrentMap is { } map)
            {
                var terrain = map.Terrain;
                var newX = target.Position.X + Rand.NextInt(-1, 2);
                var newY = target.Position.Y + Rand.NextInt(-1, 2);
                var isNewXAllowed = newX >= byte.MinValue && newX <= byte.MaxValue;
                var isNewYAllowed = newY >= byte.MinValue && newY <= byte.MaxValue;
                if (isNewXAllowed && isNewYAllowed && terrain.AIgrid[newX, newY] == 1)
                {
                    movable.Move(new Point((byte)newX, (byte)newY));
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
                maximumBaseDamage += skillDamage;
            }

            switch (damageType)
            {
                case DamageType.Wizardry:
                    minimumBaseDamage += (int)(attackerStats[Stats.MinimumWizBaseDmg] * attackerStats[Stats.WizardryAttackDamageIncrease]);
                    maximumBaseDamage += (int)(attackerStats[Stats.MaximumWizBaseDmg] * attackerStats[Stats.WizardryAttackDamageIncrease]);

                    break;
                case DamageType.Curse:
                    minimumBaseDamage += (int)(attackerStats[Stats.MinimumCurseBaseDmg] * attackerStats[Stats.CurseAttackDamageIncrease]);
                    maximumBaseDamage += (int)(attackerStats[Stats.MaximumCurseBaseDmg] * attackerStats[Stats.CurseAttackDamageIncrease]);

                    break;
                case DamageType.Physical:
                    minimumBaseDamage += (int)attackerStats[Stats.MinimumPhysBaseDmg];
                    maximumBaseDamage += (int)attackerStats[Stats.MaximumPhysBaseDmg];
                    break;
                default:
                    // the skill has some other damage type defined which is not applicable to this calculation
                    break;
            }
        }
    }
}
