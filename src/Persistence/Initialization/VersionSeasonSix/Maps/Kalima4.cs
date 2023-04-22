// <copyright file="Kalima4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 4 map.
/// </summary>
internal class Kalima4 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 27;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 4";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima4"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima4(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[190], 120, 050); // Death Angel 4
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[190], 105, 054); // Death Angel 4
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[190], 119, 057); // Death Angel 4
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[190], 110, 065); // Death Angel 4
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[190], 121, 067); // Death Angel 4
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[190], 111, 072); // Death Angel 4
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[190], 105, 086); // Death Angel 4
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[190], 118, 095); // Death Angel 4
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[190], 120, 075); // Death Angel 4
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[191], 087, 090); // Death Centurion 4
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[191], 068, 077); // Death Centurion 4
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[191], 063, 072); // Death Centurion 4
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[191], 058, 078); // Death Centurion 4
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[191], 057, 071); // Death Centurion 4
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[192], 110, 009); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[192], 118, 017); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[192], 110, 035); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[192], 121, 027); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[192], 119, 035); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[192], 114, 044); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[192], 108, 028); // Blood Soldier 4
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[193], 030, 075); // Aegis 4
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[193], 035, 021); // Aegis 4
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[193], 028, 017); // Aegis 4
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[193], 036, 011); // Aegis 4
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[193], 051, 011); // Aegis 4
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[193], 042, 012); // Aegis 4
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[193], 045, 022); // Aegis 4
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[193], 052, 024); // Aegis 4
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[193], 053, 017); // Aegis 4
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[193], 060, 009); // Aegis 4
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[193], 060, 022); // Aegis 4
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[194], 067, 022); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[194], 069, 009); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[194], 074, 014); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[194], 082, 008); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[194], 081, 019); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[194], 086, 013); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[194], 092, 006); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[194], 096, 016); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[194], 099, 009); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[194], 109, 019); // Rogue Centurion 4
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[195], 118, 084); // Necron 4
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[195], 104, 101); // Necron 4
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[195], 115, 106); // Necron 4
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[195], 093, 096); // Necron 4
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[195], 093, 084); // Necron 4
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[195], 082, 085); // Necron 4
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[195], 082, 077); // Necron 4
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[195], 074, 076); // Necron 4
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[196], 032, 050); // Schriker 4
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[196], 042, 051); // Schriker 4
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[196], 038, 058); // Schriker 4
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[196], 029, 065); // Schriker 4
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[196], 046, 066); // Schriker 4
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[196], 042, 097); // Schriker 4
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[196], 037, 109); // Schriker 4
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[196], 047, 107); // Schriker 4
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[196], 053, 093); // Schriker 4
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[196], 035, 087); // Schriker 4
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[197], 026, 076); // Illusion of Kundun 4
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 190;
            monster.Designation = "Death Angel 4";
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
                { Stats.Level, 74 },
                { Stats.MaximumHealth, 12000 },
                { Stats.MinimumPhysBaseDmg, 270 },
                { Stats.MaximumPhysBaseDmg, 295 },
                { Stats.DefenseBase, 185 },
                { Stats.AttackRatePvm, 397 },
                { Stats.DefenseRatePvm, 120 },
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
            monster.Number = 191;
            monster.Designation = "Death Centurion 4";
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
                { Stats.Level, 83 },
                { Stats.MaximumHealth, 22500 },
                { Stats.MinimumPhysBaseDmg, 350 },
                { Stats.MaximumPhysBaseDmg, 375 },
                { Stats.DefenseBase, 255 },
                { Stats.AttackRatePvm, 500 },
                { Stats.DefenseRatePvm, 175 },
                { Stats.PoisonResistance, 22f / 255 },
                { Stats.IceResistance, 22f / 255 },
                { Stats.LightningResistance, 22f / 255 },
                { Stats.FireResistance, 22f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 192;
            monster.Designation = "Blood Soldier 4";
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
                { Stats.Level, 70 },
                { Stats.MaximumHealth, 8800 },
                { Stats.MinimumPhysBaseDmg, 245 },
                { Stats.MaximumPhysBaseDmg, 265 },
                { Stats.DefenseBase, 165 },
                { Stats.AttackRatePvm, 360 },
                { Stats.DefenseRatePvm, 105 },
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
            monster.Number = 193;
            monster.Designation = "Aegis 4";
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
                { Stats.Level, 62 },
                { Stats.MaximumHealth, 6000 },
                { Stats.MinimumPhysBaseDmg, 215 },
                { Stats.MaximumPhysBaseDmg, 235 },
                { Stats.DefenseBase, 140 },
                { Stats.AttackRatePvm, 310 },
                { Stats.DefenseRatePvm, 87 },
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
            monster.Number = 194;
            monster.Designation = "Rogue Centurion 4";
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
                { Stats.Level, 66 },
                { Stats.MaximumHealth, 6900 },
                { Stats.MinimumPhysBaseDmg, 230 },
                { Stats.MaximumPhysBaseDmg, 250 },
                { Stats.DefenseBase, 150 },
                { Stats.AttackRatePvm, 330 },
                { Stats.DefenseRatePvm, 95 },
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
            monster.Number = 195;
            monster.Designation = "Necron 4";
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
                { Stats.Level, 78 },
                { Stats.MaximumHealth, 16000 },
                { Stats.MinimumPhysBaseDmg, 305 },
                { Stats.MaximumPhysBaseDmg, 330 },
                { Stats.DefenseBase, 215 },
                { Stats.AttackRatePvm, 440 },
                { Stats.DefenseRatePvm, 145 },
                { Stats.PoisonResistance, 21f / 255 },
                { Stats.IceResistance, 21f / 255 },
                { Stats.LightningResistance, 21f / 255 },
                { Stats.FireResistance, 21f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 196;
            monster.Designation = "Schriker 4";
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
                { Stats.Level, 88 },
                { Stats.MaximumHealth, 30000 },
                { Stats.MinimumPhysBaseDmg, 410 },
                { Stats.MaximumPhysBaseDmg, 435 },
                { Stats.DefenseBase, 300 },
                { Stats.AttackRatePvm, 575 },
                { Stats.DefenseRatePvm, 245 },
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
            monster.Number = 197;
            monster.Designation = "Illusion of Kundun 4";
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
                { Stats.Level, 100 },
                { Stats.MaximumHealth, 50000 },
                { Stats.MinimumPhysBaseDmg, 600 },
                { Stats.MaximumPhysBaseDmg, 625 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 7800 },
                { Stats.DefenseRatePvm, 295 },
                { Stats.PoisonResistance, 45f / 255 },
                { Stats.IceResistance, 45f / 255 },
                { Stats.LightningResistance, 45f / 255 },
                { Stats.FireResistance, 45f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}