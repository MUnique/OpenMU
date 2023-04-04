// <copyright file="CrywolfFortress.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Crywolf Fortress map.
/// </summary>
internal class CrywolfFortress : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 34;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Crywolf Fortress";

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfFortress"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CrywolfFortress(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[406], 228, 048, Direction.SouthWest); // Apostle Devin
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[407], 062, 239, Direction.SouthWest); // Werewolf Quarel
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[226], 135, 047, Direction.SouthWest); // Treiner
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[248], 099, 040, Direction.SouthEast, SpawnTrigger.Wandering); // Wandering Merchant
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[256], 096, 025, Direction.SouthEast); // Lahap
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[251], 145, 014, Direction.SouthEast); // Hanzo the Blacksmith
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[240], 113, 056, Direction.SouthWest); // Baz The Vault Keeper
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[224], 118, 011, Direction.SouthEast); // Guardsman

        yield return this.CreateMonsterSpawn(9, this.NpcDictionary[204], 121, 031, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Status
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[205], 125, 027, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Altar1
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[206], 126, 035, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Altar2
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[207], 120, 038, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Altar3
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[208], 115, 035, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Altar4
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[209], 117, 027, Direction.South, SpawnTrigger.OnceAtEventStart); // Wolf Altar5
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[310], 058, 047); // Hammer Scout
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[310], 060, 038); // Hammer Scout
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[310], 065, 024); // Hammer Scout
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[310], 070, 037); // Hammer Scout
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[310], 074, 020); // Hammer Scout
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[310], 077, 032); // Hammer Scout
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[310], 078, 051); // Hammer Scout
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[310], 079, 043); // Hammer Scout
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[310], 083, 010); // Hammer Scout
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[310], 088, 050); // Hammer Scout
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[310], 088, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[310], 094, 054); // Hammer Scout
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[310], 098, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[310], 150, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[310], 159, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[310], 162, 041); // Hammer Scout
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[310], 164, 018); // Hammer Scout
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[310], 169, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[310], 176, 032); // Hammer Scout
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[310], 178, 022); // Hammer Scout
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[310], 179, 048); // Hammer Scout
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[310], 185, 035); // Hammer Scout
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[310], 191, 027); // Hammer Scout
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[310], 194, 044); // Hammer Scout
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[310], 195, 033); // Hammer Scout
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[310], 199, 060); // Hammer Scout
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[310], 201, 040); // Hammer Scout
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[310], 205, 046); // Hammer Scout
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[310], 206, 045); // Hammer Scout
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[310], 214, 060); // Hammer Scout

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[311], 022, 100); // Lance Scout
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[311], 027, 122); // Lance Scout
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[311], 035, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[311], 047, 053); // Lance Scout
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[311], 047, 065); // Lance Scout
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[311], 079, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[311], 082, 067); // Lance Scout
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[311], 095, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[311], 103, 071); // Lance Scout
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[311], 113, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[311], 117, 099); // Lance Scout
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[311], 122, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[311], 135, 069); // Lance Scout
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[311], 138, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[311], 146, 068); // Lance Scout
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[311], 156, 069); // Lance Scout
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[311], 157, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(217, this.NpcDictionary[311], 159, 049); // Lance Scout
        yield return this.CreateMonsterSpawn(218, this.NpcDictionary[311], 170, 047); // Lance Scout
        yield return this.CreateMonsterSpawn(219, this.NpcDictionary[311], 170, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(220, this.NpcDictionary[311], 173, 097); // Lance Scout
        yield return this.CreateMonsterSpawn(221, this.NpcDictionary[311], 174, 054); // Lance Scout
        yield return this.CreateMonsterSpawn(222, this.NpcDictionary[311], 177, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(223, this.NpcDictionary[311], 197, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(224, this.NpcDictionary[311], 210, 088); // Lance Scout
        yield return this.CreateMonsterSpawn(225, this.NpcDictionary[311], 211, 069); // Lance Scout
        yield return this.CreateMonsterSpawn(226, this.NpcDictionary[311], 221, 078); // Lance Scout
        yield return this.CreateMonsterSpawn(227, this.NpcDictionary[311], 223, 120); // Lance Scout
        yield return this.CreateMonsterSpawn(228, this.NpcDictionary[311], 225, 068); // Lance Scout
        yield return this.CreateMonsterSpawn(229, this.NpcDictionary[311], 231, 078); // Lance Scout

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[312], 021, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[312], 023, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[312], 030, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[312], 056, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[312], 068, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[312], 078, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[312], 079, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[312], 083, 101); // Bow Scout
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[312], 090, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[312], 100, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[312], 114, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[312], 122, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[312], 124, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[312], 135, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[312], 145, 099); // Bow Scout
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[312], 146, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[312], 155, 115); // Bow Scout
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[312], 156, 084); // Bow Scout
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[312], 168, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[312], 178, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[312], 192, 088); // Bow Scout
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[312], 201, 089); // Bow Scout
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[312], 202, 098); // Bow Scout
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[312], 217, 094); // Bow Scout
        yield return this.CreateMonsterSpawn(324, this.NpcDictionary[312], 233, 102); // Bow Scout
        yield return this.CreateMonsterSpawn(325, this.NpcDictionary[312], 236, 088); // Bow Scout

        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[313], 019, 131); // Werewolf
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[313], 020, 145); // Werewolf
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[313], 033, 130); // Werewolf
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[313], 046, 123); // Werewolf
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[313], 071, 109); // Werewolf
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[313], 076, 145); // Werewolf
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[313], 082, 107); // Werewolf
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[313], 082, 130); // Werewolf
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[313], 088, 188, 144, 144); // Werewolf
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[313], 119, 108); // Werewolf
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[313], 123, 130); // Werewolf
        yield return this.CreateMonsterSpawn(411, this.NpcDictionary[313], 123, 145); // Werewolf
        yield return this.CreateMonsterSpawn(412, this.NpcDictionary[313], 149, 110); // Werewolf
        yield return this.CreateMonsterSpawn(413, this.NpcDictionary[313], 155, 098); // Werewolf
        yield return this.CreateMonsterSpawn(414, this.NpcDictionary[313], 163, 130); // Werewolf
        yield return this.CreateMonsterSpawn(415, this.NpcDictionary[313], 164, 144); // Werewolf
        yield return this.CreateMonsterSpawn(416, this.NpcDictionary[313], 197, 145); // Werewolf
        yield return this.CreateMonsterSpawn(417, this.NpcDictionary[313], 204, 130); // Werewolf
        yield return this.CreateMonsterSpawn(418, this.NpcDictionary[313], 206, 110); // Werewolf
        yield return this.CreateMonsterSpawn(419, this.NpcDictionary[313], 218, 110); // Werewolf
        yield return this.CreateMonsterSpawn(420, this.NpcDictionary[313], 226, 130); // Werewolf
        yield return this.CreateMonsterSpawn(421, this.NpcDictionary[313], 235, 110); // Werewolf
        yield return this.CreateMonsterSpawn(422, this.NpcDictionary[313], 237, 144); // Werewolf
        yield return this.CreateMonsterSpawn(423, this.NpcDictionary[313], 240, 131); // Werewolf

        yield return this.CreateMonsterSpawn(500, this.NpcDictionary[314], 016, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[314], 028, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[314], 065, 148); // Scout(Hero)
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[314], 092, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[314], 119, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[314], 159, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[314], 171, 148); // Scout(Hero)
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[314], 177, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[314], 236, 156); // Scout(Hero)
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[314], 236, 156); // Scout(Hero)

        yield return this.CreateMonsterSpawn(600, this.NpcDictionary[315], 030, 172); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(601, this.NpcDictionary[315], 049, 194); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(602, this.NpcDictionary[315], 051, 213); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(603, this.NpcDictionary[315], 052, 171); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(604, this.NpcDictionary[315], 059, 211); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(605, this.NpcDictionary[315], 068, 224); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(606, this.NpcDictionary[315], 073, 210); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(607, this.NpcDictionary[315], 085, 230); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(608, this.NpcDictionary[315], 092, 219); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(609, this.NpcDictionary[315], 100, 182); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(610, this.NpcDictionary[315], 102, 171); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(611, this.NpcDictionary[315], 115, 204); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(612, this.NpcDictionary[315], 117, 193); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(613, this.NpcDictionary[315], 133, 195); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(614, this.NpcDictionary[315], 133, 214); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(615, this.NpcDictionary[315], 145, 208); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(616, this.NpcDictionary[315], 148, 195); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(617, this.NpcDictionary[315], 159, 206); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(618, this.NpcDictionary[315], 170, 208); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(619, this.NpcDictionary[315], 175, 197); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(620, this.NpcDictionary[315], 199, 200); // Werewolf(Hero)
        yield return this.CreateMonsterSpawn(621, this.NpcDictionary[315], 222, 189); // Werewolf(Hero)

        yield return this.CreateMonsterSpawn(700, this.NpcDictionary[316], 047, 184); // Balram
        yield return this.CreateMonsterSpawn(701, this.NpcDictionary[316], 084, 201); // Balram
        yield return this.CreateMonsterSpawn(702, this.NpcDictionary[316], 104, 194); // Balram
        yield return this.CreateMonsterSpawn(703, this.NpcDictionary[316], 117, 184); // Balram
        yield return this.CreateMonsterSpawn(704, this.NpcDictionary[316], 146, 184); // Balram
        yield return this.CreateMonsterSpawn(705, this.NpcDictionary[316], 161, 194); // Balram
        yield return this.CreateMonsterSpawn(706, this.NpcDictionary[316], 173, 184); // Balram
        yield return this.CreateMonsterSpawn(707, this.NpcDictionary[316], 212, 193); // Balram
        yield return this.CreateMonsterSpawn(708, this.NpcDictionary[316], 228, 180); // Balram
        yield return this.CreateMonsterSpawn(709, this.NpcDictionary[316], 054, 217); // Balram
        yield return this.CreateMonsterSpawn(710, this.NpcDictionary[316], 066, 217); // Balram
        yield return this.CreateMonsterSpawn(711, this.NpcDictionary[316], 083, 217); // Balram
        yield return this.CreateMonsterSpawn(712, this.NpcDictionary[316], 100, 217); // Balram
        yield return this.CreateMonsterSpawn(713, this.NpcDictionary[316], 134, 215); // Balram
        yield return this.CreateMonsterSpawn(714, this.NpcDictionary[316], 153, 215); // Balram

        yield return this.CreateMonsterSpawn(800, this.NpcDictionary[317], 014, 164); // Soram
        yield return this.CreateMonsterSpawn(801, this.NpcDictionary[317], 025, 164); // Soram
        yield return this.CreateMonsterSpawn(802, this.NpcDictionary[317], 041, 171); // Soram
        yield return this.CreateMonsterSpawn(803, this.NpcDictionary[317], 056, 164); // Soram
        yield return this.CreateMonsterSpawn(804, this.NpcDictionary[317], 065, 234); // Soram
        yield return this.CreateMonsterSpawn(805, this.NpcDictionary[317], 096, 164); // Soram
        yield return this.CreateMonsterSpawn(806, this.NpcDictionary[317], 114, 164); // Soram
        yield return this.CreateMonsterSpawn(807, this.NpcDictionary[317], 116, 172); // Soram
        yield return this.CreateMonsterSpawn(808, this.NpcDictionary[317], 140, 230); // Soram
        yield return this.CreateMonsterSpawn(809, this.NpcDictionary[317], 145, 174); // Soram
        yield return this.CreateMonsterSpawn(810, this.NpcDictionary[317], 147, 164); // Soram
        yield return this.CreateMonsterSpawn(811, this.NpcDictionary[317], 168, 229); // Soram
        yield return this.CreateMonsterSpawn(812, this.NpcDictionary[317], 177, 167); // Soram
        yield return this.CreateMonsterSpawn(813, this.NpcDictionary[317], 233, 169); // Soram
        yield return this.CreateMonsterSpawn(814, this.NpcDictionary[317], 115, 221); // Soram
        yield return this.CreateMonsterSpawn(815, this.NpcDictionary[317], 103, 206); // Soram
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 204;
            monster.Designation = "Wolf Status";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 205;
            monster.Designation = "Wolf Altar1";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 206;
            monster.Designation = "Wolf Altar2";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 207;
            monster.Designation = "Wolf Altar3";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 208;
            monster.Designation = "Wolf Altar4";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 209;
            monster.Designation = "Wolf Altar5";
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 2 },
                { Stats.MaximumHealth, 50 },
                { Stats.MinimumPhysBaseDmg, 15 },
                { Stats.MaximumPhysBaseDmg, 30 },
                { Stats.DefenseBase, 70 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 310;
            monster.Designation = "Hammer Scout";
            monster.MoveRange = 6;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 97 },
                { Stats.MaximumHealth, 45000 },
                { Stats.MinimumPhysBaseDmg, 530 },
                { Stats.MaximumPhysBaseDmg, 560 },
                { Stats.DefenseBase, 420 },
                { Stats.AttackRatePvm, 670 },
                { Stats.DefenseRatePvm, 250 },
                { Stats.PoisonResistance, 15f / 255 },
                { Stats.IceResistance, 15f / 255 },
                { Stats.FireResistance, 15f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 311;
            monster.Designation = "Lance Scout";
            monster.MoveRange = 6;
            monster.AttackRange = 3;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 97 },
                { Stats.MaximumHealth, 45000 },
                { Stats.MinimumPhysBaseDmg, 530 },
                { Stats.MaximumPhysBaseDmg, 560 },
                { Stats.DefenseBase, 420 },
                { Stats.AttackRatePvm, 670 },
                { Stats.DefenseRatePvm, 250 },
                { Stats.PoisonResistance, 17f / 255 },
                { Stats.IceResistance, 17f / 255 },
                { Stats.FireResistance, 17f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 312;
            monster.Designation = "Bow Scout";
            monster.MoveRange = 6;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 97 },
                { Stats.MaximumHealth, 45000 },
                { Stats.MinimumPhysBaseDmg, 530 },
                { Stats.MaximumPhysBaseDmg, 560 },
                { Stats.DefenseBase, 420 },
                { Stats.AttackRatePvm, 670 },
                { Stats.DefenseRatePvm, 250 },
                { Stats.PoisonResistance, 19f / 255 },
                { Stats.IceResistance, 19f / 255 },
                { Stats.FireResistance, 19f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 313;
            monster.Designation = "Werewolf";
            monster.MoveRange = 6;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 118 },
                { Stats.MaximumHealth, 110000 },
                { Stats.MinimumPhysBaseDmg, 830 },
                { Stats.MaximumPhysBaseDmg, 850 },
                { Stats.DefenseBase, 680 },
                { Stats.AttackRatePvm, 950 },
                { Stats.DefenseRatePvm, 355 },
                { Stats.PoisonResistance, 21f / 255 },
                { Stats.IceResistance, 21f / 255 },
                { Stats.FireResistance, 21f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 314;
            monster.Designation = "Scout(Hero)";
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
                { Stats.Level, 123 },
                { Stats.MaximumHealth, 120000 },
                { Stats.MinimumPhysBaseDmg, 890 },
                { Stats.MaximumPhysBaseDmg, 910 },
                { Stats.DefenseBase, 740 },
                { Stats.AttackRatePvm, 980 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.PoisonResistance, 23f / 255 },
                { Stats.IceResistance, 23f / 255 },
                { Stats.FireResistance, 23f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 315;
            monster.Designation = "Werewolf(Hero)";
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
                { Stats.Level, 127 },
                { Stats.MaximumHealth, 123000 },
                { Stats.MinimumPhysBaseDmg, 964 },
                { Stats.MaximumPhysBaseDmg, 1015 },
                { Stats.DefenseBase, 800 },
                { Stats.AttackRatePvm, 1027 },
                { Stats.DefenseRatePvm, 397 },
                { Stats.PoisonResistance, 25f / 255 },
                { Stats.IceResistance, 25f / 255 },
                { Stats.FireResistance, 25f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 316;
            monster.Designation = "Balram";
            monster.MoveRange = 6;
            monster.AttackRange = 3;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 132 },
                { Stats.MaximumHealth, 140000 },
                { Stats.MinimumPhysBaseDmg, 1075 },
                { Stats.MaximumPhysBaseDmg, 1140 },
                { Stats.DefenseBase, 885 },
                { Stats.AttackRatePvm, 1100 },
                { Stats.DefenseRatePvm, 440 },
                { Stats.PoisonResistance, 27f / 255 },
                { Stats.IceResistance, 27f / 255 },
                { Stats.FireResistance, 27f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 317;
            monster.Designation = "Soram";
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
                { Stats.MaximumHealth, 164000 },
                { Stats.MinimumPhysBaseDmg, 1200 },
                { Stats.MaximumPhysBaseDmg, 1300 },
                { Stats.DefenseBase, 982 },
                { Stats.AttackRatePvm, 1173 },
                { Stats.DefenseRatePvm, 500 },
                { Stats.PoisonResistance, 29f / 255 },
                { Stats.IceResistance, 29f / 255 },
                { Stats.FireResistance, 29f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}