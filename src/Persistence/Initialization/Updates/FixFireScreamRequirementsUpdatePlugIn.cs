// <copyright file="FixFireScreamRequirementsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update corrects the energy and leadership requirements of the fire scream skill to 150 and 70.
/// They were swapped, so a dark lord with less than 150 leadership could learn the skill but not use it.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixFireScreamRequirementsUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixFireScreamRequirementsUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("35E4FFDD-A427-4B0F-86F6-610233AFAC82")]
public class FixFireScreamRequirementsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Fire Scream requirements";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update corrects the energy and leadership requirements of the fire scream skill to 150 and 70. They were swapped, so a dark lord with less than 150 leadership could learn the skill but not use it.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 22, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.FireScream) is not { } fireScream)
        {
            return ValueTask.CompletedTask;
        }

        var energy = FindRequirement(fireScream, Stats.TotalEnergy);
        var leadership = FindRequirement(fireScream, Stats.TotalLeadership);

        // Only the swapped initial values are corrected, so a deliberately changed configuration stays as it is.
        if (energy?.MinimumValue == 70 && leadership?.MinimumValue == 150)
        {
            energy.MinimumValue = 150;
            leadership.MinimumValue = 70;
        }

        return ValueTask.CompletedTask;
    }

    private static AttributeRequirement? FindRequirement(Skill skill, AttributeDefinition stat)
    {
        return skill.Requirements.FirstOrDefault(r => r.Attribute == stat);
    }
}
