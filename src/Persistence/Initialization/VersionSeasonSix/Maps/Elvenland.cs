// <copyright file="Elvenland.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Elvenland map.
/// </summary>
internal class Elvenland : BaseMapInitializer
{
    /// <inheritdoc/>
    public static readonly byte Number = 51;

    /// <inheritdoc/>
    private static readonly string Name = "Elvenland";


    /// <summary>
    /// Initializes a new instance of the <see cref="Elvenland"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Elvenland(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[256], 37, 242, Direction.SouthWest); // Lahap
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[415], 44, 229, Direction.SouthWest); // Silvia
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[416], 29, 237, Direction.South); // Rhea
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[417], 37, 218, Direction.SouthWest); // Marce
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[257], 44, 189, Direction.SouthEast); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[257], 57, 231, Direction.SouthWest); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[257], 74, 74, 219, 220, 1, Direction.SouthWest); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[240], 51, 229, Direction.SouthWest); // Safety Guardian
        yield return this.CreateMonsterSpawn(9, this.NpcDictionary[385], 55, 243, Direction.SouthWest); // Mirage
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[452], 45, 243, Direction.SouthWest); // Seed Master
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[453], 49, 243, Direction.SouthWest); // Seed Researcher
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[540], 49, 216, Direction.South); // Lugard
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[492], 22, 225, Direction.South); // Moss
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[579], 20, 214, Direction.SouthEast); // David
        yield return this.CreateMonsterSpawn(15, this.NpcDictionary[568], 55, 199, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[418], 0, 128, 128, 245, 80); // Strange Rabbit
        yield return this.CreateMonsterSpawn(22, this.NpcDictionary[419], 0, 251, 128, 245, 45); // Polluted Butterfly
        yield return this.CreateMonsterSpawn(23, this.NpcDictionary[420], 0, 128, 0, 128, 45); // Hideous Rabbit
        yield return this.CreateMonsterSpawn(24, this.NpcDictionary[421], 0, 128, 0, 128, 30); // Werewolf
        yield return this.CreateMonsterSpawn(25, this.NpcDictionary[422], 128, 251, 0, 128, 30); // Cursed Lich
        yield return this.CreateMonsterSpawn(26, this.NpcDictionary[423], 128, 251, 0, 128, 20); // Totem Golem
        yield return this.CreateMonsterSpawn(27, this.NpcDictionary[424], 128, 251, 0, 128, 20); // Grizzly
        yield return this.CreateMonsterSpawn(28, this.NpcDictionary[425], 128, 251, 0, 128, 20); // Captain Grizzly
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 418;
            monster.Designation = "Strange Rabbit";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 4 },
                { Stats.MaximumHealth, 60 },
                { Stats.MinimumPhysBaseDmg, 10 },
                { Stats.MaximumPhysBaseDmg, 13 },
                { Stats.DefenseBase, 3 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 3 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 419;
            monster.Designation = "Polluted Butterfly";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 13 },
                { Stats.MaximumHealth, 230 },
                { Stats.MinimumPhysBaseDmg, 37 },
                { Stats.MaximumPhysBaseDmg, 42 },
                { Stats.DefenseBase, 13 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 13 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 420;
            monster.Designation = "Hideous Rabbit";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 19 },
                { Stats.MaximumHealth, 520 },
                { Stats.MinimumPhysBaseDmg, 68 },
                { Stats.MaximumPhysBaseDmg, 72 },
                { Stats.DefenseBase, 22 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 22 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 421;
            monster.Designation = "Werewolf";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 24 },
                { Stats.MaximumHealth, 720 },
                { Stats.MinimumPhysBaseDmg, 85 },
                { Stats.MaximumPhysBaseDmg, 90 },
                { Stats.DefenseBase, 30 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 422;
            monster.Designation = "Cursed Lich";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Meteorite);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 30 },
                { Stats.MaximumHealth, 900 },
                { Stats.MinimumPhysBaseDmg, 105 },
                { Stats.MaximumPhysBaseDmg, 110 },
                { Stats.DefenseBase, 33 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 33 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.WaterResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 423;
            monster.Designation = "Totem Golem";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 36 },
                { Stats.MaximumHealth, 1200 },
                { Stats.MinimumPhysBaseDmg, 120 },
                { Stats.MaximumPhysBaseDmg, 125 },
                { Stats.DefenseBase, 50 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 50 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 424;
            monster.Designation = "Grizzly";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 43 },
                { Stats.MaximumHealth, 2400 },
                { Stats.MinimumPhysBaseDmg, 130 },
                { Stats.MaximumPhysBaseDmg, 145 },
                { Stats.DefenseBase, 65 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 65 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 425;
            monster.Designation = "Captain Grizzly";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 48 },
                { Stats.MaximumHealth, 3000 },
                { Stats.MinimumPhysBaseDmg, 150 },
                { Stats.MaximumPhysBaseDmg, 155 },
                { Stats.DefenseBase, 80 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 70 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}