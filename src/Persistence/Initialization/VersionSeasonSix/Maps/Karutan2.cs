// <copyright file="Karutan2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Karutan2 map.
/// </summary>
internal class Karutan2 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 81;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Karutan 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="Karutan2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Karutan2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Karutan1.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[571], 058, 160); // Orcus
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[571], 053, 165); // Orcus
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[571], 067, 180); // Orcus
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[571], 070, 169); // Orcus
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[571], 061, 172); // Orcus
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[571], 134, 141); // Orcus
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[571], 143, 149); // Orcus
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[571], 143, 139); // Orcus
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[571], 131, 146); // Orcus
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[571], 136, 156); // Orcus
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[571], 197, 157); // Orcus
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[571], 205, 158); // Orcus
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[571], 211, 157); // Orcus
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[571], 211, 167); // Orcus
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[571], 204, 170); // Orcus
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[571], 198, 162); // Orcus
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[571], 064, 134); // Orcus
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[571], 126, 124); // Orcus
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[571], 132, 117); // Orcus
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[571], 214, 133); // Orcus
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[571], 208, 126); // Orcus
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[571], 067, 175); // Orcus

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[572], 065, 163); // Gollock
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[572], 067, 171); // Gollock
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[572], 058, 173); // Gollock
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[572], 138, 138); // Gollock
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[572], 140, 154); // Gollock
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[572], 148, 144); // Gollock
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[572], 201, 158); // Gollock
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[572], 208, 171); // Gollock
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[572], 214, 162); // Gollock
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[572], 202, 166); // Gollock
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[572], 206, 134); // Gollock
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[572], 216, 137); // Gollock
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[572], 140, 117); // Gollock
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[572], 061, 145); // Gollock
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[572], 066, 142); // Gollock

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[575], 078, 042); // Condra
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[575], 071, 043); // Condra
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[575], 076, 060); // Condra
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[575], 058, 059); // Condra
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[575], 060, 046); // Condra
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[575], 082, 048); // Condra
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[575], 094, 056); // Condra
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[575], 114, 061); // Condra
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[575], 115, 049); // Condra
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[575], 127, 060); // Condra
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[575], 127, 053); // Condra
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[575], 130, 049); // Condra
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[575], 152, 047); // Condra
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[575], 160, 035); // Condra
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[575], 186, 041); // Condra
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[575], 201, 037); // Condra
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[575], 207, 051); // Condra
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[575], 207, 076); // Condra
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[575], 186, 060); // Condra
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[575], 171, 068); // Condra
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[575], 137, 081); // Condra
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[575], 140, 063); // Condra
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[575], 097, 078); // Condra
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[575], 126, 089); // Condra
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[575], 106, 082); // Condra
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[575], 098, 092); // Condra
        yield return this.CreateMonsterSpawn(326, this.NpcDictionary[575], 081, 095); // Condra
        yield return this.CreateMonsterSpawn(327, this.NpcDictionary[575], 069, 088); // Condra
        yield return this.CreateMonsterSpawn(328, this.NpcDictionary[575], 100, 104); // Condra
        yield return this.CreateMonsterSpawn(329, this.NpcDictionary[575], 119, 081); // Condra
        yield return this.CreateMonsterSpawn(330, this.NpcDictionary[575], 156, 080); // Condra
        yield return this.CreateMonsterSpawn(331, this.NpcDictionary[575], 200, 083); // Condra
        yield return this.CreateMonsterSpawn(332, this.NpcDictionary[575], 203, 062); // Condra
        yield return this.CreateMonsterSpawn(333, this.NpcDictionary[575], 206, 040); // Condra
        yield return this.CreateMonsterSpawn(334, this.NpcDictionary[575], 169, 039); // Condra
        yield return this.CreateMonsterSpawn(335, this.NpcDictionary[575], 169, 030); // Condra
        yield return this.CreateMonsterSpawn(336, this.NpcDictionary[575], 101, 057); // Condra
        yield return this.CreateMonsterSpawn(337, this.NpcDictionary[575], 076, 053); // Condra
        yield return this.CreateMonsterSpawn(338, this.NpcDictionary[575], 078, 103); // Condra
        yield return this.CreateMonsterSpawn(339, this.NpcDictionary[575], 097, 085); // Condra
        yield return this.CreateMonsterSpawn(340, this.NpcDictionary[575], 138, 113); // Condra
        yield return this.CreateMonsterSpawn(341, this.NpcDictionary[575], 145, 108); // Condra
        yield return this.CreateMonsterSpawn(342, this.NpcDictionary[575], 178, 064); // Condra
        yield return this.CreateMonsterSpawn(343, this.NpcDictionary[575], 201, 058); // Condra
        yield return this.CreateMonsterSpawn(344, this.NpcDictionary[575], 201, 087); // Condra
        yield return this.CreateMonsterSpawn(345, this.NpcDictionary[575], 221, 048); // Condra
        yield return this.CreateMonsterSpawn(346, this.NpcDictionary[575], 211, 065); // Condra
        yield return this.CreateMonsterSpawn(347, this.NpcDictionary[575], 074, 105); // Condra
        yield return this.CreateMonsterSpawn(348, this.NpcDictionary[575], 071, 131); // Condra
        yield return this.CreateMonsterSpawn(349, this.NpcDictionary[575], 059, 121); // Condra
        yield return this.CreateMonsterSpawn(350, this.NpcDictionary[575], 054, 136); // Condra
        yield return this.CreateMonsterSpawn(351, this.NpcDictionary[575], 058, 140); // Condra
        yield return this.CreateMonsterSpawn(352, this.NpcDictionary[575], 062, 138); // Condra
        yield return this.CreateMonsterSpawn(353, this.NpcDictionary[575], 130, 121); // Condra
        yield return this.CreateMonsterSpawn(354, this.NpcDictionary[575], 132, 103); // Condra
        yield return this.CreateMonsterSpawn(355, this.NpcDictionary[575], 152, 105); // Condra
        yield return this.CreateMonsterSpawn(356, this.NpcDictionary[575], 200, 108); // Condra
        yield return this.CreateMonsterSpawn(357, this.NpcDictionary[575], 213, 126); // Condra
        yield return this.CreateMonsterSpawn(358, this.NpcDictionary[575], 211, 095); // Condra
        yield return this.CreateMonsterSpawn(359, this.NpcDictionary[575], 182, 072); // Condra
        yield return this.CreateMonsterSpawn(360, this.NpcDictionary[575], 185, 047); // Condra
        yield return this.CreateMonsterSpawn(361, this.NpcDictionary[575], 164, 030); // Condra

        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[576], 087, 042); // Narcondra
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[576], 081, 054); // Narcondra
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[576], 107, 062); // Narcondra
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[576], 110, 055); // Narcondra
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[576], 122, 058); // Narcondra
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[576], 092, 080); // Narcondra
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[576], 141, 056); // Narcondra
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[576], 170, 035); // Narcondra
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[576], 190, 192, 038, 038); // Narcondra
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[576], 195, 061); // Narcondra
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[576], 210, 043); // Narcondra
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[576], 202, 080); // Narcondra
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[576], 140, 074); // Narcondra
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[576], 133, 083); // Narcondra
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[576], 125, 085); // Narcondra
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[576], 104, 089); // Narcondra
        yield return this.CreateMonsterSpawn(416, this.NpcDictionary[576], 078, 098); // Narcondra
        yield return this.CreateMonsterSpawn(417, this.NpcDictionary[576], 065, 090); // Narcondra
        yield return this.CreateMonsterSpawn(418, this.NpcDictionary[576], 066, 126); // Narcondra
        yield return this.CreateMonsterSpawn(419, this.NpcDictionary[576], 078, 109); // Narcondra
        yield return this.CreateMonsterSpawn(420, this.NpcDictionary[576], 132, 109); // Narcondra
        yield return this.CreateMonsterSpawn(421, this.NpcDictionary[576], 187, 081); // Narcondra
        yield return this.CreateMonsterSpawn(422, this.NpcDictionary[576], 197, 099); // Narcondra
        yield return this.CreateMonsterSpawn(423, this.NpcDictionary[576], 206, 110); // Narcondra
        yield return this.CreateMonsterSpawn(424, this.NpcDictionary[576], 217, 122); // Narcondra
        yield return this.CreateMonsterSpawn(425, this.NpcDictionary[576], 210, 130); // Narcondra
        yield return this.CreateMonsterSpawn(426, this.NpcDictionary[576], 148, 120); // Narcondra
        yield return this.CreateMonsterSpawn(427, this.NpcDictionary[576], 153, 113); // Narcondra
        yield return this.CreateMonsterSpawn(428, this.NpcDictionary[576], 069, 140); // Narcondra
        yield return this.CreateMonsterSpawn(429, this.NpcDictionary[576], 053, 129); // Narcondra
        yield return this.CreateMonsterSpawn(430, this.NpcDictionary[576], 095, 095); // Narcondra
        yield return this.CreateMonsterSpawn(431, this.NpcDictionary[576], 084, 100); // Narcondra
        yield return this.CreateMonsterSpawn(432, this.NpcDictionary[576], 150, 084); // Narcondra
        yield return this.CreateMonsterSpawn(433, this.NpcDictionary[576], 181, 057); // Narcondra
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 572;
            monster.Designation = "Gollock";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 10;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 108 },
                { Stats.MaximumHealth, 72000 },
                { Stats.MinimumPhysBaseDmg, 685 },
                { Stats.MaximumPhysBaseDmg, 735 },
                { Stats.DefenseBase, 545 },
                { Stats.AttackRatePvm, 1020 },
                { Stats.DefenseRatePvm, 315 },
                { Stats.PoisonResistance, 100f / 255 },
                { Stats.IceResistance, 100f / 255 },
                { Stats.LightningResistance, 100f / 255 },
                { Stats.FireResistance, 240f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 575;
            monster.Designation = "Condra";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 10;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 90000 },
                { Stats.MinimumPhysBaseDmg, 735 },
                { Stats.MaximumPhysBaseDmg, 790 },
                { Stats.DefenseBase, 610 },
                { Stats.AttackRatePvm, 1200 },
                { Stats.DefenseRatePvm, 406 },
                { Stats.PoisonResistance, 255f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 255f / 255 },
                { Stats.FireResistance, 255f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 576;
            monster.Designation = "Narcondra";
            monster.MoveRange = 6;
            monster.AttackRange = 2;
            monster.ViewRange = 10;
            monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 120 },
                { Stats.MaximumHealth, 96000 },
                { Stats.MinimumPhysBaseDmg, 750 },
                { Stats.MaximumPhysBaseDmg, 815 },
                { Stats.DefenseBase, 640 },
                { Stats.AttackRatePvm, 1265 },
                { Stats.DefenseRatePvm, 425 },
                { Stats.PoisonResistance, 255f / 255 },
                { Stats.IceResistance, 150f / 255 },
                { Stats.LightningResistance, 255f / 255 },
                { Stats.FireResistance, 255f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}