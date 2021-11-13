// <copyright file="SummoningOrbConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The summoning orb consume handler.
/// There is only one "Orb" item definition which allows to learn different skills, depending on the item level.
/// This consume handler determines the skill by adding the item level to the skill number
/// of the <see cref="ItemDefinition.Skill"/>.
/// </summary>
public class SummoningOrbConsumeHandler : LearnablesConsumeHandler
{
    /// <summary>
    /// Gets the learnable skill.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>
    /// The skill to learn.
    /// </returns>
    protected override Skill GetLearnableSkill(Item item, GameConfiguration gameConfiguration)
    {
        item.ThrowNotInitializedProperty(item.Definition?.Skill is null, "Definition.Skill");

        var baseSkillNumber = item.Definition.Skill.Number;
        var targetSkillNumber = baseSkillNumber + item.Level;
        return gameConfiguration.Skills.First(s => s.Number == targetSkillNumber);
    }
}