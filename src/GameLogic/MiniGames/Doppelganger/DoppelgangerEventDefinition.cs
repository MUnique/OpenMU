// <copyright file="DoppelgangerEventDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Describes the run of the doppelganger event.
/// </summary>
public class DoppelgangerEventDefinition
{
    /// <summary>
    /// Gets or sets the number of monsters which may reach the magic circle until the event fails.
    /// </summary>
    public int MaximumGoalCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the interval in which the remaining time and the positions of the players
    /// are sent to the clients. The client doesn't count down the time by itself.
    /// </summary>
    public TimeSpan PlayInfoInterval { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the interval in which a herd of monsters spawns at the start of the path.
    /// </summary>
    public TimeSpan HerdInterval { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the numbers of the monsters of which a herd consists. Each monster
    /// of a herd is chosen randomly.
    /// </summary>
    public IList<short> HerdMonsterNumbers { get; set; } = new List<short>();

    /// <summary>
    /// Gets or sets the base count of monsters of a herd, depending on the elapsed game time.
    /// One monster is added for each player in the event, starting with the second one.
    /// </summary>
    public IList<DoppelgangerHerdSize> HerdSizes { get; set; } = new List<DoppelgangerHerdSize>();

    /// <summary>
    /// Gets or sets the chance (between 0 and 1) that a monster of a herd attacks players in its range,
    /// without being attacked first.
    /// </summary>
    public double AttackFirstChance { get; set; } = 0.7;

    /// <summary>
    /// Gets or sets the numbers of the monsters which always attack players in their range.
    /// </summary>
    public IList<short> AlwaysAttackingMonsterNumbers { get; set; } = new List<short>();

    /// <summary>
    /// Gets or sets the additional monsters which spawn once at the start of the path at a specific time.
    /// </summary>
    public IList<DoppelgangerMonsterSpawn> AdditionalSpawns { get; set; } = new List<DoppelgangerMonsterSpawn>();

    /// <summary>
    /// Gets or sets the number of the ice walker monster.
    /// </summary>
    public short IceWalkerNumber { get; set; } = 531;

    /// <summary>
    /// Gets or sets the elapsed game time after which the ice walkers appear. One ice walker
    /// appears for each player in the event. They stay at a random position on the path.
    /// </summary>
    public TimeSpan IceWalkerSpawnTime { get; set; } = TimeSpan.FromMinutes(6).Subtract(TimeSpan.FromSeconds(5));

    /// <summary>
    /// Gets or sets the time in which the players have to kill the ice walkers. Otherwise, they disappear.
    /// </summary>
    public TimeSpan IceWalkerMissionDuration { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the lowest position index on the path at which an ice walker appears.
    /// </summary>
    public int IceWalkerMinimumPosition { get; set; } = 3;

    /// <summary>
    /// Gets or sets the highest position index on the path at which an ice walker appears.
    /// </summary>
    public int IceWalkerMaximumPosition { get; set; } = 18;

    /// <summary>
    /// Gets or sets the paths of the monsters, one for each event map.
    /// </summary>
    public IList<DoppelgangerPath> Paths { get; set; } = new List<DoppelgangerPath>();

    /// <summary>
    /// Creates the definition of the original season 6 event.
    /// </summary>
    /// <returns>The created definition.</returns>
    public static DoppelgangerEventDefinition CreateDefault()
    {
        const short terribleButcher = 529;
        const short madButcher = 530;
        const short doppelganger = 533;
        const short doppelgangerDarkLord = 538;

        return new DoppelgangerEventDefinition
        {
            // The dark lord clone (538) only appears as additional spawn.
            HerdMonsterNumbers = [doppelganger, 534, 535, 536, 537, 539],
            AlwaysAttackingMonsterNumbers = [doppelganger],
            HerdSizes =
            [
                new DoppelgangerHerdSize { StartsAfter = TimeSpan.Zero, BaseCount = 1 },
                new DoppelgangerHerdSize { StartsAfter = TimeSpan.FromMinutes(3), BaseCount = 2 },
                new DoppelgangerHerdSize { StartsAfter = TimeSpan.FromMinutes(6), BaseCount = 3 },
            ],
            AdditionalSpawns =
            [
                new DoppelgangerMonsterSpawn { SpawnTime = TimeSpan.FromMinutes(1), MonsterNumbers = [madButcher, doppelgangerDarkLord] },
                new DoppelgangerMonsterSpawn { SpawnTime = TimeSpan.FromMinutes(4), MonsterNumbers = [madButcher, doppelgangerDarkLord] },
                new DoppelgangerMonsterSpawn { SpawnTime = TimeSpan.FromMinutes(7), MonsterNumbers = [terribleButcher, doppelgangerDarkLord] },
            ],
            Paths = CreateDefaultPaths(),
        };
    }

    /// <summary>
    /// Gets the base count of monsters of a herd after the specified elapsed game time.
    /// </summary>
    /// <param name="elapsed">The elapsed game time.</param>
    /// <returns>The base count of monsters of a herd.</returns>
    public int GetHerdBaseCount(TimeSpan elapsed)
    {
        return this.HerdSizes
            .Where(size => size.StartsAfter <= elapsed)
            .MaxBy(size => size.StartsAfter)?.BaseCount ?? 0;
    }

    /// <summary>
    /// Gets the position index of the point on the path of the specified map.
    /// </summary>
    /// <param name="mapNumber">The map number.</param>
    /// <param name="point">The point.</param>
    /// <returns>
    /// The index of the first area of the path which contains the point, or 0 when there is
    /// no such area. The index 0 is the start of the monsters and the last index is the magic circle.
    /// </returns>
    public int GetPathPosition(short mapNumber, Point point)
    {
        if (this.Paths.FirstOrDefault(path => path.MapNumber == mapNumber) is not { } path)
        {
            return 0;
        }

        for (var i = 0; i < path.Areas.Count; i++)
        {
            if (path.Areas[i].Contains(point))
            {
                return i;
            }
        }

        return 0;
    }

    /// <summary>
    /// Creates the paths of the original season 6 event. Each path consists of 23 areas,
    /// from the start of the monsters (index 0) to the magic circle (index 22), which is
    /// located next to the entrance of the players.
    /// </summary>
    private static IList<DoppelgangerPath> CreateDefaultPaths()
    {
        return
        [
            // Doppelganger 1 (Ice), map 65
            new DoppelgangerPath
            {
                MapNumber = 65,
                Areas =
                [
                    new(220, 99, 230, 107),
                    new(218, 95, 230, 100),
                    new(217, 91, 225, 96),
                    new(217, 87, 224, 92),
                    new(217, 83, 224, 88),
                    new(217, 79, 224, 84),
                    new(217, 75, 224, 80),
                    new(215, 72, 225, 76),
                    new(212, 70, 224, 73),
                    new(209, 67, 222, 71),
                    new(206, 64, 218, 68),
                    new(202, 62, 216, 65),
                    new(200, 59, 213, 63),
                    new(196, 57, 210, 60),
                    new(193, 55, 208, 58),
                    new(192, 51, 203, 56),
                    new(193, 48, 201, 52),
                    new(193, 44, 201, 49),
                    new(193, 40, 201, 45),
                    new(193, 37, 201, 41),
                    new(194, 34, 201, 38),
                    new(192, 30, 202, 35),
                    new(192, 24, 202, 31),
                ],
            },
            // Doppelganger 2 (Fire), map 66
            new DoppelgangerPath
            {
                MapNumber = 66,
                Areas =
                [
                    new(108, 179, 119, 186),
                    new(108, 174, 119, 180),
                    new(109, 169, 117, 175),
                    new(109, 163, 117, 170),
                    new(109, 158, 117, 164),
                    new(109, 153, 117, 159),
                    new(109, 147, 118, 154),
                    new(109, 142, 119, 148),
                    new(110, 137, 120, 143),
                    new(112, 132, 122, 138),
                    new(114, 127, 126, 133),
                    new(116, 123, 131, 128),
                    new(121, 119, 133, 124),
                    new(124, 114, 137, 120),
                    new(127, 109, 139, 115),
                    new(130, 104, 140, 110),
                    new(132, 98, 141, 105),
                    new(133, 93, 141, 99),
                    new(133, 88, 141, 94),
                    new(133, 83, 141, 89),
                    new(133, 78, 141, 84),
                    new(133, 74, 143, 79),
                    new(133, 66, 143, 75),
                ],
            },
            // Doppelganger 3 (Water), map 67
            new DoppelgangerPath
            {
                MapNumber = 67,
                Areas =
                [
                    new(107, 149, 114, 155),
                    new(105, 144, 114, 150),
                    new(105, 140, 113, 145),
                    new(105, 135, 113, 141),
                    new(105, 131, 113, 136),
                    new(105, 127, 113, 132),
                    new(105, 123, 114, 128),
                    new(105, 119, 113, 124),
                    new(105, 115, 113, 120),
                    new(105, 111, 113, 116),
                    new(105, 107, 113, 112),
                    new(105, 103, 113, 108),
                    new(105, 99, 113, 104),
                    new(105, 95, 113, 100),
                    new(105, 91, 113, 96),
                    new(105, 86, 113, 92),
                    new(105, 82, 113, 87),
                    new(105, 78, 113, 83),
                    new(105, 74, 113, 79),
                    new(105, 70, 113, 75),
                    new(105, 66, 113, 71),
                    new(104, 62, 115, 67),
                    new(104, 55, 115, 63),
                ],
            },
            // Doppelganger 4 (Ground), map 68
            new DoppelgangerPath
            {
                MapNumber = 68,
                Areas =
                [
                    new(38, 105, 48, 113),
                    new(38, 99, 47, 106),
                    new(38, 94, 45, 100),
                    new(38, 89, 45, 95),
                    new(34, 84, 51, 90),
                    new(40, 80, 54, 85),
                    new(44, 76, 61, 81),
                    new(48, 71, 65, 77),
                    new(53, 66, 65, 72),
                    new(57, 62, 67, 67),
                    new(59, 57, 66, 63),
                    new(59, 50, 67, 58),
                    new(66, 50, 72, 57),
                    new(71, 49, 77, 57),
                    new(76, 49, 84, 57),
                    new(83, 50, 89, 57),
                    new(88, 46, 98, 57),
                    new(90, 41, 99, 47),
                    new(91, 35, 98, 42),
                    new(91, 29, 98, 36),
                    new(91, 23, 99, 30),
                    new(91, 19, 101, 24),
                    new(91, 10, 101, 20),
                ],
            },
        ];
    }
}
