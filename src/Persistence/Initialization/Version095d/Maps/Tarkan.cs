// <copyright file="Tarkan.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Tarkan map.
/// </summary>
internal class Tarkan : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tarkan"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Tarkan(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 8;

    /// <inheritdoc/>
    protected override string MapName => "Tarkan";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[61], 7, 205);
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[61], 5, 214);
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[58], 8, 219);
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[61], 6, 228);
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[59], 11, 241);
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[58], 18, 238);
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[62], 146, 53);
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[61], 72, 167);
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[61], 83, 176);
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[61], 63, 173);
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[62], 148, 43);
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[62], 155, 40);
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[62], 134, 57);
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[62], 151, 34);
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[62], 149, 28);
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[62], 141, 74);
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[62], 142, 68);
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[61], 11, 234);
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[62], 141, 24);
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[62], 137, 20);
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[62], 132, 15);
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[60], 148, 66);
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[62], 134, 82);
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[62], 135, 89);
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[62], 136, 96);
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[62], 148, 91);
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[62], 157, 89);
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[62], 158, 80);
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[62], 154, 73);
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[62], 162, 101);
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[62], 161, 110);
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[62], 171, 116);
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[62], 180, 112);
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[62], 190, 112);
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[62], 197, 108);
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[62], 205, 105);
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[62], 186, 120);
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[60], 181, 123);
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[62], 173, 129);
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[62], 169, 136);
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[62], 162, 141);
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[62], 159, 148);
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[62], 151, 155);
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[62], 152, 147);
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[62], 152, 133);
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[62], 162, 129);
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[62], 152, 125);
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[62], 152, 116);
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[62], 171, 106);
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[62], 151, 101);
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[62], 142, 100);
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[62], 140, 37);
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[62], 133, 65);
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[60], 134, 72);
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[60], 128, 74);
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[57], 123, 73);
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[60], 115, 86);
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[60], 121, 87);
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[60], 128, 98);
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[60], 121, 99);
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[60], 108, 95);
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[60], 104, 87);
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[60], 107, 80);
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[60], 109, 64);
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[60], 108, 55);
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[60], 118, 42);
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[60], 126, 42);
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[60], 133, 44);
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[60], 125, 26);
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[60], 113, 34);
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[60], 105, 33);
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[60], 108, 43);
        yield return this.CreateMonsterSpawn(172, this.NpcDictionary[60], 103, 49);
        yield return this.CreateMonsterSpawn(173, this.NpcDictionary[60], 97, 46);
        yield return this.CreateMonsterSpawn(174, this.NpcDictionary[60], 88, 41);
        yield return this.CreateMonsterSpawn(175, this.NpcDictionary[60], 95, 26);
        yield return this.CreateMonsterSpawn(176, this.NpcDictionary[60], 96, 36);
        yield return this.CreateMonsterSpawn(177, this.NpcDictionary[60], 96, 56);
        yield return this.CreateMonsterSpawn(178, this.NpcDictionary[60], 102, 61);
        yield return this.CreateMonsterSpawn(179, this.NpcDictionary[60], 104, 70);
        yield return this.CreateMonsterSpawn(180, this.NpcDictionary[60], 96, 78);
        yield return this.CreateMonsterSpawn(181, this.NpcDictionary[60], 97, 66);
        yield return this.CreateMonsterSpawn(182, this.NpcDictionary[57], 81, 63);
        yield return this.CreateMonsterSpawn(183, this.NpcDictionary[57], 70, 57);
        yield return this.CreateMonsterSpawn(184, this.NpcDictionary[57], 65, 64);
        yield return this.CreateMonsterSpawn(185, this.NpcDictionary[57], 64, 71);
        yield return this.CreateMonsterSpawn(186, this.NpcDictionary[57], 56, 75);
        yield return this.CreateMonsterSpawn(187, this.NpcDictionary[57], 46, 71);
        yield return this.CreateMonsterSpawn(188, this.NpcDictionary[57], 38, 71);
        yield return this.CreateMonsterSpawn(189, this.NpcDictionary[57], 35, 62);
        yield return this.CreateMonsterSpawn(190, this.NpcDictionary[57], 27, 75);
        yield return this.CreateMonsterSpawn(191, this.NpcDictionary[57], 34, 78);
        yield return this.CreateMonsterSpawn(192, this.NpcDictionary[57], 25, 83);
        yield return this.CreateMonsterSpawn(193, this.NpcDictionary[57], 21, 92);
        yield return this.CreateMonsterSpawn(194, this.NpcDictionary[58], 38, 87);
        yield return this.CreateMonsterSpawn(195, this.NpcDictionary[57], 38, 98);
        yield return this.CreateMonsterSpawn(196, this.NpcDictionary[57], 48, 99);
        yield return this.CreateMonsterSpawn(197, this.NpcDictionary[57], 55, 100);
        yield return this.CreateMonsterSpawn(198, this.NpcDictionary[57], 62, 91);
        yield return this.CreateMonsterSpawn(199, this.NpcDictionary[57], 70, 92);
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[57], 79, 89);
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[57], 85, 91);
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[60], 91, 86);
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[57], 89, 71);
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[57], 90, 50);
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[58], 69, 120);
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[58], 61, 121);
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[58], 54, 120);
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[58], 43, 121);
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[58], 35, 121);
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[58], 24, 120);
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[58], 18, 127);
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[58], 25, 127);
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[58], 32, 132);
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[58], 29, 98);
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[58], 42, 134);
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[58], 35, 141);
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[58], 26, 149);
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[58], 26, 160);
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[58], 33, 157);
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[58], 40, 150);
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[58], 52, 138);
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[58], 61, 134);
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[60], 74, 81);
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[58], 75, 133);
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[58], 81, 126);
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[58], 87, 120);
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[58], 77, 118);
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[58], 96, 123);
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[58], 107, 119);
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[58], 112, 124);
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[58], 119, 139);
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[58], 107, 139);
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[58], 105, 130);
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[58], 107, 149);
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[58], 86, 131);
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[61], 73, 125);
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[58], 76, 145);
        yield return this.CreateMonsterSpawn(238, this.NpcDictionary[58], 69, 139);
        yield return this.CreateMonsterSpawn(239, this.NpcDictionary[58], 60, 143);
        yield return this.CreateMonsterSpawn(240, this.NpcDictionary[58], 52, 157);
        yield return this.CreateMonsterSpawn(241, this.NpcDictionary[58], 52, 165);
        yield return this.CreateMonsterSpawn(242, this.NpcDictionary[58], 45, 175);
        yield return this.CreateMonsterSpawn(243, this.NpcDictionary[58], 33, 168);
        yield return this.CreateMonsterSpawn(244, this.NpcDictionary[58], 35, 175);
        yield return this.CreateMonsterSpawn(245, this.NpcDictionary[58], 42, 164);
        yield return this.CreateMonsterSpawn(246, this.NpcDictionary[58], 64, 164);
        yield return this.CreateMonsterSpawn(247, this.NpcDictionary[61], 131, 178);
        yield return this.CreateMonsterSpawn(248, this.NpcDictionary[58], 79, 156);
        yield return this.CreateMonsterSpawn(249, this.NpcDictionary[58], 93, 137);
        yield return this.CreateMonsterSpawn(250, this.NpcDictionary[58], 115, 154);
        yield return this.CreateMonsterSpawn(251, this.NpcDictionary[58], 124, 152);
        yield return this.CreateMonsterSpawn(252, this.NpcDictionary[58], 129, 141);
        yield return this.CreateMonsterSpawn(253, this.NpcDictionary[58], 125, 129);
        yield return this.CreateMonsterSpawn(254, this.NpcDictionary[58], 124, 161);
        yield return this.CreateMonsterSpawn(255, this.NpcDictionary[58], 115, 168);
        yield return this.CreateMonsterSpawn(256, this.NpcDictionary[58], 120, 177);
        yield return this.CreateMonsterSpawn(257, this.NpcDictionary[61], 127, 171);
        yield return this.CreateMonsterSpawn(258, this.NpcDictionary[61], 127, 188);
        yield return this.CreateMonsterSpawn(259, this.NpcDictionary[61], 121, 196);
        yield return this.CreateMonsterSpawn(260, this.NpcDictionary[61], 123, 208);
        yield return this.CreateMonsterSpawn(261, this.NpcDictionary[61], 121, 217);
        yield return this.CreateMonsterSpawn(262, this.NpcDictionary[61], 106, 225);
        yield return this.CreateMonsterSpawn(263, this.NpcDictionary[61], 109, 217);
        yield return this.CreateMonsterSpawn(264, this.NpcDictionary[61], 102, 212);
        yield return this.CreateMonsterSpawn(265, this.NpcDictionary[61], 98, 207);
        yield return this.CreateMonsterSpawn(266, this.NpcDictionary[61], 88, 208);
        yield return this.CreateMonsterSpawn(267, this.NpcDictionary[61], 80, 207);
        yield return this.CreateMonsterSpawn(268, this.NpcDictionary[61], 78, 198);
        yield return this.CreateMonsterSpawn(269, this.NpcDictionary[61], 88, 189);
        yield return this.CreateMonsterSpawn(270, this.NpcDictionary[61], 83, 185);
        yield return this.CreateMonsterSpawn(271, this.NpcDictionary[58], 98, 193);
        yield return this.CreateMonsterSpawn(272, this.NpcDictionary[61], 73, 179);
        yield return this.CreateMonsterSpawn(273, this.NpcDictionary[58], 71, 184);
        yield return this.CreateMonsterSpawn(274, this.NpcDictionary[61], 161, 188);
        yield return this.CreateMonsterSpawn(275, this.NpcDictionary[61], 114, 209);
        yield return this.CreateMonsterSpawn(276, this.NpcDictionary[61], 153, 187);
        yield return this.CreateMonsterSpawn(277, this.NpcDictionary[61], 166, 190);
        yield return this.CreateMonsterSpawn(278, this.NpcDictionary[61], 159, 195);
        yield return this.CreateMonsterSpawn(279, this.NpcDictionary[61], 172, 195);
        yield return this.CreateMonsterSpawn(280, this.NpcDictionary[61], 179, 194);
        yield return this.CreateMonsterSpawn(281, this.NpcDictionary[61], 183, 201);
        yield return this.CreateMonsterSpawn(282, this.NpcDictionary[61], 177, 207);
        yield return this.CreateMonsterSpawn(283, this.NpcDictionary[58], 176, 214);
        yield return this.CreateMonsterSpawn(284, this.NpcDictionary[61], 173, 221);
        yield return this.CreateMonsterSpawn(285, this.NpcDictionary[58], 160, 203);
        yield return this.CreateMonsterSpawn(286, this.NpcDictionary[63], 161, 225);
        yield return this.CreateMonsterSpawn(287, this.NpcDictionary[61], 161, 218);
        yield return this.CreateMonsterSpawn(288, this.NpcDictionary[61], 157, 214);
        yield return this.CreateMonsterSpawn(289, this.NpcDictionary[61], 153, 209);
        yield return this.CreateMonsterSpawn(290, this.NpcDictionary[61], 150, 203);
        yield return this.CreateMonsterSpawn(291, this.NpcDictionary[61], 167, 225);
        yield return this.CreateMonsterSpawn(292, this.NpcDictionary[61], 93, 150);
        yield return this.CreateMonsterSpawn(293, this.NpcDictionary[61], 169, 204);
        yield return this.CreateMonsterSpawn(294, this.NpcDictionary[61], 44, 213);
        yield return this.CreateMonsterSpawn(295, this.NpcDictionary[61], 39, 210);
        yield return this.CreateMonsterSpawn(296, this.NpcDictionary[61], 31, 207);
        yield return this.CreateMonsterSpawn(297, this.NpcDictionary[58], 24, 212);
        yield return this.CreateMonsterSpawn(298, this.NpcDictionary[58], 27, 229);
        yield return this.CreateMonsterSpawn(299, this.NpcDictionary[61], 31, 236);
        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[61], 29, 245);
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[58], 36, 225);
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[58], 42, 244);
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[61], 50, 241);
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[61], 50, 230);
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[61], 43, 222);
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[61], 29, 220);
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[61], 15, 218);
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[61], 19, 205);
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[61], 11, 246);
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[61], 6, 240);
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[61], 157, 224);
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[61], 85, 141);
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[58], 22, 244);
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[58], 52, 204);
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[58], 165, 219);
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[57], 114, 78);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 57;
            monster.Designation = "Iron Wheel";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 17000 },
                { Stats.MinimumPhysBaseDmg, 280 },
                { Stats.MaximumPhysBaseDmg, 330 },
                { Stats.DefenseBase, 215 },
                { Stats.AttackRatePvm, 446 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 9f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 58;
            monster.Designation = "Tantallos";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(20 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 83 },
                { Stats.MaximumHealth, 22000 },
                { Stats.MinimumPhysBaseDmg, 335 },
                { Stats.MaximumPhysBaseDmg, 385 },
                { Stats.DefenseBase, 250 },
                { Stats.AttackRatePvm, 500 },
                { Stats.DefenseRatePvm, 175 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 9f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 59;
            monster.Designation = "Zaikan";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 34000 },
                { Stats.MinimumPhysBaseDmg, 510 },
                { Stats.MaximumPhysBaseDmg, 590 },
                { Stats.DefenseBase, 400 },
                { Stats.AttackRatePvm, 550 },
                { Stats.DefenseRatePvm, 185 },
                { Stats.PoisonResistance, 13f / 255 },
                { Stats.IceResistance, 13f / 255 },
                { Stats.WaterResistance, 13f / 255 },
                { Stats.FireResistance, 15f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 60;
            monster.Designation = "Bloody Wolf";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 76 },
                { Stats.MaximumHealth, 13500 },
                { Stats.MinimumPhysBaseDmg, 260 },
                { Stats.MaximumPhysBaseDmg, 300 },
                { Stats.DefenseBase, 200 },
                { Stats.AttackRatePvm, 410 },
                { Stats.DefenseRatePvm, 130 },
                { Stats.PoisonResistance, 8f / 255 },
                { Stats.IceResistance, 8f / 255 },
                { Stats.WaterResistance, 8f / 255 },
                { Stats.FireResistance, 8f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 61;
            monster.Designation = "Beam Knight";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(20 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 84 },
                { Stats.MaximumHealth, 25000 },
                { Stats.MinimumPhysBaseDmg, 375 },
                { Stats.MaximumPhysBaseDmg, 425 },
                { Stats.DefenseBase, 275 },
                { Stats.AttackRatePvm, 530 },
                { Stats.DefenseRatePvm, 190 },
                { Stats.PoisonResistance, 10f / 255 },
                { Stats.IceResistance, 10f / 255 },
                { Stats.WaterResistance, 10f / 255 },
                { Stats.FireResistance, 10f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 62;
            monster.Designation = "Mutant";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 72 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 250 },
                { Stats.MaximumPhysBaseDmg, 280 },
                { Stats.DefenseBase, 190 },
                { Stats.AttackRatePvm, 365 },
                { Stats.DefenseRatePvm, 120 },
                { Stats.PoisonResistance, 8f / 255 },
                { Stats.IceResistance, 8f / 255 },
                { Stats.WaterResistance, 8f / 255 },
                { Stats.FireResistance, 8f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 63;
            monster.Designation = "Death Beam Knight";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 93 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 590 },
                { Stats.MaximumPhysBaseDmg, 650 },
                { Stats.DefenseBase, 420 },
                { Stats.AttackRatePvm, 575 },
                { Stats.DefenseRatePvm, 220 },
                { Stats.PoisonResistance, 13f / 255 },
                { Stats.IceResistance, 13f / 255 },
                { Stats.WaterResistance, 13f / 255 },
                { Stats.FireResistance, 17f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}