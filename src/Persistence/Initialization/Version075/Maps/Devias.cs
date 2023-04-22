// <copyright file="Devias.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Devias map.
/// </summary>
internal class Devias : BaseMapInitializer
{
    /// <summary>
    /// The number of this map.
    /// </summary>
    internal const byte Number = 2;

    /// <summary>
    /// The name of the map.
    /// </summary>
    internal const string Name = "Devias";

    /// <summary>
    /// Initializes a new instance of the <see cref="Devias"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Devias(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[244], 226, 25, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[245], 225, 41, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[246], 186, 47, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[247], 224, 79, Direction.NorthEast);
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[247], 219, 79, Direction.NorthEast);
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[247], 169, 45, Direction.NorthWest);
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[247], 169, 39, Direction.NorthWest);
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[241], 215, 45, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(9, this.NpcDictionary[240], 218, 63, Direction.South);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // Monsters:
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[20], 194, 194, 165, 165, 10);
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[20], 36, 36, 25, 25, 10);
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[20], 210, 242, 210, 220, 15);
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[20], 0, 251, 128, 245, 200);
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[25], 0, 128, 128, 245, 75);
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[23], 0, 128, 0, 128, 75);
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[22], 0, 128, 0, 128, 75);
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[24], 128, 251, 0, 128, 65);
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[21], 128, 251, 0, 128, 35);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 19;
            monster.Designation = "Yeti";
            monster.MoveRange = 2;
            monster.AttackRange = 4;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 30 },
                { Stats.MaximumHealth, 900 },
                { Stats.MinimumPhysBaseDmg, 105 },
                { Stats.MaximumPhysBaseDmg, 110 },
                { Stats.DefenseBase, 37 },
                { Stats.AttackRatePvm, 150 },
                { Stats.DefenseRatePvm, 37 },
                { Stats.IceResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 20;
            monster.Designation = "Elite Yeti";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 36 },
                { Stats.MaximumHealth, 1200 },
                { Stats.MinimumPhysBaseDmg, 120 },
                { Stats.MaximumPhysBaseDmg, 125 },
                { Stats.DefenseBase, 50 },
                { Stats.AttackRatePvm, 180 },
                { Stats.DefenseRatePvm, 43 },
                { Stats.PoisonResistance, 1f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.WaterResistance, 1f / 255 },
                { Stats.FireResistance, 1f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 21;
            monster.Designation = "Assassin";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 26 },
                { Stats.MaximumHealth, 800 },
                { Stats.MinimumPhysBaseDmg, 95 },
                { Stats.MaximumPhysBaseDmg, 100 },
                { Stats.DefenseBase, 33 },
                { Stats.AttackRatePvm, 130 },
                { Stats.DefenseRatePvm, 33 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 22;
            monster.Designation = "Ice Monster";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Ice);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 22 },
                { Stats.MaximumHealth, 650 },
                { Stats.MinimumPhysBaseDmg, 80 },
                { Stats.MaximumPhysBaseDmg, 85 },
                { Stats.DefenseBase, 27 },
                { Stats.AttackRatePvm, 110 },
                { Stats.DefenseRatePvm, 27 },
                { Stats.IceResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 23;
            monster.Designation = "Hommerd";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 24 },
                { Stats.MaximumHealth, 700 },
                { Stats.MinimumPhysBaseDmg, 85 },
                { Stats.MaximumPhysBaseDmg, 90 },
                { Stats.DefenseBase, 29 },
                { Stats.AttackRatePvm, 120 },
                { Stats.DefenseRatePvm, 29 },
                { Stats.IceResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 24;
            monster.Designation = "Worm";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 20 },
                { Stats.MaximumHealth, 600 },
                { Stats.MinimumPhysBaseDmg, 75 },
                { Stats.MaximumPhysBaseDmg, 80 },
                { Stats.DefenseBase, 25 },
                { Stats.AttackRatePvm, 100 },
                { Stats.DefenseRatePvm, 25 },
                { Stats.IceResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 25;
            monster.Designation = "Ice Queen";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(50 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PowerWave);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 52 },
                { Stats.MaximumHealth, 4000 },
                { Stats.MinimumPhysBaseDmg, 155 },
                { Stats.MaximumPhysBaseDmg, 165 },
                { Stats.DefenseBase, 90 },
                { Stats.AttackRatePvm, 260 },
                { Stats.DefenseRatePvm, 76 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 5f / 255 },
                { Stats.WaterResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}