// <copyright file="BarracksOfBalgass.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Barracks of Balgass map.
/// </summary>
internal class BarracksOfBalgass : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 41;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Barracks of Balgass";

    /// <summary>
    /// Initializes a new instance of the <see cref="BarracksOfBalgass"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BarracksOfBalgass(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc />
    protected override void InitializeDropItemGroups()
    {
        base.InitializeDropItemGroups();

        var flameOfCondor = this.Context.CreateNew<DropItemGroup>();
        flameOfCondor.SetGuid(this.MapNumber, 1);
        flameOfCondor.Chance = 0.001;
        flameOfCondor.Description = "Flame of Condor";
        flameOfCondor.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 13 && item.Number == 52));
        this.MapDefinition!.DropItemGroups.Add(flameOfCondor);
        this.GameConfiguration.DropItemGroups.Add(flameOfCondor);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[408], 119, 168, Direction.South); // Gatekeeper
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[409], 039, 101); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[409], 044, 110); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[409], 052, 134); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[409], 067, 112); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[409], 087, 090); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[409], 090, 067); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[409], 090, 078); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[409], 071, 101); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[409], 064, 132); // Balram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[409], 109, 101); // Balram (Trainee Soldier)

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[410], 098, 096); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[410], 122, 090); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[410], 125, 099); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[410], 137, 140); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[410], 140, 150); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[410], 044, 127); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[410], 057, 123); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[410], 079, 086); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[410], 101, 069); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[410], 121, 118); // Death Spirit (Trainee Soldier)

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[411], 127, 127); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[411], 126, 166, 164, 164); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[411], 123, 160); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[411], 132, 156); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[411], 132, 136); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[411], 116, 103); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[411], 129, 146); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[411], 117, 098); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[411], 113, 109); // Soram (Trainee Soldier)
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 409;
            monster.Designation = "Balram (Trainee Soldier)";
            monster.MoveRange = 6;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 75000 },
                { Stats.MinimumPhysBaseDmg, 550 },
                { Stats.MaximumPhysBaseDmg, 650 },
                { Stats.DefenseBase, 480 },
                { Stats.AttackRatePvm, 1200 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.PoisonResistance, 23f / 255 },
                { Stats.IceResistance, 23f / 255 },
                { Stats.LightningResistance, 23f / 255 },
                { Stats.FireResistance, 23f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 410;
            monster.Designation = "Death Spirit (Trainee Soldier)";
            monster.MoveRange = 6;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 119 },
                { Stats.MaximumHealth, 85000 },
                { Stats.MinimumPhysBaseDmg, 580 },
                { Stats.MaximumPhysBaseDmg, 680 },
                { Stats.DefenseBase, 490 },
                { Stats.AttackRatePvm, 1200 },
                { Stats.DefenseRatePvm, 390 },
                { Stats.PoisonResistance, 25f / 255 },
                { Stats.IceResistance, 25f / 255 },
                { Stats.LightningResistance, 25f / 255 },
                { Stats.FireResistance, 25f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 411;
            monster.Designation = "Soram (Trainee Soldier)";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 119 },
                { Stats.MaximumHealth, 90000 },
                { Stats.MinimumPhysBaseDmg, 600 },
                { Stats.MaximumPhysBaseDmg, 700 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 1200 },
                { Stats.DefenseRatePvm, 390 },
                { Stats.PoisonResistance, 25f / 255 },
                { Stats.IceResistance, 25f / 255 },
                { Stats.LightningResistance, 25f / 255 },
                { Stats.FireResistance, 25f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}