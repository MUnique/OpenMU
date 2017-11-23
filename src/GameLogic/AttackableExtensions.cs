// <copyright file="AttackableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Extensions for <see cref="IAttackable"/>s. Contains the damage calculation.
    /// </summary>
    public static class AttackableExtensions
    {
        /// <summary>
        /// Calculates the damage, using a skill.
        /// </summary>
        /// <param name="attacker">The object which is attacking.</param>
        /// <param name="defender">The object which is defending.</param>
        /// <param name="skill">The skill which is used.</param>
        /// <returns>The hit information.</returns>
        public static HitInfo CalculateDamage(this IAttackable attacker, IAttackable defender, SkillEntry skill)
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
                var defenseAttribute = GetDefenseAttribute(attacker);
                dmg -= (int)defender.Attributes[defenseAttribute];
                attributes |= DamageAttributes.IgnoreDefense;
            }

            dmg = Math.Max(dmg, 0);

            dmg = (int)(dmg * defender.Attributes[Stats.DamageReceiveDecrement]);
            dmg = (int)(dmg * attacker.Attributes[Stats.AttackDamageIncrease]);

            if (skill != null)
            {
                dmg = (int)(dmg * attacker.Attributes[Stats.SkillMultiplier]);
            }

            bool isDoubleDamage = Rand.NextRandomBool(attacker.Attributes[Stats.DoubleDamageChance]);
            if (isDoubleDamage)
            {
                dmg *= 2;
                attributes |= DamageAttributes.Double;
            }

            // now we have the final damage calculated. We have to calculate which part of the damage damages the shield and which the health.
            HitInfo hi;
            var shieldBypass = Rand.NextRandomBool(attacker.Attributes[Stats.ShieldBypassChance]);
            if (shieldBypass || defender.Attributes[Stats.CurrentShield] < 1)
            {
                hi = new HitInfo((uint)dmg, 0, 0);
            }
            else
            {
                var shieldRatio = 0.90;
                shieldRatio -= attacker.Attributes[Stats.ShieldDecreaseRateIncrease];
                shieldRatio += defender.Attributes[Stats.ShieldRateIncrease];
                shieldRatio = Math.Max(0, shieldRatio);
                shieldRatio = Math.Min(1, shieldRatio);
                hi = new HitInfo((uint)(dmg * (1 - shieldRatio)), (uint)(dmg * shieldRatio), attributes);
            }

            return hi;
        }

        private static bool IsAttackSuccessfulTo(this IAttackable attacker, IAttackable defender)
        {
            var hitChance = attacker.GetHitChanceTo(defender);
            return Rand.NextRandomBool(hitChance);
        }

        private static float GetHitChanceTo(this IAttackable attacker, IAttackable defender)
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

        private static AttributeDefinition GetDefenseAttribute(this IAttackable attacker)
        {
            if (attacker is Player)
            {
                return Stats.DefensePvp;
            }

            return Stats.DefensePvm;
        }

        /// <summary>
        /// Returns the base damage if the attacker, using a specific skill.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="skill">Skill which is used.</param>
        /// <param name="basemin">Minimum base damage.</param>
        /// <param name="basemax">Maximum base damage.</param>
        private static void GetBaseDmg(this IAttackable attacker, SkillEntry skill, out int basemin, out int basemax)
        {
            basemin = 0;
            basemax = 0;
            var attackerStats = attacker.Attributes;
            DamageType damageType = DamageType.Physical;
            if (skill != null)
            {
                damageType = skill.Skill.DamageType;
                var skillDamage = skill.Skill.AttackDamage.Where(d => d.Level == skill.Level).Select(d => d.Damage).FirstOrDefault();
                basemin += skillDamage;
                basemax += skillDamage;
            }

            if (damageType == DamageType.Physical)
            {
                basemin = (int)attackerStats[Stats.MinimumPhysBaseDmg];
                basemax = (int)attackerStats[Stats.MaximumPhysBaseDmg];
            }
            else if (damageType == DamageType.Wizardry)
            {
                basemin = (int)attackerStats[Stats.MinimumWizBaseDmg];
                basemax = (int)attackerStats[Stats.MaximumWizBaseDmg];
            }
            else if (damageType == DamageType.Curse)
            {
                basemin = (int)attackerStats[Stats.MinimumCurseBaseDmg];
                basemax = (int)attackerStats[Stats.MaximumCurseBaseDmg];
            }
        }
    }
}
