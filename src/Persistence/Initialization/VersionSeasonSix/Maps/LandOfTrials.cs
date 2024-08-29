// <copyright file="LandOfTrials.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Land of Trials map.
/// </summary>
internal class LandOfTrials : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 31;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Land_of_Trials";

    /// <summary>
    /// Initializes a new instance of the <see cref="LandOfTrials"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public LandOfTrials(IContext context, GameConfiguration gameConfiguration)
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
        // Monsters:
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[290], 069, 103); // Lizard Warrior
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[290], 077, 102); // Lizard Warrior
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[290], 067, 085); // Lizard Warrior
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[290], 069, 075); // Lizard Warrior
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[290], 070, 065); // Lizard Warrior
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[290], 233, 036); // Lizard Warrior
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[290], 228, 109); // Lizard Warrior
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[290], 212, 022); // Lizard Warrior
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[290], 204, 061); // Lizard Warrior
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[290], 209, 067); // Lizard Warrior
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[290], 218, 065); // Lizard Warrior
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[290], 234, 075); // Lizard Warrior
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[290], 234, 066); // Lizard Warrior
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[290], 234, 055); // Lizard Warrior
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[290], 231, 095); // Lizard Warrior
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[290], 222, 087); // Lizard Warrior
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[290], 226, 063); // Lizard Warrior
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[290], 214, 053); // Lizard Warrior
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[290], 223, 039); // Lizard Warrior
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[290], 191, 021); // Lizard Warrior
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[290], 050, 060); // Lizard Warrior
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[290], 054, 065); // Lizard Warrior
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[290], 059, 059); // Lizard Warrior
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[290], 082, 074); // Lizard Warrior
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[290], 092, 083); // Lizard Warrior
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[290], 094, 092); // Lizard Warrior
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[290], 139, 050); // Lizard Warrior
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[290], 157, 034); // Lizard Warrior
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[290], 170, 055); // Lizard Warrior
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[290], 172, 062); // Lizard Warrior
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[290], 142, 066); // Lizard Warrior
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[290], 153, 059); // Lizard Warrior
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[290], 155, 070); // Lizard Warrior
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[290], 162, 061); // Lizard Warrior
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[290], 165, 070); // Lizard Warrior
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[290], 170, 074); // Lizard Warrior
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[290], 167, 082); // Lizard Warrior
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[290], 156, 089); // Lizard Warrior
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[290], 144, 077); // Lizard Warrior
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[290], 087, 077); // Lizard Warrior
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[290], 122, 081); // Lizard Warrior
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[290], 103, 091); // Lizard Warrior

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[291], 105, 190); // Fire Golem
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[291], 088, 183); // Fire Golem
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[291], 225, 141); // Fire Golem
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[291], 064, 213); // Fire Golem
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[291], 048, 207); // Fire Golem
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[291], 018, 173); // Fire Golem
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[291], 038, 172); // Fire Golem
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[291], 028, 185); // Fire Golem
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[291], 031, 209); // Fire Golem
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[291], 040, 231); // Fire Golem
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[291], 056, 227); // Fire Golem
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[291], 081, 221); // Fire Golem
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[291], 080, 234); // Fire Golem
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[291], 138, 173); // Fire Golem
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[291], 134, 229); // Fire Golem
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[291], 155, 227); // Fire Golem
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[291], 145, 209); // Fire Golem
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[291], 144, 198); // Fire Golem
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[291], 131, 215); // Fire Golem
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[291], 120, 225); // Fire Golem
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[291], 052, 188); // Fire Golem
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[291], 222, 221); // Fire Golem
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[291], 231, 211); // Fire Golem
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[291], 208, 217); // Fire Golem
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[291], 233, 199); // Fire Golem
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[291], 197, 204); // Fire Golem
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[291], 198, 192); // Fire Golem
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[291], 187, 175); // Fire Golem
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[291], 216, 183); // Fire Golem
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[291], 193, 167); // Fire Golem

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[292], 098, 175); // Queen Bee
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[292], 224, 147); // Queen Bee
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[292], 037, 222); // Queen Bee
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[292], 048, 221); // Queen Bee
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[292], 072, 232); // Queen Bee
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[292], 075, 204); // Queen Bee
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[292], 027, 174); // Queen Bee
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[292], 041, 185); // Queen Bee
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[292], 071, 194); // Queen Bee
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[292], 092, 225); // Queen Bee
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[292], 097, 200); // Queen Bee
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[292], 187, 159); // Queen Bee
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[292], 229, 227); // Queen Bee
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[292], 182, 177); // Queen Bee
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[292], 191, 182); // Queen Bee
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[292], 196, 173); // Queen Bee
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[292], 198, 151); // Queen Bee
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[292], 183, 140); // Queen Bee
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[292], 175, 127); // Queen Bee
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[292], 191, 120); // Queen Bee
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[292], 152, 112); // Queen Bee
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[292], 146, 126); // Queen Bee
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[292], 159, 121); // Queen Bee
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[292], 120, 132); // Queen Bee
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[292], 133, 168); // Queen Bee
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[292], 141, 182); // Queen Bee
        yield return this.CreateMonsterSpawn(326, this.NpcDictionary[292], 144, 169); // Queen Bee
        yield return this.CreateMonsterSpawn(327, this.NpcDictionary[292], 128, 224); // Queen Bee
        yield return this.CreateMonsterSpawn(328, this.NpcDictionary[292], 141, 221); // Queen Bee
        yield return this.CreateMonsterSpawn(329, this.NpcDictionary[292], 163, 225); // Queen Bee
        yield return this.CreateMonsterSpawn(330, this.NpcDictionary[292], 155, 218); // Queen Bee
        yield return this.CreateMonsterSpawn(331, this.NpcDictionary[292], 123, 146); // Queen Bee
        yield return this.CreateMonsterSpawn(332, this.NpcDictionary[292], 133, 145); // Queen Bee
        yield return this.CreateMonsterSpawn(333, this.NpcDictionary[292], 128, 136); // Queen Bee
        yield return this.CreateMonsterSpawn(334, this.NpcDictionary[292], 138, 133); // Queen Bee
        yield return this.CreateMonsterSpawn(335, this.NpcDictionary[292], 095, 138); // Queen Bee
        yield return this.CreateMonsterSpawn(336, this.NpcDictionary[292], 223, 204); // Queen Bee
        yield return this.CreateMonsterSpawn(337, this.NpcDictionary[292], 208, 208); // Queen Bee
        yield return this.CreateMonsterSpawn(338, this.NpcDictionary[292], 227, 180); // Queen Bee
        yield return this.CreateMonsterSpawn(339, this.NpcDictionary[292], 224, 191); // Queen Bee

        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[293], 225, 132); // Poison Golem
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[293], 224, 124); // Poison Golem
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[293], 218, 110); // Poison Golem
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[293], 219, 119); // Poison Golem
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[293], 227, 116); // Poison Golem
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[293], 230, 102); // Poison Golem
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[293], 222, 098); // Poison Golem
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[293], 230, 086); // Poison Golem
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[293], 220, 074); // Poison Golem
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[293], 227, 077); // Poison Golem
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[293], 063, 134); // Poison Golem
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[293], 076, 140); // Poison Golem
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[293], 070, 149); // Poison Golem
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[293], 081, 150); // Poison Golem
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[293], 058, 124); // Poison Golem
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[293], 066, 120); // Poison Golem
        yield return this.CreateMonsterSpawn(416, this.NpcDictionary[293], 071, 113); // Poison Golem
        yield return this.CreateMonsterSpawn(417, this.NpcDictionary[293], 080, 111); // Poison Golem
        yield return this.CreateMonsterSpawn(418, this.NpcDictionary[293], 114, 088); // Poison Golem
        yield return this.CreateMonsterSpawn(419, this.NpcDictionary[293], 124, 088); // Poison Golem
        yield return this.CreateMonsterSpawn(420, this.NpcDictionary[293], 131, 079); // Poison Golem
        yield return this.CreateMonsterSpawn(421, this.NpcDictionary[293], 137, 083); // Poison Golem
        yield return this.CreateMonsterSpawn(422, this.NpcDictionary[293], 155, 079); // Poison Golem
        yield return this.CreateMonsterSpawn(423, this.NpcDictionary[293], 163, 089); // Poison Golem
        yield return this.CreateMonsterSpawn(424, this.NpcDictionary[293], 180, 086); // Poison Golem
        yield return this.CreateMonsterSpawn(425, this.NpcDictionary[293], 188, 105); // Poison Golem
        yield return this.CreateMonsterSpawn(426, this.NpcDictionary[293], 188, 099); // Poison Golem
        yield return this.CreateMonsterSpawn(427, this.NpcDictionary[293], 186, 092); // Poison Golem
        yield return this.CreateMonsterSpawn(428, this.NpcDictionary[293], 182, 129); // Poison Golem
        yield return this.CreateMonsterSpawn(429, this.NpcDictionary[293], 189, 138); // Poison Golem
        yield return this.CreateMonsterSpawn(430, this.NpcDictionary[293], 166, 122); // Poison Golem
        yield return this.CreateMonsterSpawn(431, this.NpcDictionary[293], 151, 121); // Poison Golem
        yield return this.CreateMonsterSpawn(432, this.NpcDictionary[293], 067, 095); // Poison Golem
        yield return this.CreateMonsterSpawn(433, this.NpcDictionary[293], 087, 140); // Poison Golem
        yield return this.CreateMonsterSpawn(434, this.NpcDictionary[293], 103, 133); // Poison Golem
        yield return this.CreateMonsterSpawn(435, this.NpcDictionary[293], 111, 138); // Poison Golem
        yield return this.CreateMonsterSpawn(436, this.NpcDictionary[293], 117, 143); // Poison Golem
        yield return this.CreateMonsterSpawn(437, this.NpcDictionary[293], 152, 098); // Poison Golem
        yield return this.CreateMonsterSpawn(438, this.NpcDictionary[293], 150, 106); // Poison Golem

        yield return this.CreateMonsterSpawn(500, this.NpcDictionary[294], 228, 042); // Ax Warrior
        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[294], 182, 031); // Ax Warrior
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[294], 185, 015); // Ax Warrior
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[294], 193, 012); // Ax Warrior
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[294], 196, 026); // Ax Warrior
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[294], 202, 012); // Ax Warrior
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[294], 214, 014); // Ax Warrior
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[294], 202, 020); // Ax Warrior
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[294], 206, 028); // Ax Warrior
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[294], 221, 017); // Ax Warrior
        yield return this.CreateMonsterSpawn(510, this.NpcDictionary[294], 224, 025); // Ax Warrior
        yield return this.CreateMonsterSpawn(511, this.NpcDictionary[294], 228, 031); // Ax Warrior
        yield return this.CreateMonsterSpawn(512, this.NpcDictionary[294], 218, 031); // Ax Warrior
        yield return this.CreateMonsterSpawn(513, this.NpcDictionary[294], 211, 039); // Ax Warrior
        yield return this.CreateMonsterSpawn(514, this.NpcDictionary[294], 235, 045); // Ax Warrior
        yield return this.CreateMonsterSpawn(515, this.NpcDictionary[294], 223, 054); // Ax Warrior
        yield return this.CreateMonsterSpawn(516, this.NpcDictionary[294], 057, 012); // Ax Warrior
        yield return this.CreateMonsterSpawn(517, this.NpcDictionary[294], 062, 011); // Ax Warrior
        yield return this.CreateMonsterSpawn(518, this.NpcDictionary[294], 058, 019); // Ax Warrior
        yield return this.CreateMonsterSpawn(519, this.NpcDictionary[294], 067, 017); // Ax Warrior
        yield return this.CreateMonsterSpawn(520, this.NpcDictionary[294], 038, 035); // Ax Warrior
        yield return this.CreateMonsterSpawn(521, this.NpcDictionary[294], 059, 040); // Ax Warrior
        yield return this.CreateMonsterSpawn(522, this.NpcDictionary[294], 041, 041); // Ax Warrior
        yield return this.CreateMonsterSpawn(523, this.NpcDictionary[294], 050, 037); // Ax Warrior
        yield return this.CreateMonsterSpawn(524, this.NpcDictionary[294], 052, 043); // Ax Warrior
        yield return this.CreateMonsterSpawn(525, this.NpcDictionary[294], 072, 039); // Ax Warrior
        yield return this.CreateMonsterSpawn(526, this.NpcDictionary[294], 078, 045); // Ax Warrior
        yield return this.CreateMonsterSpawn(527, this.NpcDictionary[294], 072, 050); // Ax Warrior
        yield return this.CreateMonsterSpawn(528, this.NpcDictionary[294], 064, 053); // Ax Warrior
        yield return this.CreateMonsterSpawn(529, this.NpcDictionary[294], 093, 035); // Ax Warrior
        yield return this.CreateMonsterSpawn(530, this.NpcDictionary[294], 097, 039); // Ax Warrior
        yield return this.CreateMonsterSpawn(531, this.NpcDictionary[294], 103, 036); // Ax Warrior
        yield return this.CreateMonsterSpawn(532, this.NpcDictionary[294], 102, 028); // Ax Warrior
        yield return this.CreateMonsterSpawn(533, this.NpcDictionary[294], 053, 052); // Ax Warrior
        yield return this.CreateMonsterSpawn(534, this.NpcDictionary[294], 043, 062); // Ax Warrior
        yield return this.CreateMonsterSpawn(535, this.NpcDictionary[294], 127, 036); // Ax Warrior
        yield return this.CreateMonsterSpawn(536, this.NpcDictionary[294], 130, 029); // Ax Warrior
        yield return this.CreateMonsterSpawn(537, this.NpcDictionary[294], 137, 029); // Ax Warrior
        yield return this.CreateMonsterSpawn(538, this.NpcDictionary[294], 133, 037); // Ax Warrior
        yield return this.CreateMonsterSpawn(539, this.NpcDictionary[294], 132, 046); // Ax Warrior
        yield return this.CreateMonsterSpawn(540, this.NpcDictionary[294], 143, 037); // Ax Warrior
        yield return this.CreateMonsterSpawn(541, this.NpcDictionary[294], 150, 033); // Ax Warrior
        yield return this.CreateMonsterSpawn(542, this.NpcDictionary[294], 163, 037); // Ax Warrior
        yield return this.CreateMonsterSpawn(543, this.NpcDictionary[294], 170, 045); // Ax Warrior
        yield return this.CreateMonsterSpawn(544, this.NpcDictionary[294], 143, 056); // Ax Warrior
        yield return this.CreateMonsterSpawn(545, this.NpcDictionary[294], 059, 071); // Ax Warrior
        yield return this.CreateMonsterSpawn(546, this.NpcDictionary[294], 073, 069); // Ax Warrior

        yield return this.CreateMonsterSpawn(600, this.NpcDictionary[295], 220, 211); // Erohim
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 290;
            monster.Designation = "Lizard Warrior";
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
                { Stats.Level, 78 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 270 },
                { Stats.MaximumPhysBaseDmg, 320 },
                { Stats.DefenseBase, 210 },
                { Stats.AttackRatePvm, 430 },
                { Stats.DefenseRatePvm, 140 },
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
            monster.Number = 291;
            monster.Designation = "Fire Golem";
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
                { Stats.Level, 102 },
                { Stats.MaximumHealth, 55000 },
                { Stats.MinimumPhysBaseDmg, 560 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 550 },
                { Stats.AttackRatePvm, 870 },
                { Stats.DefenseRatePvm, 310 },
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
            monster.Number = 292;
            monster.Designation = "Queen Bee";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lightning);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 92 },
                { Stats.MaximumHealth, 34500 },
                { Stats.MinimumPhysBaseDmg, 489 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 360 },
                { Stats.AttackRatePvm, 620 },
                { Stats.DefenseRatePvm, 240 },
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
            monster.Number = 293;
            monster.Designation = "Poison Golem";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
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
            monster.Number = 294;
            monster.Designation = "Axe Warrior";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 75 },
                { Stats.MaximumHealth, 11500 },
                { Stats.MinimumPhysBaseDmg, 255 },
                { Stats.MaximumPhysBaseDmg, 290 },
                { Stats.DefenseBase, 195 },
                { Stats.AttackRatePvm, 385 },
                { Stats.DefenseRatePvm, 125 },
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
            monster.Number = 295;
            monster.Designation = "Erohim";
            monster.MoveRange = 3;
            monster.AttackRange = 6;
            monster.ViewRange = 8;
            monster.MoveDelay = new TimeSpan(650 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(43200 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 128 },
                { Stats.MaximumHealth, 3000000 },
                { Stats.MinimumPhysBaseDmg, 1500 },
                { Stats.MaximumPhysBaseDmg, 2000 },
                { Stats.DefenseBase, 1000 },
                { Stats.AttackRatePvm, 1500 },
                { Stats.DefenseRatePvm, 800 },
                { Stats.PoisonResistance, 254f / 255 },
                { Stats.IceResistance, 100f / 255 },
                { Stats.LightningResistance, 100f / 255 },
                { Stats.FireResistance, 100f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }

    /// <inheritdoc/>
    protected override void InitializeDropItemGroups()
    {
        base.InitializeDropItemGroups();

        var jewelsDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        jewelsDropItemGroup.SetGuid(this.MapNumber, 1);
        jewelsDropItemGroup.Chance = 0.001;
        jewelsDropItemGroup.ItemType = SpecialItemType.RandomItem;
        jewelsDropItemGroup.Description = "Jewel of Guardian";

        var jewelOfGuardian = this.GameConfiguration.Items.First(y => y.Group == 14 && y.Number == 31);
        jewelsDropItemGroup.PossibleItems.Add(jewelOfGuardian);

        this.MapDefinition!.DropItemGroups.Add(jewelsDropItemGroup);
        this.GameConfiguration.DropItemGroups.Add(jewelsDropItemGroup);
    }
}