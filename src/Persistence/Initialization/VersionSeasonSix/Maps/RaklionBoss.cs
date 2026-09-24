// <copyright file="RaklionBoss.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Map initialization for the raklion boss map.
/// </summary>
/// <seealso cref="BaseMapInitializer" />
internal class RaklionBoss : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 58;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "LaCleon Boss";

    /// <summary>
    /// The wave number of the spider eggs, which appear when the raklion event starts.
    /// </summary>
    internal const byte SpiderEggWaveNumber = 1;

    /// <summary>
    /// The wave number of Selupan, which appears when all spider eggs are destroyed.
    /// </summary>
    internal const byte SelupanWaveNumber = 2;

    /// <summary>
    /// The wave number of the monsters, which are summoned by Selupan.
    /// </summary>
    internal const byte SummonWaveNumber = 3;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaklionBoss"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public RaklionBoss(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Raklion.Number;

    /// <inheritdoc />
    /// <remarks>
    /// The monsters are spawned by the raklion event, depending on its state.
    /// </remarks>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.GameConfiguration.Monsters.First(m => m.Number == 459), 145, 145, 31, 31, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SelupanWaveNumber); // Selupan (the boss)

        var spiderEggs1 = this.GameConfiguration.Monsters.First(m => m.Number == 460);
        var spiderEggs2 = this.GameConfiguration.Monsters.First(m => m.Number == 461);
        var spiderEggs3 = this.GameConfiguration.Monsters.First(m => m.Number == 462);
        yield return this.CreateMonsterSpawn(100, spiderEggs1, 141, 141, 27, 27, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(101, spiderEggs2, 141, 141, 28, 28, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(102, spiderEggs3, 142, 142, 28, 28, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(103, spiderEggs1, 146, 146, 34, 34, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(104, spiderEggs3, 147, 147, 34, 34, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(105, spiderEggs2, 146, 146, 33, 33, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(106, spiderEggs1, 152, 152, 31, 31, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(107, spiderEggs1, 152, 152, 30, 30, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(108, spiderEggs2, 151, 151, 30, 30, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(109, spiderEggs3, 146, 146, 29, 29, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(110, spiderEggs3, 146, 146, 30, 30, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(111, spiderEggs2, 147, 147, 29, 29, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(112, spiderEggs1, 145, 145, 24, 24, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(113, spiderEggs1, 144, 144, 24, 24, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);
        yield return this.CreateMonsterSpawn(114, spiderEggs2, 144, 144, 25, 25, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SpiderEggWaveNumber);

        var coolutin = this.GameConfiguration.Monsters.First(m => m.Number == 457);
        yield return this.CreateMonsterSpawn(200, coolutin, 144, 144, 25, 25, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(201, coolutin, 144, 144, 29, 29, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(202, coolutin, 144, 144, 32, 32, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(203, coolutin, 144, 144, 35, 35, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(204, coolutin, 144, 144, 38, 38, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(205, coolutin, 153, 153, 24, 24, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(206, coolutin, 153, 153, 28, 28, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(207, coolutin, 153, 153, 32, 32, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(208, coolutin, 153, 153, 35, 35, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
        yield return this.CreateMonsterSpawn(209, coolutin, 153, 153, 38, 38, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, SummonWaveNumber);
    }

    /// <inheritdoc />
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 459;
            monster.Designation = "Selupan";
            monster.MoveRange = 3;
            monster.AttackRange = 10;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 145 },
                { Stats.MaximumHealth, 4800000 },
                { Stats.MinimumPhysBaseDmg, 2500 },
                { Stats.MaximumPhysBaseDmg, 3000 },
                { Stats.DefenseBase, 1250 },
                { Stats.AttackRatePvm, 2200 },
                { Stats.DefenseRatePvm, 1800 },
                { Stats.PoisonResistance, 254f / 255 },
                { Stats.IceResistance, 254f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
                { Stats.SkillMultiplier, 2 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
            /*TODO: Selupan appears to have 4 different types of skill attacks, which have different damage multipliers and magic effects.*/
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 460;
            monster.Designation = "Spider Eggs 1";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 10 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 500 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 410 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 280 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.WaterResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 461;
            monster.Designation = "Spider Eggs 2";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 10 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 500 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 410 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 280 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.WaterResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 462;
            monster.Designation = "Spider Eggs 3";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 10 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 500 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 410 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 280 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.WaterResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}