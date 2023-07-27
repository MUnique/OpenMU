// <copyright file="DevilSquare3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 3.
/// </summary>
internal class DevilSquare3 : BaseMapInitializer
{

    /// <summary>
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 9;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Devil Square 3";

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare3"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare3(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 3;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 49;
        const byte x2 = 79;
        const byte y1 = 138;
        const byte y2 = 173;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[41], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Death Cow
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[37], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Devil

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[35], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Death Gorgon
        if (this.NpcDictionary.TryGetValue(180, out var shriker))
        {
            yield return this.CreateMonsterSpawn(4, shriker, x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Shriker
        }
        else
        {
            // In lower versions without Kalima, there is no Shriker, but a Cursed Wizard, which is of comparable strength
            yield return this.CreateMonsterSpawn(5, this.NpcDictionary[34], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Cursed Wizard
        }

        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[64], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Orc Archer
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[65], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Elite Orc

        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[67], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Metal Balrog
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 64;
            monster.Designation = "Orc Archer";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 70 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 220 },
                { Stats.MaximumPhysBaseDmg, 250 },
                { Stats.DefenseBase, 170 },
                { Stats.AttackRatePvm, 350 },
                { Stats.DefenseRatePvm, 115 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 65;
            monster.Designation = "Elite Orc";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 74 },
                { Stats.MaximumHealth, 14000 },
                { Stats.MinimumPhysBaseDmg, 260 },
                { Stats.MaximumPhysBaseDmg, 290 },
                { Stats.DefenseBase, 190 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 125 },
                { Stats.PoisonResistance, 8f / 255 },
                { Stats.IceResistance, 8f / 255 },
                { Stats.WaterResistance, 8f / 255 },
                { Stats.FireResistance, 8f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 67;
            monster.Designation = "Metal Balrog";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(200 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 77 },
                { Stats.MaximumHealth, 26000 },
                { Stats.MinimumPhysBaseDmg, 300 },
                { Stats.MaximumPhysBaseDmg, 360 },
                { Stats.DefenseBase, 220 },
                { Stats.AttackRatePvm, 385 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 9f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}