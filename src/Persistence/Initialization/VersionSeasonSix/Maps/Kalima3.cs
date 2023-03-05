// <copyright file="Kalima3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 3 map.
/// </summary>
internal class Kalima3 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 26;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 3";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima3"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima3(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[182], 120, 050); // Death Angel 3
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[182], 105, 054); // Death Angel 3
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[182], 119, 057); // Death Angel 3
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[182], 110, 065); // Death Angel 3
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[182], 121, 067); // Death Angel 3
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[182], 111, 072); // Death Angel 3
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[182], 105, 086); // Death Angel 3
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[182], 118, 095); // Death Angel 3
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[182], 120, 075); // Death Angel 3
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[183], 087, 090); // Death Centurion 3
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[183], 068, 077); // Death Centurion 3
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[183], 063, 072); // Death Centurion 3
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[183], 058, 078); // Death Centurion 3
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[183], 057, 071); // Death Centurion 3
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[184], 110, 009); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[184], 118, 017); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[184], 110, 035); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[184], 121, 027); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[184], 119, 035); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[184], 114, 044); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[184], 108, 028); // Blood Soldier 3
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[185], 030, 075); // Aegis 3
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[185], 035, 021); // Aegis 3
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[185], 028, 017); // Aegis 3
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[185], 036, 011); // Aegis 3
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[185], 051, 011); // Aegis 3
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[185], 042, 012); // Aegis 3
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[185], 045, 022); // Aegis 3
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[185], 052, 024); // Aegis 3
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[185], 053, 017); // Aegis 3
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[185], 060, 009); // Aegis 3
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[185], 060, 022); // Aegis 3
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[186], 067, 022); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[186], 069, 009); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[186], 074, 014); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[186], 082, 008); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[186], 081, 019); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[186], 086, 013); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[186], 092, 006); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[186], 096, 016); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[186], 099, 009); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[186], 109, 019); // Rogue Centurion 3
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[187], 118, 084); // Necron 3
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[187], 104, 101); // Necron 3
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[187], 115, 106); // Necron 3
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[187], 093, 096); // Necron 3
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[187], 093, 084); // Necron 3
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[187], 082, 085); // Necron 3
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[187], 082, 077); // Necron 3
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[187], 074, 076); // Necron 3
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[188], 032, 050); // Schriker 3
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[188], 042, 051); // Schriker 3
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[188], 038, 058); // Schriker 3
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[188], 029, 065); // Schriker 3
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[188], 046, 066); // Schriker 3
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[188], 042, 097); // Schriker 3
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[188], 037, 109); // Schriker 3
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[188], 047, 107); // Schriker 3
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[188], 053, 093); // Schriker 3
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[187], 035, 087); // Schriker 3
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[189], 026, 076); // Illusion of Kundun 3
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 182;
            monster.Designation = "Death Angel 3";
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
                { Stats.Level, 53 },
                { Stats.MaximumHealth, 4400 },
                { Stats.MinimumPhysBaseDmg, 175 },
                { Stats.MaximumPhysBaseDmg, 185 },
                { Stats.DefenseBase, 105 },
                { Stats.AttackRatePvm, 273 },
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
            monster.Number = 183;
            monster.Designation = "Death Centurion 3";
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
                { Stats.Level, 63 },
                { Stats.MaximumHealth, 6600 },
                { Stats.MinimumPhysBaseDmg, 225 },
                { Stats.MaximumPhysBaseDmg, 235 },
                { Stats.DefenseBase, 150 },
                { Stats.AttackRatePvm, 325 },
                { Stats.DefenseRatePvm, 92 },
                { Stats.PoisonResistance, 19f / 255 },
                { Stats.IceResistance, 19f / 255 },
                { Stats.LightningResistance, 19f / 255 },
                { Stats.FireResistance, 19f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 184;
            monster.Designation = "Blood Soldier 3";
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
                { Stats.Level, 50 },
                { Stats.MaximumHealth, 3650 },
                { Stats.MinimumPhysBaseDmg, 160 },
                { Stats.MaximumPhysBaseDmg, 170 },
                { Stats.DefenseBase, 90 },
                { Stats.AttackRatePvm, 250 },
                { Stats.DefenseRatePvm, 600 },
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
            monster.Number = 185;
            monster.Designation = "Aegis 3";
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
                { Stats.Level, 46 },
                { Stats.MaximumHealth, 2700 },
                { Stats.MinimumPhysBaseDmg, 145 },
                { Stats.MaximumPhysBaseDmg, 155 },
                { Stats.DefenseBase, 73 },
                { Stats.AttackRatePvm, 220 },
                { Stats.DefenseRatePvm, 54 },
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
            monster.Number = 186;
            monster.Designation = "Rogue Centurion 3";
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
                { Stats.Level, 48 },
                { Stats.MaximumHealth, 3100 },
                { Stats.MinimumPhysBaseDmg, 152 },
                { Stats.MaximumPhysBaseDmg, 162 },
                { Stats.DefenseBase, 80 },
                { Stats.AttackRatePvm, 232 },
                { Stats.DefenseRatePvm, 56 },
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
            monster.Number = 187;
            monster.Designation = "Necron 3";
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
                { Stats.Level, 58 },
                { Stats.MaximumHealth, 5300 },
                { Stats.MinimumPhysBaseDmg, 195 },
                { Stats.MaximumPhysBaseDmg, 205 },
                { Stats.DefenseBase, 126 },
                { Stats.AttackRatePvm, 296 },
                { Stats.DefenseRatePvm, 77 },
                { Stats.PoisonResistance, 18f / 255 },
                { Stats.IceResistance, 18f / 255 },
                { Stats.LightningResistance, 18f / 255 },
                { Stats.FireResistance, 18f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 188;
            monster.Designation = "Schriker 3";
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
                { Stats.Level, 69 },
                { Stats.MaximumHealth, 8400 },
                { Stats.MinimumPhysBaseDmg, 260 },
                { Stats.MaximumPhysBaseDmg, 270 },
                { Stats.DefenseBase, 180 },
                { Stats.AttackRatePvm, 365 },
                { Stats.DefenseRatePvm, 110 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.LightningResistance, 20f / 255 },
                { Stats.FireResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 189;
            monster.Designation = "Illusion of Kundun 3";
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
                { Stats.Level, 81 },
                { Stats.MaximumHealth, 14000 },
                { Stats.MinimumPhysBaseDmg, 350 },
                { Stats.MaximumPhysBaseDmg, 360 },
                { Stats.DefenseBase, 255 },
                { Stats.AttackRatePvm, 460 },
                { Stats.DefenseRatePvm, 160 },
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