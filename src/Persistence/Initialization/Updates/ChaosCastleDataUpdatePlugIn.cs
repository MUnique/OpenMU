// <copyright file="ChaosCastleDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The chaos castle update plugin.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("13059991-F3C8-4050-A201-6D6A67E57541")]
public class ChaosCastleDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Chaos Castle Data";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update creates the configuration data for the chaos castle event.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ChaosCastleDataUpdate;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 03, 05, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        // First check if an update is required.
        if (gameConfiguration.MiniGameDefinitions.Any(def => def.Type == MiniGameType.ChaosCastle))
        {
            // There is already a chaos castle definition, so we can skip this update
            return;
        }

        var initializer = new ChaosCastleInitializer(context, gameConfiguration);
        initializer.Initialize();

        var chaosCastleMaps = gameConfiguration.Maps.Where(map => map.Number is >= 17 and <= 23 or 53);
        foreach (var chaosCastleMap in chaosCastleMaps)
        {
            chaosCastleMap.UpdateTerrainFromResources();
        }
    }
}