// <copyright file="Dungeon.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Dungeon map.
/// </summary>
internal class Dungeon : BaseMapInitializer
{
    /// <summary>
    /// The number of the map.
    /// </summary>
    internal const byte Number = 1;

    /// <summary>
    /// The name of the map.
    /// </summary>
    internal const string Name = "Dungeon";

    /// <summary>
    /// Initializes a new instance of the <see cref="Dungeon"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Dungeon(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[8], 42, 122);
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[8], 4, 73);
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[8], 12, 56);
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[8], 2, 78);
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[16], 108, 9);
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[14], 120, 232);
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[14], 98, 221);
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[14], 100, 220);
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[14], 100, 225);
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[14], 108, 232);
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[14], 107, 230);
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[12], 65, 205);
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[14], 84, 244);
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[15], 61, 169);
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[14], 94, 242);
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[14], 108, 195);
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[14], 94, 208);
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[11], 69, 167);
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[12], 87, 177);
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[11], 136, 149);
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[12], 70, 195);
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[14], 62, 181);
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[12], 61, 213);
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[11], 133, 212);
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[12], 69, 222);
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[12], 57, 247);
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[12], 49, 246);
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[14], 31, 247);
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[14], 3, 246);
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[14], 29, 246);
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[14], 83, 198);
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[17], 18, 220);
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[14], 33, 209);
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[11], 135, 212);
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[12], 86, 196);
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[11], 151, 212);
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[11], 168, 220);
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[12], 46, 210);
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[14], 45, 201);
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[12], 46, 205);
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[11], 46, 146);
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[11], 49, 182);
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[11], 44, 162);
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[11], 45, 180);
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[11], 45, 183);
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[11], 164, 232);
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[12], 137, 241);
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[12], 130, 244);
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[11], 92, 157);
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[11], 102, 174);
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[11], 113, 166);
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[15], 217, 229);
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[11], 222, 223);
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[11], 163, 184);
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[11], 164, 186);
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[17], 230, 173);
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[17], 236, 167);
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[11], 150, 193);
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[11], 142, 218);
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[12], 134, 240);
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[17], 241, 173);
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[11], 199, 116);
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[5], 170, 126);
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[14], 120, 243);
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[11], 240, 247);
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[11], 242, 247);
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[11], 241, 226);
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[11], 246, 220);
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[11], 243, 201);
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[5], 163, 114);
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[11], 244, 192);
        yield return this.CreateMonsterSpawn(172, this.NpcDictionary[11], 213, 248);
        yield return this.CreateMonsterSpawn(173, this.NpcDictionary[17], 173, 200);
        yield return this.CreateMonsterSpawn(174, this.NpcDictionary[11], 148, 149);
        yield return this.CreateMonsterSpawn(175, this.NpcDictionary[5], 163, 125);
        yield return this.CreateMonsterSpawn(176, this.NpcDictionary[15], 141, 110);
        yield return this.CreateMonsterSpawn(177, this.NpcDictionary[5], 145, 118);
        yield return this.CreateMonsterSpawn(178, this.NpcDictionary[13], 112, 93);
        yield return this.CreateMonsterSpawn(179, this.NpcDictionary[15], 98, 54);
        yield return this.CreateMonsterSpawn(180, this.NpcDictionary[14], 99, 49);
        yield return this.CreateMonsterSpawn(181, this.NpcDictionary[13], 104, 65);
        yield return this.CreateMonsterSpawn(182, this.NpcDictionary[13], 183, 27);
        yield return this.CreateMonsterSpawn(183, this.NpcDictionary[8], 187, 15);
        yield return this.CreateMonsterSpawn(184, this.NpcDictionary[8], 158, 17);
        yield return this.CreateMonsterSpawn(185, this.NpcDictionary[16], 159, 41);
        yield return this.CreateMonsterSpawn(186, this.NpcDictionary[16], 115, 7);
        yield return this.CreateMonsterSpawn(187, this.NpcDictionary[5], 123, 120);
        yield return this.CreateMonsterSpawn(188, this.NpcDictionary[15], 114, 110);
        yield return this.CreateMonsterSpawn(189, this.NpcDictionary[15], 68, 110);
        yield return this.CreateMonsterSpawn(190, this.NpcDictionary[15], 76, 111);
        yield return this.CreateMonsterSpawn(191, this.NpcDictionary[16], 124, 20);
        yield return this.CreateMonsterSpawn(192, this.NpcDictionary[16], 90, 6);
        yield return this.CreateMonsterSpawn(193, this.NpcDictionary[13], 114, 20);
        yield return this.CreateMonsterSpawn(194, this.NpcDictionary[5], 138, 86);
        yield return this.CreateMonsterSpawn(195, this.NpcDictionary[16], 212, 83);
        yield return this.CreateMonsterSpawn(196, this.NpcDictionary[16], 211, 90);
        yield return this.CreateMonsterSpawn(197, this.NpcDictionary[16], 226, 58);
        yield return this.CreateMonsterSpawn(198, this.NpcDictionary[16], 239, 56);
        yield return this.CreateMonsterSpawn(199, this.NpcDictionary[16], 235, 23);
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[13], 247, 87);
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[11], 247, 7);
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[11], 240, 9);
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[11], 241, 9);
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[9], 28, 18);
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[9], 22, 4);
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[9], 18, 29);
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[9], 18, 23);
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[9], 34, 4);
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[8], 119, 47);
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[9], 22, 90);
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[9], 33, 78);
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[8], 25, 100);
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[16], 29, 80);
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[9], 42, 117);
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[10], 14, 107);
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[8], 21, 82);
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[10], 22, 65);
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[10], 39, 77);
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[10], 29, 66);
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[8], 16, 55);
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[8], 35, 55);
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[8], 116, 47);
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[8], 122, 46);
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[8], 193, 30);
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[5], 120, 75);
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[13], 149, 94);
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[5], 81, 58);
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[13], 126, 94);
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[5], 71, 53);
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[5], 69, 88);
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[16], 86, 105);
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[16], 88, 104);
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[5], 98, 111);
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[15], 156, 125);
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[17], 165, 171);
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[17], 208, 162);
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[17], 220, 161);
        yield return this.CreateMonsterSpawn(238, this.NpcDictionary[17], 176, 188);
        yield return this.CreateMonsterSpawn(239, this.NpcDictionary[17], 168, 150);
        yield return this.CreateMonsterSpawn(240, this.NpcDictionary[17], 156, 171);
        yield return this.CreateMonsterSpawn(241, this.NpcDictionary[17], 176, 161);
        yield return this.CreateMonsterSpawn(242, this.NpcDictionary[17], 185, 161);
        yield return this.CreateMonsterSpawn(243, this.NpcDictionary[17], 181, 171);
        yield return this.CreateMonsterSpawn(244, this.NpcDictionary[17], 198, 168);
        yield return this.CreateMonsterSpawn(245, this.NpcDictionary[17], 190, 172);
        yield return this.CreateMonsterSpawn(246, this.NpcDictionary[11], 227, 248);
        yield return this.CreateMonsterSpawn(247, this.NpcDictionary[11], 181, 246);
        yield return this.CreateMonsterSpawn(248, this.NpcDictionary[14], 152, 247);
        yield return this.CreateMonsterSpawn(249, this.NpcDictionary[17], 128, 232);
        yield return this.CreateMonsterSpawn(250, this.NpcDictionary[14], 156, 247);
        yield return this.CreateMonsterSpawn(251, this.NpcDictionary[12], 193, 224);
        yield return this.CreateMonsterSpawn(252, this.NpcDictionary[12], 194, 222);
        yield return this.CreateMonsterSpawn(253, this.NpcDictionary[12], 194, 221);
        yield return this.CreateMonsterSpawn(254, this.NpcDictionary[11], 86, 150);
        yield return this.CreateMonsterSpawn(255, this.NpcDictionary[11], 98, 145);
        yield return this.CreateMonsterSpawn(256, this.NpcDictionary[12], 79, 154);
        yield return this.CreateMonsterSpawn(257, this.NpcDictionary[12], 78, 189);
        yield return this.CreateMonsterSpawn(258, this.NpcDictionary[14], 97, 187);
        yield return this.CreateMonsterSpawn(259, this.NpcDictionary[14], 105, 208);
        yield return this.CreateMonsterSpawn(260, this.NpcDictionary[14], 85, 208);
        yield return this.CreateMonsterSpawn(261, this.NpcDictionary[14], 84, 220);
        yield return this.CreateMonsterSpawn(262, this.NpcDictionary[14], 85, 229);
        yield return this.CreateMonsterSpawn(263, this.NpcDictionary[12], 100, 198);
        yield return this.CreateMonsterSpawn(264, this.NpcDictionary[12], 95, 192);
        yield return this.CreateMonsterSpawn(265, this.NpcDictionary[12], 90, 187);
        yield return this.CreateMonsterSpawn(266, this.NpcDictionary[14], 68, 227);
        yield return this.CreateMonsterSpawn(267, this.NpcDictionary[14], 61, 238);
        yield return this.CreateMonsterSpawn(268, this.NpcDictionary[14], 73, 245);
        yield return this.CreateMonsterSpawn(269, this.NpcDictionary[12], 73, 241);
        yield return this.CreateMonsterSpawn(270, this.NpcDictionary[14], 3, 232);
        yield return this.CreateMonsterSpawn(271, this.NpcDictionary[14], 3, 219);
        yield return this.CreateMonsterSpawn(272, this.NpcDictionary[14], 7, 219);
        yield return this.CreateMonsterSpawn(273, this.NpcDictionary[14], 17, 211);
        yield return this.CreateMonsterSpawn(274, this.NpcDictionary[17], 13, 235);
        yield return this.CreateMonsterSpawn(275, this.NpcDictionary[17], 27, 236);
        yield return this.CreateMonsterSpawn(276, this.NpcDictionary[11], 45, 188);
        yield return this.CreateMonsterSpawn(277, this.NpcDictionary[11], 53, 188);
        yield return this.CreateMonsterSpawn(278, this.NpcDictionary[11], 52, 166);
        yield return this.CreateMonsterSpawn(279, this.NpcDictionary[11], 62, 154);
        yield return this.CreateMonsterSpawn(280, this.NpcDictionary[11], 70, 154);
        yield return this.CreateMonsterSpawn(281, this.NpcDictionary[11], 77, 152);
        yield return this.CreateMonsterSpawn(282, this.NpcDictionary[11], 110, 145);
        yield return this.CreateMonsterSpawn(283, this.NpcDictionary[11], 119, 146);
        yield return this.CreateMonsterSpawn(284, this.NpcDictionary[11], 120, 159);
        yield return this.CreateMonsterSpawn(285, this.NpcDictionary[11], 111, 188);
        yield return this.CreateMonsterSpawn(286, this.NpcDictionary[11], 125, 195);
        yield return this.CreateMonsterSpawn(287, this.NpcDictionary[11], 135, 196);
        yield return this.CreateMonsterSpawn(288, this.NpcDictionary[11], 144, 187);
        yield return this.CreateMonsterSpawn(289, this.NpcDictionary[11], 125, 218);
        yield return this.CreateMonsterSpawn(290, this.NpcDictionary[17], 143, 232);
        yield return this.CreateMonsterSpawn(291, this.NpcDictionary[17], 50, 211);
        yield return this.CreateMonsterSpawn(292, this.NpcDictionary[11], 197, 245);
        yield return this.CreateMonsterSpawn(293, this.NpcDictionary[11], 247, 211);
        yield return this.CreateMonsterSpawn(294, this.NpcDictionary[11], 231, 186);
        yield return this.CreateMonsterSpawn(295, this.NpcDictionary[11], 223, 187);
        yield return this.CreateMonsterSpawn(296, this.NpcDictionary[11], 213, 186);
        yield return this.CreateMonsterSpawn(297, this.NpcDictionary[11], 209, 175);
        yield return this.CreateMonsterSpawn(298, this.NpcDictionary[15], 193, 148);
        yield return this.CreateMonsterSpawn(299, this.NpcDictionary[15], 188, 146);
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[15], 206, 152);
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[15], 212, 151);
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[17], 220, 172);
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[17], 247, 168);
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[15], 247, 176);
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[17], 52, 222);
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[17], 50, 235);
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[17], 38, 234);
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[17], 16, 227);
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[17], 25, 220);
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[17], 235, 108);
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[17], 228, 105);
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[17], 214, 116);
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[11], 183, 128);
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[15], 175, 127);
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[15], 175, 118);
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[15], 183, 123);
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[5], 87, 114);
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[5], 65, 127);
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[15], 68, 125);
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[15], 80, 97);
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[5], 70, 76);
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[13], 168, 96);
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[13], 196, 100);
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[16], 240, 77);
        yield return this.CreateMonsterSpawn(326, this.NpcDictionary[16], 247, 77);
        yield return this.CreateMonsterSpawn(327, this.NpcDictionary[16], 248, 20);
        yield return this.CreateMonsterSpawn(328, this.NpcDictionary[16], 244, 9);
        yield return this.CreateMonsterSpawn(329, this.NpcDictionary[16], 229, 11);
        yield return this.CreateMonsterSpawn(330, this.NpcDictionary[8], 205, 24);
        yield return this.CreateMonsterSpawn(331, this.NpcDictionary[16], 155, 32);
        yield return this.CreateMonsterSpawn(332, this.NpcDictionary[16], 137, 33);
        yield return this.CreateMonsterSpawn(333, this.NpcDictionary[16], 122, 29);
        yield return this.CreateMonsterSpawn(334, this.NpcDictionary[9], 118, 11);
        yield return this.CreateMonsterSpawn(335, this.NpcDictionary[16], 48, 11);
        yield return this.CreateMonsterSpawn(336, this.NpcDictionary[9], 38, 23);
        yield return this.CreateMonsterSpawn(337, this.NpcDictionary[9], 160, 10);
        yield return this.CreateMonsterSpawn(338, this.NpcDictionary[9], 194, 18);
        yield return this.CreateMonsterSpawn(339, this.NpcDictionary[9], 213, 12);
        yield return this.CreateMonsterSpawn(340, this.NpcDictionary[9], 221, 14);
        yield return this.CreateMonsterSpawn(341, this.NpcDictionary[9], 243, 16);
        yield return this.CreateMonsterSpawn(342, this.NpcDictionary[9], 233, 66);
        yield return this.CreateMonsterSpawn(343, this.NpcDictionary[8], 39, 112);
        yield return this.CreateMonsterSpawn(344, this.NpcDictionary[8], 26, 111);
        yield return this.CreateMonsterSpawn(345, this.NpcDictionary[8], 15, 113);
        yield return this.CreateMonsterSpawn(346, this.NpcDictionary[8], 12, 97);
        yield return this.CreateMonsterSpawn(347, this.NpcDictionary[8], 44, 56);
        yield return this.CreateMonsterSpawn(348, this.NpcDictionary[10], 41, 88);
        yield return this.CreateMonsterSpawn(349, this.NpcDictionary[10], 19, 120);
        yield return this.CreateMonsterSpawn(350, this.NpcDictionary[10], 37, 98);
        yield return this.CreateMonsterSpawn(351, this.NpcDictionary[9], 30, 105);
        yield return this.CreateMonsterSpawn(352, this.NpcDictionary[9], 27, 53);
        yield return this.CreateMonsterSpawn(353, this.NpcDictionary[9], 10, 68);
        yield return this.CreateMonsterSpawn(354, this.NpcDictionary[9], 33, 90);
        yield return this.CreateMonsterSpawn(355, this.NpcDictionary[9], 12, 121);
        yield return this.CreateMonsterSpawn(356, this.NpcDictionary[18], 43, 111);
        yield return this.CreateMonsterSpawn(357, this.NpcDictionary[18], 8, 123);
        yield return this.CreateMonsterSpawn(358, this.NpcDictionary[18], 7, 56);
        yield return this.CreateMonsterSpawn(359, this.NpcDictionary[18], 26, 67);
        yield return this.CreateMonsterSpawn(361, this.NpcDictionary[8], 42, 122);
        yield return this.CreateMonsterSpawn(362, this.NpcDictionary[8], 4, 73);
        yield return this.CreateMonsterSpawn(363, this.NpcDictionary[8], 12, 56);
        yield return this.CreateMonsterSpawn(364, this.NpcDictionary[8], 2, 78);
        yield return this.CreateMonsterSpawn(365, this.NpcDictionary[16], 108, 9);
        yield return this.CreateMonsterSpawn(366, this.NpcDictionary[14], 120, 232);
        yield return this.CreateMonsterSpawn(367, this.NpcDictionary[14], 107, 230);
        yield return this.CreateMonsterSpawn(368, this.NpcDictionary[12], 65, 205);
        yield return this.CreateMonsterSpawn(369, this.NpcDictionary[14], 84, 244);
        yield return this.CreateMonsterSpawn(370, this.NpcDictionary[15], 61, 169);
        yield return this.CreateMonsterSpawn(371, this.NpcDictionary[14], 94, 242);
        yield return this.CreateMonsterSpawn(372, this.NpcDictionary[14], 108, 195);
        yield return this.CreateMonsterSpawn(373, this.NpcDictionary[14], 94, 208);
        yield return this.CreateMonsterSpawn(374, this.NpcDictionary[11], 69, 167);
        yield return this.CreateMonsterSpawn(375, this.NpcDictionary[12], 87, 177);
        yield return this.CreateMonsterSpawn(376, this.NpcDictionary[11], 136, 149);
        yield return this.CreateMonsterSpawn(377, this.NpcDictionary[12], 70, 195);
        yield return this.CreateMonsterSpawn(378, this.NpcDictionary[14], 62, 181);
        yield return this.CreateMonsterSpawn(379, this.NpcDictionary[12], 61, 213);
        yield return this.CreateMonsterSpawn(380, this.NpcDictionary[11], 133, 212);
        yield return this.CreateMonsterSpawn(381, this.NpcDictionary[12], 69, 222);
        yield return this.CreateMonsterSpawn(382, this.NpcDictionary[12], 57, 247);
        yield return this.CreateMonsterSpawn(383, this.NpcDictionary[12], 49, 246);
        yield return this.CreateMonsterSpawn(384, this.NpcDictionary[14], 31, 247);
        yield return this.CreateMonsterSpawn(385, this.NpcDictionary[14], 3, 246);
        yield return this.CreateMonsterSpawn(386, this.NpcDictionary[14], 29, 246);
        yield return this.CreateMonsterSpawn(387, this.NpcDictionary[14], 83, 198);
        yield return this.CreateMonsterSpawn(388, this.NpcDictionary[17], 18, 220);
        yield return this.CreateMonsterSpawn(389, this.NpcDictionary[14], 33, 209);
        yield return this.CreateMonsterSpawn(390, this.NpcDictionary[11], 135, 212);
        yield return this.CreateMonsterSpawn(391, this.NpcDictionary[12], 86, 196);
        yield return this.CreateMonsterSpawn(392, this.NpcDictionary[11], 151, 212);
        yield return this.CreateMonsterSpawn(393, this.NpcDictionary[11], 168, 220);
        yield return this.CreateMonsterSpawn(394, this.NpcDictionary[12], 46, 210);
        yield return this.CreateMonsterSpawn(395, this.NpcDictionary[14], 45, 201);
        yield return this.CreateMonsterSpawn(396, this.NpcDictionary[12], 46, 205);
        yield return this.CreateMonsterSpawn(397, this.NpcDictionary[11], 46, 146);
        yield return this.CreateMonsterSpawn(398, this.NpcDictionary[11], 49, 182);
        yield return this.CreateMonsterSpawn(399, this.NpcDictionary[11], 44, 162);
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[11], 45, 180);
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[11], 45, 183);
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[11], 164, 232);
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[12], 137, 241);
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[12], 130, 244);
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[11], 92, 157);
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[11], 102, 174);
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[11], 113, 166);
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[15], 217, 229);
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[11], 222, 223);
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[11], 163, 184);
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[11], 164, 186);
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[17], 230, 173);
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[17], 236, 167);
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[11], 150, 193);
        yield return this.CreateMonsterSpawn(416, this.NpcDictionary[11], 142, 218);
        yield return this.CreateMonsterSpawn(417, this.NpcDictionary[12], 134, 240);
        yield return this.CreateMonsterSpawn(418, this.NpcDictionary[17], 241, 173);
        yield return this.CreateMonsterSpawn(419, this.NpcDictionary[11], 199, 116);
        yield return this.CreateMonsterSpawn(420, this.NpcDictionary[5], 170, 126);
        yield return this.CreateMonsterSpawn(421, this.NpcDictionary[14], 120, 243);
        yield return this.CreateMonsterSpawn(422, this.NpcDictionary[11], 240, 247);
        yield return this.CreateMonsterSpawn(423, this.NpcDictionary[11], 242, 247);
        yield return this.CreateMonsterSpawn(424, this.NpcDictionary[11], 241, 226);
        yield return this.CreateMonsterSpawn(425, this.NpcDictionary[11], 246, 220);
        yield return this.CreateMonsterSpawn(426, this.NpcDictionary[11], 243, 201);
        yield return this.CreateMonsterSpawn(427, this.NpcDictionary[5], 163, 114);
        yield return this.CreateMonsterSpawn(428, this.NpcDictionary[11], 244, 192);
        yield return this.CreateMonsterSpawn(429, this.NpcDictionary[11], 213, 248);
        yield return this.CreateMonsterSpawn(430, this.NpcDictionary[17], 173, 200);
        yield return this.CreateMonsterSpawn(431, this.NpcDictionary[11], 148, 149);
        yield return this.CreateMonsterSpawn(432, this.NpcDictionary[5], 163, 125);
        yield return this.CreateMonsterSpawn(433, this.NpcDictionary[15], 141, 110);
        yield return this.CreateMonsterSpawn(434, this.NpcDictionary[5], 145, 118);
        yield return this.CreateMonsterSpawn(435, this.NpcDictionary[13], 112, 93);
        yield return this.CreateMonsterSpawn(436, this.NpcDictionary[15], 98, 54);
        yield return this.CreateMonsterSpawn(437, this.NpcDictionary[14], 99, 49);
        yield return this.CreateMonsterSpawn(438, this.NpcDictionary[13], 104, 65);
        yield return this.CreateMonsterSpawn(439, this.NpcDictionary[13], 183, 27);
        yield return this.CreateMonsterSpawn(440, this.NpcDictionary[8], 187, 15);
        yield return this.CreateMonsterSpawn(441, this.NpcDictionary[8], 158, 17);
        yield return this.CreateMonsterSpawn(442, this.NpcDictionary[16], 159, 41);
        yield return this.CreateMonsterSpawn(443, this.NpcDictionary[16], 115, 7);
        yield return this.CreateMonsterSpawn(444, this.NpcDictionary[5], 123, 120);
        yield return this.CreateMonsterSpawn(445, this.NpcDictionary[15], 114, 110);
        yield return this.CreateMonsterSpawn(446, this.NpcDictionary[15], 68, 110);
        yield return this.CreateMonsterSpawn(447, this.NpcDictionary[15], 76, 111);
        yield return this.CreateMonsterSpawn(448, this.NpcDictionary[16], 124, 20);
        yield return this.CreateMonsterSpawn(449, this.NpcDictionary[16], 90, 6);
        yield return this.CreateMonsterSpawn(450, this.NpcDictionary[13], 114, 20);
        yield return this.CreateMonsterSpawn(451, this.NpcDictionary[5], 138, 86);
        yield return this.CreateMonsterSpawn(452, this.NpcDictionary[16], 212, 83);
        yield return this.CreateMonsterSpawn(453, this.NpcDictionary[16], 211, 90);
        yield return this.CreateMonsterSpawn(454, this.NpcDictionary[16], 226, 58);
        yield return this.CreateMonsterSpawn(455, this.NpcDictionary[16], 239, 56);
        yield return this.CreateMonsterSpawn(456, this.NpcDictionary[16], 235, 23);
        yield return this.CreateMonsterSpawn(457, this.NpcDictionary[13], 247, 87);
        yield return this.CreateMonsterSpawn(458, this.NpcDictionary[11], 247, 7);
        yield return this.CreateMonsterSpawn(459, this.NpcDictionary[11], 240, 9);
        yield return this.CreateMonsterSpawn(460, this.NpcDictionary[11], 241, 9);
        yield return this.CreateMonsterSpawn(461, this.NpcDictionary[9], 28, 18);
        yield return this.CreateMonsterSpawn(462, this.NpcDictionary[9], 22, 4);
        yield return this.CreateMonsterSpawn(463, this.NpcDictionary[9], 18, 29);
        yield return this.CreateMonsterSpawn(464, this.NpcDictionary[9], 18, 23);
        yield return this.CreateMonsterSpawn(465, this.NpcDictionary[9], 34, 4);
        yield return this.CreateMonsterSpawn(466, this.NpcDictionary[8], 119, 47);
        yield return this.CreateMonsterSpawn(467, this.NpcDictionary[9], 22, 90);
        yield return this.CreateMonsterSpawn(468, this.NpcDictionary[9], 33, 78);
        yield return this.CreateMonsterSpawn(469, this.NpcDictionary[8], 25, 100);
        yield return this.CreateMonsterSpawn(470, this.NpcDictionary[16], 29, 80);
        yield return this.CreateMonsterSpawn(471, this.NpcDictionary[9], 42, 117);
        yield return this.CreateMonsterSpawn(472, this.NpcDictionary[10], 14, 107);
        yield return this.CreateMonsterSpawn(473, this.NpcDictionary[8], 21, 82);
        yield return this.CreateMonsterSpawn(474, this.NpcDictionary[10], 22, 65);
        yield return this.CreateMonsterSpawn(475, this.NpcDictionary[10], 39, 77);
        yield return this.CreateMonsterSpawn(476, this.NpcDictionary[10], 29, 66);
        yield return this.CreateMonsterSpawn(477, this.NpcDictionary[8], 16, 55);
        yield return this.CreateMonsterSpawn(478, this.NpcDictionary[8], 35, 55);
        yield return this.CreateMonsterSpawn(479, this.NpcDictionary[8], 116, 47);
        yield return this.CreateMonsterSpawn(480, this.NpcDictionary[8], 122, 46);
        yield return this.CreateMonsterSpawn(481, this.NpcDictionary[8], 193, 30);
        yield return this.CreateMonsterSpawn(482, this.NpcDictionary[5], 120, 75);
        yield return this.CreateMonsterSpawn(483, this.NpcDictionary[13], 149, 94);
        yield return this.CreateMonsterSpawn(484, this.NpcDictionary[5], 81, 58);
        yield return this.CreateMonsterSpawn(485, this.NpcDictionary[13], 126, 94);
        yield return this.CreateMonsterSpawn(486, this.NpcDictionary[5], 71, 53);
        yield return this.CreateMonsterSpawn(487, this.NpcDictionary[5], 69, 88);
        yield return this.CreateMonsterSpawn(488, this.NpcDictionary[16], 86, 105);
        yield return this.CreateMonsterSpawn(489, this.NpcDictionary[16], 88, 104);
        yield return this.CreateMonsterSpawn(490, this.NpcDictionary[5], 98, 111);
        yield return this.CreateMonsterSpawn(491, this.NpcDictionary[15], 156, 125);
        yield return this.CreateMonsterSpawn(492, this.NpcDictionary[17], 165, 171);
        yield return this.CreateMonsterSpawn(493, this.NpcDictionary[17], 208, 162);
        yield return this.CreateMonsterSpawn(494, this.NpcDictionary[17], 220, 161);
        yield return this.CreateMonsterSpawn(495, this.NpcDictionary[17], 176, 188);
        yield return this.CreateMonsterSpawn(496, this.NpcDictionary[17], 168, 150);
        yield return this.CreateMonsterSpawn(497, this.NpcDictionary[17], 156, 171);
        yield return this.CreateMonsterSpawn(498, this.NpcDictionary[17], 176, 161);
        yield return this.CreateMonsterSpawn(499, this.NpcDictionary[17], 185, 161);
        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[17], 181, 171);
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[17], 198, 168);
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[17], 190, 172);
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[11], 227, 248);
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[11], 181, 246);
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[14], 152, 247);
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[17], 128, 232);
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[14], 156, 247);
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[12], 193, 224);
        yield return this.CreateMonsterSpawn(510, this.NpcDictionary[12], 194, 222);
        yield return this.CreateMonsterSpawn(511, this.NpcDictionary[12], 194, 221);
        yield return this.CreateMonsterSpawn(512, this.NpcDictionary[11], 86, 150);
        yield return this.CreateMonsterSpawn(513, this.NpcDictionary[11], 98, 145);
        yield return this.CreateMonsterSpawn(514, this.NpcDictionary[12], 79, 154);
        yield return this.CreateMonsterSpawn(515, this.NpcDictionary[12], 78, 189);
        yield return this.CreateMonsterSpawn(516, this.NpcDictionary[14], 97, 187);
        yield return this.CreateMonsterSpawn(517, this.NpcDictionary[14], 105, 208);
        yield return this.CreateMonsterSpawn(518, this.NpcDictionary[14], 85, 208);
        yield return this.CreateMonsterSpawn(519, this.NpcDictionary[14], 84, 220);
        yield return this.CreateMonsterSpawn(520, this.NpcDictionary[14], 85, 229);
        yield return this.CreateMonsterSpawn(521, this.NpcDictionary[12], 100, 198);
        yield return this.CreateMonsterSpawn(522, this.NpcDictionary[12], 95, 192);
        yield return this.CreateMonsterSpawn(523, this.NpcDictionary[12], 90, 187);
        yield return this.CreateMonsterSpawn(524, this.NpcDictionary[14], 68, 227);
        yield return this.CreateMonsterSpawn(525, this.NpcDictionary[14], 61, 238);
        yield return this.CreateMonsterSpawn(526, this.NpcDictionary[14], 73, 245);
        yield return this.CreateMonsterSpawn(527, this.NpcDictionary[12], 73, 241);
        yield return this.CreateMonsterSpawn(528, this.NpcDictionary[14], 3, 232);
        yield return this.CreateMonsterSpawn(529, this.NpcDictionary[14], 3, 219);
        yield return this.CreateMonsterSpawn(530, this.NpcDictionary[14], 7, 219);
        yield return this.CreateMonsterSpawn(531, this.NpcDictionary[14], 17, 211);
        yield return this.CreateMonsterSpawn(532, this.NpcDictionary[17], 13, 235);
        yield return this.CreateMonsterSpawn(533, this.NpcDictionary[17], 27, 236);
        yield return this.CreateMonsterSpawn(534, this.NpcDictionary[11], 45, 188);
        yield return this.CreateMonsterSpawn(535, this.NpcDictionary[11], 53, 188);
        yield return this.CreateMonsterSpawn(536, this.NpcDictionary[11], 52, 166);
        yield return this.CreateMonsterSpawn(537, this.NpcDictionary[11], 62, 154);
        yield return this.CreateMonsterSpawn(538, this.NpcDictionary[11], 70, 154);
        yield return this.CreateMonsterSpawn(539, this.NpcDictionary[11], 77, 152);
        yield return this.CreateMonsterSpawn(540, this.NpcDictionary[11], 110, 145);
        yield return this.CreateMonsterSpawn(541, this.NpcDictionary[11], 119, 146);
        yield return this.CreateMonsterSpawn(542, this.NpcDictionary[11], 120, 159);
        yield return this.CreateMonsterSpawn(543, this.NpcDictionary[11], 111, 188);
        yield return this.CreateMonsterSpawn(544, this.NpcDictionary[11], 125, 195);
        yield return this.CreateMonsterSpawn(545, this.NpcDictionary[11], 135, 196);
        yield return this.CreateMonsterSpawn(546, this.NpcDictionary[11], 144, 187);
        yield return this.CreateMonsterSpawn(547, this.NpcDictionary[11], 125, 218);
        yield return this.CreateMonsterSpawn(548, this.NpcDictionary[17], 143, 232);
        yield return this.CreateMonsterSpawn(549, this.NpcDictionary[17], 50, 211);
        yield return this.CreateMonsterSpawn(550, this.NpcDictionary[11], 197, 245);
        yield return this.CreateMonsterSpawn(551, this.NpcDictionary[11], 247, 211);
        yield return this.CreateMonsterSpawn(552, this.NpcDictionary[11], 231, 186);
        yield return this.CreateMonsterSpawn(553, this.NpcDictionary[11], 223, 187);
        yield return this.CreateMonsterSpawn(554, this.NpcDictionary[11], 213, 186);
        yield return this.CreateMonsterSpawn(555, this.NpcDictionary[11], 209, 175);
        yield return this.CreateMonsterSpawn(556, this.NpcDictionary[15], 193, 148);
        yield return this.CreateMonsterSpawn(557, this.NpcDictionary[15], 188, 146);
        yield return this.CreateMonsterSpawn(558, this.NpcDictionary[15], 206, 152);
        yield return this.CreateMonsterSpawn(559, this.NpcDictionary[15], 212, 151);
        yield return this.CreateMonsterSpawn(560, this.NpcDictionary[17], 220, 172);
        yield return this.CreateMonsterSpawn(561, this.NpcDictionary[17], 247, 168);
        yield return this.CreateMonsterSpawn(562, this.NpcDictionary[15], 247, 176);
        yield return this.CreateMonsterSpawn(563, this.NpcDictionary[17], 52, 222);
        yield return this.CreateMonsterSpawn(564, this.NpcDictionary[17], 50, 235);
        yield return this.CreateMonsterSpawn(565, this.NpcDictionary[17], 38, 234);
        yield return this.CreateMonsterSpawn(566, this.NpcDictionary[17], 16, 227);
        yield return this.CreateMonsterSpawn(567, this.NpcDictionary[17], 25, 220);
        yield return this.CreateMonsterSpawn(568, this.NpcDictionary[17], 235, 108);
        yield return this.CreateMonsterSpawn(569, this.NpcDictionary[17], 228, 105);
        yield return this.CreateMonsterSpawn(570, this.NpcDictionary[17], 214, 116);
        yield return this.CreateMonsterSpawn(571, this.NpcDictionary[11], 183, 128);
        yield return this.CreateMonsterSpawn(572, this.NpcDictionary[15], 175, 127);
        yield return this.CreateMonsterSpawn(573, this.NpcDictionary[15], 175, 118);
        yield return this.CreateMonsterSpawn(574, this.NpcDictionary[15], 183, 123);
        yield return this.CreateMonsterSpawn(575, this.NpcDictionary[5], 87, 114);
        yield return this.CreateMonsterSpawn(576, this.NpcDictionary[5], 65, 127);
        yield return this.CreateMonsterSpawn(577, this.NpcDictionary[15], 68, 125);
        yield return this.CreateMonsterSpawn(578, this.NpcDictionary[15], 80, 97);
        yield return this.CreateMonsterSpawn(579, this.NpcDictionary[5], 70, 76);
        yield return this.CreateMonsterSpawn(580, this.NpcDictionary[13], 168, 96);
        yield return this.CreateMonsterSpawn(581, this.NpcDictionary[13], 196, 100);
        yield return this.CreateMonsterSpawn(582, this.NpcDictionary[16], 240, 77);
        yield return this.CreateMonsterSpawn(583, this.NpcDictionary[16], 247, 77);
        yield return this.CreateMonsterSpawn(584, this.NpcDictionary[16], 248, 20);
        yield return this.CreateMonsterSpawn(585, this.NpcDictionary[16], 244, 9);
        yield return this.CreateMonsterSpawn(586, this.NpcDictionary[16], 229, 11);
        yield return this.CreateMonsterSpawn(587, this.NpcDictionary[8], 205, 24);
        yield return this.CreateMonsterSpawn(588, this.NpcDictionary[16], 155, 32);
        yield return this.CreateMonsterSpawn(589, this.NpcDictionary[16], 137, 33);
        yield return this.CreateMonsterSpawn(590, this.NpcDictionary[16], 122, 29);
        yield return this.CreateMonsterSpawn(591, this.NpcDictionary[9], 118, 11);
        yield return this.CreateMonsterSpawn(592, this.NpcDictionary[16], 48, 11);
        yield return this.CreateMonsterSpawn(593, this.NpcDictionary[9], 38, 23);
        yield return this.CreateMonsterSpawn(594, this.NpcDictionary[9], 160, 10);
        yield return this.CreateMonsterSpawn(595, this.NpcDictionary[9], 194, 18);
        yield return this.CreateMonsterSpawn(596, this.NpcDictionary[9], 213, 12);
        yield return this.CreateMonsterSpawn(597, this.NpcDictionary[9], 221, 14);
        yield return this.CreateMonsterSpawn(598, this.NpcDictionary[9], 243, 16);
        yield return this.CreateMonsterSpawn(599, this.NpcDictionary[9], 233, 66);
        yield return this.CreateMonsterSpawn(600, this.NpcDictionary[8], 39, 112);
        yield return this.CreateMonsterSpawn(601, this.NpcDictionary[8], 26, 111);
        yield return this.CreateMonsterSpawn(602, this.NpcDictionary[8], 15, 113);
        yield return this.CreateMonsterSpawn(603, this.NpcDictionary[8], 12, 97);
        yield return this.CreateMonsterSpawn(604, this.NpcDictionary[8], 44, 56);
        yield return this.CreateMonsterSpawn(605, this.NpcDictionary[10], 41, 88);
        yield return this.CreateMonsterSpawn(606, this.NpcDictionary[10], 19, 120);
        yield return this.CreateMonsterSpawn(607, this.NpcDictionary[10], 37, 98);
        yield return this.CreateMonsterSpawn(608, this.NpcDictionary[9], 30, 105);
        yield return this.CreateMonsterSpawn(609, this.NpcDictionary[9], 27, 53);
        yield return this.CreateMonsterSpawn(610, this.NpcDictionary[9], 10, 68);
        yield return this.CreateMonsterSpawn(611, this.NpcDictionary[9], 33, 90);
        yield return this.CreateMonsterSpawn(612, this.NpcDictionary[9], 12, 121);
        yield return this.CreateMonsterSpawn(613, this.NpcDictionary[18], 43, 111);
        yield return this.CreateMonsterSpawn(614, this.NpcDictionary[18], 8, 123);
        yield return this.CreateMonsterSpawn(615, this.NpcDictionary[18], 7, 56);
        yield return this.CreateMonsterSpawn(616, this.NpcDictionary[18], 26, 67);

        // Traps:
        yield return this.CreateMonsterSpawn(701, this.NpcDictionary[101], 10, 26, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(702, this.NpcDictionary[101], 11, 26, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(703, this.NpcDictionary[101], 27, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(704, this.NpcDictionary[101], 24, 5, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(705, this.NpcDictionary[101], 24, 4, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(706, this.NpcDictionary[101], 27, 11, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(707, this.NpcDictionary[101], 23, 24, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(708, this.NpcDictionary[101], 27, 21, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(709, this.NpcDictionary[101], 19, 19, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(720, this.NpcDictionary[101], 22, 24, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(721, this.NpcDictionary[101], 23, 29, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(722, this.NpcDictionary[101], 23, 28, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(723, this.NpcDictionary[101], 33, 9, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(724, this.NpcDictionary[101], 35, 9, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(725, this.NpcDictionary[101], 39, 18, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(726, this.NpcDictionary[101], 39, 17, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(727, this.NpcDictionary[101], 39, 16, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(728, this.NpcDictionary[102], 45, 224, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(729, this.NpcDictionary[101], 48, 193, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(730, this.NpcDictionary[101], 49, 193, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(731, this.NpcDictionary[102], 66, 71, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(732, this.NpcDictionary[102], 80, 61, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(733, this.NpcDictionary[101], 90, 164, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(734, this.NpcDictionary[101], 92, 164, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(735, this.NpcDictionary[101], 91, 164, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(736, this.NpcDictionary[100], 126, 99, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(737, this.NpcDictionary[100], 123, 99, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(738, this.NpcDictionary[100], 120, 99, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(739, this.NpcDictionary[100], 117, 99, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(740, this.NpcDictionary[100], 136, 95, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(741, this.NpcDictionary[100], 139, 95, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(742, this.NpcDictionary[101], 128, 212, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(743, this.NpcDictionary[101], 130, 213, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(744, this.NpcDictionary[101], 143, 214, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(745, this.NpcDictionary[101], 155, 230, Direction.NorthEast);
        yield return this.CreateMonsterSpawn(746, this.NpcDictionary[100], 172, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(747, this.NpcDictionary[100], 166, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(748, this.NpcDictionary[102], 169, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(749, this.NpcDictionary[102], 175, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(750, this.NpcDictionary[100], 178, 12, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(751, this.NpcDictionary[100], 177, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(752, this.NpcDictionary[100], 180, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(753, this.NpcDictionary[100], 183, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(754, this.NpcDictionary[100], 186, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(755, this.NpcDictionary[100], 189, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(756, this.NpcDictionary[100], 186, 151, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(757, this.NpcDictionary[100], 196, 33, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(758, this.NpcDictionary[100], 193, 33, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(759, this.NpcDictionary[100], 202, 93, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(760, this.NpcDictionary[100], 205, 93, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(761, this.NpcDictionary[100], 198, 130, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(762, this.NpcDictionary[100], 202, 150, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(763, this.NpcDictionary[100], 232, 46, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(764, this.NpcDictionary[100], 232, 40, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(765, this.NpcDictionary[100], 232, 37, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(766, this.NpcDictionary[100], 227, 61, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(767, this.NpcDictionary[100], 229, 93, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(768, this.NpcDictionary[100], 232, 93, Direction.SouthWest);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 5;
            monster.Designation = "Hell Hound";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 38 },
                { Stats.MaximumHealth, 1400 },
                { Stats.MinimumPhysBaseDmg, 125 },
                { Stats.MaximumPhysBaseDmg, 130 },
                { Stats.DefenseBase, 55 },
                { Stats.AttackRatePvm, 190 },
                { Stats.DefenseRatePvm, 45 },
                { Stats.FireResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 8;
            monster.Designation = "Poison Bull";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 46 },
                { Stats.MaximumHealth, 2500 },
                { Stats.MinimumPhysBaseDmg, 145 },
                { Stats.MaximumPhysBaseDmg, 150 },
                { Stats.DefenseBase, 75 },
                { Stats.AttackRatePvm, 230 },
                { Stats.DefenseRatePvm, 61 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.PoisonDamageMultiplier, 0.03f },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 9;
            monster.Designation = "Thunder Lich";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lightning);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 44 },
                { Stats.MaximumHealth, 2000 },
                { Stats.MinimumPhysBaseDmg, 140 },
                { Stats.MaximumPhysBaseDmg, 145 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 220 },
                { Stats.DefenseRatePvm, 55 },
                { Stats.WaterResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 10;
            monster.Designation = "Dark Knight";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 48 },
                { Stats.MaximumHealth, 3000 },
                { Stats.MinimumPhysBaseDmg, 150 },
                { Stats.MaximumPhysBaseDmg, 155 },
                { Stats.DefenseBase, 80 },
                { Stats.AttackRatePvm, 240 },
                { Stats.DefenseRatePvm, 70 },
                { Stats.PoisonResistance, 3f / 255 },
                { Stats.IceResistance, 3f / 255 },
                { Stats.WaterResistance, 3f / 255 },
                { Stats.FireResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 11;
            monster.Designation = "Ghost";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 32 },
                { Stats.MaximumHealth, 1000 },
                { Stats.MinimumPhysBaseDmg, 110 },
                { Stats.MaximumPhysBaseDmg, 115 },
                { Stats.DefenseBase, 40 },
                { Stats.AttackRatePvm, 160 },
                { Stats.DefenseRatePvm, 39 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.WaterResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 12;
            monster.Designation = "Larva";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 25 },
                { Stats.MaximumHealth, 750 },
                { Stats.MinimumPhysBaseDmg, 90 },
                { Stats.MaximumPhysBaseDmg, 95 },
                { Stats.DefenseBase, 31 },
                { Stats.AttackRatePvm, 125 },
                { Stats.DefenseRatePvm, 31 },
                { Stats.PoisonDamageMultiplier, 0.03f },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 13;
            monster.Designation = "Hell Spider";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PowerWave);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 40 },
                { Stats.MaximumHealth, 1600 },
                { Stats.MinimumPhysBaseDmg, 130 },
                { Stats.MaximumPhysBaseDmg, 135 },
                { Stats.DefenseBase, 60 },
                { Stats.AttackRatePvm, 200 },
                { Stats.DefenseRatePvm, 47 },
                { Stats.PoisonResistance, 3f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 15;
            monster.Designation = "Skeleton Archer";
            monster.MoveRange = 2;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 34 },
                { Stats.MaximumHealth, 1100 },
                { Stats.MinimumPhysBaseDmg, 115 },
                { Stats.MaximumPhysBaseDmg, 120 },
                { Stats.DefenseBase, 45 },
                { Stats.AttackRatePvm, 170 },
                { Stats.DefenseRatePvm, 41 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 16;
            monster.Designation = "Elite Skeleton";
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 42 },
                { Stats.MaximumHealth, 1800 },
                { Stats.MinimumPhysBaseDmg, 135 },
                { Stats.MaximumPhysBaseDmg, 140 },
                { Stats.DefenseBase, 65 },
                { Stats.AttackRatePvm, 210 },
                { Stats.DefenseRatePvm, 49 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 17;
            monster.Designation = "Cyclops";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 4;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 28 },
                { Stats.MaximumHealth, 850 },
                { Stats.MinimumPhysBaseDmg, 100 },
                { Stats.MaximumPhysBaseDmg, 105 },
                { Stats.DefenseBase, 35 },
                { Stats.AttackRatePvm, 140 },
                { Stats.DefenseRatePvm, 35 },
                { Stats.PoisonResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 18;
            monster.Designation = "Gorgon";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 55 },
                { Stats.MaximumHealth, 6000 },
                { Stats.MinimumPhysBaseDmg, 165 },
                { Stats.MaximumPhysBaseDmg, 175 },
                { Stats.DefenseBase, 100 },
                { Stats.AttackRatePvm, 275 },
                { Stats.DefenseRatePvm, 82 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.WaterResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var trap = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(trap);
            trap.Number = 100;
            trap.Designation = "Lance Trap";
            trap.MoveRange = 0;
            trap.AttackRange = 4;
            trap.ViewRange = 4;
            trap.ObjectKind = NpcObjectKind.Trap;
            trap.IntelligenceTypeName = typeof(AttackSingleWhenPressedTrapIntelligence).FullName;
            trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
            trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            trap.Attribute = 1;
            trap.NumberOfMaximumItemDrops = 0;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 1000 },
                { Stats.MinimumPhysBaseDmg, 100 },
                { Stats.MaximumPhysBaseDmg, 110 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 500 },
            };

            trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var trap = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(trap);
            trap.Number = 101;
            trap.Designation = "Iron Stick Trap";
            trap.MoveRange = 0;
            trap.AttackRange = 0;
            trap.ObjectKind = NpcObjectKind.Trap;
            trap.IntelligenceTypeName = typeof(AttackSingleWhenPressedTrapIntelligence).FullName;
            trap.ViewRange = 1;
            trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
            trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            trap.Attribute = 1;
            trap.NumberOfMaximumItemDrops = 0;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 1000 },
                { Stats.MinimumPhysBaseDmg, 110 },
                { Stats.MaximumPhysBaseDmg, 130 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 500 },
            };

            trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var trap = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(trap);
            trap.Number = 102;
            trap.Designation = "Fire Trap";
            trap.MoveRange = 0;
            trap.AttackRange = 2;
            trap.ObjectKind = NpcObjectKind.Trap;
            trap.IntelligenceTypeName = typeof(AttackAreaTargetInDirectionTrapIntelligence).FullName;
            trap.ViewRange = 1;
            trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
            trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
            trap.Attribute = 1;
            trap.NumberOfMaximumItemDrops = 0;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 1000 },
                { Stats.MinimumPhysBaseDmg, 130 },
                { Stats.MaximumPhysBaseDmg, 150 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 500 },
            };

            trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }
    }
}