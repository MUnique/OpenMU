// <copyright file="Kalima7.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 7 map.
/// </summary>
internal class Kalima7 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 36;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 7";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima7"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima7(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[334], 120, 050); // Death Angel 7
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[334], 105, 054); // Death Angel 7
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[334], 119, 057); // Death Angel 7
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[334], 110, 065); // Death Angel 7
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[334], 121, 067); // Death Angel 7
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[334], 111, 072); // Death Angel 7
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[334], 105, 086); // Death Angel 7
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[334], 118, 095); // Death Angel 7
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[334], 120, 075); // Death Angel 7
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[336], 087, 090); // Death Centurion 7
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[336], 068, 077); // Death Centurion 7
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[336], 063, 072); // Death Centurion 7
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[336], 058, 078); // Death Centurion 7
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[336], 057, 071); // Death Centurion 7
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[333], 110, 009); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[333], 118, 017); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[333], 110, 035); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[333], 121, 027); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[333], 119, 035); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[333], 114, 044); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[333], 108, 028); // Blood Soldier 7
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[331], 030, 075); // Aegis 7
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[331], 035, 021); // Aegis 7
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[331], 028, 017); // Aegis 7
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[331], 036, 011); // Aegis 7
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[331], 051, 011); // Aegis 7
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[331], 042, 012); // Aegis 7
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[331], 045, 022); // Aegis 7
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[331], 052, 024); // Aegis 7
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[331], 053, 017); // Aegis 7
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[331], 060, 009); // Aegis 7
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[331], 060, 022); // Aegis 7
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[332], 067, 022); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[332], 069, 009); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[332], 074, 014); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[332], 082, 008); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[332], 081, 019); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[332], 086, 013); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[332], 092, 006); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[332], 096, 016); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[332], 099, 009); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[332], 109, 019); // Rogue Centurion 7
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[335], 118, 084); // Necron 7
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[335], 104, 101); // Necron 7
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[335], 115, 106); // Necron 7
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[335], 093, 096); // Necron 7
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[335], 093, 084); // Necron 7
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[335], 082, 085); // Necron 7
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[335], 082, 077); // Necron 7
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[335], 074, 076); // Necron 7
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[337], 032, 050); // Schriker 7
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[337], 042, 051); // Schriker 7
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[337], 038, 058); // Schriker 7
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[337], 029, 065); // Schriker 7
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[337], 046, 066); // Schriker 7
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[337], 042, 097); // Schriker 7
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[337], 037, 109); // Schriker 7
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[337], 047, 107); // Schriker 7
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[337], 053, 093); // Schriker 7
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[337], 035, 087); // Schriker 7
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[275], 026, 076); // Illusion of Kundun 7
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 334;
            monster.Designation = "Death Angel 7";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 121 },
                { Stats.MaximumHealth, 110000 },
                { Stats.MinimumPhysBaseDmg, 870 },
                { Stats.MaximumPhysBaseDmg, 915 },
                { Stats.DefenseBase, 720 },
                { Stats.AttackRatePvm, 960 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.PoisonResistance, 26f / 255 },
                { Stats.IceResistance, 26f / 255 },
                { Stats.LightningResistance, 26f / 255 },
                { Stats.FireResistance, 26f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 336;
            monster.Designation = "Death Centurion 7";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 130 },
                { Stats.MaximumHealth, 145000 },
                { Stats.MinimumPhysBaseDmg, 1040 },
                { Stats.MaximumPhysBaseDmg, 1085 },
                { Stats.DefenseBase, 865 },
                { Stats.AttackRatePvm, 1080 },
                { Stats.DefenseRatePvm, 440 },
                { Stats.PoisonResistance, 28f / 255 },
                { Stats.IceResistance, 28f / 255 },
                { Stats.LightningResistance, 28f / 255 },
                { Stats.FireResistance, 28f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 333;
            monster.Designation = "Blood Soldier 7";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 97300 },
                { Stats.MinimumPhysBaseDmg, 805 },
                { Stats.MaximumPhysBaseDmg, 845 },
                { Stats.DefenseBase, 660 },
                { Stats.AttackRatePvm, 920 },
                { Stats.DefenseRatePvm, 345 },
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
            monster.Number = 331;
            monster.Designation = "Aegis 7";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 111 },
                { Stats.MaximumHealth, 82000 },
                { Stats.MinimumPhysBaseDmg, 712 },
                { Stats.MaximumPhysBaseDmg, 752 },
                { Stats.DefenseBase, 584 },
                { Stats.AttackRatePvm, 850 },
                { Stats.DefenseRatePvm, 315 },
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
            monster.Number = 332;
            monster.Designation = "Rogue Centurion 7";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 114 },
                { Stats.MaximumHealth, 88000 },
                { Stats.MinimumPhysBaseDmg, 751 },
                { Stats.MaximumPhysBaseDmg, 791 },
                { Stats.DefenseBase, 615 },
                { Stats.AttackRatePvm, 880 },
                { Stats.DefenseRatePvm, 330 },
                { Stats.PoisonResistance, 24f / 255 },
                { Stats.IceResistance, 24f / 255 },
                { Stats.LightningResistance, 24f / 255 },
                { Stats.FireResistance, 24f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 335;
            monster.Designation = "Necron 7";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 125 },
                { Stats.MaximumHealth, 125000 },
                { Stats.MinimumPhysBaseDmg, 951 },
                { Stats.MaximumPhysBaseDmg, 996 },
                { Stats.DefenseBase, 783 },
                { Stats.AttackRatePvm, 1015 },
                { Stats.DefenseRatePvm, 405 },
                { Stats.PoisonResistance, 27f / 255 },
                { Stats.IceResistance, 27f / 255 },
                { Stats.LightningResistance, 27f / 255 },
                { Stats.FireResistance, 27f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 337;
            monster.Designation = "Schriker 7";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 135 },
                { Stats.MaximumHealth, 170000 },
                { Stats.MinimumPhysBaseDmg, 1168 },
                { Stats.MaximumPhysBaseDmg, 1213 },
                { Stats.DefenseBase, 992 },
                { Stats.AttackRatePvm, 1190 },
                { Stats.DefenseRatePvm, 485 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.LightningResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 275;
            monster.Designation = "Illusion of Kundun 7";
            monster.MoveRange = 3;
            monster.AttackRange = 10;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10800 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 140000 },
                { Stats.MinimumPhysBaseDmg, 1070 },
                { Stats.MaximumPhysBaseDmg, 1105 },
                { Stats.DefenseBase, 892 },
                { Stats.AttackRatePvm, 1200 },
                { Stats.DefenseRatePvm, 450 },
                { Stats.PoisonResistance, 60f / 255 },
                { Stats.IceResistance, 60f / 255 },
                { Stats.LightningResistance, 60f / 255 },
                { Stats.FireResistance, 60f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}