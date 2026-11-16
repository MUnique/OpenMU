// <copyright file="Devias.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
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
    /// Initializes a new instance of the <see cref="Devias"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Devias(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.Devias);

    /// <summary>
    /// Gets the automatic monster spawns of Devias.
    /// </summary>
    /// <remarks>
    /// This is the layout of Season 6, which places the monsters in smaller areas and at single spots,
    /// instead of spreading them over quarters of the map like the former data of version 0.75.
    /// All versions use it, because no better data of the older versions is known and it keeps the monster
    /// levels of the field maps consistent across the versions.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInBase"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(200, 24, 170, 245, 80, 200, 30), // Worm
        new(201, 23, 170, 245, 80, 200, 10), // Hommerd
        new(202, 22, 50, 166, 6, 244, 40), // Ice Monster
        new(203, 23, 50, 166, 6, 244, 30), // Hommerd
        new(204, 19, 50, 166, 6, 244, 40), // Yeti
        new(205, 21, 50, 166, 6, 244, 30), // Assassin
        new(206, 20, 50, 166, 6, 244, 20), // Elite Yeti
        new(207, 20, 5, 50, 50, 155, 20), // Elite Yeti
        new(208, 20, 6, 80, 184, 244, 10), // Elite Yeti
        new(209, 25, 6, 80, 184, 244, 3), // Ice Queen
        new(210, 25, 74, 74, 239, 239, 1), // Ice Queen
        new(211, 25, 70, 70, 212, 212, 1), // Ice Queen
        new(212, 25, 45, 45, 189, 189, 1), // Ice Queen
        new(213, 25, 113, 113, 230, 230, 1), // Ice Queen
        new(214, 25, 94, 94, 202, 202, 1), // Ice Queen, moved from (94, 203), where no monster can spawn
        new(215, 25, 63, 63, 160, 160, 1), // Ice Queen
        new(216, 20, 203, 203, 244, 244, 1), // Elite Yeti
        new(217, 20, 194, 194, 241, 241, 1), // Elite Yeti
        new(218, 20, 203, 203, 232, 232, 1), // Elite Yeti
        new(219, 20, 210, 210, 223, 223, 1), // Elite Yeti, moved from (209, 224), where no monster can spawn
        new(220, 20, 186, 186, 224, 224, 1), // Elite Yeti
        new(221, 20, 200, 200, 171, 171, 1), // Elite Yeti
        new(222, 20, 207, 207, 158, 158, 1), // Elite Yeti
        new(223, 20, 221, 221, 177, 177, 1), // Elite Yeti
        new(224, 20, 213, 213, 193, 193, 1), // Elite Yeti
        new(225, 20, 196, 196, 218, 218, 1), // Elite Yeti
        new(226, 20, 230, 230, 200, 200, 1), // Elite Yeti
        new(227, 20, 221, 221, 160, 160, 1), // Elite Yeti
        new(228, 20, 205, 205, 149, 149, 1), // Elite Yeti
        new(229, 20, 185, 185, 165, 165, 1), // Elite Yeti, moved from (184, 166), where no monster can spawn
        new(230, 20, 212, 212, 170, 170, 1), // Elite Yeti
        new(231, 25, 116, 116, 210, 210, 1), // Ice Queen
        new(232, 20, 203, 203, 201, 201, 1), // Elite Yeti
        new(233, 24, 170, 170, 6, 6, 1), // Worm
        new(234, 24, 161, 161, 58, 58, 1), // Worm
        new(235, 24, 180, 180, 78, 78, 1), // Worm
        new(236, 20, 190, 190, 146, 146, 1), // Elite Yeti
        new(237, 25, 51, 51, 227, 227, 1), // Ice Queen
        new(238, 20, 189, 189, 160, 160, 1), // Elite Yeti
        new(239, 20, 207, 207, 183, 183, 1), // Elite Yeti
        new(240, 20, 218, 218, 201, 201, 1), // Elite Yeti
        new(241, 20, 238, 238, 209, 209, 1), // Elite Yeti, moved from (240, 207), where no monster can spawn
        new(242, 20, 238, 238, 184, 184, 1), // Elite Yeti
        new(243, 20, 227, 227, 167, 167, 1), // Elite Yeti
        new(244, 20, 202, 202, 209, 209, 1), // Elite Yeti
        new(245, 20, 113, 113, 105, 105, 1), // Elite Yeti
        new(246, 20, 96, 96, 103, 103, 1), // Elite Yeti
        new(247, 20, 102, 102, 117, 117, 1), // Elite Yeti
        new(248, 20, 241, 241, 198, 198, 1), // Elite Yeti
        new(249, 20, 228, 228, 184, 184, 1), // Elite Yeti
        new(250, 20, 193, 193, 231, 231, 1), // Elite Yeti
        new(251, 20, 214, 214, 152, 152, 1), // Elite Yeti
        new(252, 20, 197, 197, 183, 183, 1), // Elite Yeti
        new(253, 24, 162, 162, 11, 11, 1), // Worm
        new(254, 24, 244, 244, 80, 80, 1), // Worm
        new(255, 24, 238, 238, 88, 88, 1), // Worm
        new(256, 24, 229, 229, 83, 83, 1), // Worm
    ];

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

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
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 19;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Yeti);
            monster.MoveRange = 2;
            monster.AttackRange = 4;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.EliteYeti);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Assassin);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.IceMonster);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Hommerd);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Worm);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.IceQueen);
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