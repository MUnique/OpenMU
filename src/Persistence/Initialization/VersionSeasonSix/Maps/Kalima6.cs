// <copyright file="Kalima6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Kalima 6 map.
/// </summary>
internal class Kalima6 : KalimaBase
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 29;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kalima 6";

    /// <summary>
    /// Initializes a new instance of the <see cref="Kalima6"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Kalima6(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[268], 120, 050); // Death Angel 6
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[268], 105, 054); // Death Angel 6
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[268], 119, 057); // Death Angel 6
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[268], 110, 065); // Death Angel 6
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[268], 121, 067); // Death Angel 6
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[268], 111, 072); // Death Angel 6
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[268], 105, 086); // Death Angel 6
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[268], 118, 095); // Death Angel 6
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[268], 120, 075); // Death Angel 6
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[269], 087, 090); // Death Centurion 6
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[269], 068, 077); // Death Centurion 6
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[269], 063, 072); // Death Centurion 6
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[269], 058, 078); // Death Centurion 6
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[269], 057, 071); // Death Centurion 6
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[270], 110, 009); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[270], 118, 017); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[270], 110, 035); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[270], 121, 027); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[270], 119, 035); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[270], 114, 044); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[270], 108, 028); // Blood Soldier 6
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[271], 030, 075); // Aegis 6
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[271], 035, 021); // Aegis 6
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[271], 028, 017); // Aegis 6
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[271], 036, 011); // Aegis 6
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[271], 051, 011); // Aegis 6
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[271], 042, 012); // Aegis 6
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[271], 045, 022); // Aegis 6
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[271], 052, 024); // Aegis 6
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[271], 053, 017); // Aegis 6
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[271], 060, 009); // Aegis 6
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[271], 060, 022); // Aegis 6
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[272], 067, 022); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[272], 069, 009); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[272], 074, 014); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[272], 082, 008); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[272], 081, 019); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[272], 086, 013); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[272], 092, 006); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[272], 096, 016); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[272], 099, 009); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[272], 109, 019); // Rogue Centurion 6
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[273], 118, 084); // Necron 6
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[273], 104, 101); // Necron 6
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[273], 115, 106); // Necron 6
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[273], 093, 096); // Necron 6
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[273], 093, 084); // Necron 6
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[273], 082, 085); // Necron 6
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[273], 082, 077); // Necron 6
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[273], 074, 076); // Necron 6
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[274], 032, 050); // Schriker 6
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[274], 042, 051); // Schriker 6
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[274], 038, 058); // Schriker 6
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[274], 029, 065); // Schriker 6
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[274], 046, 066); // Schriker 6
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[274], 042, 097); // Schriker 6
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[274], 037, 109); // Schriker 6
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[274], 047, 107); // Schriker 6
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[274], 053, 093); // Schriker 6
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[274], 035, 087); // Schriker 6
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[338], 026, 076); // Illusion of Kundun 6
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 268;
            monster.Designation = "Death Angel 6";
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
                { Stats.Level, 105 },
                { Stats.MaximumHealth, 65000 },
                { Stats.MinimumPhysBaseDmg, 610 },
                { Stats.MaximumPhysBaseDmg, 645 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 760 },
                { Stats.DefenseRatePvm, 275 },
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
            monster.Number = 269;
            monster.Designation = "Death Centurion 6";
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
                { Stats.Level, 114 },
                { Stats.MaximumHealth, 87500 },
                { Stats.MinimumPhysBaseDmg, 753 },
                { Stats.MaximumPhysBaseDmg, 788 },
                { Stats.DefenseBase, 620 },
                { Stats.AttackRatePvm, 900 },
                { Stats.DefenseRatePvm, 335 },
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
            monster.Number = 270;
            monster.Designation = "Blood Soldier 6";
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
                { Stats.Level, 101 },
                { Stats.MaximumHealth, 56000 },
                { Stats.MinimumPhysBaseDmg, 570 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 460 },
                { Stats.AttackRatePvm, 713 },
                { Stats.DefenseRatePvm, 255 },
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
            monster.Number = 271;
            monster.Designation = "Aegis 6";
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
                { Stats.Level, 95 },
                { Stats.MaximumHealth, 42000 },
                { Stats.MinimumPhysBaseDmg, 520 },
                { Stats.MaximumPhysBaseDmg, 550 },
                { Stats.DefenseBase, 410 },
                { Stats.AttackRatePvm, 655 },
                { Stats.DefenseRatePvm, 230 },
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
            monster.Number = 272;
            monster.Designation = "Rogue Centurion 6";
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
                { Stats.Level, 98 },
                { Stats.MaximumHealth, 48500 },
                { Stats.MinimumPhysBaseDmg, 540 },
                { Stats.MaximumPhysBaseDmg, 570 },
                { Stats.DefenseBase, 432 },
                { Stats.AttackRatePvm, 680 },
                { Stats.DefenseRatePvm, 240 },
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
            monster.Number = 273;
            monster.Designation = "Necron 6";
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
                { Stats.Level, 109 },
                { Stats.MaximumHealth, 75500 },
                { Stats.MinimumPhysBaseDmg, 672 },
                { Stats.MaximumPhysBaseDmg, 707 },
                { Stats.DefenseBase, 550 },
                { Stats.AttackRatePvm, 820 },
                { Stats.DefenseRatePvm, 303 },
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
            monster.Number = 274;
            monster.Designation = "Schriker 6";
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
                { Stats.Level, 119 },
                { Stats.MaximumHealth, 105000 },
                { Stats.MinimumPhysBaseDmg, 850 },
                { Stats.MaximumPhysBaseDmg, 885 },
                { Stats.DefenseBase, 710 },
                { Stats.AttackRatePvm, 1000 },
                { Stats.DefenseRatePvm, 370 },
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
            monster.Number = 338;
            monster.Designation = "Illusion of Kundun 6";
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