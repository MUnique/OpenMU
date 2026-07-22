// <copyright file="KanturuEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Kanturu event map.
/// </summary>
internal class KanturuEvent : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 39;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kanturu Event";

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEvent"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public KanturuEvent(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <summary>
    /// Players who die inside the Kanturu Event map respawn at Kanturu Relics (map 38).
    /// </summary>
    protected override byte SafezoneMapNumber => KanturuRelics.Number;

    /// <inheritdoc/>
    protected override void CreateMapAttributeRequirements()
    {
        this.CreateRequirement(Stats.MoonstonePendantEquipped, 1);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[368], 77, 177, Direction.SouthWest); // Elpis NPC
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // Laser traps (auto-spawn on map load)
        var laserTrap = this.NpcDictionary[106];
        yield return this.CreateMonsterSpawn(100, laserTrap, 60, 108);
        yield return this.CreateMonsterSpawn(101, laserTrap, 173, 61);
        yield return this.CreateMonsterSpawn(102, laserTrap, 173, 64);
        yield return this.CreateMonsterSpawn(103, laserTrap, 173, 67);
        yield return this.CreateMonsterSpawn(104, laserTrap, 173, 70);
        yield return this.CreateMonsterSpawn(105, laserTrap, 173, 73);
        yield return this.CreateMonsterSpawn(106, laserTrap, 173, 76);
        yield return this.CreateMonsterSpawn(107, laserTrap, 173, 79);
        yield return this.CreateMonsterSpawn(108, laserTrap, 179, 89);
        yield return this.CreateMonsterSpawn(109, laserTrap, 176, 86);
        yield return this.CreateMonsterSpawn(110, laserTrap, 173, 82);
        yield return this.CreateMonsterSpawn(111, laserTrap, 201, 94);
        yield return this.CreateMonsterSpawn(112, laserTrap, 204, 92);
        yield return this.CreateMonsterSpawn(113, laserTrap, 207, 91);
        yield return this.CreateMonsterSpawn(114, laserTrap, 210, 89);
        yield return this.CreateMonsterSpawn(115, laserTrap, 212, 88);
        yield return this.CreateMonsterSpawn(116, laserTrap, 215, 86);
        yield return this.CreateMonsterSpawn(117, laserTrap, 217, 84);
        yield return this.CreateMonsterSpawn(118, laserTrap, 218, 81);
        yield return this.CreateMonsterSpawn(119, laserTrap, 218, 78);
        yield return this.CreateMonsterSpawn(120, laserTrap, 218, 73);
        yield return this.CreateMonsterSpawn(121, laserTrap, 218, 70);
        yield return this.CreateMonsterSpawn(122, laserTrap, 218, 67);
        yield return this.CreateMonsterSpawn(123, laserTrap, 218, 64);
        yield return this.CreateMonsterSpawn(124, laserTrap, 217, 60);
        yield return this.CreateMonsterSpawn(125, laserTrap, 214, 57);
        yield return this.CreateMonsterSpawn(126, laserTrap, 211, 54);
        yield return this.CreateMonsterSpawn(127, laserTrap, 208, 54);
        yield return this.CreateMonsterSpawn(128, laserTrap, 205, 54);
        yield return this.CreateMonsterSpawn(129, laserTrap, 201, 54);
        yield return this.CreateMonsterSpawn(130, laserTrap, 198, 54);
        yield return this.CreateMonsterSpawn(131, laserTrap, 193, 54);
        yield return this.CreateMonsterSpawn(132, laserTrap, 190, 54);
        yield return this.CreateMonsterSpawn(133, laserTrap, 185, 54);
        yield return this.CreateMonsterSpawn(134, laserTrap, 182, 54);
        yield return this.CreateMonsterSpawn(135, laserTrap, 178, 56);
        yield return this.CreateMonsterSpawn(136, laserTrap, 176, 58);
        yield return this.CreateMonsterSpawn(137, laserTrap, 174, 59);

        // --- Event wave spawns (OnceAtWaveStart) ---
        // Boss positions: Maya Left (202,83), Maya Right (189,82), Nightmare (78,141)
        // Maya room (bounded by laser traps): X:174-217, Y:54-83
        // Nightmare zone: X:75-88, Y:97-141

        // Wave 0: Maya body rises when battle starts (fixed position below the fight room)
        var maya = this.NpcDictionary[364];
        yield return this.CreateMonsterSpawn(299, maya, 188, 188, 110, 110, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 0);

        // Wave 1: Phase 1 — 30 Blade Hunter + 10 Dreadfear (Maya room)
        var bladeHunter = this.NpcDictionary[354];
        var dreadfear = this.NpcDictionary[360];
        yield return this.CreateMonsterSpawn(200, bladeHunter, 175, 215, 58, 86, 30, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 1);
        yield return this.CreateMonsterSpawn(201, dreadfear,   175, 215, 58, 86, 10, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 1);

        // Wave 2: Phase 1 Boss — Maya Left Hand
        var mayaLeft = this.NpcDictionary[362];
        yield return this.CreateMonsterSpawn(210, mayaLeft, 202, 202, 83, 83, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 2);

        // Wave 3: Phase 2 — 30 Blade Hunter + 10 Dreadfear (Maya room)
        yield return this.CreateMonsterSpawn(220, bladeHunter, 175, 215, 58, 86, 30, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 3);
        yield return this.CreateMonsterSpawn(221, dreadfear,   175, 215, 58, 86, 10, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 3);

        // Wave 4: Phase 2 Boss — Maya Right Hand
        var mayaRight = this.NpcDictionary[363];
        yield return this.CreateMonsterSpawn(230, mayaRight, 189, 189, 82, 82, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 4);

        // Wave 5: Phase 3 — 10 Dreadfear + 10 Twin Tale (Maya room)
        var twinTale = this.NpcDictionary[359];
        yield return this.CreateMonsterSpawn(240, dreadfear, 175, 215, 58, 86, 10, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 5);
        yield return this.CreateMonsterSpawn(241, twinTale,  175, 215, 58, 86, 10, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 5);

        // Wave 6: Phase 3 Bosses — Maya Left Hand + Maya Right Hand
        yield return this.CreateMonsterSpawn(250, mayaLeft,  202, 202, 83, 83, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 6);
        yield return this.CreateMonsterSpawn(251, mayaRight, 189, 189, 82, 82, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 6);

        // Wave 7: Nightmare Prep — 15 Genocider + 15 Dreadfear + 15 Persona (Nightmare zone)
        var genocider = this.NpcDictionary[357];
        var persona = this.NpcDictionary[358];
        yield return this.CreateMonsterSpawn(260, genocider, 75, 88, 97, 137, 15, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 7);
        yield return this.CreateMonsterSpawn(261, dreadfear, 75, 88, 97, 137, 15, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 7);
        yield return this.CreateMonsterSpawn(262, persona,   75, 88, 97, 137, 15, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 7);

        // Wave 8: Nightmare
        var nightmare = this.NpcDictionary[361];
        yield return this.CreateMonsterSpawn(270, nightmare, 78, 78, 143, 143, 1, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, 8);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // Maya (#364) - full body boss, rises at event start
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 364;
            monster.Designation = "Maya";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(0);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 7;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 5_000_000 },
                { Stats.MinimumPhysBaseDmg, 2500 },
                { Stats.MaximumPhysBaseDmg, 3000 },
                { Stats.DefenseBase, 6500 },
                { Stats.AttackRatePvm, 2800 },
                { Stats.DefenseRatePvm, 2200 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.LightningResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        // Nightmare (#361)
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 361;
            monster.Designation = "Nightmare";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(0);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 5;
            // Nightmare uses a Decay (poison) area attack — applies poison DoT to players on each hit.
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Decay);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 145 },
                { Stats.MaximumHealth, 1_500_000 },
                { Stats.MinimumPhysBaseDmg, 3000 },
                { Stats.MaximumPhysBaseDmg, 3500 },
                { Stats.DefenseBase, 7500 },
                { Stats.AttackRatePvm, 3000 },
                { Stats.DefenseRatePvm, 2500 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.LightningResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        // Maya Left Hand (#362)
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 362;
            monster.Designation = "Maya (Hand Left)";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(0);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 3;
            // Maya Left Hand uses IceStorm — AoE ice attack that hits a 3×3 tile area around the target.
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IceStorm);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 130 },
                { Stats.MaximumHealth, 400_000 },
                { Stats.MinimumPhysBaseDmg, 2000 },
                { Stats.MaximumPhysBaseDmg, 2500 },
                { Stats.DefenseBase, 5000 },
                { Stats.AttackRatePvm, 2000 },
                { Stats.DefenseRatePvm, 1500 },
                { Stats.PoisonResistance, 40f / 255 },
                { Stats.IceResistance, 40f / 255 },
                { Stats.LightningResistance, 40f / 255 },
                { Stats.FireResistance, 40f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        // Maya Right Hand (#363)
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 363;
            monster.Designation = "Maya (Hand Right)";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(0);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 3;
            // Maya Right Hand uses IceStorm — same AoE ice attack as the left hand.
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IceStorm);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 130 },
                { Stats.MaximumHealth, 350_000 },
                { Stats.MinimumPhysBaseDmg, 2000 },
                { Stats.MaximumPhysBaseDmg, 2500 },
                { Stats.DefenseBase, 5000 },
                { Stats.AttackRatePvm, 2100 },
                { Stats.DefenseRatePvm, 1600 },
                { Stats.PoisonResistance, 40f / 255 },
                { Stats.IceResistance, 40f / 255 },
                { Stats.LightningResistance, 40f / 255 },
                { Stats.FireResistance, 40f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}
