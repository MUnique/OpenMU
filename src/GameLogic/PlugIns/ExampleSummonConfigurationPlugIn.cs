// <copyright file="ExampleSummonConfigurationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Linq;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Example plug-in which replaces the default goblin summon with a Tantal and adjusts its stats.
/// </summary>
[Guid("27F0E8B5-61D3-4FB4-96F2-8B1ED2BB8C54")]
[PlugIn(nameof(ExampleSummonConfigurationPlugIn), "Example plug-in which replaces the goblin summon with a Tantal and adjusts its stats.")]
public class ExampleSummonConfigurationPlugIn : ISummonConfigurationPlugIn
{
    /// <inheritdoc />
    public MonsterDefinition? CreateSummonMonsterDefinition(Player player, Skill skill, MonsterDefinition? defaultDefinition)
    {
        if (skill.Number != 30) // goblin summon
        {
            return defaultDefinition;
        }

        var baseDefinition = player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == 58); // Tantal
        if (baseDefinition is null)
        {
            return defaultDefinition;
        }

        var clone = baseDefinition.Clone(player.GameContext.Configuration);
        var healthAttribute = clone.Attributes.FirstOrDefault(a => a.AttributeDefinition == Stats.MaximumHealth);
        if (healthAttribute is not null)
        {
            healthAttribute.Value *= 2;
        }

        return clone;
    }
}

