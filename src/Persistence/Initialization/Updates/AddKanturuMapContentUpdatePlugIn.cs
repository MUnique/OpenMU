// <copyright file="AddKanturuMapContentUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the bosses and the wave spawns of the Kanturu Refinery Tower event to an
/// existing Season 6 database.
/// </summary>
/// <remarks>
/// <see cref="AddKanturuDataUpdatePlugIn"/> only adds the <see cref="MiniGameDefinition"/>.
/// The bosses (Maya, both of her hands and Nightmare) and the wave spawns are part of
/// the map initializer, which only runs when a database is created from scratch. Without
/// them the event starts but nothing ever spawns, so this update adds the missing content
/// to the already existing map definition instead of creating a second one.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("7B4E9D26-1A83-4F5C-9E70-2C8B6D41A395")]
public class AddKanturuMapContentUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Kanturu map content";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Kanturu bosses (Maya, her hands, Nightmare) and the event wave spawns.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddKanturuMapContent;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 08, 30, 2, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new KanturuMapContentSeeder(context, gameConfiguration).Seed();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Creates the bosses through the map initializer and adds the wave spawns to the
    /// map definition which already exists in the database.
    /// </summary>
    private sealed class KanturuMapContentSeeder : VersionSeasonSix.Maps.KanturuEvent
    {
        public KanturuMapContentSeeder(IContext context, GameConfiguration gameConfiguration)
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

            if (map.MonsterSpawns.Any(s => s.SpawnTrigger == SpawnTrigger.OnceAtWaveStart))
            {
                // The wave spawns are already there.
                return;
            }

            foreach (var (number, monsterNumber, x1, x2, y1, y2, quantity, waveNumber) in EventWaveSpawns)
            {
                this.AddWaveSpawn(map, number, this.NpcDictionary[monsterNumber], x1, x2, y1, y2, quantity, waveNumber);
            }
        }

        /// <summary>
        /// Creates one wave spawn area and adds it to the map.
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
