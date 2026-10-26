// <copyright file="SkillResourceExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Extensions to check the resources of a player for a skill, without consuming them.
/// </summary>
internal static class SkillResourceExtensions
{
    /// <summary>
    /// Checks that the player has enough mana AND ability (AG) to cast the skill.
    /// Both resources are consumed by skills via <see cref="Skill.ConsumeRequirements"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillEntry">The skill entry.</param>
    /// <returns><c>true</c>, if the player has enough resources to cast the skill.</returns>
    internal static bool HasEnoughResources(this Player player, SkillEntry? skillEntry)
    {
        if (skillEntry?.Skill is not { } skill || player.Attributes is null)
        {
            return true;
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            int required = player.GetRequiredValue(requirement, skillEntry);
            if (player.Attributes[requirement.Attribute] < required)
            {
                return false;
            }
        }

        return true;
    }
}
