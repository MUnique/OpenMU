// <copyright file="TargettedSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to perform a skill which is explicitly aimed to a target.
    /// </summary>
    public class TargettedSkillAction
    {
        /// <summary>
        /// Performs the skill.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="target">The target.</param>
        /// <param name="skillId">The skill identifier.</param>
        public void PerformSkill(Player player, IAttackable target, ushort skillId)
        {
            SkillEntry skillEntry = player.SkillList.GetSkill(skillId);
            var skill = skillEntry.Skill;
            if (skill.SkillType == SkillType.PassiveBoost)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            if (!target.Alive)
            {
                return;
            }

            var targetPlayer = target as Player;
            bool targetIsPlayer = targetPlayer != null; // is it a player?

            // enough mana, ag etc?
            if (!player.TryConsumeForSkill(skill))
            {
                return;
            }

            player.ForEachObservingPlayer(obs => obs.PlayerView.WorldView.ShowSkillAnimation(player, target, skill), true);
            if (!player.IsInRange(target.X, target.Y, skill.Range + 2))
            {
                return;
            }

            if (skill.SkillType == SkillType.DirectHit || skill.SkillType == SkillType.CastleSiegeSkill)
            {
                target.AttackBy(player, skillEntry);
            }

            if (skill.SkillType == SkillType.Buff && skill.MagicEffectDef != null && targetIsPlayer)
            {
                // TODO: Summoned Monsters should be able to receive a buff too - add an interface ICanReceiveBuff which contains the MagicEffectList
                if (skillEntry.BuffPowerUp == null)
                {
                    this.CreateBuffPowerUp(player, skillEntry);
                }

                var magicEffect = new MagicEffect(skillEntry.BuffPowerUp, skill.MagicEffectDef, new TimeSpan(0, 0, 0, 0, (int)skillEntry.PowerUpDuration.Value));
                targetPlayer.MagicEffectList.AddEffect(magicEffect);
            }
        }

        private void CreateBuffPowerUp(Player player, SkillEntry skillEntry)
        {
            var skill = skillEntry.Skill;
            var powerUpDef = skill.MagicEffectDef.PowerUpDefinition;
            skillEntry.BuffPowerUp = this.GetElement(player, powerUpDef.Boost);
            skillEntry.PowerUpDuration = this.GetElement(player, powerUpDef.Duration);
        }

        private IElement GetElement(Player player, PowerUpDefinitionValue value)
        {
            var relations = value.RelatedValues;
            IElement result = value.ConstantValue;
            if (relations != null)
            {
                var elements = relations
                    .Select(r => new AttributeRelationshipElement(
                                       new[] { player.Attributes.GetOrCreateAttribute(r.InputAttribute) },
                                       r.InputOperand,
                                       r.InputOperator))
                    .Cast<IElement>();
                if (value.ConstantValue != null)
                {
                    elements = elements.Concat(value.ConstantValue.GetAsEnumerable());
                }

                result = new AttributeRelationshipElement(elements.ToList(), 1.0F, InputOperator.Multiply);
            }

            if (result == null)
            {
                throw new ArgumentException();
            }

            return result;
        }
    }
}
