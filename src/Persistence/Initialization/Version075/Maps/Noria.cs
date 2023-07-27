// <copyright file="Noria.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Noria map.
/// </summary>
internal class Noria : BaseMapInitializer
{
    /// <summary>
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 3;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Noria";

    /// <summary>
    /// Initializes a new instance of the <see cref="Noria"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Noria(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[253], 169, 109, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[253], 193, 110, Direction.South);
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[242], 173, 125, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[243], 195, 124, Direction.South);
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[240], 172, 96, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[238], 180, 103, Direction.SouthWest);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[26], 128, 251, 0, 128, 155);
        yield return this.CreateMonsterSpawn(22, this.NpcDictionary[27], 128, 251, 0, 128, 125);
        yield return this.CreateMonsterSpawn(23, this.NpcDictionary[28], 0, 128, 0, 128, 125);
        yield return this.CreateMonsterSpawn(24, this.NpcDictionary[29], 0, 128, 0, 128, 125);
        yield return this.CreateMonsterSpawn(25, this.NpcDictionary[30], 0, 251, 128, 245, 125);
        yield return this.CreateMonsterSpawn(26, this.NpcDictionary[31], 0, 251, 128, 245, 125);
        yield return this.CreateMonsterSpawn(27, this.NpcDictionary[32], 128, 251, 128, 245, 100);
        yield return this.CreateMonsterSpawn(28, this.NpcDictionary[33], 0, 128, 128, 245, 125);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 26;
            monster.Designation = "Goblin";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 3 },
                { Stats.MaximumHealth, 45 },
                { Stats.MinimumPhysBaseDmg, 7 },
                { Stats.MaximumPhysBaseDmg, 10 },
                { Stats.DefenseBase, 2 },
                { Stats.AttackRatePvm, 13 },
                { Stats.DefenseRatePvm, 2 },
                { Stats.WindResistance, 0f / 255 },
                { Stats.PoisonResistance, 0f / 255 },
                { Stats.IceResistance, 0f / 255 },
                { Stats.WaterResistance, 0f / 255 },
                { Stats.FireResistance, 0f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 27;
            monster.Designation = "Chain Scorpion";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 5 },
                { Stats.MaximumHealth, 80 },
                { Stats.MinimumPhysBaseDmg, 13 },
                { Stats.MaximumPhysBaseDmg, 17 },
                { Stats.DefenseBase, 4 },
                { Stats.AttackRatePvm, 23 },
                { Stats.DefenseRatePvm, 4 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 28;
            monster.Designation = "Beetle Monster";
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
                { Stats.Level, 10 },
                { Stats.MaximumHealth, 165 },
                { Stats.MinimumPhysBaseDmg, 26 },
                { Stats.MaximumPhysBaseDmg, 31 },
                { Stats.DefenseBase, 10 },
                { Stats.AttackRatePvm, 44 },
                { Stats.DefenseRatePvm, 10 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 29;
            monster.Designation = "Hunter";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 13 },
                { Stats.MaximumHealth, 220 },
                { Stats.MinimumPhysBaseDmg, 36 },
                { Stats.MaximumPhysBaseDmg, 41 },
                { Stats.DefenseBase, 13 },
                { Stats.AttackRatePvm, 56 },
                { Stats.DefenseRatePvm, 13 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 30;
            monster.Designation = "Forest Monster";
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
                { Stats.Level, 15 },
                { Stats.MaximumHealth, 295 },
                { Stats.MinimumPhysBaseDmg, 46 },
                { Stats.MaximumPhysBaseDmg, 51 },
                { Stats.DefenseBase, 15 },
                { Stats.AttackRatePvm, 68 },
                { Stats.DefenseRatePvm, 15 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 31;
            monster.Designation = "Agon";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 16 },
                { Stats.MaximumHealth, 340 },
                { Stats.MinimumPhysBaseDmg, 51 },
                { Stats.MaximumPhysBaseDmg, 57 },
                { Stats.DefenseBase, 16 },
                { Stats.AttackRatePvm, 74 },
                { Stats.DefenseRatePvm, 16 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 32;
            monster.Designation = "Stone Golem";
            monster.MoveRange = 2;
            monster.AttackRange = 2;
            monster.ViewRange = 3;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 18 },
                { Stats.MaximumHealth, 465 },
                { Stats.MinimumPhysBaseDmg, 62 },
                { Stats.MaximumPhysBaseDmg, 68 },
                { Stats.DefenseBase, 20 },
                { Stats.AttackRatePvm, 86 },
                { Stats.DefenseRatePvm, 20 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 33;
            monster.Designation = "Elite Goblin";
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
                { Stats.Level, 8 },
                { Stats.MaximumHealth, 120 },
                { Stats.MinimumPhysBaseDmg, 19 },
                { Stats.MaximumPhysBaseDmg, 23 },
                { Stats.DefenseBase, 8 },
                { Stats.AttackRatePvm, 33 },
                { Stats.DefenseRatePvm, 8 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}