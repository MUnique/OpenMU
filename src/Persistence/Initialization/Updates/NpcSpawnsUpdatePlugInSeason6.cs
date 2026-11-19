// <copyright file="NpcSpawnsUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the NPC spawns of Season 6 to the Season 6 data: positions, looking directions and missing or surplus NPCs.
/// </summary>
/// <remarks>
/// The NPCs of several maps stood a few fields away from their Season 6 position or looked into another direction,
/// some NPCs of the event maps had no direction at all, some NPCs of 0.75 and 0.95d were taken over by Season 6
/// although Season 6 doesn't have them there, and some NPCs had a second spawn which Season 6 doesn't have.
/// Spawns are identified by their number, like in the map initializers, which define the same values.
/// The update is optional, because it overwrites the positions and directions of these NPCs, which a server owner
/// may have customized. A surplus NPC is only removed when its spawn still has the expected NPC, so a spawn
/// which the owner reused for another NPC is kept; an NPC which the owner removed is only added again when it
/// is one of the NPCs which this update adds.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.NpcSpawnsUpdatePlugInSeason6_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.NpcSpawnsUpdatePlugInSeason6_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("84849961-193C-4564-9CDE-1B64D39B37E2")]
public class NpcSpawnsUpdatePlugInSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Season 6 NPC spawns";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Brings the NPCs to their Season 6 positions and looking directions (Lorencia, Devias, Noria, Arena, Atlans, Elvenland, Aida, Crywolf Fortress, Valley of Loren, Karutan, Kanturu and Illusion Temple), adds the missing Elf Soldier of Devias, the two missing Elf Soldiers of Noria and the Golden Archer of Devias, and removes the NPCs which Season 6 doesn't have: of the older versions the Potion Girls of Noria and Lost Tower, the Safety Guardians of Lost Tower, Arena and Atlans, the guard at 114/125 and the Golden Archer of Lorencia; Lugard, Priest Devin and Mirage of Devias, which Season 6 has only in Elvenland and Crywolf Fortress; and the second Crown of Valley of Loren. Customized positions and directions of these NPCs are overwritten.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the NPC spawns which get the Season 6 position and direction, or which are added when they're missing.
    /// </summary>
    internal static IReadOnlyList<NpcSpawnCorrection> Corrections { get; } =
    [
        new(Aida.Number, 1, 369, 78, 13, Direction.South), // Osbourne
        new(Aida.Number, 2, 370, 86, 13, Direction.SouthWest), // Jerridon
        new(Arena.Number, 2, 200, 63, 160, Direction.SouthWest), // Soccer Ball
        new(Arena.Number, 3, 239, 72, 164, Direction.SouthWest), // Arena Guard
        new(Atlans.Number, 10, 229, 17, 25, Direction.South), // Marlon
        new(CrywolfFortress.Number, 3, 226, 135, 47, Direction.South), // Pet Trainer
        new(CrywolfFortress.Number, 5, 256, 96, 25, Direction.South), // Lahap
        new(CrywolfFortress.Number, 7, 240, 113, 56, Direction.South), // Safety Guardian
        new(CrywolfFortress.Number, 8, 224, 118, 11, Direction.East), // Guardsman
        new(Devias.Number, 36, 257, 219, 73, Direction.SouthEast), // Elf Soldier
        new(Devias.Number, 37, 566, 204, 61, Direction.SouthWest), // Mercenary Guild Felicia
        new(Devias.Number, 38, 379, 32, 31, Direction.SouthEast), // Fireworks Girl
        new(Devias.Number, 40, 233, 209, 30, Direction.SouthWest), // Messenger Of Arch
        new(Devias.Number, 41, 233, 219, 9, Direction.SouthEast), // Messenger Of Arch
        new(Devias.Number, 43, 257, 172, 44, Direction.SouthWest, true), // Elf Soldier
        new(Devias.Number, 44, 236, 226, 66, Direction.SouthWest, true), // Golden Archer
        new(Elvenland.Number, 1, 256, 37, 241, Direction.SouthWest), // Lahap
        new(Elvenland.Number, 3, 416, 29, 236, Direction.South), // Rhea
        new(Elvenland.Number, 4, 417, 36, 219, Direction.South), // Marce
        new(Elvenland.Number, 5, 257, 43, 191, Direction.SouthEast), // Elf Soldier
        new(Elvenland.Number, 6, 257, 57, 232, Direction.SouthEast), // Elf Soldier
        new(Elvenland.Number, 8, 240, 50, 229, Direction.SouthWest), // Safety Guardian
        new(Elvenland.Number, 10, 452, 46, 243, Direction.SouthWest), // Seed Master
        new(Elvenland.Number, 11, 453, 50, 243, Direction.SouthWest), // Seed Researcher
        new(IllusionTemple1.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple1.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple1.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple1.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple1.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple1.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(IllusionTemple2.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple2.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple2.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple2.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple2.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple2.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(IllusionTemple3.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple3.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple3.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple3.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple3.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple3.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(IllusionTemple4.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple4.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple4.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple4.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple4.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple4.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(IllusionTemple5.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple5.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple5.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple5.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple5.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple5.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(IllusionTemple6.Number, 100, 380, 207, 47, Direction.South), // Stone Statue
        new(IllusionTemple6.Number, 101, 380, 134, 121, Direction.South), // Stone Statue
        new(IllusionTemple6.Number, 110, 381, 139, 46, Direction.SouthEast), // M U Allies General
        new(IllusionTemple6.Number, 111, 382, 194, 123, Direction.SouthEast), // Illusion Elder
        new(IllusionTemple6.Number, 112, 383, 141, 59, Direction.South), // Alliance Item Storage
        new(IllusionTemple6.Number, 113, 384, 194, 113, Direction.South), // Illusion Item Storage
        new(KanturuEvent.Number, 1, 368, 77, 177, Direction.South), // Elphis
        new(Karutan1.Number, 5, 240, 157, 127, Direction.SouthWest), // Safety Guardian
        new(Lorencia.Number, 19, 257, 170, 129, Direction.SouthWest), // Elf Soldier
        new(Lorencia.Number, 20, 257, 132, 90, Direction.SouthEast), // Elf Soldier
        new(Lorencia.Number, 24, 543, 144, 127, Direction.South), // Gens Duprian
        new(Noria.Number, 10, 544, 167, 100, Direction.South), // Gens Vanert
        new(Noria.Number, 15, 257, 166, 85, Direction.South, true), // Elf Soldier
        new(Noria.Number, 16, 257, 201, 129, Direction.South, true), // Elf Soldier
        new(ValleyOfLoren.Number, 3, 217, 167, 195, Direction.NorthWest), // Crown Switch1
        new(ValleyOfLoren.Number, 20, 376, 90, 44, Direction.SouthWest), // Pamela The Supplier
        new(ValleyOfLoren.Number, 21, 377, 89, 218, Direction.SouthEast), // Angela The Supplier
    ];

    /// <summary>
    /// Gets the NPC spawns which Season 6 doesn't have: those of 0.75 and 0.95d, and second spawns of NPCs
    /// which Season 6 has only on another map or only once.
    /// </summary>
    internal static IReadOnlyList<(byte MapNumber, short SpawnNumber, short NpcNumber)> RemovedSpawns { get; } =
    [
        (Lorencia.Number, 9, 247), // Guard
        (Lorencia.Number, 15, 236), // Golden Archer, which Season 6 has in Devias
        (Noria.Number, 1, 253), // Potion Girl
        (Noria.Number, 2, 253), // Potion Girl
        (LostTower.Number, 1, 253), // Potion Girl
        (LostTower.Number, 2, 240), // Safety Guardian
        (Arena.Number, 1, 240), // Safety Guardian
        (Atlans.Number, 1, 240), // Safety Guardian
        (Devias.Number, 30, 540), // Lugard, which Season 6 has in Elvenland
        (Devias.Number, 31, 406), // Priest Devin, which Season 6 has in Crywolf Fortress
        (Devias.Number, 32, 385), // Mirage, which Season 6 has in Elvenland
        (ValleyOfLoren.Number, 2, 216), // second Crown on the spot of the first
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var (mapNumber, spawnNumber, npcNumber) in RemovedSpawns)
        {
            if (FindMap(gameConfiguration, mapNumber) is { } map
                && FindSpawn(map, spawnNumber) is { } spawn
                && spawn.MonsterDefinition?.Number == npcNumber)
            {
                map.MonsterSpawns.Remove(spawn);
            }
        }

        foreach (var correction in Corrections)
        {
            if (FindMap(gameConfiguration, correction.MapNumber) is not { } map)
            {
                continue;
            }

            var spawn = FindSpawn(map, correction.SpawnNumber);
            if (spawn is null)
            {
                if (!correction.IsAdded
                    || gameConfiguration.Monsters.FirstOrDefault(m => m.Number == correction.NpcNumber) is not { } npc)
                {
                    continue;
                }

                spawn = context.CreateNew<MonsterSpawnArea>();
                spawn.SetGuid(correction.MapNumber, correction.SpawnNumber);
                spawn.GameMap = map;
                spawn.MonsterDefinition = npc;
                spawn.Quantity = 1;
                spawn.SpawnTrigger = SpawnTrigger.Automatic;
                map.MonsterSpawns.Add(spawn);
            }

            // Only where the spawn still has its NPC; a spawn which the owner reused for another NPC is kept.
            if (spawn.MonsterDefinition?.Number != correction.NpcNumber)
            {
                continue;
            }

            spawn.X1 = spawn.X2 = correction.X;
            spawn.Y1 = spawn.Y2 = correction.Y;
            spawn.Direction = correction.Direction;
        }

        return ValueTask.CompletedTask;
    }

    private static GameMapDefinition? FindMap(GameConfiguration gameConfiguration, byte mapNumber)
        => gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber);

    private static MonsterSpawnArea? FindSpawn(GameMapDefinition map, short spawnNumber)
    {
        var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawnNumber);
        return map.MonsterSpawns.FirstOrDefault(s => s.GetId() == id);
    }
}
