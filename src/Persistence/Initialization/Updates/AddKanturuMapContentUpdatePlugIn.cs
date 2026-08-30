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
/// The bosses (Maya, both of her hands and Nightmare) and the nine wave spawns are part of
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
        private const short MayaBodyNumber = 364;
        private const short MayaLeftHandNumber = 362;
        private const short MayaRightHandNumber = 363;
        private const short NightmareNumber = 361;
        private const short BladeHunterNumber = 354;
        private const short DreadfearNumber = 360;
        private const short TwinTaleNumber = 359;
        private const short GenociderNumber = 357;
        private const short PersonaNumber = 358;

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

            var maya = this.NpcDictionary[MayaBodyNumber];
            var mayaLeft = this.NpcDictionary[MayaLeftHandNumber];
            var mayaRight = this.NpcDictionary[MayaRightHandNumber];
            var nightmare = this.NpcDictionary[NightmareNumber];
            var bladeHunter = this.NpcDictionary[BladeHunterNumber];
            var dreadfear = this.NpcDictionary[DreadfearNumber];
            var twinTale = this.NpcDictionary[TwinTaleNumber];
            var genocider = this.NpcDictionary[GenociderNumber];
            var persona = this.NpcDictionary[PersonaNumber];

            // Wave 0: Maya's body rises when the battle starts.
            this.AddWaveSpawn(map, 299, maya, 188, 188, 110, 110, 1, 0);

            // Wave 1: Phase 1 - 30 Blade Hunter + 10 Dreadfear.
            this.AddWaveSpawn(map, 200, bladeHunter, 175, 215, 58, 86, 30, 1);
            this.AddWaveSpawn(map, 201, dreadfear, 175, 215, 58, 86, 10, 1);

            // Wave 2: Phase 1 boss - Maya's left hand.
            this.AddWaveSpawn(map, 210, mayaLeft, 202, 202, 83, 83, 1, 2);

            // Wave 3: Phase 2 - 30 Blade Hunter + 10 Dreadfear.
            this.AddWaveSpawn(map, 220, bladeHunter, 175, 215, 58, 86, 30, 3);
            this.AddWaveSpawn(map, 221, dreadfear, 175, 215, 58, 86, 10, 3);

            // Wave 4: Phase 2 boss - Maya's right hand.
            this.AddWaveSpawn(map, 230, mayaRight, 189, 189, 82, 82, 1, 4);

            // Wave 5: Phase 3 - 10 Dreadfear + 10 Twin Tale.
            this.AddWaveSpawn(map, 240, dreadfear, 175, 215, 58, 86, 10, 5);
            this.AddWaveSpawn(map, 241, twinTale, 175, 215, 58, 86, 10, 5);

            // Wave 6: Phase 3 bosses - both of Maya's hands.
            this.AddWaveSpawn(map, 250, mayaLeft, 202, 202, 83, 83, 1, 6);
            this.AddWaveSpawn(map, 251, mayaRight, 189, 189, 82, 82, 1, 6);

            // Wave 7: Nightmare preparation - 15 Genocider + 15 Dreadfear + 15 Persona.
            this.AddWaveSpawn(map, 260, genocider, 75, 88, 97, 137, 15, 7);
            this.AddWaveSpawn(map, 261, dreadfear, 75, 88, 97, 137, 15, 7);
            this.AddWaveSpawn(map, 262, persona, 75, 88, 97, 137, 15, 7);

            // Wave 8: Nightmare.
            this.AddWaveSpawn(map, 270, nightmare, 78, 78, 143, 143, 1, 8);
        }

        /// <summary>
        /// Creates one wave spawn area and adds it to the map.
        /// </summary>
        /// <remarks>
        /// This mirrors <c>BaseMapInitializer.CreateMonsterSpawn</c>, which can't be used here:
        /// it derives the id from the map definition which the initializer creates itself, and
        /// it's only reachable through the <c>CreateMonsterSpawns</c> iterator, which would also
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
