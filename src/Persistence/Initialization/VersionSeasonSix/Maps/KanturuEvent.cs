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
    /// The monster number of Maya's body.
    /// </summary>
    protected const short MayaBodyNumber = 364;

    private const short MayaLeftHandNumber = 362;

    private const short MayaRightHandNumber = 363;

    private const short NightmareNumber = 361;

    private const short BladeHunterNumber = 354;

    private const short DreadfearNumber = 360;

    private const short TwinTaleNumber = 359;

    private const short GenociderNumber = 357;

    private const short PersonaNumber = 358;

    private const short ElpisNpcNumber = 368;

    private const short LaserTrapNumber = 106;

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEvent"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public KanturuEvent(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the spawn areas of the event waves, which are started by the
    /// <c>KanturuContext</c> in the order of their wave number.
    /// </summary>
    /// <remarks>
    /// It's shared with <c>AddKanturuMapContentUpdatePlugIn</c>, which adds these spawns to
    /// databases which were created before the event existed.
    /// Boss positions: Maya Left (202, 83), Maya Right (189, 82), Nightmare (78, 143).
    /// Maya room (bounded by laser traps): X:174-217, Y:54-83. Nightmare zone: X:75-88, Y:97-143.
    /// </remarks>
    protected static IEnumerable<(short Number, short MonsterNumber, byte X1, byte X2, byte Y1, byte Y2, short Quantity, byte WaveNumber)> EventWaveSpawns { get; } =
    [

        // Wave 0: Maya body rises when the battle starts (fixed position below the fight room).
        (299, MayaBodyNumber, 188, 188, 110, 110, 1, 0),

        // Wave 1: Phase 1 — 30 Blade Hunter + 10 Dreadfear (Maya room).
        (200, BladeHunterNumber, 175, 215, 58, 86, 30, 1),
        (201, DreadfearNumber, 175, 215, 58, 86, 10, 1),

        // Wave 2: Phase 1 boss — Maya's left hand.
        (210, MayaLeftHandNumber, 202, 202, 83, 83, 1, 2),

        // Wave 3: Phase 2 — 30 Blade Hunter + 10 Dreadfear (Maya room).
        (220, BladeHunterNumber, 175, 215, 58, 86, 30, 3),
        (221, DreadfearNumber, 175, 215, 58, 86, 10, 3),

        // Wave 4: Phase 2 boss — Maya's right hand.
        (230, MayaRightHandNumber, 189, 189, 82, 82, 1, 4),

        // Wave 5: Phase 3 — 10 Dreadfear + 10 Twin Tale (Maya room).
        (240, DreadfearNumber, 175, 215, 58, 86, 10, 5),
        (241, TwinTaleNumber, 175, 215, 58, 86, 10, 5),

        // Wave 6: Phase 3 bosses — both of Maya's hands.
        (250, MayaLeftHandNumber, 202, 202, 83, 83, 1, 6),
        (251, MayaRightHandNumber, 189, 189, 82, 82, 1, 6),

        // Wave 7: Nightmare preparation — 15 Genocider + 15 Dreadfear + 15 Persona (Nightmare zone).
        (260, GenociderNumber, 75, 88, 97, 137, 15, 7),
        (261, DreadfearNumber, 75, 88, 97, 137, 15, 7),
        (262, PersonaNumber, 75, 88, 97, 137, 15, 7),

        // Wave 8: Nightmare.
        (270, NightmareNumber, 78, 78, 143, 143, 1, 8),
    ];

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <summary>
    /// Gets the safezone map number. Players who die inside the Kanturu Event map
    /// respawn at Kanturu Relics (map 38).
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
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[ElpisNpcNumber], 77, 177, Direction.SouthWest); // Elpis NPC
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // Laser traps (auto-spawn on map load)
        var laserTrap = this.NpcDictionary[LaserTrapNumber];
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

        foreach (var (number, monsterNumber, x1, x2, y1, y2, quantity, waveNumber) in EventWaveSpawns)
        {
            yield return this.CreateMonsterSpawn(number, this.NpcDictionary[monsterNumber], x1, x2, y1, y2, quantity, Direction.Undefined, SpawnTrigger.OnceAtWaveStart, waveNumber);
        }
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // Maya (#364) - full body boss, rises at event start
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = MayaBodyNumber;
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
            monster.Number = NightmareNumber;
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
            monster.Number = MayaLeftHandNumber;
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
            monster.Number = MayaRightHandNumber;
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
