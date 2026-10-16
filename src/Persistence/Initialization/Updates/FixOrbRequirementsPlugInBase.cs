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
/// They were initialized with the requirement factors of the item files instead of the calculated
/// requirements, see <see cref="OrbRequirements"/>.
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

    /// <summary>
    /// The corrected requirements of the orbs of the fairy elf, which exist in all versions.
    /// </summary>
    private protected static readonly IReadOnlyList<(byte Number, int Energy, int Strength, int Agility)> FairyElfOrbs =
    [
        (8, 52, 0, 0), // Orb of Healing
        (9, 72, 0, 0), // Orb of Greater Defense
        (10, 92, 0, 0), // Orb of Greater Damage
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
    /// Gets the corrected requirements of the items of group 12, by item number.
    /// A value of 0 means the requirement doesn't need a correction.
    /// </summary>
    private protected abstract IReadOnlyList<(byte Number, int Energy, int Strength, int Agility)> Corrections { get; }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var correction in this.Corrections)
        {
            if (gameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == correction.Number) is not { } item)
            {
                continue;
            }

            SetRequirement(Stats.TotalEnergy, correction.Energy);
            SetRequirement(Stats.TotalStrength, correction.Strength);
            SetRequirement(Stats.TotalAgility, correction.Agility);

            void SetRequirement(AttributeDefinition stat, int value)
            {
                if (value == 0)
                {
                    return;
                }

                if (item.Requirements.FirstOrDefault(r => r.Attribute == stat) is not { } requirement)
                {
                    requirement = context.CreateNew<AttributeRequirement>();
                    requirement.Attribute = stat.GetPersistent(gameConfiguration);
                    item.Requirements.Add(requirement);
                }

                requirement.MinimumValue = value;
            }
        }

        return ValueTask.CompletedTask;
    }
}
