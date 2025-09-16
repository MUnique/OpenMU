// <copyright file="FixItemRequirementsPlugIn2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates some item requirements for elf bows which were initialized wrongly.
/// This plugin fixes configurations that were created after the initial fix but before the base data was corrected.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("A7C9D4E1-8F2B-4A3C-9E6D-7B8F9A0E1C2D")]
public class FixItemRequirementsPlugIn2 : UpdatePlugInBase
{
    private static readonly List<(int Group, int Number, int StrengthRequirement, int AgilityRequirement, int EnergyRequirement, int VitalityRequirement)> RequirementCorrections =
    [
        (4, 0, 20, 80, 0, 0), // Short Bow
        (4, 1, 30, 90, 0, 0), // Bow
        (4, 2, 30, 90, 0, 0), // Elven Bow
        (4, 3, 30, 90, 0, 0), // Battle Bow
        (4, 4, 30, 100, 0, 0), // Tiger Bow
        (4, 5, 30, 100, 0, 0), // Silver Bow
        (4, 6, 40, 150, 0, 0), // Chaos Nature Bow
    ];

    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Item Requirements (Elf Bows) v2";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Updates some item requirements for elf bows which were initialized wrongly. This fixes configurations created after the initial fix but before the base data was corrected.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixItemRequirements2;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 09, 16, 19, 44, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var reqUpdate in RequirementCorrections)
        {
            var item = gameConfiguration.Items.FirstOrDefault(x => x.Number == reqUpdate.Number && x.Group == reqUpdate.Group);
            if (item == null)
            {
                continue;
            }

            UpdateRequirement(Stats.TotalStrengthRequirementValue, reqUpdate.StrengthRequirement);
            UpdateRequirement(Stats.TotalAgilityRequirementValue, reqUpdate.AgilityRequirement);
            UpdateRequirement(Stats.TotalEnergyRequirementValue, reqUpdate.EnergyRequirement);
            UpdateRequirement(Stats.TotalVitalityRequirementValue, reqUpdate.VitalityRequirement);

            void UpdateRequirement(AttributeDefinition stat, int newValue)
            {
                var requirement = item.Requirements.FirstOrDefault(r => r.Attribute == stat);
                if (requirement is null && newValue == 0)
                {
                    return;
                }

                if (requirement is null)
                {
                    requirement = context.CreateNew<AttributeRequirement>();
                    requirement.Attribute = stat.GetPersistent(gameConfiguration);
                    item.Requirements.Add(requirement);
                }

                requirement.MinimumValue = newValue;
            }
        }
    }
}