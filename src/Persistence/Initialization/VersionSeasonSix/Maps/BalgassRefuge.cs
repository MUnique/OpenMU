// <copyright file="BalgassRefuge.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Barracks of Balgass map.
/// </summary>
internal class BalgassRefuge : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 42;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Balgass Refuge";

    /// <summary>
    /// Initializes a new instance of the <see cref="BalgassRefuge"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BalgassRefuge(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[410], 104, 179); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[410], 104, 200); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[410], 085, 179); // Death Spirit (Trainee Soldier)
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[410], 086, 199); // Death Spirit (Trainee Soldier)

        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[411], 092, 174); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[411], 111, 190); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[411], 094, 202); // Soram (Trainee Soldier)
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[411], 082, 190); // Soram (Trainee Soldier)

        yield return this.CreateMonsterSpawn(20, this.NpcDictionary[412], 097, 187); // Dark Elf (Trainee Soldier)
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 412;
            monster.Designation = "Dark Elf (Trainee Soldier)";
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
                { Stats.Level, 128 },
                { Stats.MaximumHealth, 1500000 },
                { Stats.MinimumPhysBaseDmg, 800 },
                { Stats.MaximumPhysBaseDmg, 900 },
                { Stats.DefenseBase, 900 },
                { Stats.AttackRatePvm, 1500 },
                { Stats.DefenseRatePvm, 400 },
                { Stats.PoisonResistance, 254f / 255 },
                { Stats.IceResistance, 23f / 255 },
                { Stats.LightningResistance, 23f / 255 },
                { Stats.FireResistance, 23f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}