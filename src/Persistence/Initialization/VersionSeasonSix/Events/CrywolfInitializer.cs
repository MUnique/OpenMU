// <copyright file="CrywolfInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the crywolf event: it creates the monsters of the army of Balgass and their spawns,
/// and lets the statue and the altars of the fortress always be on the map.
/// </summary>
/// <remarks>
/// All methods only create or change what's missing, so they can be applied to existing data as well.
/// </remarks>
internal class CrywolfInitializer : InitializerBase
{
    /// <summary>
    /// The number of the statue of the holy wolf.
    /// </summary>
    internal const short StatueNumber = 204;

    /// <summary>
    /// The number of the last altar.
    /// </summary>
    internal const short LastAltarNumber = 209;

    /// <summary>
    /// The index of the warp entry of the crywolf fortress, which is expected by the client.
    /// </summary>
    internal const ushort WarpIndex = 26;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CrywolfInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        this.CreateMonsters();
        this.CreateSpawns();
        this.CreateTerrainVariants();
    }

    /// <summary>
    /// Creates the monsters of the army of Balgass, which don't exist yet. The values are the ones of the original game.
    /// </summary>
    internal void CreateMonsters()
    {
        this.CreateMonster(340, "Dark Elf", 135, 1_500_000, 800, 900, 900, 1500, 370, 6, 6, 10, 23);
        this.CreateMonster(341, "Soram", 134, 100_000, 600, 700, 500, 1500, 397, 6, 4, 7, 25);
        this.CreateMonster(344, "Balram", 134, 90_000, 600, 700, 500, 1500, 370, 6, 7, 7, 23);
        this.CreateMonster(345, "Death Spirit", 134, 95_000, 600, 700, 500, 1500, 397, 6, 6, 7, 25);
        this.CreateMonster(348, "Tanker", 133, 450_000, 800, 1000, 840, 1500, 370, 0, 6, 7, 23);
        this.CreateMonster(349, "Balgass", 135, 500_000, 1000, 1500, 800, 2000, 370, 6, 6, 7, 23);
    }

    /// <summary>
    /// Lets the statue and the altars of existing data always be on the crywolf map, as NPCs which can't be attacked.
    /// The event shows their state.
    /// </summary>
    internal void ConfigureStatueAndAltars()
    {
        foreach (var monster in this.GameConfiguration.Monsters.Where(IsStatueOrAltar))
        {
            monster.ObjectKind = NpcObjectKind.PassiveNpc;
        }

        if (this.GetMap() is not { } map)
        {
            return;
        }

        foreach (var spawn in map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition is { } definition && IsStatueOrAltar(definition)))
        {
            spawn.SpawnTrigger = SpawnTrigger.Automatic;
        }
    }

    /// <summary>
    /// Creates the spawns of the army of Balgass on the crywolf map, which don't exist yet.
    /// </summary>
    internal void CreateSpawns()
    {
        if (this.GetMap() is not { } map)
        {
            return;
        }

        foreach (var spawn in CrywolfSpawns.Spawns)
        {
            var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawn.Number);
            if (map.MonsterSpawns.Any(area => area.GetId() == id)
                || this.GameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == spawn.MonsterNumber) is not { } monsterDefinition)
            {
                continue;
            }

            var area = this.Context.CreateNew<MonsterSpawnArea>();
            area.SetGuid(map.Number, spawn.Number);
            area.GameMap = map;
            area.MonsterDefinition = monsterDefinition;
            area.Quantity = 1;
            area.Direction = Direction.Undefined;
            area.SpawnTrigger = SpawnTrigger.OnceAtWaveStart;
            area.WaveNumber = spawn.WaveNumber;
            area.X1 = spawn.X;
            area.X2 = spawn.X;
            area.Y1 = spawn.Y;
            area.Y2 = spawn.Y;
            map.MonsterSpawns.Add(area);
        }
    }

    /// <summary>
    /// Adds the terrains of the crywolf map while it's occupied and during the war, if they don't exist yet.
    /// Like the terrain files of the client, their numbers are the ones of the occupation states (1 and 2).
    /// </summary>
    internal void CreateTerrainVariants()
    {
        if (this.GetMap() is not { } map)
        {
            return;
        }

        foreach (var (number, description, fileName) in new (short, string, string)[] { (1, "Occupied", "Terrain35_OCCUPIED.att"), (2, "War", "Terrain35_WAR.att") })
        {
            if (map.TerrainVariants.Any(variant => variant.Number == number)
                || TerrainUpdateHelper.ReadTerrainResource(fileName) is not { } terrainData)
            {
                continue;
            }

            var variant = this.Context.CreateNew<GameMapTerrainVariant>();
            variant.Number = number;
            variant.Description = description;
            variant.TerrainData = terrainData;
            map.TerrainVariants.Add(variant);
        }
    }

    /// <summary>
    /// Adds the warp entry of the crywolf fortress to existing data, if it doesn't exist yet.
    /// </summary>
    internal void CreateWarpEntry()
    {
        if (this.GameConfiguration.WarpList.Any(warp => warp.Index == WarpIndex)
            || this.GetMap()?.ExitGates.FirstOrDefault(gate => gate is { IsSpawnGate: true, X1: 229, Y1: 37 }) is not { } gate)
        {
            return;
        }

        var warpInfo = this.Context.CreateNew<WarpInfo>();
        warpInfo.Index = WarpIndex;
        warpInfo.Name = "Crywolf";
        warpInfo.Costs = 10000;
        warpInfo.LevelRequirement = 190;
        warpInfo.Gate = gate;
        this.GameConfiguration.WarpList.Add(warpInfo);
    }

    private static bool IsStatueOrAltar(MonsterDefinition monster)
    {
        return monster.Number is >= StatueNumber and <= LastAltarNumber;
    }

    private GameMapDefinition? GetMap()
    {
        return this.GameConfiguration.Maps.FirstOrDefault(map => map.Number == CrywolfFortress.Number);
    }

    private void CreateMonster(
        short number,
        string designation,
        byte level,
        int health,
        int minimumDamage,
        int maximumDamage,
        int defense,
        int attackRate,
        int defenseRate,
        byte moveRange,
        byte attackRange,
        short viewRange,
        byte resistance)
    {
        if (this.GameConfiguration.Monsters.Any(monster => monster.Number == number))
        {
            return;
        }

        var monster = this.Context.CreateNew<MonsterDefinition>();
        this.GameConfiguration.Monsters.Add(monster);
        monster.Number = number;
        monster.Designation = designation;
        monster.MoveRange = moveRange;
        monster.AttackRange = attackRange;
        monster.ViewRange = viewRange;
        monster.MoveDelay = TimeSpan.FromMilliseconds(700);
        monster.AttackDelay = TimeSpan.FromMilliseconds(1700);
        monster.RespawnDelay = TimeSpan.FromSeconds(10);
        monster.Attribute = 2;
        monster.NumberOfMaximumItemDrops = 1;
        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, level },
            { Stats.MaximumHealth, health },
            { Stats.MinimumPhysBaseDmg, minimumDamage },
            { Stats.MaximumPhysBaseDmg, maximumDamage },
            { Stats.DefenseBase, defense },
            { Stats.AttackRatePvm, attackRate },
            { Stats.DefenseRatePvm, defenseRate },
            { Stats.PoisonResistance, resistance / 255f },
            { Stats.IceResistance, resistance / 255f },
            { Stats.LightningResistance, resistance / 255f },
            { Stats.FireResistance, resistance / 255f },
        };

        monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        monster.SetGuid(monster.Number);
    }
}
