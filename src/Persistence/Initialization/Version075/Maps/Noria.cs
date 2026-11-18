// <copyright file="Noria.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

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
    /// Initializes a new instance of the <see cref="Noria"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Noria(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.Noria);

    /// <summary>
    /// Gets the automatic monster spawns of Noria.
    /// </summary>
    /// <remarks>
    /// This is the layout of Season 6 without the spiders, which Season 6 added for its first hunting quest.
    /// It places the monsters in smaller areas instead of spreading them over quarters of the map like the
    /// former data of version 0.75. All versions use it, because no better data of the older versions is known
    /// and it keeps the monster levels of the field maps consistent across the versions.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInBase"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(201, 26, 109, 154, 46, 181, 10), // Goblin
        new(202, 26, 136, 247, 156, 204, 10), // Goblin
        new(203, 26, 135, 247, 42, 73, 10), // Goblin
        new(204, 27, 109, 154, 46, 181, 20), // Chain Scorpion
        new(205, 27, 135, 247, 42, 73, 20), // Chain Scorpion
        new(206, 27, 136, 247, 156, 204, 20), // Chain Scorpion
        new(207, 33, 106, 247, 3, 40, 20), // Elite Goblin
        new(208, 33, 59, 128, 6, 106, 20), // Elite Goblin
        new(209, 28, 106, 247, 3, 40, 20), // Beetle Monster
        new(210, 28, 59, 128, 6, 106, 20), // Beetle Monster
        new(211, 29, 106, 247, 3, 40, 20), // Hunter
        new(212, 29, 59, 128, 6, 106, 20), // Hunter
        new(213, 30, 7, 63, 4, 176, 10), // Forest Monster
        new(214, 30, 41, 107, 107, 181, 10), // Forest Monster
        new(215, 30, 105, 247, 184, 246, 10), // Forest Monster
        new(216, 31, 7, 63, 4, 176, 20), // Agon
        new(217, 31, 41, 107, 107, 181, 20), // Agon
        new(218, 31, 105, 247, 184, 246, 20), // Agon
        new(219, 32, 4, 115, 168, 248, 40), // Stone Golem
    ];

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

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
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 26;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Goblin);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.ChainScorpion);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.BeetleMonster);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Hunter);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.ForestMonster);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Agon);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.StoneGolem);
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
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.EliteGoblin);
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