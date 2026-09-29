// <copyright file="AddCrywolfEventUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the data of the crywolf event: the monsters of the army of Balgass and their spawns on the crywolf map.
/// The statue and the altars of the fortress are always on the map now, as NPCs which can't be attacked,
/// the fortress can be warped to, and the map has the terrains of the occupation states.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9C4E1A73-2F58-4B6D-8E09-5A3D7C1F2B84")]
public class AddCrywolfEventUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Crywolf event";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the monsters of the crywolf event and their spawns, lets the statue and the altars of the fortress always be on the map, adds the warp entry of the fortress and the terrains of the occupation states.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddCrywolfEvent;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 28, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var initializer = new CrywolfInitializer(context, gameConfiguration);
        initializer.CreateMonsters();
        initializer.ConfigureStatueAndAltars();
        initializer.CreateSpawns();
        initializer.CreateWarpEntry();
        initializer.CreateTerrainVariants();
        return ValueTask.CompletedTask;
    }
}
