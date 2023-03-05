// <copyright file="Kalima2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 2 map.
/// </summary>
internal class Kalima2 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 25;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima2(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[174], 120, 050); // Death Angel 2
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[174], 105, 054); // Death Angel 2
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[174], 119, 057); // Death Angel 2
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[174], 110, 065); // Death Angel 2
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[174], 121, 067); // Death Angel 2
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[174], 111, 072); // Death Angel 2
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[174], 105, 086); // Death Angel 2
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[174], 118, 095); // Death Angel 2
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[174], 120, 075); // Death Angel 2
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[175], 087, 090); // Death Centurion 2
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[175], 068, 077); // Death Centurion 2
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[175], 063, 072); // Death Centurion 2
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[175], 058, 078); // Death Centurion 2
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[175], 057, 071); // Death Centurion 2
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[176], 110, 009); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[176], 118, 017); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[176], 110, 035); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[176], 121, 027); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[176], 119, 035); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[176], 114, 044); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[176], 108, 028); // Blood Soldier 2
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[177], 030, 075); // Aegis 2
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[177], 035, 021); // Aegis 2
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[177], 028, 017); // Aegis 2
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[177], 036, 011); // Aegis 2
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[177], 051, 011); // Aegis 2
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[177], 042, 012); // Aegis 2
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[177], 045, 022); // Aegis 2
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[177], 052, 024); // Aegis 2
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[177], 053, 017); // Aegis 2
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[177], 060, 009); // Aegis 2
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[177], 060, 022); // Aegis 2
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[178], 067, 022); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[178], 069, 009); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[178], 074, 014); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[178], 082, 008); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[178], 081, 019); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[178], 086, 013); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[178], 092, 006); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[178], 096, 016); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[178], 099, 009); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[178], 109, 019); // Rogue Centurion 2
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[179], 118, 084); // Necron 2
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[179], 104, 101); // Necron 2
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[179], 115, 106); // Necron 2
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[179], 093, 096); // Necron 2
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[179], 093, 084); // Necron 2
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[179], 082, 085); // Necron 2
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[179], 082, 077); // Necron 2
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[179], 074, 076); // Necron 2
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[180], 032, 050); // Schriker 2
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[180], 042, 051); // Schriker 2
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[180], 038, 058); // Schriker 2
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[180], 029, 065); // Schriker 2
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[180], 046, 066); // Schriker 2
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[180], 042, 097); // Schriker 2
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[180], 037, 109); // Schriker 2
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[180], 047, 107); // Schriker 2
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[180], 053, 093); // Schriker 2
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[180], 035, 087); // Schriker 2
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[181], 026, 076); // Illusion of Kundun 2
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 174;
            monster.Designation = "Death Angel 2";
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
                { Stats.Level, 40 },
                { Stats.MaximumHealth, 1800 },
                { Stats.MinimumPhysBaseDmg, 130 },
                { Stats.MaximumPhysBaseDmg, 137 },
                { Stats.DefenseBase, 57 },
                { Stats.AttackRatePvm, 182 },
                { Stats.DefenseRatePvm, 45 },
                { Stats.PoisonResistance, 14f / 255 },
                { Stats.IceResistance, 14f / 255 },
                { Stats.LightningResistance, 14f / 255 },
                { Stats.FireResistance, 14f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 175;
            monster.Designation = "Death Centurion 2";
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
                { Stats.Level, 48 },
                { Stats.MaximumHealth, 3000 },
                { Stats.MinimumPhysBaseDmg, 155 },
                { Stats.MaximumPhysBaseDmg, 162 },
                { Stats.DefenseBase, 81 },
                { Stats.AttackRatePvm, 235 },
                { Stats.DefenseRatePvm, 58 },
                { Stats.PoisonResistance, 16f / 255 },
                { Stats.IceResistance, 16f / 255 },
                { Stats.LightningResistance, 16f / 255 },
                { Stats.FireResistance, 16f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 176;
            monster.Designation = "Blood Soldier 2";
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
                { Stats.Level, 37 },
                { Stats.MaximumHealth, 1400 },
                { Stats.MinimumPhysBaseDmg, 120 },
                { Stats.MaximumPhysBaseDmg, 127 },
                { Stats.DefenseBase, 50 },
                { Stats.AttackRatePvm, 163 },
                { Stats.DefenseRatePvm, 40 },
                { Stats.PoisonResistance, 13f / 255 },
                { Stats.IceResistance, 13f / 255 },
                { Stats.LightningResistance, 13f / 255 },
                { Stats.FireResistance, 13f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 177;
            monster.Designation = "Aegis 2";
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
                { Stats.Level, 32 },
                { Stats.MaximumHealth, 1000 },
                { Stats.MinimumPhysBaseDmg, 105 },
                { Stats.MaximumPhysBaseDmg, 112 },
                { Stats.DefenseBase, 42 },
                { Stats.AttackRatePvm, 135 },
                { Stats.DefenseRatePvm, 33 },
                { Stats.PoisonResistance, 11f / 255 },
                { Stats.IceResistance, 11f / 255 },
                { Stats.LightningResistance, 11f / 255 },
                { Stats.FireResistance, 11f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 178;
            monster.Designation = "Rogue Centurion 2";
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
                { Stats.Level, 34 },
                { Stats.MaximumHealth, 1200 },
                { Stats.MinimumPhysBaseDmg, 112 },
                { Stats.MaximumPhysBaseDmg, 119 },
                { Stats.DefenseBase, 45 },
                { Stats.AttackRatePvm, 147 },
                { Stats.DefenseRatePvm, 36 },
                { Stats.PoisonResistance, 12f / 255 },
                { Stats.IceResistance, 12f / 255 },
                { Stats.LightningResistance, 12f / 255 },
                { Stats.FireResistance, 12f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 179;
            monster.Designation = "Necron 2";
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
                { Stats.Level, 44 },
                { Stats.MaximumHealth, 2300 },
                { Stats.MinimumPhysBaseDmg, 140 },
                { Stats.MaximumPhysBaseDmg, 147 },
                { Stats.DefenseBase, 68 },
                { Stats.AttackRatePvm, 205 },
                { Stats.DefenseRatePvm, 50 },
                { Stats.PoisonResistance, 15f / 255 },
                { Stats.IceResistance, 15f / 255 },
                { Stats.LightningResistance, 15f / 255 },
                { Stats.FireResistance, 15f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 180;
            monster.Designation = "Schriker 2";
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
                { Stats.Level, 53 },
                { Stats.MaximumHealth, 3900 },
                { Stats.MinimumPhysBaseDmg, 180 },
                { Stats.MaximumPhysBaseDmg, 187 },
                { Stats.DefenseBase, 100 },
                { Stats.AttackRatePvm, 275 },
                { Stats.DefenseRatePvm, 67 },
                { Stats.PoisonResistance, 17f / 255 },
                { Stats.IceResistance, 17f / 255 },
                { Stats.LightningResistance, 17f / 255 },
                { Stats.FireResistance, 17f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 181;
            monster.Designation = "Illusion of Kundun 2";
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
                { Stats.Level, 65 },
                { Stats.MaximumHealth, 6000 },
                { Stats.MinimumPhysBaseDmg, 220 },
                { Stats.MaximumPhysBaseDmg, 227 },
                { Stats.DefenseBase, 140 },
                { Stats.AttackRatePvm, 355 },
                { Stats.DefenseRatePvm, 100 },
                { Stats.PoisonResistance, 35f / 255 },
                { Stats.IceResistance, 35f / 255 },
                { Stats.LightningResistance, 35f / 255 },
                { Stats.FireResistance, 35f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}