// <copyright file="FixItemRequirementsPlugIn.cs" company="MUnique">
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
/// Updates some item requirements for elf bows which were intialized wrongly.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("9E8DB2CB-1972-40D3-9129-6964ABFEB4DC")]
public class FixItemRequirementsPlugIn : UpdatePlugInBase
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

        /*
         These are handled different, so they don't need to be corrected.
         See ItemExtensions.GetRequirement.
        (15, 0, 0, 0, 100, 0), // Scroll of Poison
        (15, 1, 0, 0, 100, 0), // Scroll of Meteorite
        (15, 2, 0, 0, 100, 0), // Scroll of Lighting
        (15, 3, 0, 0, 100, 0), // Scroll of Fire Ball
        (15, 4, 0, 0, 100, 0), // Scroll of Flame
        (15, 5, 0, 0, 100, 0), // Scroll of Teleport
        (15, 6, 0, 0, 100, 0), // Scroll of Ice
        (15, 7, 0, 0, 100, 0), // Scroll of Twister
        (15, 8, 0, 0, 100, 0), // Scroll of Evil Spirit
        (15, 9, 0, 0, 100, 0), // Scroll of Hellfire
        (15, 10, 0, 0, 100, 0), // Scroll of Power Wave
        (15, 11, 0, 0, 110, 0), // Scroll of Aqua Beam
        (15, 12, 0, 0, 150, 0), // Scroll of Cometfall
        (15, 13, 0, 0, 200, 0), // Scroll of Inferno
        (15, 14, 0, 0, 188, 0), // Scroll of Teleport Ally
        (15, 15, 0, 0, 126, 0), // Scroll of Soul Barrier
        (15, 16, 0, 0, 243, 0), // Scroll of Decay
        (15, 17, 0, 0, 223, 0), // Scroll of Ice Storm
        (15, 18, 0, 0, 258, 0), // Scroll of Nova

        (15, 19, 0, 0, 75, 0), // Chain Lightning Parchment
        (15, 20, 0, 0, 93, 0), // Drain Life Parchment
        (15, 21, 0, 0, 216, 0), // Lightning Shock Parchment
        (15, 22, 0, 0, 111, 0), // Damage Reflection Parchment
        (15, 23, 0, 0, 181, 0), // Berserker Parchment
        (15, 24, 0, 0, 100, 0), // Sleep Parchment
        (15, 26, 0, 0, 173, 0), // Weakness Parchment
        (15, 27, 0, 0, 201, 0), // Innovation Parchment
        (15, 28, 0, 0, 118, 0), // Scroll of Wizardry Enhance
        (15, 29, 0, 0, 118, 0), // Scroll of Gigantic Storm
        (15, 34, 0, 0, ?, 0), // Ignore Defense Parchment
        (15, 35, 0, 0, ?, 0), // Increase Health Parchment
        (15, 36, 0, 0, ?, 0), // Increase Block Parchment
        */
    ];

    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Item Requirements (Elf Bows)";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Updates some item requirements for elf bows which were intialized wrongly.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixItemRequirements;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 11, 03, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var reqUpdate in RequirementCorrections)
        {
            var item = gameConfiguration.Items.First(x => x.Number == reqUpdate.Number && x.Group == reqUpdate.Group);
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