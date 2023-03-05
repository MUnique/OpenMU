// <copyright file="Kalima5.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 5 map.
/// </summary>
internal class Kalima5 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 28;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 5";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima5"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima5(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[260], 120, 050); // Death Angel 5
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[260], 105, 054); // Death Angel 5
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[260], 119, 057); // Death Angel 5
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[260], 110, 065); // Death Angel 5
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[260], 121, 067); // Death Angel 5
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[260], 111, 072); // Death Angel 5
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[260], 105, 086); // Death Angel 5
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[260], 118, 095); // Death Angel 5
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[260], 120, 075); // Death Angel 5
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[261], 087, 090); // Death Centurion 5
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[261], 068, 077); // Death Centurion 5
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[261], 063, 072); // Death Centurion 5
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[261], 058, 078); // Death Centurion 5
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[261], 057, 071); // Death Centurion 5
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[262], 110, 009); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[262], 118, 017); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[262], 110, 035); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[262], 121, 027); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[262], 119, 035); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[262], 114, 044); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[262], 108, 028); // Blood Soldier 5
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[263], 030, 075); // Aegis 5
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[263], 035, 021); // Aegis 5
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[263], 028, 017); // Aegis 5
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[263], 036, 011); // Aegis 5
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[263], 051, 011); // Aegis 5
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[263], 042, 012); // Aegis 5
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[263], 045, 022); // Aegis 5
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[263], 052, 024); // Aegis 5
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[263], 053, 017); // Aegis 5
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[263], 060, 009); // Aegis 5
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[263], 060, 022); // Aegis 5
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[264], 067, 022); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[264], 069, 009); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[264], 074, 014); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[264], 082, 008); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[264], 081, 019); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[264], 086, 013); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[264], 092, 006); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[264], 096, 016); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[264], 099, 009); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[264], 109, 019); // Rogue Centurion 5
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[265], 118, 084); // Necron 5
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[265], 104, 101); // Necron 5
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[265], 115, 106); // Necron 5
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[265], 093, 096); // Necron 5
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[265], 093, 084); // Necron 5
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[265], 082, 085); // Necron 5
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[265], 082, 077); // Necron 5
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[265], 074, 076); // Necron 5
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[266], 032, 050); // Schriker 5
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[266], 042, 051); // Schriker 5
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[266], 038, 058); // Schriker 5
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[266], 029, 065); // Schriker 5
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[266], 046, 066); // Schriker 5
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[266], 042, 097); // Schriker 5
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[266], 037, 109); // Schriker 5
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[266], 047, 107); // Schriker 5
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[266], 053, 093); // Schriker 5
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[266], 035, 087); // Schriker 5
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[267], 026, 076); // Illusion of Kundun 5
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 260;
            monster.Designation = "Death Angel 5";
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
                { Stats.Level, 88 },
                { Stats.MaximumHealth, 31000 },
                { Stats.MinimumPhysBaseDmg, 408 },
                { Stats.MaximumPhysBaseDmg, 443 },
                { Stats.DefenseBase, 315 },
                { Stats.AttackRatePvm, 587 },
                { Stats.DefenseRatePvm, 195 },
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
            monster.Number = 261;
            monster.Designation = "Death Centurion 5";
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
                { Stats.Level, 98 },
                { Stats.MaximumHealth, 48000 },
                { Stats.MinimumPhysBaseDmg, 546 },
                { Stats.MaximumPhysBaseDmg, 581 },
                { Stats.DefenseBase, 460 },
                { Stats.AttackRatePvm, 715 },
                { Stats.DefenseRatePvm, 250 },
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
            monster.Number = 262;
            monster.Designation = "Blood Soldier 5";
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
                { Stats.Level, 85 },
                { Stats.MaximumHealth, 26000 },
                { Stats.MinimumPhysBaseDmg, 365 },
                { Stats.MaximumPhysBaseDmg, 395 },
                { Stats.DefenseBase, 280 },
                { Stats.AttackRatePvm, 540 },
                { Stats.DefenseRatePvm, 177 },
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
            monster.Number = 263;
            monster.Designation = "Aegis 5";
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
                { Stats.Level, 79 },
                { Stats.MaximumHealth, 18000 },
                { Stats.MinimumPhysBaseDmg, 310 },
                { Stats.MaximumPhysBaseDmg, 340 },
                { Stats.DefenseBase, 230 },
                { Stats.AttackRatePvm, 460 },
                { Stats.DefenseRatePvm, 163 },
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
            monster.Number = 264;
            monster.Designation = "Rogue Centurion 5";
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
                { Stats.Level, 82 },
                { Stats.MaximumHealth, 21000 },
                { Stats.MinimumPhysBaseDmg, 335 },
                { Stats.MaximumPhysBaseDmg, 365 },
                { Stats.DefenseBase, 250 },
                { Stats.AttackRatePvm, 490 },
                { Stats.DefenseRatePvm, 168 },
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
            monster.Number = 265;
            monster.Designation = "Necron 5";
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
                { Stats.Level, 93 },
                { Stats.MaximumHealth, 38500 },
                { Stats.MinimumPhysBaseDmg, 470 },
                { Stats.MaximumPhysBaseDmg, 505 },
                { Stats.DefenseBase, 370 },
                { Stats.AttackRatePvm, 642 },
                { Stats.DefenseRatePvm, 220 },
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
            monster.Number = 266;
            monster.Designation = "Schriker 5";
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
                { Stats.Level, 105 },
                { Stats.MaximumHealth, 60000 },
                { Stats.MinimumPhysBaseDmg, 640 },
                { Stats.MaximumPhysBaseDmg, 675 },
                { Stats.DefenseBase, 515 },
                { Stats.AttackRatePvm, 810 },
                { Stats.DefenseRatePvm, 290 },
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
            monster.Number = 267;
            monster.Designation = "Illusion of Kundun 5";
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
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 100000 },
                { Stats.MinimumPhysBaseDmg, 835 },
                { Stats.MaximumPhysBaseDmg, 870 },
                { Stats.DefenseBase, 680 },
                { Stats.AttackRatePvm, 1000 },
                { Stats.DefenseRatePvm, 360 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.LightningResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}