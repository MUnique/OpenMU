// <copyright file="AddImperialGuardianDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the imperial guardian event configuration to an existing Season 6 database.
/// </summary>
/// <remarks>
/// It adds the <see cref="MiniGameDefinition"/>s, lets Jerint open the entrance window, lets the
/// suspicious scrap of paper drop from monsters, and lets players who die inside the event maps
/// respawn at Devias. The monster spawns of the event maps are changed, so that they're spawned
/// by the event, zone by zone, and their directions are corrected, so that the gates are shown properly.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("4D8E2A17-B63C-4F95-A0E1-7C2B9F5D3E68")]
public class AddImperialGuardianDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Imperial Guardian data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the imperial guardian event configuration.";

    private static readonly byte[] MapNumbers =
    [
        FortressOfImperialGuardian1.Number,
        FortressOfImperialGuardian2.Number,
        FortressOfImperialGuardian3.Number,
        FortressOfImperialGuardian4.Number,
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddImperialGuardianData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 24, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (new EventTicketItems(context, gameConfiguration).CreateSuspiciousScrapOfPaperDropGroup() is { } scrapOfPaperDropGroup)
        {
            foreach (var map in gameConfiguration.Maps)
            {
                map.DropItemGroups.Add(scrapOfPaperDropGroup);
            }
        }

        var devias = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Devias.Number);
        foreach (var map in gameConfiguration.Maps.Where(map => MapNumbers.Contains((byte)map.Number)))
        {
            map.SafezoneMap = devias;
            foreach (var spawn in ImperialGuardianSpawns.GetSpawns((byte)map.Number))
            {
                var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawn.Number);
                if (map.MonsterSpawns.FirstOrDefault(area => area.GetId() == id) is { } spawnArea)
                {
                    spawnArea.SpawnTrigger = SpawnTrigger.OnceAtWaveStart;
                    spawnArea.WaveNumber = spawn.WaveNumber;
                    spawnArea.Direction = spawn.Direction;
                }
            }
        }

        var initializer = new ImperialGuardianInitializer(context, gameConfiguration);
        initializer.CreateMiniGameDefinitions();
        initializer.ConfigureJerint();
        return ValueTask.CompletedTask;
    }
}
