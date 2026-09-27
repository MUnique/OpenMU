// <copyright file="LearnableSkillRequirements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The skill which an item (orb, scroll, parchment, crystal) teaches, and what it takes to learn it.
/// </summary>
/// <remarks>
/// A skill which can't be used after learning it shouldn't be learnable either, so learning it
/// requires the highest of the requirements of the item and the requirements of the skill.
/// </remarks>
public static class LearnableSkillRequirements
{
    /// <summary>
    /// Gets the skill which is learned by consuming an item.
    /// </summary>
    /// <param name="definition">The definition of the item.</param>
    /// <param name="itemLevel">The level of the item.</param>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>The skill, or <c>null</c> if the item teaches none.</returns>
    public static Skill? GetLearnableSkill(ItemDefinition definition, byte itemLevel, GameConfiguration configuration)
    {
        if (definition.Skill is not { } skill)
        {
            return null;
        }

        // There is only one Orb of Summoning, which teaches a different skill per item level.
        if (IsSummoningOrb(definition))
        {
            var skillNumber = skill.Number + itemLevel;
            return configuration.Skills.FirstOrDefault(s => s.Number == skillNumber);
        }

        return skill;
    }

    /// <summary>
    /// Determines whether the item teaches a different skill per item level.
    /// </summary>
    /// <param name="definition">The definition of the item.</param>
    /// <returns><c>true</c>, if it's the Orb of Summoning.</returns>
    public static bool IsSummoningOrb(ItemDefinition definition)
    {
        return definition.Group == ItemConstants.SummonOrb.Group && definition.Number == ItemConstants.SummonOrb.Number;
    }

    /// <summary>
    /// Gets the requirements to learn the skill with the item, by the attribute of the character which has to reach them.
    /// </summary>
    /// <param name="definition">The definition of the item. It's not wearable, so its requirements don't depend on its level or options.</param>
    /// <param name="skill">The skill which is learned with the item.</param>
    /// <returns>The required value per attribute.</returns>
    public static IReadOnlyDictionary<AttributeDefinition, int> GetRequirements(ItemDefinition definition, Skill skill)
    {
        var result = new Dictionary<AttributeDefinition, int>();
        foreach (var requirement in definition.Requirements)
        {
            if (requirement.Attribute is { } attribute)
            {
                Raise(result, ItemExtensions.GetRequiredAttribute(attribute), requirement.MinimumValue);
            }
        }

        foreach (var requirement in skill.Requirements)
        {
            if (requirement.Attribute is { } attribute)
            {
                Raise(result, attribute, requirement.MinimumValue);
            }
        }

        return result;
    }

    /// <summary>
    /// Determines whether the player complies with the requirements to use the skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skill">The skill.</param>
    /// <returns><c>true</c>, if the player can use the skill.</returns>
    public static bool CompliesRequirements(this Player player, Skill skill)
    {
        return skill.Requirements.All(r => r.Attribute is null || player.Attributes![r.Attribute] >= r.MinimumValue);
    }

    private static void Raise(Dictionary<AttributeDefinition, int> requirements, AttributeDefinition attribute, int value)
    {
        if (!requirements.TryGetValue(attribute, out var current) || current < value)
        {
            requirements[attribute] = value;
        }
    }
}
