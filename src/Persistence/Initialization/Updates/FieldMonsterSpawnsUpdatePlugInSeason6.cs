// <copyright file="FieldMonsterSpawnsUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Replaces the automatic monster spawns of Devias, Noria and Elvenland with the Season 6 layout,
/// which places the monsters in smaller areas and at single spots instead of quarters of the map,
/// and removes the duplicate monster spawns of Dungeon and Lost Tower.
/// </summary>
/// <remarks>
/// The update is optional, because it overwrites the spawns of the field monsters of these maps, which a
/// server owner may have customized. Spawns of other monsters, NPCs and spawns with another trigger are kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInSeason6_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInSeason6_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("5B20A974-D2AA-4B49-ADF6-2B5F97D15828")]
public class FieldMonsterSpawnsUpdatePlugInSeason6 : FieldMonsterSpawnsUpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string SeasonSixPlugInName = "Season 6 monster spawns of Devias, Noria and Elvenland";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string SeasonSixPlugInDescription = "Replaces the monster spawns of Devias, Noria and Elvenland, which are spread over quarters of the map, with the Season 6 layout of smaller areas and single spots. It adds the spiders to Noria and Elvenland. Customized spawns of the field monsters of these maps are overwritten. Also removes the duplicate monster spawns of Dungeon (254) and Lost Tower (2).";

    /// <inheritdoc />
    public override string Name => SeasonSixPlugInName;

    /// <inheritdoc />
    public override string Description => SeasonSixPlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    protected override void ApplyVersionSpecificChanges(IContext context, GameConfiguration gameConfiguration)
    {
        ApplyLayout(context, gameConfiguration, Devias.Number, Devias.MonsterSpawnLayout);
        ApplyLayout(context, gameConfiguration, Noria.Number, Noria.MonsterSpawnLayout);
        ApplyLayout(context, gameConfiguration, Elvenland.Number, Elvenland.MonsterSpawnLayout);
    }

    /// <summary>
    /// Ensures that the automatic spawns of the monsters of the layout on the map are exactly the ones of the layout.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="mapNumber">The number of the map.</param>
    /// <param name="layout">The spawns of the layout.</param>
    private static void ApplyLayout(IContext context, GameConfiguration gameConfiguration, byte mapNumber, IReadOnlyList<FieldMonsterSpawn> layout)
    {
        if (gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { } map)
        {
            return;
        }

        var layoutIds = new HashSet<Guid>();
        foreach (var spawn in layout)
        {
            var monster = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == spawn.MonsterNumber)
                          ?? throw new InvalidOperationException($"The spawns of map {mapNumber} need monster {spawn.MonsterNumber}, which is missing in the game configuration.");
            var id = GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawn.Number);
            layoutIds.Add(id);

            // Updated in place when it's already there, so applying the update again doesn't duplicate anything.
            var area = map.MonsterSpawns.FirstOrDefault(s => s.GetId() == id);
            if (area is null)
            {
                area = context.CreateNew<MonsterSpawnArea>();
                area.SetGuid(id);
                map.MonsterSpawns.Add(area);
            }

            area.GameMap = map;
            area.MonsterDefinition = monster;
            area.Quantity = spawn.Quantity;
            area.Direction = Direction.Undefined;
            area.SpawnTrigger = SpawnTrigger.Automatic;
            area.X1 = spawn.X1;
            area.X2 = spawn.X2;
            area.Y1 = spawn.Y1;
            area.Y2 = spawn.Y2;
            area.WaveNumber = 0;
        }

        var layoutMonsters = layout.Select(spawn => spawn.MonsterNumber).ToHashSet();
        var replacedSpawns = map.MonsterSpawns
            .Where(s => s.SpawnTrigger == SpawnTrigger.Automatic
                        && s.MonsterDefinition is { } monster
                        && layoutMonsters.Contains(monster.Number)
                        && !layoutIds.Contains(s.GetId()))
            .ToList();
        foreach (var spawn in replacedSpawns)
        {
            map.MonsterSpawns.Remove(spawn);
        }
    }
}
