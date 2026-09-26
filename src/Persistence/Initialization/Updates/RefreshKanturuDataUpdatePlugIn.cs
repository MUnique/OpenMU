// <copyright file="RefreshKanturuDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Refreshes all Kanturu Refinery Tower data of an existing Season 6 database in
/// one go: the event map safezone, the participant limit, and the Nightmare summon
/// waves. The start configuration itself is deleted once, so it's recreated from
/// scratch with defaults on the next startup; no JSON migration is needed.
/// Every other step only fills in missing or seeded values, so customized values
/// are preserved and re-running stays a no-op.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("E5AFDD7A-3DE8-4955-8BB5-4231F6A87749")]
public class RefreshKanturuDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Refresh Kanturu data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Sets the Kanturu event safezone to Kanturu Relics, seeds 15 participants and the Nightmare summon waves, and resets the start configuration to defaults; customized values of the remaining data are preserved.";

    /// <summary>
    /// The first wave number of the Nightmare summons.
    /// </summary>
    internal const byte FirstSummonWaveNumber = 9;

    private const int PreviousMaximumPlayerCount = 10;

    private const int CurrentMaximumPlayerCount = 15;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RefreshKanturuData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 25, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        FixSafezoneMap(gameConfiguration);
        FixMaximumPlayerCount(gameConfiguration);
        await DeleteStartConfigurationAsync(context, gameConfiguration).ConfigureAwait(false);
        new KanturuSummonWaveSeeder(context, gameConfiguration).Seed();
    }

    /// <summary>
    /// Sets the safezone of the Kanturu event map to Kanturu Relics, so that players
    /// who die inside the event respawn there instead of on the event map itself.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static void FixSafezoneMap(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Maps.FirstOrDefault(m => m.Number == KanturuEvent.Number) is { } eventMap
            && eventMap.SafezoneMap is not { Number: KanturuRelics.Number })
        {
            eventMap.SafezoneMap = gameConfiguration.Maps.FirstOrDefault(m => m.Number == KanturuRelics.Number);
        }
    }

    /// <summary>
    /// Deletes the persisted start configuration entirely, so it's recreated from
    /// scratch with defaults on the next startup. This replaces JSON migration:
    /// whatever archaeology the row holds is discarded once, and the update never
    /// needs to run again.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    private static async ValueTask DeleteStartConfigurationAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var stale in gameConfiguration.PlugInConfigurations
            .Where(c => c.TypeId == typeof(KanturuStartPlugIn).GUID)
            .ToList())
        {
            await context.DeleteAsync(stale).ConfigureAwait(false);
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
    /// Adds the summon waves which are missing from the map definition.
    /// </summary>
    private sealed class KanturuSummonWaveSeeder : KanturuEvent
    {
        public KanturuSummonWaveSeeder(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        public void Seed()
        {
            if (this.GameConfiguration.Maps.FirstOrDefault(m => m.Number == Number) is not { } map)
            {
                return;
            }

            foreach (var (number, monsterNumber, x1, x2, y1, y2, quantity, waveNumber) in EventWaveSpawns)
            {
                if (waveNumber < FirstSummonWaveNumber)
                {
                    continue;
                }

                if (!this.NpcDictionary.TryGetValue(monsterNumber, out var monster))
                {
                    throw new InvalidOperationException($"Kanturu summon wave {waveNumber} needs monster {monsterNumber}, which is missing in the game configuration.");
                }

                this.AddSummonSpawn(map, number, monster, x1, x2, y1, y2, quantity, waveNumber);
            }
        }

        /// <summary>
        /// Creates one summon spawn area and adds it to the map, unless the wave is
        /// already there. It mirrors the wave spawn creation of <see cref="AddKanturuMapContentUpdatePlugIn"/>.
        /// </summary>
        private void AddSummonSpawn(GameMapDefinition map, short number, MonsterDefinition monster, byte x1, byte x2, byte y1, byte y2, short quantity, byte waveNumber)
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
