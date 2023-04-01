// <copyright file="Raklion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Raklion map.
/// </summary>
internal class Raklion : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 57;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "LaCleon"; // Raklion

    /// <summary>
    /// Initializes a new instance of the <see cref="Raklion"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Raklion(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[454], 203, 204); // Ice Walker
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[454], 196, 203); // Ice Walker
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[454], 188, 203); // Ice Walker
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[454], 179, 204); // Ice Walker
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[454], 204, 104); // Ice Walker
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[454], 154, 129); // Ice Walker
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[454], 141, 113); // Ice Walker

        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[455], 171, 207); // Giant Mammoth
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[455], 174, 214); // Giant Mammoth
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[455], 164, 212); // Giant Mammoth
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[455], 142, 126); // Giant Mammoth
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[455], 223, 150); // Giant Mammoth

        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[456], 168, 217); // Ice Giant
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[456], 157, 217); // Ice Giant
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[456], 153, 210); // Ice Giant
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[456], 172, 188); // Ice Giant
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[456], 166, 187); // Ice Giant
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[456], 162, 182); // Ice Giant
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[456], 192, 110); // Ice Giant
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[456], 197, 132); // Ice Giant
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[456], 202, 149); // Ice Giant
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[456], 152, 136); // Ice Giant
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[456], 136, 137); // Ice Giant
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[456], 136, 106); // Ice Giant
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[456], 116, 148); // Ice Giant
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[456], 095, 195); // Ice Giant

        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[457], 149, 197); // Coolutin
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[457], 153, 194); // Coolutin
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[457], 143, 214); // Coolutin
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[457], 207, 026); // Coolutin
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[457], 210, 039); // Coolutin
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[457], 225, 089); // Coolutin
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[457], 212, 095); // Coolutin
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[457], 196, 120); // Coolutin
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[457], 185, 140); // Coolutin
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[457], 133, 099); // Coolutin
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[457], 130, 135); // Coolutin

        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[458], 120, 181); // Iron Knight
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[458], 134, 184); // Iron Knight
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[458], 111, 219); // Iron Knight
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[458], 101, 217); // Iron Knight
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[458], 090, 213); // Iron Knight
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[458], 098, 178); // Iron Knight
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[458], 123, 142); // Iron Knight
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[458], 143, 135); // Iron Knight
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[458], 144, 107); // Iron Knight
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[458], 127, 099); // Iron Knight
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[458], 161, 135); // Iron Knight
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[458], 191, 144); // Iron Knight
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[458], 189, 102); // Iron Knight
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[458], 196, 104); // Iron Knight
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[458], 220, 092); // Iron Knight
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[458], 233, 113); // Iron Knight
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[458], 232, 140); // Iron Knight
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[458], 222, 081); // Iron Knight
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[458], 203, 028); // Iron Knight
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[458], 211, 033); // Iron Knight
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[458], 215, 042); // Iron Knight
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[458], 203, 049); // Iron Knight

        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[562], 114, 101); // Dark Mammoth
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[562], 105, 107); // Dark Mammoth
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[562], 102, 109); // Dark Mammoth
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[562], 109, 101); // Dark Mammoth
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[562], 092, 117); // Dark Mammoth
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[562], 087, 116); // Dark Mammoth

        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[563], 033, 095); // Dark Giant
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[563], 034, 079); // Dark Giant
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[563], 037, 088); // Dark Giant
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[563], 058, 156); // Dark Giant
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[563], 065, 143); // Dark Giant
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[563], 038, 145); // Dark Giant
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[563], 070, 101); // Dark Giant
        yield return this.CreateMonsterSpawn(172, this.NpcDictionary[563], 062, 088); // Dark Giant
        yield return this.CreateMonsterSpawn(173, this.NpcDictionary[563], 073, 087); // Dark Giant
        yield return this.CreateMonsterSpawn(174, this.NpcDictionary[563], 033, 220); // Dark Giant
        yield return this.CreateMonsterSpawn(175, this.NpcDictionary[563], 047, 195); // Dark Giant
        yield return this.CreateMonsterSpawn(176, this.NpcDictionary[563], 098, 031); // Dark Giant
        yield return this.CreateMonsterSpawn(177, this.NpcDictionary[563], 123, 045); // Dark Giant
        yield return this.CreateMonsterSpawn(178, this.NpcDictionary[563], 140, 023); // Dark Giant
        yield return this.CreateMonsterSpawn(179, this.NpcDictionary[563], 039, 148); // Dark Giant
        yield return this.CreateMonsterSpawn(180, this.NpcDictionary[563], 068, 139); // Dark Giant
        yield return this.CreateMonsterSpawn(181, this.NpcDictionary[563], 057, 159); // Dark Giant
        yield return this.CreateMonsterSpawn(182, this.NpcDictionary[563], 032, 089); // Dark Giant
        yield return this.CreateMonsterSpawn(183, this.NpcDictionary[563], 038, 227); // Dark Giant
        yield return this.CreateMonsterSpawn(184, this.NpcDictionary[563], 026, 216); // Dark Giant
        yield return this.CreateMonsterSpawn(185, this.NpcDictionary[563], 049, 219); // Dark Giant
        yield return this.CreateMonsterSpawn(186, this.NpcDictionary[563], 029, 195); // Dark Giant
        yield return this.CreateMonsterSpawn(187, this.NpcDictionary[563], 071, 120); // Dark Giant
        yield return this.CreateMonsterSpawn(188, this.NpcDictionary[563], 037, 133); // Dark Giant

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[564], 123, 040); // Dark Coolutin
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[564], 117, 042); // Dark Coolutin
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[564], 037, 222); // Dark Coolutin
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[564], 052, 199); // Dark Coolutin
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[564], 034, 146); // Dark Coolutin
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[564], 063, 159); // Dark Coolutin
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[564], 062, 135); // Dark Coolutin
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[564], 060, 093); // Dark Coolutin
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[564], 033, 115); // Dark Coolutin
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[564], 059, 036); // Dark Coolutin
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[564], 064, 032); // Dark Coolutin
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[564], 035, 068); // Dark Coolutin
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[564], 046, 047); // Dark Coolutin
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[564], 107, 037); // Dark Coolutin
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[564], 028, 199); // Dark Coolutin

        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[565], 127, 043); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[565], 140, 032); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[565], 135, 029); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[565], 101, 023); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[565], 093, 026); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[565], 064, 039); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[565], 091, 031); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[565], 070, 095); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[565], 034, 121); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[565], 030, 119); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[565], 033, 224); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[565], 052, 191); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[565], 032, 195); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[565], 031, 051); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[565], 037, 046); // Dark Iron Knight
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[565], 066, 137); // Dark Iron Knight
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 454;
            monster.Designation = "Ice Walker";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Ice);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 102 },
                { Stats.MaximumHealth, 68000 },
                { Stats.MinimumPhysBaseDmg, 1310 },
                { Stats.MaximumPhysBaseDmg, 1965 },
                { Stats.DefenseBase, 615 },
                { Stats.AttackRatePvm, 1190 },
                { Stats.DefenseRatePvm, 800 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 455;
            monster.Designation = "Giant Mammoth";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 112 },
                { Stats.MaximumHealth, 77000 },
                { Stats.MinimumPhysBaseDmg, 1441 },
                { Stats.MaximumPhysBaseDmg, 2017 },
                { Stats.DefenseBase, 585 },
                { Stats.AttackRatePvm, 1350 },
                { Stats.DefenseRatePvm, 840 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 456;
            monster.Designation = "Ice Giant";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 122 },
                { Stats.MaximumHealth, 84000 },
                { Stats.MinimumPhysBaseDmg, 1585 },
                { Stats.MaximumPhysBaseDmg, 2060 },
                { Stats.DefenseBase, 620 },
                { Stats.AttackRatePvm, 1570 },
                { Stats.DefenseRatePvm, 770 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 457;
            monster.Designation = "Coolutin";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Ice);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 132 },
                { Stats.MaximumHealth, 88000 },
                { Stats.MinimumPhysBaseDmg, 1743 },
                { Stats.MaximumPhysBaseDmg, 2092 },
                { Stats.DefenseBase, 650 },
                { Stats.AttackRatePvm, 1940 },
                { Stats.DefenseRatePvm, 840 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 458;
            monster.Designation = "Iron Knight";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 142 },
                { Stats.MaximumHealth, 95000 },
                { Stats.MinimumPhysBaseDmg, 1917 },
                { Stats.MaximumPhysBaseDmg, 2301 },
                { Stats.DefenseBase, 660 },
                { Stats.AttackRatePvm, 2000 },
                { Stats.DefenseRatePvm, 800 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 562;
            monster.Designation = "Dark Mammoth";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 237000 },
                { Stats.MinimumPhysBaseDmg, 1741 },
                { Stats.MaximumPhysBaseDmg, 2317 },
                { Stats.DefenseBase, 785 },
                { Stats.AttackRatePvm, 2240 },
                { Stats.DefenseRatePvm, 1440 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 30f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 563;
            monster.Designation = "Dark Giant";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 143 },
                { Stats.MaximumHealth, 254000 },
                { Stats.MinimumPhysBaseDmg, 1885 },
                { Stats.MaximumPhysBaseDmg, 2360 },
                { Stats.DefenseBase, 820 },
                { Stats.AttackRatePvm, 2647 },
                { Stats.DefenseRatePvm, 970 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 30f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 564;
            monster.Designation = "Dark Coolutin";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 145 },
                { Stats.MaximumHealth, 248000 },
                { Stats.MinimumPhysBaseDmg, 2043 },
                { Stats.MaximumPhysBaseDmg, 2392 },
                { Stats.DefenseBase, 850 },
                { Stats.AttackRatePvm, 2142 },
                { Stats.DefenseRatePvm, 1440 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 30f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 565;
            monster.Designation = "Dark Iron Knight";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 148 },
                { Stats.MaximumHealth, 265000 },
                { Stats.MinimumPhysBaseDmg, 2217 },
                { Stats.MaximumPhysBaseDmg, 2601 },
                { Stats.DefenseBase, 860 },
                { Stats.AttackRatePvm, 2441 },
                { Stats.DefenseRatePvm, 1430 },
                { Stats.PoisonResistance, 150f / 255 },
                { Stats.IceResistance, 30f / 255 },
                { Stats.LightningResistance, 30f / 255 },
                { Stats.FireResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}