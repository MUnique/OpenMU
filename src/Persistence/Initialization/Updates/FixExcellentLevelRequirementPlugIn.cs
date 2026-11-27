// <copyright file="FixExcellentLevelRequirementPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update lets the excellent version of the weapons, shields and armor pieces and of the Horn of Fenrir need the
/// same level as the normal one.
/// </summary>
/// <remarks>
/// The data raised the level requirement of every excellent item except the lucky items. With an
/// <see cref="ItemStatProfile.ExcellentLevelAdd"/> of 20, as in the original game, e.g. an excellent Dragon Knight Armor
/// would need level 400. The built-in item stat profiles add no level, so with them the update changes no requirement.
/// The original game servers don't raise it for weapons, shields and armor pieces (see
/// <see cref="ExcellentLevelRequirement"/>).
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.FixExcellentLevelRequirementPlugIn_Name), Description = nameof(PlugInResources.FixExcellentLevelRequirementPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("ED3D4F9D-301E-48FA-9EA6-F85E47FDDD73")]
public class FixExcellentLevelRequirementPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Excellent Level Requirement";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Switches off \"Excellent Raises Level Requirement\" for the weapons, shields and armor pieces with a level requirement and for the Horn of Fenrir, so that their excellent version needs the same level as the normal one, as on the original game servers.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    /// <remarks>
    /// Mandatory, because the value was wrong in the data, and the item exchange file carries it to MuEditor, which would
    /// otherwise offer to change these items in the client to the wrong value.
    /// </remarks>
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 04, 21, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        ExcellentLevelRequirement.Apply(gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
