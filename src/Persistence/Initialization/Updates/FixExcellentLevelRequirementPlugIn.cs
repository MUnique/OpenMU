// <copyright file="FixExcellentLevelRequirementPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update lets the excellent version of the 380 items and the Horn of Fenrir need the same level as the normal one.
/// </summary>
/// <remarks>
/// The level requirement of every excellent item was raised, so e.g. an excellent Dragon Knight Armor needed level 400.
/// The original game server doesn't raise it for weapons and armor pieces, and the client shows the level without
/// the raise for these items (see <see cref="ExcellentLevelRequirement"/>).
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixExcellentLevelRequirementPlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixExcellentLevelRequirementPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
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
    internal const string PlugInDescription = "Lets the excellent version of the 380 items and the Horn of Fenrir need the same level as the normal one.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
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
