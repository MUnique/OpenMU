// <copyright file="LearnablesConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Consume handler for items (e.g. scrolls, orbs) which add a skill when being consumed.
    /// </summary>
    public class LearnablesConsumeHandler : IItemConsumeHandler
    {
        /// <inheritdoc/>
        public bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return false;
            }

            if (item == null)
            {
                return false;
            }

            // Check Requirements
            if (!player.CompliesRequirements(item))
            {
                return false;
            }

            var skill = this.GetLearnableSkill(item, player.GameContext.Configuration);

            if (skill == null || player.SkillList.ContainsSkill(skill.Number.ToUnsigned()))
            {
                return false;
            }

            player.SkillList.AddLearnedSkill(skill);
            player.Inventory.RemoveItem(item);
            player.PersistenceContext.Delete(item);
            return true;
        }

        /// <summary>
        /// Gets the learnable skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>The skill to learn.</returns>
        protected virtual Skill GetLearnableSkill(Item item, GameConfiguration gameConfiguration)
        {
            return item.Definition.Skill;
        }
    }
}
