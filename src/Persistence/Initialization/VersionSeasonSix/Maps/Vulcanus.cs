// <copyright file="Vulcanus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Exile map.
/// </summary>
internal class Vulcanus : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 63;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Vulcanus";

    /// <summary>
    /// Initializes a new instance of the <see cref="Vulcanus"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Vulcanus(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[479], 122, 135, Direction.SouthWest); // Gatekeeper Titus
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[481], 064, 124, 054, 089, 3); // Zombie Fighter
        yield return this.CreateMonsterSpawn(22, this.NpcDictionary[481], 065, 105, 012, 047, 3); // Zombie Fighter
        yield return this.CreateMonsterSpawn(23, this.NpcDictionary[481], 022, 064, 010, 098, 4); // Zombie Fighter
        yield return this.CreateMonsterSpawn(24, this.NpcDictionary[483], 021, 077, 128, 186, 4); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(25, this.NpcDictionary[483], 028, 105, 189, 232, 4); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(26, this.NpcDictionary[485], 123, 174, 186, 219, 2); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(27, this.NpcDictionary[485], 172, 230, 215, 234, 2); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(28, this.NpcDictionary[485], 175, 238, 171, 194, 2); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(29, this.NpcDictionary[485], 203, 243, 195, 214); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(30, this.NpcDictionary[488], 198, 226, 106, 148, 3); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(31, this.NpcDictionary[488], 228, 248, 107, 133); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(32, this.NpcDictionary[488], 168, 198, 101, 131); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(33, this.NpcDictionary[491], 143, 178, 008, 063, 2); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(34, this.NpcDictionary[491], 182, 210, 013, 070, 2); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(35, this.NpcDictionary[491], 211, 244, 039, 072); // Ruthless Lava Giant

        yield return this.CreateMonsterSpawn(41, this.NpcDictionary[480], 107, 060); // Zombie Fighter
        yield return this.CreateMonsterSpawn(42, this.NpcDictionary[480], 115, 060); // Zombie Fighter
        yield return this.CreateMonsterSpawn(43, this.NpcDictionary[480], 115, 067); // Zombie Fighter
        yield return this.CreateMonsterSpawn(44, this.NpcDictionary[480], 073, 078); // Zombie Fighter
        yield return this.CreateMonsterSpawn(45, this.NpcDictionary[480], 078, 085); // Zombie Fighter
        yield return this.CreateMonsterSpawn(46, this.NpcDictionary[480], 085, 081); // Zombie Fighter
        yield return this.CreateMonsterSpawn(47, this.NpcDictionary[480], 112, 084); // Zombie Fighter
        yield return this.CreateMonsterSpawn(48, this.NpcDictionary[480], 118, 082); // Zombie Fighter
        yield return this.CreateMonsterSpawn(49, this.NpcDictionary[480], 117, 088); // Zombie Fighter
        yield return this.CreateMonsterSpawn(50, this.NpcDictionary[480], 088, 059); // Zombie Fighter
        yield return this.CreateMonsterSpawn(51, this.NpcDictionary[480], 091, 066); // Zombie Fighter
        yield return this.CreateMonsterSpawn(52, this.NpcDictionary[480], 095, 026); // Zombie Fighter
        yield return this.CreateMonsterSpawn(53, this.NpcDictionary[480], 100, 030); // Zombie Fighter
        yield return this.CreateMonsterSpawn(54, this.NpcDictionary[480], 079, 044); // Zombie Fighter
        yield return this.CreateMonsterSpawn(55, this.NpcDictionary[480], 085, 043); // Zombie Fighter
        yield return this.CreateMonsterSpawn(56, this.NpcDictionary[480], 073, 020); // Zombie Fighter
        yield return this.CreateMonsterSpawn(57, this.NpcDictionary[480], 079, 023); // Zombie Fighter
        yield return this.CreateMonsterSpawn(58, this.NpcDictionary[480], 041, 018); // Zombie Fighter
        yield return this.CreateMonsterSpawn(59, this.NpcDictionary[480], 050, 014); // Zombie Fighter
        yield return this.CreateMonsterSpawn(60, this.NpcDictionary[480], 033, 044); // Zombie Fighter
        yield return this.CreateMonsterSpawn(61, this.NpcDictionary[480], 035, 038); // Zombie Fighter
        yield return this.CreateMonsterSpawn(62, this.NpcDictionary[480], 032, 062); // Zombie Fighter
        yield return this.CreateMonsterSpawn(63, this.NpcDictionary[480], 037, 061); // Zombie Fighter
        yield return this.CreateMonsterSpawn(64, this.NpcDictionary[480], 033, 081); // Zombie Fighter
        yield return this.CreateMonsterSpawn(65, this.NpcDictionary[480], 039, 088); // Zombie Fighter
        yield return this.CreateMonsterSpawn(66, this.NpcDictionary[480], 033, 091); // Zombie Fighter
        yield return this.CreateMonsterSpawn(67, this.NpcDictionary[480], 055, 064); // Zombie Fighter
        yield return this.CreateMonsterSpawn(68, this.NpcDictionary[480], 065, 068); // Zombie Fighter
        yield return this.CreateMonsterSpawn(69, this.NpcDictionary[480], 100, 080); // Zombie Fighter
        yield return this.CreateMonsterSpawn(70, this.NpcDictionary[480], 099, 072); // Zombie Fighter
        yield return this.CreateMonsterSpawn(71, this.NpcDictionary[480], 046, 042); // Zombie Fighter
        yield return this.CreateMonsterSpawn(72, this.NpcDictionary[480], 055, 039); // Zombie Fighter
        yield return this.CreateMonsterSpawn(73, this.NpcDictionary[480], 068, 058); // Zombie Fighter
        yield return this.CreateMonsterSpawn(74, this.NpcDictionary[480], 097, 020); // Zombie Fighter
        yield return this.CreateMonsterSpawn(75, this.NpcDictionary[480], 065, 049); // Zombie Fighter
        yield return this.CreateMonsterSpawn(76, this.NpcDictionary[480], 027, 098); // Zombie Fighter
        yield return this.CreateMonsterSpawn(77, this.NpcDictionary[480], 038, 055); // Zombie Fighter
        yield return this.CreateMonsterSpawn(78, this.NpcDictionary[480], 097, 035); // Zombie Fighter
        yield return this.CreateMonsterSpawn(79, this.NpcDictionary[480], 104, 077); // Zombie Fighter
        yield return this.CreateMonsterSpawn(80, this.NpcDictionary[480], 089, 087); // Zombie Fighter
        yield return this.CreateMonsterSpawn(81, this.NpcDictionary[480], 071, 090); // Zombie Fighter
        yield return this.CreateMonsterSpawn(82, this.NpcDictionary[480], 052, 057); // Zombie Fighter
        yield return this.CreateMonsterSpawn(83, this.NpcDictionary[480], 060, 076); // Zombie Fighter
        yield return this.CreateMonsterSpawn(84, this.NpcDictionary[480], 073, 043); // Zombie Fighter
        yield return this.CreateMonsterSpawn(85, this.NpcDictionary[480], 061, 038); // Zombie Fighter
        yield return this.CreateMonsterSpawn(86, this.NpcDictionary[480], 047, 085); // Zombie Fighter
        yield return this.CreateMonsterSpawn(87, this.NpcDictionary[480], 082, 064); // Zombie Fighter
        yield return this.CreateMonsterSpawn(88, this.NpcDictionary[480], 121, 072); // Zombie Fighter

        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[482], 079, 078); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[482], 110, 065); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[482], 099, 024); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[482], 081, 040); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[482], 072, 024); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[482], 038, 042); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[482], 028, 059); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[482], 031, 086); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[482], 057, 068); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[482], 062, 063); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[482], 076, 063); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[482], 092, 060); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[482], 082, 018); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[482], 047, 054); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[482], 040, 073); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[482], 044, 024); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[482], 048, 019); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[482], 053, 022); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[482], 027, 139); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[482], 055, 145); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[482], 063, 139); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[482], 064, 159); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[482], 090, 180); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[482], 029, 175); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[482], 036, 198); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[482], 067, 185); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[482], 050, 159); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[482], 035, 132); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[482], 035, 149); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[482], 051, 152); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[482], 059, 132); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[482], 080, 152); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[482], 053, 175); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[482], 045, 164); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[482], 033, 212); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[482], 039, 230); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[482], 057, 203); // Resurrected Gladiator
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[482], 091, 012); // Resurrected Gladiator

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[484], 029, 132); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[484], 035, 141); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[484], 033, 178); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[484], 035, 172); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[484], 034, 155); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[484], 056, 138); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[484], 060, 143); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[484], 063, 165); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[484], 069, 161); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[484], 068, 169); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[484], 072, 144); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[484], 097, 182); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[484], 093, 177); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[484], 059, 179); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[484], 084, 165); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[484], 035, 221); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[484], 044, 227); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[484], 046, 218); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[484], 041, 202); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[484], 059, 210); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[484], 069, 215); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[484], 082, 217); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[484], 080, 200); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[484], 084, 207); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[484], 086, 199); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[484], 094, 216); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[484], 101, 220); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[484], 072, 223); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[484], 037, 204); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[484], 032, 205); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(230, this.NpcDictionary[484], 064, 202); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(231, this.NpcDictionary[484], 073, 177); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(232, this.NpcDictionary[484], 140, 215); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(233, this.NpcDictionary[484], 147, 195); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(234, this.NpcDictionary[484], 165, 195); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(235, this.NpcDictionary[484], 179, 183); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(236, this.NpcDictionary[484], 190, 184); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(237, this.NpcDictionary[484], 182, 227); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(238, this.NpcDictionary[484], 208, 225); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(239, this.NpcDictionary[484], 229, 196); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(240, this.NpcDictionary[484], 232, 210); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(241, this.NpcDictionary[484], 218, 227); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(242, this.NpcDictionary[484], 222, 179); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(243, this.NpcDictionary[484], 106, 228); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(244, this.NpcDictionary[484], 143, 224); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(245, this.NpcDictionary[484], 199, 226); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(246, this.NpcDictionary[484], 184, 180); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(247, this.NpcDictionary[484], 144, 206); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(248, this.NpcDictionary[484], 166, 203); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(249, this.NpcDictionary[484], 100, 213); // Ash Slaughterer
        yield return this.CreateMonsterSpawn(250, this.NpcDictionary[484], 075, 218); // Ash Slaughterer

        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[486], 137, 202); // Blood Assassin
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[486], 154, 204); // Blood Assassin
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[486], 148, 214); // Blood Assassin
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[486], 133, 213); // Blood Assassin
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[486], 156, 188); // Blood Assassin
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[486], 176, 193); // Blood Assassin
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[486], 185, 173); // Blood Assassin
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[486], 193, 178); // Blood Assassin
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[486], 201, 190); // Blood Assassin
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[486], 228, 182); // Blood Assassin
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[486], 232, 191); // Blood Assassin
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[486], 236, 203); // Blood Assassin
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[486], 226, 203); // Blood Assassin
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[486], 174, 228); // Blood Assassin
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[486], 178, 223); // Blood Assassin
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[486], 189, 224); // Blood Assassin
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[486], 207, 219); // Blood Assassin
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[486], 211, 231); // Blood Assassin
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[486], 218, 221); // Blood Assassin
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[486], 208, 208); // Blood Assassin
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[486], 173, 111); // Blood Assassin
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[486], 179, 108); // Blood Assassin
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[486], 193, 125); // Blood Assassin
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[486], 198, 113); // Blood Assassin
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[486], 199, 107); // Blood Assassin
        yield return this.CreateMonsterSpawn(326, this.NpcDictionary[486], 209, 132); // Blood Assassin
        yield return this.CreateMonsterSpawn(327, this.NpcDictionary[486], 215, 113); // Blood Assassin
        yield return this.CreateMonsterSpawn(328, this.NpcDictionary[486], 234, 118); // Blood Assassin
        yield return this.CreateMonsterSpawn(329, this.NpcDictionary[486], 241, 126); // Blood Assassin
        yield return this.CreateMonsterSpawn(330, this.NpcDictionary[486], 204, 143); // Blood Assassin
        yield return this.CreateMonsterSpawn(331, this.NpcDictionary[486], 138, 195); // Blood Assassin
        yield return this.CreateMonsterSpawn(332, this.NpcDictionary[486], 218, 200); // Blood Assassin
        yield return this.CreateMonsterSpawn(333, this.NpcDictionary[486], 230, 125); // Blood Assassin

        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[487], 178, 115); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[487], 195, 106); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[487], 204, 124); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[487], 216, 127); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[487], 218, 116); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[487], 240, 119); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[487], 236, 122); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[487], 202, 134); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[487], 185, 123); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[487], 219, 136); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[487], 211, 149); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[487], 198, 129); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[487], 205, 118); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[487], 225, 189); // Cruel Blood Assassin
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[487], 213, 183); // Cruel Blood Assassin

        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[489], 178, 073); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[489], 152, 056); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[489], 160, 050); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[489], 157, 031); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[489], 150, 018); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[489], 188, 015); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[489], 196, 012); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[489], 194, 025); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[489], 207, 028); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(510, this.NpcDictionary[489], 214, 026); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(511, this.NpcDictionary[489], 212, 051); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(512, this.NpcDictionary[489], 217, 044); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(513, this.NpcDictionary[489], 234, 050); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(514, this.NpcDictionary[489], 237, 066); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(515, this.NpcDictionary[489], 233, 061); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(516, this.NpcDictionary[489], 211, 066); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(517, this.NpcDictionary[489], 222, 066); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(518, this.NpcDictionary[489], 183, 063); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(519, this.NpcDictionary[489], 186, 039); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(520, this.NpcDictionary[489], 175, 046); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(521, this.NpcDictionary[489], 170, 027); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(522, this.NpcDictionary[489], 143, 078); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(523, this.NpcDictionary[489], 152, 089); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(524, this.NpcDictionary[489], 184, 070); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(525, this.NpcDictionary[489], 195, 061); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(526, this.NpcDictionary[489], 179, 056); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(527, this.NpcDictionary[489], 195, 047); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(528, this.NpcDictionary[489], 166, 042); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(529, this.NpcDictionary[489], 165, 011); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(530, this.NpcDictionary[489], 154, 064); // Burning Lava Giant
        yield return this.CreateMonsterSpawn(531, this.NpcDictionary[489], 158, 086); // Burning Lava Giant

        yield return this.CreateMonsterSpawn(601, this.NpcDictionary[490], 193, 019); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(602, this.NpcDictionary[490], 209, 023); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(603, this.NpcDictionary[490], 216, 050); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(604, this.NpcDictionary[490], 237, 052); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(605, this.NpcDictionary[490], 235, 058); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(606, this.NpcDictionary[490], 205, 066); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(607, this.NpcDictionary[490], 174, 175, 068, 068); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(608, this.NpcDictionary[490], 161, 057); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(609, this.NpcDictionary[490], 154, 052); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(610, this.NpcDictionary[490], 151, 012); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(611, this.NpcDictionary[490], 162, 032); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(612, this.NpcDictionary[490], 191, 042); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(613, this.NpcDictionary[490], 215, 061); // Ruthless Lava Giant
        yield return this.CreateMonsterSpawn(614, this.NpcDictionary[490], 164, 078); // Ruthless Lava Giant
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 480;
            monster.Designation = "Zombie Fighter";
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
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 20000 },
                { Stats.MinimumPhysBaseDmg, 300 },
                { Stats.MaximumPhysBaseDmg, 345 },
                { Stats.DefenseBase, 200 },
                { Stats.AttackRatePvm, 750 },
                { Stats.DefenseRatePvm, 170 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 481;
            monster.Designation = "Zombie Fighter";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 120 },
                { Stats.MaximumHealth, 5000000 },
                { Stats.MinimumPhysBaseDmg, 850 },
                { Stats.MaximumPhysBaseDmg, 950 },
                { Stats.DefenseBase, 210 },
                { Stats.AttackRatePvm, 1190 },
                { Stats.DefenseRatePvm, 820 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 200f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 482;
            monster.Designation = "Resurrected Gladiator";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 93 },
                { Stats.MaximumHealth, 23000 },
                { Stats.MinimumPhysBaseDmg, 320 },
                { Stats.MaximumPhysBaseDmg, 360 },
                { Stats.DefenseBase, 220 },
                { Stats.AttackRatePvm, 780 },
                { Stats.DefenseRatePvm, 200 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 483;
            monster.Designation = "Resurrected Gladiator";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 121 },
                { Stats.MaximumHealth, 513000 },
                { Stats.MinimumPhysBaseDmg, 860 },
                { Stats.MaximumPhysBaseDmg, 960 },
                { Stats.DefenseBase, 230 },
                { Stats.AttackRatePvm, 1210 },
                { Stats.DefenseRatePvm, 825 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 484;
            monster.Designation = "Ash Slaughterer";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 100 },
                { Stats.MaximumHealth, 27000 },
                { Stats.MinimumPhysBaseDmg, 335 },
                { Stats.MaximumPhysBaseDmg, 370 },
                { Stats.DefenseBase, 270 },
                { Stats.AttackRatePvm, 810 },
                { Stats.DefenseRatePvm, 230 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 485;
            monster.Designation = "Ash Slaughterer";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 122 },
                { Stats.MaximumHealth, 520000 },
                { Stats.MinimumPhysBaseDmg, 870 },
                { Stats.MaximumPhysBaseDmg, 970 },
                { Stats.DefenseBase, 280 },
                { Stats.AttackRatePvm, 1230 },
                { Stats.DefenseRatePvm, 830 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 486;
            monster.Designation = "Blood Assassin";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 107 },
                { Stats.MaximumHealth, 37000 },
                { Stats.MinimumPhysBaseDmg, 550 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 475 },
                { Stats.AttackRatePvm, 900 },
                { Stats.DefenseRatePvm, 300 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 487;
            monster.Designation = "Cruel Blood Assassin";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 109 },
                { Stats.MaximumHealth, 40000 },
                { Stats.MinimumPhysBaseDmg, 570 },
                { Stats.MaximumPhysBaseDmg, 620 },
                { Stats.DefenseBase, 490 },
                { Stats.AttackRatePvm, 950 },
                { Stats.DefenseRatePvm, 325 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 488;
            monster.Designation = "Cruel Blood Assassin";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 123 },
                { Stats.MaximumHealth, 523000 },
                { Stats.MinimumPhysBaseDmg, 880 },
                { Stats.MaximumPhysBaseDmg, 980 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 1250 },
                { Stats.DefenseRatePvm, 835 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 489;
            monster.Designation = "Burning Lava Giant";
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
                { Stats.Level, 111 },
                { Stats.MaximumHealth, 50000 },
                { Stats.MinimumPhysBaseDmg, 600 },
                { Stats.MaximumPhysBaseDmg, 650 },
                { Stats.DefenseBase, 520 },
                { Stats.AttackRatePvm, 970 },
                { Stats.DefenseRatePvm, 330 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 490;
            monster.Designation = "Ruthless Lava Giant";
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
                { Stats.Level, 113 },
                { Stats.MaximumHealth, 55000 },
                { Stats.MinimumPhysBaseDmg, 630 },
                { Stats.MaximumPhysBaseDmg, 680 },
                { Stats.DefenseBase, 535 },
                { Stats.AttackRatePvm, 1000 },
                { Stats.DefenseRatePvm, 340 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 491;
            monster.Designation = "Ruthless Lava Giant";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 124 },
                { Stats.MaximumHealth, 530000 },
                { Stats.MinimumPhysBaseDmg, 890 },
                { Stats.MaximumPhysBaseDmg, 990 },
                { Stats.DefenseBase, 545 },
                { Stats.AttackRatePvm, 1270 },
                { Stats.DefenseRatePvm, 840 },
                { Stats.PoisonResistance, 200f / 255 },
                { Stats.IceResistance, 200f / 255 },
                { Stats.FireResistance, 250f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}