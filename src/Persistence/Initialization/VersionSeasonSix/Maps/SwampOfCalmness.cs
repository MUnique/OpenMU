// <copyright file="SwampOfCalmness.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Swamp Of Calmness map.
/// </summary>
internal class SwampOfCalmness : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 56;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Swamp Of Calmness";

    /// <summary>
    /// Initializes a new instance of the <see cref="SwampOfCalmness"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SwampOfCalmness(IContext context, GameConfiguration gameConfiguration)
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
        // Monsters: 1 O'Clock Island
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[443], 068, 192); // Sapi-Tres
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[443], 068, 202); // Sapi-Tres
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[443], 068, 215); // Sapi-Tres
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[443], 067, 235); // Sapi-Tres
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[443], 086, 220); // Sapi-Tres
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[443], 046, 203); // Sapi-Tres
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[443], 042, 229); // Sapi-Tres
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[443], 052, 232); // Sapi-Tres
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[443], 042, 218); // Sapi-Tres
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[443], 068, 224); // Sapi-Tres
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[443], 050, 202); // Sapi-Tres
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[448], 057, 203); // Ghost Napin
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[448], 034, 231); // Ghost Napin
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[448], 048, 227); // Ghost Napin
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[448], 057, 237); // Ghost Napin
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[448], 068, 230); // Ghost Napin
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[448], 082, 223); // Ghost Napin
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[448], 041, 200); // Ghost Napin
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[448], 029, 235); // Ghost Napin
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[448], 026, 226); // Ghost Napin
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[448], 034, 192); // Ghost Napin
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[448], 083, 219); // Ghost Napin
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[448], 033, 184); // Ghost Napin
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[449], 012, 182); // Blaze Napin
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[449], 019, 182); // Blaze Napin
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[449], 010, 192); // Blaze Napin
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[449], 014, 224); // Blaze Napin
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[449], 020, 233); // Blaze Napin
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[449], 030, 186); // Blaze Napin
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[449], 025, 209); // Blaze Napin
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[449], 046, 234); // Blaze Napin
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[449], 020, 211); // Blaze Napin
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[449], 030, 230); // Blaze Napin
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[449], 085, 223); // Blaze Napin
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[449], 023, 212); // Blaze Napin
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[449], 023, 205); // Blaze Napin
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[449], 020, 207); // Blaze Napin
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[449], 028, 188); // Blaze Napin
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[449], 028, 183); // Blaze Napin
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[557], 110, 210); // Sapi Queen
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[557], 134, 232); // Sapi Queen
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[557], 133, 225); // Sapi Queen
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[557], 132, 186); // Sapi Queen
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[557], 129, 164); // Sapi Queen
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[557], 099, 167); // Sapi Queen
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[557], 111, 170); // Sapi Queen
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[557], 102, 168); // Sapi Queen
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[557], 108, 231); // Sapi Queen
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[557], 108, 229); // Sapi Queen
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[557], 106, 191); // Sapi Queen
        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[557], 108, 188); // Sapi Queen
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[558], 107, 224); // Ice Napin
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[558], 109, 219); // Ice Napin
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[558], 111, 231); // Ice Napin
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[558], 105, 230); // Ice Napin
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[558], 110, 190); // Ice Napin
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[558], 110, 185); // Ice Napin
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[558], 112, 166); // Ice Napin
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[558], 109, 167); // Ice Napin
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[558], 132, 165); // Ice Napin
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[558], 134, 189); // Ice Napin
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[558], 135, 229); // Ice Napin
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[559], 129, 229); // Shadow Master
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[559], 136, 223); // Shadow Master
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[559], 134, 210); // Shadow Master
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[559], 136, 185); // Shadow Master
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[559], 130, 183); // Shadow Master
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[559], 129, 169); // Shadow Master
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[559], 134, 169); // Shadow Master
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[559], 100, 170); // Shadow Master
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[559], 112, 213); // Shadow Master
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[559], 107, 213); // Shadow Master

        // Monsters: 5 O'Clock Island
        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[441], 198, 198); // Sapi-Unus
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[441], 207, 202); // Sapi-Unus
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[441], 198, 208); // Sapi-Unus
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[441], 198, 214); // Sapi-Unus
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[441], 173, 215); // Sapi-Unus
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[441], 173, 222); // Sapi-Unus
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[441], 220, 184); // Sapi-Unus
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[441], 226, 184); // Sapi-Unus
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[441], 203, 198); // Sapi-Unus
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[441], 233, 204); // Sapi-Unus
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[441], 229, 202); // Sapi-Unus
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[441], 236, 201); // Sapi-Unus
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[441], 226, 210); // Sapi-Unus
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[441], 237, 210); // Sapi-Unus
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[442], 210, 196); // Sapi-Duo
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[442], 220, 192); // Sapi-Duo
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[442], 220, 217); // Sapi-Duo
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[442], 224, 217); // Sapi-Duo
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[442], 216, 177); // Sapi-Duo
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[442], 230, 173); // Sapi-Duo
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[442], 218, 171); // Sapi-Duo
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[442], 227, 169); // Sapi-Duo
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[442], 231, 197); // Sapi-Duo
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[442], 189, 226); // Sapi-Duo
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[442], 201, 228); // Sapi-Duo
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[442], 207, 234); // Sapi-Duo
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[442], 228, 194); // Sapi-Duo
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[442], 235, 194); // Sapi-Duo
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[447], 195, 222); // Thunder Napin
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[447], 180, 218); // Thunder Napin
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[447], 177, 223); // Thunder Napin
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[447], 190, 233); // Thunder Napin
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[447], 199, 234); // Thunder Napin
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[447], 214, 234); // Thunder Napin
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[447], 224, 234); // Thunder Napin
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[447], 231, 235); // Thunder Napin
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[447], 238, 233); // Thunder Napin
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[447], 236, 238); // Thunder Napin
        yield return this.CreateMonsterSpawn(238, this.NpcDictionary[447], 239, 227); // Thunder Napin
        yield return this.CreateMonsterSpawn(239, this.NpcDictionary[557], 215, 150); // Sapi Queen
        yield return this.CreateMonsterSpawn(240, this.NpcDictionary[557], 218, 142); // Sapi Queen
        yield return this.CreateMonsterSpawn(241, this.NpcDictionary[557], 222, 152); // Sapi Queen
        yield return this.CreateMonsterSpawn(242, this.NpcDictionary[557], 212, 137); // Sapi Queen
        yield return this.CreateMonsterSpawn(243, this.NpcDictionary[557], 205, 131); // Sapi Queen
        yield return this.CreateMonsterSpawn(244, this.NpcDictionary[557], 197, 127); // Sapi Queen
        yield return this.CreateMonsterSpawn(245, this.NpcDictionary[557], 191, 150); // Sapi Queen
        yield return this.CreateMonsterSpawn(246, this.NpcDictionary[557], 200, 145); // Sapi Queen
        yield return this.CreateMonsterSpawn(247, this.NpcDictionary[557], 201, 153); // Sapi Queen
        yield return this.CreateMonsterSpawn(248, this.NpcDictionary[558], 218, 136); // Ice Napin
        yield return this.CreateMonsterSpawn(249, this.NpcDictionary[558], 210, 148); // Ice Napin
        yield return this.CreateMonsterSpawn(250, this.NpcDictionary[558], 226, 151); // Ice Napin
        yield return this.CreateMonsterSpawn(251, this.NpcDictionary[558], 215, 133); // Ice Napin
        yield return this.CreateMonsterSpawn(252, this.NpcDictionary[558], 177, 142); // Ice Napin
        yield return this.CreateMonsterSpawn(253, this.NpcDictionary[558], 178, 123); // Ice Napin
        yield return this.CreateMonsterSpawn(254, this.NpcDictionary[558], 180, 130); // Ice Napin
        yield return this.CreateMonsterSpawn(255, this.NpcDictionary[558], 186, 113); // Ice Napin
        yield return this.CreateMonsterSpawn(256, this.NpcDictionary[558], 195, 108); // Ice Napin
        yield return this.CreateMonsterSpawn(257, this.NpcDictionary[558], 181, 139); // Ice Napin
        yield return this.CreateMonsterSpawn(258, this.NpcDictionary[558], 180, 127); // Ice Napin
        yield return this.CreateMonsterSpawn(259, this.NpcDictionary[558], 213, 147); // Ice Napin
        yield return this.CreateMonsterSpawn(260, this.NpcDictionary[558], 223, 148); // Ice Napin
        yield return this.CreateMonsterSpawn(261, this.NpcDictionary[559], 202, 113); // Shadow Master
        yield return this.CreateMonsterSpawn(262, this.NpcDictionary[559], 190, 112); // Shadow Master
        yield return this.CreateMonsterSpawn(263, this.NpcDictionary[559], 198, 110); // Shadow Master
        yield return this.CreateMonsterSpawn(264, this.NpcDictionary[559], 182, 124); // Shadow Master
        yield return this.CreateMonsterSpawn(265, this.NpcDictionary[559], 176, 129); // Shadow Master
        yield return this.CreateMonsterSpawn(266, this.NpcDictionary[559], 181, 143); // Shadow Master
        yield return this.CreateMonsterSpawn(267, this.NpcDictionary[559], 178, 136); // Shadow Master
        yield return this.CreateMonsterSpawn(268, this.NpcDictionary[559], 189, 144); // Shadow Master

        // Monsters: 7 O'Clock Island
        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[444], 222, 010); // Shadow Pawn
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[444], 228, 010); // Shadow Pawn
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[444], 224, 014); // Shadow Pawn
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[444], 217, 024); // Shadow Pawn
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[444], 229, 027); // Shadow Pawn
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[444], 209, 029); // Shadow Pawn
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[444], 206, 039); // Shadow Pawn
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[444], 236, 056); // Shadow Pawn
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[444], 236, 068); // Shadow Pawn
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[444], 236, 080); // Shadow Pawn
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[444], 232, 088); // Shadow Pawn
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[444], 221, 088); // Shadow Pawn
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[444], 217, 082); // Shadow Pawn
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[444], 232, 011); // Shadow Pawn
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[444], 206, 030); // Shadow Pawn
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[444], 233, 072); // Shadow Pawn
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[445], 234, 009); // Shadow Knight
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[445], 232, 015); // Shadow Knight
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[445], 237, 014); // Shadow Knight
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[445], 232, 021); // Shadow Knight
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[445], 225, 028); // Shadow Knight
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[445], 213, 027); // Shadow Knight
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[445], 205, 033); // Shadow Knight
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[445], 206, 044); // Shadow Knight
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[445], 236, 063); // Shadow Knight
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[445], 236, 074); // Shadow Knight
        yield return this.CreateMonsterSpawn(326, this.NpcDictionary[445], 236, 086); // Shadow Knight
        yield return this.CreateMonsterSpawn(327, this.NpcDictionary[445], 216, 086); // Shadow Knight
        yield return this.CreateMonsterSpawn(328, this.NpcDictionary[445], 230, 074); // Shadow Knight
        yield return this.CreateMonsterSpawn(329, this.NpcDictionary[445], 220, 080); // Shadow Knight
        yield return this.CreateMonsterSpawn(330, this.NpcDictionary[445], 226, 089); // Shadow Knight
        yield return this.CreateMonsterSpawn(331, this.NpcDictionary[448], 206, 049); // Ghost Napin
        yield return this.CreateMonsterSpawn(332, this.NpcDictionary[448], 232, 068); // Ghost Napin
        yield return this.CreateMonsterSpawn(333, this.NpcDictionary[448], 231, 082); // Ghost Napin
        yield return this.CreateMonsterSpawn(334, this.NpcDictionary[448], 221, 084); // Ghost Napin
        yield return this.CreateMonsterSpawn(335, this.NpcDictionary[448], 236, 090); // Ghost Napin
        yield return this.CreateMonsterSpawn(336, this.NpcDictionary[448], 220, 027); // Ghost Napin
        yield return this.CreateMonsterSpawn(337, this.NpcDictionary[448], 210, 034); // Ghost Napin
        yield return this.CreateMonsterSpawn(338, this.NpcDictionary[448], 233, 077); // Ghost Napin
        yield return this.CreateMonsterSpawn(339, this.NpcDictionary[557], 195, 041); // Sapi Queen
        yield return this.CreateMonsterSpawn(340, this.NpcDictionary[557], 175, 041); // Sapi Queen
        yield return this.CreateMonsterSpawn(341, this.NpcDictionary[557], 162, 041); // Sapi Queen
        yield return this.CreateMonsterSpawn(342, this.NpcDictionary[557], 121, 021); // Sapi Queen
        yield return this.CreateMonsterSpawn(343, this.NpcDictionary[557], 123, 019); // Sapi Queen
        yield return this.CreateMonsterSpawn(344, this.NpcDictionary[557], 123, 022); // Sapi Queen
        yield return this.CreateMonsterSpawn(345, this.NpcDictionary[557], 163, 028); // Sapi Queen
        yield return this.CreateMonsterSpawn(346, this.NpcDictionary[557], 164, 025); // Sapi Queen
        yield return this.CreateMonsterSpawn(347, this.NpcDictionary[557], 167, 026); // Sapi Queen
        yield return this.CreateMonsterSpawn(348, this.NpcDictionary[557], 151, 043); // Sapi Queen
        yield return this.CreateMonsterSpawn(349, this.NpcDictionary[557], 148, 026); // Sapi Queen
        yield return this.CreateMonsterSpawn(350, this.NpcDictionary[557], 145, 023); // Sapi Queen
        yield return this.CreateMonsterSpawn(351, this.NpcDictionary[557], 158, 017); // Sapi Queen
        yield return this.CreateMonsterSpawn(352, this.NpcDictionary[558], 153, 039); // Ice Napin
        yield return this.CreateMonsterSpawn(353, this.NpcDictionary[558], 157, 036); // Ice Napin
        yield return this.CreateMonsterSpawn(354, this.NpcDictionary[558], 157, 038); // Ice Napin
        yield return this.CreateMonsterSpawn(355, this.NpcDictionary[558], 155, 014); // Ice Napin
        yield return this.CreateMonsterSpawn(356, this.NpcDictionary[558], 165, 028); // Ice Napin
        yield return this.CreateMonsterSpawn(357, this.NpcDictionary[558], 134, 034); // Ice Napin
        yield return this.CreateMonsterSpawn(358, this.NpcDictionary[558], 128, 029); // Ice Napin
        yield return this.CreateMonsterSpawn(359, this.NpcDictionary[559], 141, 045); // Shadow Master
        yield return this.CreateMonsterSpawn(360, this.NpcDictionary[559], 145, 046); // Shadow Master
        yield return this.CreateMonsterSpawn(361, this.NpcDictionary[559], 151, 014); // Shadow Master
        yield return this.CreateMonsterSpawn(362, this.NpcDictionary[559], 144, 044); // Shadow Master
        yield return this.CreateMonsterSpawn(363, this.NpcDictionary[559], 132, 028); // Shadow Master
        yield return this.CreateMonsterSpawn(364, this.NpcDictionary[559], 152, 017); // Shadow Master
        yield return this.CreateMonsterSpawn(365, this.NpcDictionary[559], 131, 031); // Shadow Master

        // Monsters: 11 O'Clock Island
        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[443], 036, 044); // Sapi-Tres
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[443], 041, 044); // Sapi-Tres
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[443], 036, 051); // Sapi-Tres
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[443], 040, 049); // Sapi-Tres
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[443], 024, 049); // Sapi-Tres
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[443], 010, 044); // Sapi-Tres
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[443], 010, 050); // Sapi-Tres
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[443], 013, 047); // Sapi-Tres
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[443], 050, 044); // Sapi-Tres
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[443], 052, 011); // Sapi-Tres
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[443], 051, 027); // Sapi-Tres
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[443], 058, 020); // Sapi-Tres
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[443], 068, 015); // Sapi-Tres
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[443], 069, 023); // Sapi-Tres
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[443], 039, 093, 020, 020); // Sapi-Tres
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[443], 041, 026); // Sapi-Tres
        yield return this.CreateMonsterSpawn(416, this.NpcDictionary[445], 027, 053); // Shadow Knight
        yield return this.CreateMonsterSpawn(417, this.NpcDictionary[445], 032, 053); // Shadow Knight
        yield return this.CreateMonsterSpawn(418, this.NpcDictionary[445], 029, 043); // Shadow Knight
        yield return this.CreateMonsterSpawn(419, this.NpcDictionary[445], 009, 047); // Shadow Knight
        yield return this.CreateMonsterSpawn(420, this.NpcDictionary[445], 008, 034); // Shadow Knight
        yield return this.CreateMonsterSpawn(421, this.NpcDictionary[445], 009, 022); // Shadow Knight
        yield return this.CreateMonsterSpawn(422, this.NpcDictionary[445], 012, 011); // Shadow Knight
        yield return this.CreateMonsterSpawn(423, this.NpcDictionary[445], 019, 011); // Shadow Knight
        yield return this.CreateMonsterSpawn(424, this.NpcDictionary[445], 027, 023); // Shadow Knight
        yield return this.CreateMonsterSpawn(425, this.NpcDictionary[445], 033, 023); // Shadow Knight
        yield return this.CreateMonsterSpawn(426, this.NpcDictionary[445], 054, 015); // Shadow Knight
        yield return this.CreateMonsterSpawn(427, this.NpcDictionary[445], 055, 024); // Shadow Knight
        yield return this.CreateMonsterSpawn(428, this.NpcDictionary[445], 042, 014); // Shadow Knight
        yield return this.CreateMonsterSpawn(429, this.NpcDictionary[445], 062, 016); // Shadow Knight
        yield return this.CreateMonsterSpawn(430, this.NpcDictionary[446], 016, 015); // Shadow Look
        yield return this.CreateMonsterSpawn(431, this.NpcDictionary[446], 024, 010); // Shadow Look
        yield return this.CreateMonsterSpawn(432, this.NpcDictionary[446], 027, 011); // Shadow Look
        yield return this.CreateMonsterSpawn(433, this.NpcDictionary[446], 020, 030); // Shadow Look
        yield return this.CreateMonsterSpawn(434, this.NpcDictionary[446], 013, 032); // Shadow Look
        yield return this.CreateMonsterSpawn(435, this.NpcDictionary[446], 018, 045); // Shadow Look
        yield return this.CreateMonsterSpawn(436, this.NpcDictionary[446], 045, 027); // Shadow Look
        yield return this.CreateMonsterSpawn(437, this.NpcDictionary[446], 052, 020); // Shadow Look
        yield return this.CreateMonsterSpawn(438, this.NpcDictionary[446], 023, 023); // Shadow Look
        yield return this.CreateMonsterSpawn(439, this.NpcDictionary[557], 012, 087); // Sapi Queen
        yield return this.CreateMonsterSpawn(440, this.NpcDictionary[557], 014, 085); // Sapi Queen
        yield return this.CreateMonsterSpawn(441, this.NpcDictionary[557], 021, 128); // Sapi Queen
        yield return this.CreateMonsterSpawn(442, this.NpcDictionary[557], 023, 126); // Sapi Queen
        yield return this.CreateMonsterSpawn(443, this.NpcDictionary[557], 023, 102); // Sapi Queen
        yield return this.CreateMonsterSpawn(444, this.NpcDictionary[557], 026, 097); // Sapi Queen
        yield return this.CreateMonsterSpawn(445, this.NpcDictionary[557], 037, 120); // Sapi Queen
        yield return this.CreateMonsterSpawn(446, this.NpcDictionary[557], 040, 117); // Sapi Queen
        yield return this.CreateMonsterSpawn(447, this.NpcDictionary[557], 053, 109); // Sapi Queen
        yield return this.CreateMonsterSpawn(448, this.NpcDictionary[557], 052, 075); // Sapi Queen
        yield return this.CreateMonsterSpawn(449, this.NpcDictionary[558], 015, 089); // Ice Napin
        yield return this.CreateMonsterSpawn(450, this.NpcDictionary[558], 026, 100); // Ice Napin
        yield return this.CreateMonsterSpawn(451, this.NpcDictionary[558], 012, 112); // Ice Napin
        yield return this.CreateMonsterSpawn(452, this.NpcDictionary[558], 021, 124); // Ice Napin
        yield return this.CreateMonsterSpawn(453, this.NpcDictionary[558], 024, 121); // Ice Napin
        yield return this.CreateMonsterSpawn(454, this.NpcDictionary[558], 023, 112); // Ice Napin
        yield return this.CreateMonsterSpawn(455, this.NpcDictionary[558], 041, 123); // Ice Napin
        yield return this.CreateMonsterSpawn(456, this.NpcDictionary[558], 043, 118); // Ice Napin
        yield return this.CreateMonsterSpawn(457, this.NpcDictionary[558], 035, 114); // Ice Napin
        yield return this.CreateMonsterSpawn(458, this.NpcDictionary[558], 057, 108); // Ice Napin
        yield return this.CreateMonsterSpawn(459, this.NpcDictionary[558], 053, 104); // Ice Napin
        yield return this.CreateMonsterSpawn(460, this.NpcDictionary[558], 060, 078); // Ice Napin
        yield return this.CreateMonsterSpawn(461, this.NpcDictionary[558], 015, 127); // Ice Napin
        yield return this.CreateMonsterSpawn(462, this.NpcDictionary[558], 011, 118); // Ice Napin
        yield return this.CreateMonsterSpawn(463, this.NpcDictionary[558], 018, 112); // Ice Napin
        yield return this.CreateMonsterSpawn(464, this.NpcDictionary[559], 054, 113); // Shadow Master
        yield return this.CreateMonsterSpawn(465, this.NpcDictionary[559], 045, 097); // Shadow Master
        yield return this.CreateMonsterSpawn(466, this.NpcDictionary[559], 042, 082); // Shadow Master
        yield return this.CreateMonsterSpawn(467, this.NpcDictionary[559], 052, 079); // Shadow Master
        yield return this.CreateMonsterSpawn(468, this.NpcDictionary[559], 056, 077); // Shadow Master
        yield return this.CreateMonsterSpawn(469, this.NpcDictionary[559], 044, 123); // Shadow Master
        yield return this.CreateMonsterSpawn(470, this.NpcDictionary[559], 041, 111); // Shadow Master
        yield return this.CreateMonsterSpawn(471, this.NpcDictionary[559], 057, 105); // Shadow Master
        yield return this.CreateMonsterSpawn(472, this.NpcDictionary[559], 026, 116); // Shadow Master
        yield return this.CreateMonsterSpawn(473, this.NpcDictionary[559], 014, 104); // Shadow Master
        yield return this.CreateMonsterSpawn(474, this.NpcDictionary[559], 013, 093); // Shadow Master
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 441;
            monster.Designation = "Sapi-Unus";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 95 },
                { Stats.MaximumHealth, 37000 },
                { Stats.MinimumPhysBaseDmg, 475 },
                { Stats.MaximumPhysBaseDmg, 510 },
                { Stats.DefenseBase, 370 },
                { Stats.AttackRatePvm, 700 },
                { Stats.DefenseRatePvm, 220 },
                { Stats.PoisonResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 441 Sapi-Unus

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 442;
            monster.Designation = "Sapi-Duo";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 96 },
                { Stats.MaximumHealth, 39500 },
                { Stats.MinimumPhysBaseDmg, 485 },
                { Stats.MaximumPhysBaseDmg, 505 },
                { Stats.DefenseBase, 375 },
                { Stats.AttackRatePvm, 730 },
                { Stats.DefenseRatePvm, 225 },
                { Stats.PoisonResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 442 Sapi-Duo

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 443;
            monster.Designation = "Sapi-Tres";
            monster.MoveRange = 4;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 102 },
                { Stats.MaximumHealth, 58000 },
                { Stats.MinimumPhysBaseDmg, 590 },
                { Stats.MaximumPhysBaseDmg, 625 },
                { Stats.DefenseBase, 470 },
                { Stats.AttackRatePvm, 880 },
                { Stats.DefenseRatePvm, 275 },
                { Stats.PoisonResistance, 40f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 443 Sapi-Tres

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 444;
            monster.Designation = "Shadow Pawn";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 98 },
                { Stats.MaximumHealth, 48500 },
                { Stats.MinimumPhysBaseDmg, 540 },
                { Stats.MaximumPhysBaseDmg, 575 },
                { Stats.DefenseBase, 430 },
                { Stats.AttackRatePvm, 830 },
                { Stats.DefenseRatePvm, 240 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 444 Shadow Pawn

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 445;
            monster.Designation = "Shadow Knight";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Ice);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 100 },
                { Stats.MaximumHealth, 52000 },
                { Stats.MinimumPhysBaseDmg, 570 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 455 },
                { Stats.AttackRatePvm, 860 },
                { Stats.DefenseRatePvm, 255 },
                { Stats.PoisonResistance, 30f / 255 },
                { Stats.IceResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 445 Shadow Knight

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 446;
            monster.Designation = "Shadow Look";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 104 },
                { Stats.MaximumHealth, 62000 },
                { Stats.MinimumPhysBaseDmg, 600 },
                { Stats.MaximumPhysBaseDmg, 645 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 950 },
                { Stats.DefenseRatePvm, 290 },
                { Stats.PoisonResistance, 40f / 255 },
                { Stats.IceResistance, 40f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 446 Shadow Look

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 447;
            monster.Designation = "Thunder Napin";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 97 },
                { Stats.MaximumHealth, 42000 },
                { Stats.MinimumPhysBaseDmg, 520 },
                { Stats.MaximumPhysBaseDmg, 555 },
                { Stats.DefenseBase, 395 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 235 },
                { Stats.LightningResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 447 Thunder Napin

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 448;
            monster.Designation = "Ghost Napin";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 106 },
                { Stats.MaximumHealth, 68000 },
                { Stats.MinimumPhysBaseDmg, 635 },
                { Stats.MaximumPhysBaseDmg, 665 },
                { Stats.DefenseBase, 535 },
                { Stats.AttackRatePvm, 983 },
                { Stats.DefenseRatePvm, 295 },
                { Stats.PoisonResistance, 30f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 448 Ghost Napin

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 449;
            monster.Designation = "Blaze Napin";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 107 },
                { Stats.MaximumHealth, 70000 },
                { Stats.MinimumPhysBaseDmg, 670 },
                { Stats.MaximumPhysBaseDmg, 725 },
                { Stats.DefenseBase, 530 },
                { Stats.AttackRatePvm, 1000 },
                { Stats.DefenseRatePvm, 303 },
                { Stats.FireResistance, 40f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 449 Blaze Napin

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 557;
            monster.Designation = "Sapi Queen";
            monster.MoveRange = 4;
            monster.AttackRange = 3;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 131 },
                { Stats.MaximumHealth, 218000 },
                { Stats.MinimumPhysBaseDmg, 1441 },
                { Stats.MaximumPhysBaseDmg, 2017 },
                { Stats.DefenseBase, 670 },
                { Stats.AttackRatePvm, 1847 },
                { Stats.DefenseRatePvm, 875 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 20f / 255 },
                { Stats.FireResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 557 Sapi Queen

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 558;
            monster.Designation = "Ice Napin";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 135 },
                { Stats.MaximumHealth, 230000 },
                { Stats.MinimumPhysBaseDmg, 1585 },
                { Stats.MaximumPhysBaseDmg, 2060 },
                { Stats.DefenseBase, 730 },
                { Stats.AttackRatePvm, 1544 },
                { Stats.DefenseRatePvm, 903 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 20f / 255 },
                { Stats.FireResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 558 Ice Napin

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 559;
            monster.Designation = "Shadow Master";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 137 },
                { Stats.MaximumHealth, 242000 },
                { Stats.MinimumPhysBaseDmg, 1743 },
                { Stats.MaximumPhysBaseDmg, 2092 },
                { Stats.DefenseBase, 700 },
                { Stats.AttackRatePvm, 1646 },
                { Stats.DefenseRatePvm, 990 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 20f / 255 },
                { Stats.FireResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 559 Shadow Master
    }
}