// <copyright file="AddRenaItemUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the missing Rena item, which is required for the item registration feature.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("6A1B7C3D-2E5F-4A7B-8C9D-1E1F2A3B4C6E")]
public class AddRenaItemUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Rena Item";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the missing Rena item, which is required for the item registration feature.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddRenaItem;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 26, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var itemDefinition = VersionSeasonSix.RenaDropSupport.GetOrCreateItem(context, gameConfiguration);
        var dropItemGroup = VersionSeasonSix.RenaDropSupport.GetOrCreateDropItemGroup(context, gameConfiguration, itemDefinition);

        // Always (re-)wire, even if the item/group already existed - a previous, incomplete run
        // must not be able to skip this step.
        VersionSeasonSix.RenaDropSupport.WireToEventMaps(gameConfiguration, dropItemGroup);
    }
}
