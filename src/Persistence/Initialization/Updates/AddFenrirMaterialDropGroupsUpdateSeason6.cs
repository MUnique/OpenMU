// <copyright file="AddFenrirMaterialDropGroupsUpdateSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the drop item groups for the materials of the Horn of Fenrir
/// (Splinter of Armor, Bless of Guardian and Claw of Beast) to Crywolf.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFenrirMaterialDropGroupsUpdateSeason6_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddFenrirMaterialDropGroupsUpdateSeason6_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("3C0E6B55-9D2F-4A71-8E43-5B7F1A2D9C64")]
public class AddFenrirMaterialDropGroupsUpdateSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Fenrir Material Drop Groups";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Adds the drops of Splinter of Armor (5 %), Bless of Guardian (2 %) and Claw of Beast (0.5 %) to all monsters of Crywolf. Materials which already drop on Crywolf by another group are skipped.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 29, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Maps.FirstOrDefault(m => m.Number == CrywolfFortress.Number && m.Discriminator == 0) is { } map)
        {
            CrywolfFortress.AddFenrirMaterialDropGroups(context, gameConfiguration, map);
        }

        return ValueTask.CompletedTask;
    }
}
