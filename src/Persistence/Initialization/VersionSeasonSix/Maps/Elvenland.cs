// <copyright file="Elvenland.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization for the Elvenland map.
/// </summary>
internal class Elvenland : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    public static readonly byte Number = 51;

    /// <summary>
    /// Initializes a new instance of the <see cref="Elvenland"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Elvenland(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the automatic monster spawns of the Season 6 layout of Elvenland.
    /// </summary>
    /// <remarks>
    /// The layout places the monsters in smaller areas and at single spots, instead of spreading them
    /// over quarters of the map. It includes the spiders, which the first hunting quest of Season 6 needs.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInSeason6"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(200, 3, 13, 56, 125, 168, 15), // Spider
        new(201, 3, 102, 142, 194, 238, 15), // Spider
        new(202, 418, 54, 88, 131, 180, 10), // Strange Rabbit
        new(203, 418, 84, 142, 178, 212, 10), // Strange Rabbit
        new(204, 421, 87, 125, 90, 170, 10), // Werewolf
        new(205, 421, 118, 164, 130, 172, 10), // Werewolf
        new(206, 419, 118, 160, 87, 141, 20), // Polluted Butterfly
        new(207, 425, 205, 205, 16, 16, 1), // Captain Grizzly
        new(208, 425, 200, 200, 54, 54, 1), // Captain Grizzly
        new(209, 425, 219, 219, 74, 74, 1), // Captain Grizzly
        new(210, 423, 230, 230, 143, 143, 1), // Totem Golem
        new(211, 423, 196, 196, 171, 171, 1), // Totem Golem
        new(212, 420, 74, 74, 81, 81, 1), // Hideous Rabbit
        new(213, 420, 102, 102, 106, 106, 1), // Hideous Rabbit
        new(214, 424, 169, 169, 124, 124, 1), // Grizzly
        new(215, 419, 93, 93, 117, 117, 1), // Polluted Butterfly
        new(216, 423, 202, 202, 193, 193, 1), // Totem Golem
        new(217, 423, 184, 184, 194, 194, 1), // Totem Golem
        new(218, 422, 135, 135, 89, 89, 1), // Cursed Lich
        new(219, 419, 139, 139, 141, 141, 1), // Polluted Butterfly
        new(220, 419, 129, 129, 119, 119, 1), // Polluted Butterfly
        new(221, 419, 130, 130, 132, 132, 1), // Polluted Butterfly
        new(222, 419, 115, 115, 109, 109, 1), // Polluted Butterfly
        new(223, 418, 45, 45, 160, 160, 1), // Strange Rabbit
        new(224, 418, 31, 31, 157, 157, 1), // Strange Rabbit
        new(225, 418, 50, 50, 149, 149, 1), // Strange Rabbit
        new(226, 418, 24, 24, 144, 144, 1), // Strange Rabbit
        new(227, 421, 68, 68, 137, 137, 1), // Werewolf
        new(228, 421, 122, 122, 189, 189, 1), // Werewolf
        new(229, 421, 133, 133, 213, 213, 1), // Werewolf
        new(230, 421, 33, 33, 130, 130, 1), // Werewolf
        new(231, 425, 205, 205, 16, 16, 1), // Captain Grizzly
        new(232, 425, 219, 219, 74, 74, 1), // Captain Grizzly
        new(233, 423, 230, 230, 143, 143, 1), // Totem Golem
        new(234, 423, 196, 196, 171, 171, 1), // Totem Golem
        new(235, 420, 74, 74, 81, 81, 1), // Hideous Rabbit
        new(236, 423, 202, 202, 193, 193, 1), // Totem Golem
        new(237, 423, 184, 184, 194, 194, 1), // Totem Golem
        new(238, 425, 185, 185, 20, 20, 1), // Captain Grizzly
        new(239, 419, 139, 139, 141, 141, 1), // Polluted Butterfly
        new(240, 419, 129, 129, 119, 119, 1), // Polluted Butterfly
        new(241, 419, 130, 130, 132, 132, 1), // Polluted Butterfly
        new(242, 418, 45, 45, 160, 160, 1), // Strange Rabbit
        new(243, 418, 31, 31, 157, 157, 1), // Strange Rabbit
        new(244, 418, 50, 50, 149, 149, 1), // Strange Rabbit
        new(245, 418, 24, 24, 144, 144, 1), // Strange Rabbit
        new(246, 421, 68, 68, 137, 137, 1), // Werewolf
        new(247, 421, 122, 122, 189, 189, 1), // Werewolf
        new(248, 421, 93, 93, 117, 117, 1), // Werewolf
        new(249, 421, 119, 119, 139, 139, 1), // Werewolf
        new(250, 419, 135, 135, 89, 89, 1), // Polluted Butterfly
        new(251, 419, 169, 169, 124, 124, 1), // Polluted Butterfly
        new(252, 422, 118, 118, 49, 49, 1), // Cursed Lich
        new(253, 423, 228, 228, 130, 130, 1), // Totem Golem
        new(254, 419, 150, 150, 110, 110, 1), // Polluted Butterfly
        new(255, 423, 226, 226, 149, 149, 1), // Totem Golem
        new(256, 425, 202, 202, 57, 57, 1), // Captain Grizzly
        new(257, 425, 205, 205, 53, 53, 1), // Captain Grizzly
        new(258, 424, 176, 176, 68, 68, 1), // Grizzly
        new(259, 425, 176, 176, 38, 38, 1), // Captain Grizzly
        new(260, 419, 111, 111, 127, 127, 1), // Polluted Butterfly
        new(261, 425, 196, 196, 38, 38, 1), // Captain Grizzly
        new(262, 425, 212, 212, 20, 20, 1), // Captain Grizzly
        new(263, 425, 230, 230, 48, 48, 1), // Captain Grizzly
        new(264, 425, 226, 226, 45, 45, 1), // Captain Grizzly
        new(265, 425, 226, 226, 51, 51, 1), // Captain Grizzly
        new(266, 425, 214, 214, 52, 52, 1), // Captain Grizzly
        new(267, 425, 173, 173, 34, 34, 1), // Captain Grizzly
        new(268, 423, 171, 171, 175, 175, 1), // Totem Golem
        new(269, 423, 174, 174, 180, 180, 1), // Totem Golem
        new(270, 423, 192, 192, 144, 144, 1), // Totem Golem
        new(271, 423, 190, 190, 150, 150, 1), // Totem Golem
        new(272, 423, 195, 195, 149, 149, 1), // Totem Golem
        new(273, 423, 223, 223, 143, 143, 1), // Totem Golem
        new(274, 424, 186, 186, 75, 75, 1), // Grizzly
        new(275, 424, 180, 180, 75, 75, 1), // Grizzly
        new(276, 424, 184, 184, 79, 79, 1), // Grizzly
        new(277, 420, 45, 45, 77, 77, 1), // Hideous Rabbit
        new(278, 425, 168, 168, 24, 24, 1), // Captain Grizzly
        new(279, 424, 195, 195, 33, 33, 1), // Grizzly
        new(280, 424, 193, 193, 88, 88, 1), // Grizzly
        new(281, 424, 192, 192, 93, 93, 1), // Grizzly
        new(282, 424, 152, 152, 51, 51, 1), // Grizzly
        new(283, 425, 177, 177, 33, 33, 1), // Captain Grizzly
        new(284, 419, 151, 151, 156, 156, 1), // Polluted Butterfly
        new(285, 419, 155, 155, 147, 147, 1), // Polluted Butterfly
        new(286, 422, 115, 115, 89, 89, 1), // Cursed Lich
        new(287, 421, 135, 135, 195, 195, 1), // Werewolf
        new(288, 418, 75, 75, 173, 173, 1), // Strange Rabbit
        new(289, 421, 75, 75, 154, 154, 1), // Werewolf
        new(290, 419, 102, 102, 139, 139, 1), // Polluted Butterfly
        new(291, 422, 121, 121, 23, 23, 1), // Cursed Lich
        new(292, 420, 68, 68, 30, 30, 1), // Hideous Rabbit
        new(293, 422, 125, 125, 38, 38, 1), // Cursed Lich
        new(294, 424, 182, 182, 98, 98, 1), // Grizzly
        new(295, 424, 173, 173, 94, 94, 1), // Grizzly
        new(296, 424, 160, 160, 81, 81, 1), // Grizzly
        new(297, 424, 152, 152, 76, 76, 1), // Grizzly
        new(298, 424, 154, 154, 80, 80, 1), // Grizzly
        new(299, 418, 123, 123, 227, 227, 1), // Strange Rabbit
        new(300, 418, 109, 109, 217, 217, 1), // Strange Rabbit
        new(301, 424, 150, 150, 55, 55, 1), // Grizzly
        new(302, 424, 156, 156, 52, 52, 1), // Grizzly
        new(303, 424, 154, 154, 56, 56, 1), // Grizzly
        new(304, 424, 163, 163, 69, 69, 1), // Grizzly
        new(305, 424, 173, 173, 80, 80, 1), // Grizzly
        new(306, 424, 189, 189, 90, 90, 1), // Grizzly
        new(307, 423, 212, 212, 150, 150, 1), // Totem Golem
        new(308, 425, 202, 202, 19, 19, 1), // Captain Grizzly
        new(309, 425, 207, 207, 20, 20, 1), // Captain Grizzly
        new(310, 425, 212, 212, 24, 24, 1), // Captain Grizzly
        new(311, 423, 200, 200, 157, 157, 1), // Totem Golem
        new(312, 423, 177, 177, 175, 175, 1), // Totem Golem
        new(313, 423, 208, 208, 125, 125, 1), // Totem Golem
        new(314, 423, 214, 214, 135, 135, 1), // Totem Golem
        new(315, 423, 227, 227, 118, 118, 1), // Totem Golem
        new(316, 424, 167, 167, 91, 91, 1), // Grizzly
        new(317, 424, 171, 171, 90, 90, 1), // Grizzly
        new(318, 425, 199, 199, 35, 35, 1), // Captain Grizzly
        new(319, 425, 194, 194, 46, 46, 1), // Captain Grizzly
        new(320, 425, 196, 196, 50, 50, 1), // Captain Grizzly
        new(321, 425, 198, 198, 47, 47, 1), // Captain Grizzly
        new(322, 425, 218, 218, 38, 38, 1), // Captain Grizzly
        new(323, 425, 207, 207, 38, 38, 1), // Captain Grizzly
        new(324, 422, 104, 104, 67, 67, 1), // Cursed Lich
        new(325, 422, 104, 104, 30, 30, 1), // Cursed Lich
        new(326, 422, 102, 102, 32, 32, 1), // Cursed Lich
        new(327, 419, 137, 137, 103, 103, 1), // Polluted Butterfly
        new(328, 421, 115, 115, 163, 163, 1), // Werewolf
        new(329, 421, 82, 82, 138, 138, 1), // Werewolf
        new(330, 419, 150, 150, 129, 129, 1), // Polluted Butterfly
        new(331, 419, 151, 151, 126, 126, 1), // Polluted Butterfly
        new(332, 418, 105, 105, 201, 201, 1), // Strange Rabbit
        new(333, 418, 106, 106, 187, 187, 1), // Strange Rabbit
        new(334, 418, 97, 97, 190, 190, 1), // Strange Rabbit
        new(335, 419, 128, 128, 169, 169, 1), // Polluted Butterfly
        new(336, 420, 72, 72, 32, 32, 1), // Hideous Rabbit
        new(337, 422, 87, 87, 39, 39, 1), // Cursed Lich
        new(338, 420, 92, 92, 58, 58, 1), // Hideous Rabbit
        new(339, 421, 118, 118, 155, 155, 1), // Werewolf
        new(340, 422, 76, 76, 15, 15, 1), // Cursed Lich
        new(341, 422, 80, 80, 16, 16, 1), // Cursed Lich
        new(342, 419, 97, 97, 105, 105, 1), // Polluted Butterfly
        new(343, 420, 69, 69, 59, 59, 1), // Hideous Rabbit
        new(344, 420, 72, 72, 60, 60, 1), // Hideous Rabbit
        new(345, 420, 70, 70, 63, 63, 1), // Hideous Rabbit
        new(346, 420, 46, 46, 82, 82, 1), // Hideous Rabbit
        new(347, 420, 57, 57, 91, 91, 1), // Hideous Rabbit
        new(348, 420, 52, 52, 69, 69, 1), // Hideous Rabbit
        new(349, 420, 65, 65, 75, 75, 1), // Hideous Rabbit
        new(350, 420, 73, 73, 84, 84, 1), // Hideous Rabbit
        new(351, 422, 127, 127, 40, 40, 1), // Cursed Lich
        new(352, 422, 135, 135, 28, 28, 1), // Cursed Lich
        new(353, 422, 135, 135, 25, 25, 1), // Cursed Lich
        new(354, 422, 96, 96, 45, 45, 1), // Cursed Lich
        new(355, 420, 83, 83, 57, 57, 1), // Hideous Rabbit
        new(356, 422, 104, 104, 49, 49, 1), // Cursed Lich
        new(357, 422, 104, 104, 63, 63, 1), // Cursed Lich
        new(358, 420, 84, 84, 42, 42, 1), // Hideous Rabbit
        new(359, 421, 132, 132, 209, 209, 1), // Werewolf
    ];

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override LocalizedString MapName => Name;

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    private static LocalizedString Name => LocalizedString.FromResource(() => MapNames.Elvenland);

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[256], 37, 242, Direction.SouthWest); // Lahap
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[415], 44, 229, Direction.SouthWest); // Silvia
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[416], 29, 237, Direction.South); // Rhea
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[417], 37, 218, Direction.SouthWest); // Marce
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[257], 44, 189, Direction.SouthEast); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[257], 57, 231, Direction.SouthWest); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[257], 74, 74, 219, 220, 1, Direction.SouthWest); // Shadow Phantom Soldier
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[240], 51, 229, Direction.SouthWest); // Safety Guardian
        yield return this.CreateMonsterSpawn(9, this.NpcDictionary[385], 55, 243, Direction.SouthWest); // Mirage
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[452], 45, 243, Direction.SouthWest); // Seed Master
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[453], 49, 243, Direction.SouthWest); // Seed Researcher
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[540], 49, 216, Direction.South); // Lugard
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[492], 22, 225, Direction.South); // Moss
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[579], 20, 214, Direction.SouthEast); // David
        yield return this.CreateMonsterSpawn(15, this.NpcDictionary[568], 55, 199, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 418;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.StrangeRabbit);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 4 },
                { Stats.MaximumHealth, 60 },
                { Stats.MinimumPhysBaseDmg, 10 },
                { Stats.MaximumPhysBaseDmg, 13 },
                { Stats.DefenseBase, 3 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 3 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 419;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.PollutedButterfly);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 13 },
                { Stats.MaximumHealth, 230 },
                { Stats.MinimumPhysBaseDmg, 37 },
                { Stats.MaximumPhysBaseDmg, 42 },
                { Stats.DefenseBase, 13 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 13 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 420;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.HideousRabbit);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 19 },
                { Stats.MaximumHealth, 520 },
                { Stats.MinimumPhysBaseDmg, 68 },
                { Stats.MaximumPhysBaseDmg, 72 },
                { Stats.DefenseBase, 22 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 22 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 421;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Werewolf421);
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 24 },
                { Stats.MaximumHealth, 720 },
                { Stats.MinimumPhysBaseDmg, 85 },
                { Stats.MaximumPhysBaseDmg, 90 },
                { Stats.DefenseBase, 30 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 30 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 422;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.CursedLich);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Meteorite);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 30 },
                { Stats.MaximumHealth, 900 },
                { Stats.MinimumPhysBaseDmg, 105 },
                { Stats.MaximumPhysBaseDmg, 110 },
                { Stats.DefenseBase, 33 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 33 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.WaterResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 423;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.TotemGolem);
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 36 },
                { Stats.MaximumHealth, 1200 },
                { Stats.MinimumPhysBaseDmg, 120 },
                { Stats.MaximumPhysBaseDmg, 125 },
                { Stats.DefenseBase, 50 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 50 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 424;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.Grizzly);
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 43 },
                { Stats.MaximumHealth, 2400 },
                { Stats.MinimumPhysBaseDmg, 130 },
                { Stats.MaximumPhysBaseDmg, 145 },
                { Stats.DefenseBase, 65 },
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 65 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 425;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.CaptainGrizzly);
            monster.MoveRange = 2;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
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
                { Stats.AttackRatePvm, 10 },
                { Stats.DefenseRatePvm, 70 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }
    }
}