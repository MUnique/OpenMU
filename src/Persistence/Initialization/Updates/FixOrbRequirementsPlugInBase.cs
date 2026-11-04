// <copyright file="FixOrbRequirementsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Fixes the energy, strength and agility requirements of the items which are used to learn skills.
/// They were initialized with the requirement factors of the item files, or with the values of the
/// skill table, instead of the calculated requirements, see <see cref="OrbRequirements"/>.
/// </summary>
public abstract class FixOrbRequirementsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plugin name.
    /// </summary>
    internal const string PlugInName = "Fix Orb Requirements";

    /// <summary>
    /// The plugin description.
    /// </summary>
    internal const string PlugInDescription = "Fixes the energy, strength and agility requirements of orbs, crystals and scrolls which are used to learn skills. E.g. the Orbs of Healing, Greater Defense and Greater Damage required 100 energy instead of 52, 72 and 92.";

    private static readonly IReadOnlyDictionary<AttributeDefinition, AttributeDefinition> RequirementValueStats = new Dictionary<AttributeDefinition, AttributeDefinition>
    {
        { Stats.TotalEnergy, Stats.TotalEnergyRequirementValue },
        { Stats.TotalStrength, Stats.TotalStrengthRequirementValue },
        { Stats.TotalAgility, Stats.TotalAgilityRequirementValue },
    };

    /// <summary>
    /// The corrected requirements of the orbs of the fairy elf, which exist in all versions.
    /// </summary>
    private protected static readonly IReadOnlyList<(byte Group, byte Number, int Energy, int Strength, int Agility)> FairyElfOrbs =
    [
        (12, 8, 52, 0, 0), // Orb of Healing
        (12, 9, 72, 0, 0), // Orb of Greater Defense
        (12, 10, 92, 0, 0), // Orb of Greater Damage
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the corrected requirements of the items, by item group and number.
    /// A value of 0 means the requirement doesn't need a correction.
    /// </summary>
    private protected abstract IReadOnlyList<(byte Group, byte Number, int Energy, int Strength, int Agility)> Corrections { get; }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var correction in this.Corrections)
        {
            if (gameConfiguration.Items.FirstOrDefault(item => item.Group == correction.Group && item.Number == correction.Number) is not { } item)
            {
                continue;
            }

            SetRequirement(item, Stats.TotalEnergy, correction.Energy);
            SetRequirement(item, Stats.TotalStrength, correction.Strength);
            SetRequirement(item, Stats.TotalAgility, correction.Agility);
        }

        return ValueTask.CompletedTask;

        void SetRequirement(ItemDefinition item, AttributeDefinition stat, int value)
        {
            if (value == 0)
            {
                return;
            }

            if (FindRequirement(item, stat) is not { } requirement)
            {
                requirement = context.CreateNew<AttributeRequirement>();
                requirement.Attribute = stat.GetPersistent(gameConfiguration);
                item.Requirements.Add(requirement);
            }

            requirement.MinimumValue = value;
        }
    }

    /// <summary>
    /// Finds the existing requirement of a stat. The scrolls of group 15 define their energy
    /// requirement with <see cref="Stats.TotalEnergyRequirementValue"/>, which the game logic maps
    /// to <see cref="Stats.TotalEnergy"/> for items which aren't wearable, so that one is kept.
    /// </summary>
    private static AttributeRequirement? FindRequirement(ItemDefinition item, AttributeDefinition stat)
    {
        var requirementValueStat = RequirementValueStats.GetValueOrDefault(stat);
        return item.Requirements.FirstOrDefault(r => r.Attribute == stat || (requirementValueStat is not null && r.Attribute == requirementValueStat));
    }
}
