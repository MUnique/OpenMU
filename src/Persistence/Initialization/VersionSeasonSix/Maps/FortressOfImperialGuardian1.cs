// <copyright file="FortressOfImperialGuardian1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// Map initialization for the Empire fortress 1 event map.
/// </summary>
internal class FortressOfImperialGuardian1 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 69;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Fortress of Imperial Guardian 1";

    /// <summary>
    /// Initializes a new instance of the <see cref="FortressOfImperialGuardian1"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public FortressOfImperialGuardian1(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Devias.Number;

    /// <inheritdoc />
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 523;
            monster.Designation = "Trap";
            monster.MoveRange = 0;
            monster.AttackRange = 1;
            monster.ViewRange = 2;
            monster.ObjectKind = NpcObjectKind.Trap;
            monster.IntelligenceTypeName = typeof(RandomAttackInRangeTrapIntelligence).FullName;
            monster.MoveDelay = new TimeSpan(0 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 57 },
                { Stats.MaximumHealth, 4500 },
                { Stats.MinimumPhysBaseDmg, 1570 },
                { Stats.MaximumPhysBaseDmg, 1880 },
                { Stats.DefenseBase, 1110 },
                { Stats.AttackRatePvm, 1085 },
                { Stats.DefenseRatePvm, 1085 },
                { Stats.PoisonResistance, 5f / 255 },
                { Stats.IceResistance, 5f / 255 },
                { Stats.WaterResistance, 5f / 255 },
                { Stats.FireResistance, 7f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 524;
            monster.Designation = "Evil Gate";
            monster.MoveRange = 0;
            monster.AttackRange = 0;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 988500 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 2515 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 2230 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 525;
            monster.Designation = "Lion Gate";
            monster.MoveRange = 0;
            monster.AttackRange = 0;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 988000 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 2615 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 2230 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 526;
            monster.Designation = "Statue";
            monster.MoveRange = 0;
            monster.AttackRange = 0;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 987500 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 2865 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 2230 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 527;
            monster.Designation = "Star Gate";
            monster.MoveRange = 0;
            monster.AttackRange = 0;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 987500 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 2865 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 2230 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 528;
            monster.Designation = "Rush Gate";
            monster.MoveRange = 0;
            monster.AttackRange = 0;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 0;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 994000 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 2750 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 2230 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 512;
            monster.Designation = "Quarter Master";
            monster.MoveRange = 6;
            monster.AttackRange = 7;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 134 },
                { Stats.MaximumHealth, 90000 },
                { Stats.MinimumPhysBaseDmg, 1100 },
                { Stats.MaximumPhysBaseDmg, 1200 },
                { Stats.DefenseBase, 992 },
                { Stats.AttackRatePvm, 1150 },
                { Stats.DefenseRatePvm, 585 },
                { Stats.PoisonResistance, 23f / 255 },
                { Stats.IceResistance, 23f / 255 },
                { Stats.WaterResistance, 23f / 255 },
                { Stats.FireResistance, 23f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 513;
            monster.Designation = "Combat Instructor";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 118 },
                { Stats.MaximumHealth, 68000 },
                { Stats.MinimumPhysBaseDmg, 931 },
                { Stats.MaximumPhysBaseDmg, 1116 },
                { Stats.DefenseBase, 750 },
                { Stats.AttackRatePvm, 1100 },
                { Stats.DefenseRatePvm, 550 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.WaterResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 514;
            monster.Designation = "Aticle's Head";
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
                { Stats.Level, 135 },
                { Stats.MaximumHealth, 300000 },
                { Stats.MinimumPhysBaseDmg, 900 },
                { Stats.MaximumPhysBaseDmg, 1300 },
                { Stats.DefenseBase, 850 },
                { Stats.AttackRatePvm, 1400 },
                { Stats.DefenseRatePvm, 797 },
                { Stats.WindResistance, 23f / 255 },
                { Stats.PoisonResistance, 23f / 255 },
                { Stats.IceResistance, 23f / 255 },
                { Stats.WaterResistance, 23f / 255 },
                { Stats.FireResistance, 23f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 515;
            monster.Designation = "Dark Ghost";
            monster.MoveRange = 6;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 134 },
                { Stats.MaximumHealth, 100000 },
                { Stats.MinimumPhysBaseDmg, 1168 },
                { Stats.MaximumPhysBaseDmg, 1250 },
                { Stats.DefenseBase, 1020 },
                { Stats.AttackRatePvm, 1270 },
                { Stats.DefenseRatePvm, 665 },
                { Stats.PoisonResistance, 25f / 255 },
                { Stats.IceResistance, 25f / 255 },
                { Stats.WaterResistance, 25f / 255 },
                { Stats.FireResistance, 25f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 516;
            monster.Designation = "Banshee";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 119 },
                { Stats.MaximumHealth, 105000 },
                { Stats.MinimumPhysBaseDmg, 980 },
                { Stats.MaximumPhysBaseDmg, 1060 },
                { Stats.DefenseBase, 830 },
                { Stats.AttackRatePvm, 1150 },
                { Stats.DefenseRatePvm, 605 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.WaterResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 517;
            monster.Designation = "Head Mounter";
            monster.MoveRange = 3;
            monster.AttackRange = 10;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10800 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 140000 },
                { Stats.MinimumPhysBaseDmg, 1000 },
                { Stats.MaximumPhysBaseDmg, 1400 },
                { Stats.DefenseBase, 950 },
                { Stats.AttackRatePvm, 1600 },
                { Stats.DefenseRatePvm, 880 },
                { Stats.PoisonResistance, 60f / 255 },
                { Stats.IceResistance, 60f / 255 },
                { Stats.WaterResistance, 60f / 255 },
                { Stats.FireResistance, 60f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 518;
            monster.Designation = "Defender";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 118 },
                { Stats.MaximumHealth, 78000 },
                { Stats.MinimumPhysBaseDmg, 951 },
                { Stats.MaximumPhysBaseDmg, 996 },
                { Stats.DefenseBase, 783 },
                { Stats.AttackRatePvm, 1015 },
                { Stats.DefenseRatePvm, 505 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.WaterResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 519;
            monster.Designation = "Forsaker";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 115 },
                { Stats.MaximumHealth, 70000 },
                { Stats.MinimumPhysBaseDmg, 870 },
                { Stats.MaximumPhysBaseDmg, 915 },
                { Stats.DefenseBase, 720 },
                { Stats.AttackRatePvm, 960 },
                { Stats.DefenseRatePvm, 470 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.WaterResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 520;
            monster.Designation = "Ocelot the Lord";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 113 },
                { Stats.MaximumHealth, 53000 },
                { Stats.MinimumPhysBaseDmg, 805 },
                { Stats.MaximumPhysBaseDmg, 845 },
                { Stats.DefenseBase, 660 },
                { Stats.AttackRatePvm, 920 },
                { Stats.DefenseRatePvm, 445 },
                { Stats.PoisonResistance, 25f / 255 },
                { Stats.IceResistance, 25f / 255 },
                { Stats.WaterResistance, 25f / 255 },
                { Stats.FireResistance, 25f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 521;
            monster.Designation = "Eric the Guard";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 120 },
                { Stats.MaximumHealth, 78500 },
                { Stats.MinimumPhysBaseDmg, 1040 },
                { Stats.MaximumPhysBaseDmg, 1085 },
                { Stats.DefenseBase, 865 },
                { Stats.AttackRatePvm, 1080 },
                { Stats.DefenseRatePvm, 540 },
                { Stats.PoisonResistance, 26f / 255 },
                { Stats.IceResistance, 26f / 255 },
                { Stats.WaterResistance, 26f / 255 },
                { Stats.FireResistance, 26f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 504;
            monster.Designation = "Gayion The Gladiator";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 138 },
                { Stats.MaximumHealth, 1800000 },
                { Stats.MinimumPhysBaseDmg, 2680 },
                { Stats.MaximumPhysBaseDmg, 4130 },
                { Stats.DefenseBase, 4550 },
                { Stats.AttackRatePvm, 2500 },
                { Stats.DefenseRatePvm, 1485 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.WaterResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 505;
            monster.Designation = "Jerry";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 1000000 },
                { Stats.MinimumPhysBaseDmg, 2100 },
                { Stats.MaximumPhysBaseDmg, 3400 },
                { Stats.DefenseBase, 5000 },
                { Stats.AttackRatePvm, 2000 },
                { Stats.DefenseRatePvm, 1500 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 506;
            monster.Designation = "Raymond";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 146 },
                { Stats.MaximumHealth, 1600000 },
                { Stats.MinimumPhysBaseDmg, 2600 },
                { Stats.MaximumPhysBaseDmg, 4000 },
                { Stats.DefenseBase, 5600 },
                { Stats.AttackRatePvm, 2600 },
                { Stats.DefenseRatePvm, 1800 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 507;
            monster.Designation = "Lucas";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 142 },
                { Stats.MaximumHealth, 1200000 },
                { Stats.MinimumPhysBaseDmg, 2200 },
                { Stats.MaximumPhysBaseDmg, 3600 },
                { Stats.DefenseBase, 5200 },
                { Stats.AttackRatePvm, 2200 },
                { Stats.DefenseRatePvm, 1200 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 508;
            monster.Designation = "Fred";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 145 },
                { Stats.MaximumHealth, 1500000 },
                { Stats.MinimumPhysBaseDmg, 2500 },
                { Stats.MaximumPhysBaseDmg, 3900 },
                { Stats.DefenseBase, 5500 },
                { Stats.AttackRatePvm, 2500 },
                { Stats.DefenseRatePvm, 1500 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 509;
            monster.Designation = "Hammerize";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 143 },
                { Stats.MaximumHealth, 1300000 },
                { Stats.MinimumPhysBaseDmg, 2300 },
                { Stats.MaximumPhysBaseDmg, 3700 },
                { Stats.DefenseBase, 5300 },
                { Stats.AttackRatePvm, 2300 },
                { Stats.DefenseRatePvm, 1300 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 510;
            monster.Designation = "Dual Berserker";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 144 },
                { Stats.MaximumHealth, 1400000 },
                { Stats.MinimumPhysBaseDmg, 2400 },
                { Stats.MaximumPhysBaseDmg, 3800 },
                { Stats.DefenseBase, 5400 },
                { Stats.AttackRatePvm, 2400 },
                { Stats.DefenseRatePvm, 1400 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 511;
            monster.Designation = "Devil Lord";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 141 },
                { Stats.MaximumHealth, 1100000 },
                { Stats.MinimumPhysBaseDmg, 2100 },
                { Stats.MaximumPhysBaseDmg, 3500 },
                { Stats.DefenseBase, 5100 },
                { Stats.AttackRatePvm, 2100 },
                { Stats.DefenseRatePvm, 1100 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 255f / 255 },
                { Stats.WaterResistance, 150f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The monsters are spawned by the imperial guardian event, zone by zone.
    /// </remarks>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return ImperialGuardianSpawns.GetSpawns(Number).Select(spawn => this.CreateMonsterSpawn(
            spawn.Number,
            this.NpcDictionary[spawn.MonsterNumber],
            spawn.X,
            spawn.X,
            spawn.Y,
            spawn.Y,
            1,
            spawn.Direction,
            SpawnTrigger.OnceAtWaveStart,
            spawn.WaveNumber));
    }
}