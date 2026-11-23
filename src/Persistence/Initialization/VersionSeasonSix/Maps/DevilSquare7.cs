// <copyright file="DevilSquare7.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 7.
/// </summary>
internal class DevilSquare7 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 32;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare7"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare7(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.DevilSquare7);

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 7;

    /// <inheritdoc/>
    /// <remarks>
    /// Devil Square gives no zen for the killed monsters, only the rank reward of the event pays zen.
    /// </remarks>
    protected override bool MonstersDropZen => false;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 50;
        const byte x2 = 79;
        const byte y1 = 138;
        const byte y2 = 173;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[434], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Gigantis
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[435], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Berserk

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[438], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Persona
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[439], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Dreadfear

        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[436], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Balram (Trainee)
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[437], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Soram (Trainee)

        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[440], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Dark Elf Boss
    }

    /// <inheritdoc />
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 434;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Gigantis434);
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 105 },
                { Stats.MaximumHealth, 30000 },
                { Stats.MinimumPhysBaseDmg, 475 },
                { Stats.MaximumPhysBaseDmg, 510 },
                { Stats.DefenseBase, 360 },
                { Stats.AttackRatePvm, 715 },
                { Stats.DefenseRatePvm, 270 },
                { Stats.IceResistance, 25 },
                { Stats.PoisonResistance, 25 },
                { Stats.LightningResistance, 25 },
                { Stats.FireResistance, 25 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 435;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Berserk);
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 106 },
                { Stats.MaximumHealth, 33000 },
                { Stats.MinimumPhysBaseDmg, 500 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 410 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 280 },
                { Stats.IceResistance, 50 },
                { Stats.PoisonResistance, 50 },
                { Stats.LightningResistance, 50 },
                { Stats.FireResistance, 50 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 436;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.BalramTrainee);
            monster.MoveRange = 6;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 37000 },
                { Stats.MinimumPhysBaseDmg, 560 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 460 },
                { Stats.AttackRatePvm, 800 },
                { Stats.DefenseRatePvm, 320 },
                { Stats.IceResistance, 23 },
                { Stats.PoisonResistance, 23 },
                { Stats.LightningResistance, 23 },
                { Stats.FireResistance, 23 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 437;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.SoramTrainee);
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 119 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 610 },
                { Stats.MaximumPhysBaseDmg, 650 },
                { Stats.DefenseBase, 510 },
                { Stats.AttackRatePvm, 835 },
                { Stats.DefenseRatePvm, 330 },
                { Stats.IceResistance, 25 },
                { Stats.PoisonResistance, 25 },
                { Stats.LightningResistance, 25 },
                { Stats.FireResistance, 25 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 438;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Persona438);
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 121 },
                { Stats.MaximumHealth, 50000 },
                { Stats.MinimumPhysBaseDmg, 640 },
                { Stats.MaximumPhysBaseDmg, 680 },
                { Stats.DefenseBase, 560 },
                { Stats.AttackRatePvm, 935 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.IceResistance, 29 },
                { Stats.PoisonResistance, 29 },
                { Stats.LightningResistance, 29 },
                { Stats.FireResistance, 29 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 439;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Dreadfear439);
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 123 },
                { Stats.MaximumHealth, 55000 },
                { Stats.MinimumPhysBaseDmg, 700 },
                { Stats.MaximumPhysBaseDmg, 740 },
                { Stats.DefenseBase, 610 },
                { Stats.AttackRatePvm, 1015 },
                { Stats.DefenseRatePvm, 380 },
                { Stats.IceResistance, 27 },
                { Stats.PoisonResistance, 27 },
                { Stats.LightningResistance, 27 },
                { Stats.FireResistance, 27 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 440;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.DarkElf);
            monster.MoveRange = 6;
            monster.AttackRange = 6;
            monster.ViewRange = 10;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 135 },
                { Stats.MaximumHealth, 75000 },
                { Stats.MinimumPhysBaseDmg, 850 },
                { Stats.MaximumPhysBaseDmg, 900 },
                { Stats.DefenseBase, 690 },
                { Stats.AttackRatePvm, 1300 },
                { Stats.DefenseRatePvm, 430 },
                { Stats.IceResistance, 23 },
                { Stats.PoisonResistance, 23 },
                { Stats.LightningResistance, 23 },
                { Stats.FireResistance, 23 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}