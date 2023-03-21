// <copyright file="DevilSquare4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;

/// <summary>
/// Initialization for the devil square map which hosts devil square 1 to 4.
/// </summary>
internal class DevilSquare4 : BaseMapInitializer
{

    /// <summary>
    /// The default number of the map.
    /// </summary>
    internal const byte Number = 9;

    /// <summary>
    /// The default name of the map.
    /// </summary>
    internal const string Name = "Devil Square 4";

    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquare4"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquare4(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte Discriminator => 4;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Noria.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        const byte x1 = 53;
        const byte x2 = 83;
        const byte y1 = 74;
        const byte y2 = 109;
        const byte quantity = 35;

        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[64], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Orc Archer
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[65], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.FirstWaveNumber); // Elite Orc

        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[60], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Bloody Wolf
        if (this.NpcDictionary.TryGetValue(294, out var axeWarrior))
        {
            yield return this.CreateMonsterSpawn(4, this.NpcDictionary[294], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Axe Warrior
        }
        else
        {
            // In lower versions without Land of Trials, there is no Axe Warrior, but a the Alquamos, which is of comparable strength
            yield return this.CreateMonsterSpawn(5, this.NpcDictionary[69], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.SecondWaveNumber); // Alquamos
        }

        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[57], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Iron Wheel
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[70], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.AutomaticDuringWave, DevilSquareInitializer.ThirdWaveNumber); // Queen Rainer

        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[66], x1, x2, y1, y2, 5, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, DevilSquareInitializer.BossWaveNumber); // Cursed King
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 66;
            monster.Designation = "Cursed King";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(70 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 86 },
                { Stats.MaximumHealth, 38000 },
                { Stats.MinimumPhysBaseDmg, 500 },
                { Stats.MaximumPhysBaseDmg, 570 },
                { Stats.DefenseBase, 350 },
                { Stats.AttackRatePvm, 525 },
                { Stats.DefenseRatePvm, 200 },
                { Stats.PoisonResistance, 17f / 255 },
                { Stats.IceResistance, 17f / 255 },
                { Stats.WaterResistance, 17f / 255 },
                { Stats.FireResistance, 17f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}