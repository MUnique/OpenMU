// <copyright file="AddKanturuDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Kanturu Refinery Tower event configuration to an existing Season 6 database.
/// </summary>
/// <remarks>
/// The <see cref="KanturuInitializer"/> only runs when a database is created from scratch,
/// so databases which were initialized before the Kanturu event existed are missing its
/// <see cref="MiniGameDefinition"/>. This update adds it without touching any other data.
/// Version 2 additionally ensures the bosses and wave spawns of the event map,
/// and refreshes the safezone, the participant limit and the start configuration.
/// Every step only fills in missing or seeded values, so customized values
/// are preserved and re-running stays a no-op.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddKanturuDataUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddKanturuDataUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("3F1B7A64-9C2E-4D58-B0A7-5E6C8D19F204")]
public class AddKanturuDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Kanturu data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Kanturu Refinery Tower event configuration.";

    private const int PreviousMaximumPlayerCount = 10;

    private const int CurrentMaximumPlayerCount = 15;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override int Version => 2;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 08, 30, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 09, 30, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (!gameConfiguration.MiniGameDefinitions.Any(d => d.Type == MiniGameType.Kanturu))
        {
            new KanturuInitializer(context, gameConfiguration).Initialize();
        }

        new KanturuContentSeeder(context, gameConfiguration).Seed();
        FixSafezoneMap(gameConfiguration);
        FixMaximumPlayerCount(gameConfiguration);
        ResetStartConfiguration(gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Sets the safezone of the Kanturu event map to Kanturu Relics, so that players
    /// who die inside the event respawn there instead of on the event map itself.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void FixSafezoneMap(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Maps.FirstOrDefault(m => m.Number == VersionSeasonSix.Maps.KanturuEvent.Number) is { } eventMap
            && eventMap.SafezoneMap is not { Number: VersionSeasonSix.Maps.KanturuRelics.Number })
        {
            eventMap.SafezoneMap = gameConfiguration.Maps.FirstOrDefault(m => m.Number == VersionSeasonSix.Maps.KanturuRelics.Number);
        }
    }

    /// <summary>
    /// Raises a seeded participant limit to 15.
    /// Customized values are preserved.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void FixMaximumPlayerCount(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.MiniGameDefinitions.FirstOrDefault(definition => definition.Type == MiniGameType.Kanturu && definition.GameLevel == 1) is { } definition
            && definition.MaximumPlayerCount == PreviousMaximumPlayerCount)
        {
            definition.MaximumPlayerCount = CurrentMaximumPlayerCount;
        }
    }

    /// <summary>
    /// Clears the persisted start configuration, so it's rebuilt from defaults on the
    /// next load. This replaces JSON migration: whatever JSON the row holds is
    /// discarded once, and the update never needs to run again. The row itself (and its
    /// active flag) is kept: deleting it would deactivate the plug-in on a running
    /// server until the next restart.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void ResetStartConfiguration(GameConfiguration gameConfiguration)
    {
        foreach (var stale in gameConfiguration.PlugInConfigurations
            .Where(c => c.TypeId == typeof(KanturuStartPlugIn).GUID))
        {
            stale.CustomConfiguration = null;
        }
    }

    /// <summary>
    /// Creates the bosses through the map initializer and adds missing wave spawns to the
    /// map definition which already exists in the database.
    /// </summary>
    private sealed class KanturuContentSeeder : VersionSeasonSix.Maps.KanturuEvent
    {
        public KanturuContentSeeder(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        public void Seed()
        {
            // The bosses are created by the map initializer; without them the wave spawns
            // below can't be resolved through the NpcDictionary.
            if (this.GameConfiguration.Monsters.All(m => m.Number != MayaBodyNumber))
            {
                this.CreateMonsters();
            }

            if (this.GameConfiguration.Maps.FirstOrDefault(m => m.Number == Number) is not { } map)
            {
                return;
            }

            foreach (var (number, monsterNumber, x1, x2, y1, y2, quantity, waveNumber) in EventWaveSpawns)
            {
                if (!this.NpcDictionary.TryGetValue(monsterNumber, out var monster))
                {
                    throw new InvalidOperationException($"Kanturu wave {waveNumber} needs monster {monsterNumber}, which is missing in the game configuration.");
                }

                this.AddWaveSpawn(map, number, monster, x1, x2, y1, y2, quantity, waveNumber);
            }
        }

        /// <summary>
        /// Creates one wave spawn area and adds it to the map, unless the wave is
        /// already there. It mirrors the wave spawn creation of the map initializer.
        /// </summary>
        /// <remarks>
        /// This mirrors <c>BaseMapInitializer.CreateMonsterSpawn</c>, which can't be used here:
        /// it assigns the map definition which the initializer creates itself, and it's only
        /// reachable through the <c>CreateMonsterSpawns</c> iterator, which would also
        /// re-create the automatic spawns of the laser traps. Those would get the same
        /// deterministic ids as the ones which already exist in the database, and the change
        /// tracker rejects that.
        /// </remarks>
        private void AddWaveSpawn(GameMapDefinition map, short number, MonsterDefinition monster, byte x1, byte x2, byte y1, byte y2, short quantity, byte waveNumber)
        {
            if (map.MonsterSpawns.Any(s => s.WaveNumber == waveNumber && s.SpawnTrigger == SpawnTrigger.OnceAtWaveStart))
            {
                // Already there: running the update multiple times must not create duplicates.
                return;
            }

            var area = this.Context.CreateNew<MonsterSpawnArea>();
            area.SetGuid(map.Number, number);
            area.GameMap = map;
            area.MonsterDefinition = monster;
            area.Quantity = quantity;
            area.Direction = Direction.Undefined;
            area.SpawnTrigger = SpawnTrigger.OnceAtWaveStart;
            area.X1 = x1;
            area.X2 = x2;
            area.Y1 = y1;
            area.Y2 = y2;
            area.WaveNumber = waveNumber;
            map.MonsterSpawns.Add(area);
        }
    }
}
