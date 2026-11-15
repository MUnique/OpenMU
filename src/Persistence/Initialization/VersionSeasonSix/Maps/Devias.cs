// <copyright file="Devias.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Devias map.
/// </summary>
internal class Devias : Version095d.Maps.Devias
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Devias"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Devias(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the automatic monster spawns of the Season 6 layout of Devias.
    /// </summary>
    /// <remarks>
    /// The layout places the monsters in smaller areas and at single spots, instead of spreading them
    /// over quarters of the map like the data of version 0.75.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInSeason6"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(200, 24, 170, 245, 80, 200, 30), // Worm
        new(201, 23, 170, 245, 80, 200, 10), // Hommerd
        new(202, 22, 50, 166, 6, 244, 40), // Ice Monster
        new(203, 23, 50, 166, 6, 244, 30), // Hommerd
        new(204, 19, 50, 166, 6, 244, 40), // Yeti
        new(205, 21, 50, 166, 6, 244, 30), // Assassin
        new(206, 20, 50, 166, 6, 244, 20), // Elite Yeti
        new(207, 20, 5, 50, 50, 155, 20), // Elite Yeti
        new(208, 20, 6, 80, 184, 244, 10), // Elite Yeti
        new(209, 25, 6, 80, 184, 244, 3), // Ice Queen
        new(210, 25, 74, 74, 239, 239, 1), // Ice Queen
        new(211, 25, 70, 70, 212, 212, 1), // Ice Queen
        new(212, 25, 45, 45, 189, 189, 1), // Ice Queen
        new(213, 25, 113, 113, 230, 230, 1), // Ice Queen
        new(214, 25, 94, 94, 202, 202, 1), // Ice Queen, moved from (94, 203), where no monster can spawn
        new(215, 25, 63, 63, 160, 160, 1), // Ice Queen
        new(216, 20, 203, 203, 244, 244, 1), // Elite Yeti
        new(217, 20, 194, 194, 241, 241, 1), // Elite Yeti
        new(218, 20, 203, 203, 232, 232, 1), // Elite Yeti
        new(219, 20, 210, 210, 223, 223, 1), // Elite Yeti, moved from (209, 224), where no monster can spawn
        new(220, 20, 186, 186, 224, 224, 1), // Elite Yeti
        new(221, 20, 200, 200, 171, 171, 1), // Elite Yeti
        new(222, 20, 207, 207, 158, 158, 1), // Elite Yeti
        new(223, 20, 221, 221, 177, 177, 1), // Elite Yeti
        new(224, 20, 213, 213, 193, 193, 1), // Elite Yeti
        new(225, 20, 196, 196, 218, 218, 1), // Elite Yeti
        new(226, 20, 230, 230, 200, 200, 1), // Elite Yeti
        new(227, 20, 221, 221, 160, 160, 1), // Elite Yeti
        new(228, 20, 205, 205, 149, 149, 1), // Elite Yeti
        new(229, 20, 185, 185, 165, 165, 1), // Elite Yeti, moved from (184, 166), where no monster can spawn
        new(230, 20, 212, 212, 170, 170, 1), // Elite Yeti
        new(231, 25, 116, 116, 210, 210, 1), // Ice Queen
        new(232, 20, 203, 203, 201, 201, 1), // Elite Yeti
        new(233, 24, 170, 170, 6, 6, 1), // Worm
        new(234, 24, 161, 161, 58, 58, 1), // Worm
        new(235, 24, 180, 180, 78, 78, 1), // Worm
        new(236, 20, 190, 190, 146, 146, 1), // Elite Yeti
        new(237, 25, 51, 51, 227, 227, 1), // Ice Queen
        new(238, 20, 189, 189, 160, 160, 1), // Elite Yeti
        new(239, 20, 207, 207, 183, 183, 1), // Elite Yeti
        new(240, 20, 218, 218, 201, 201, 1), // Elite Yeti
        new(241, 20, 238, 238, 209, 209, 1), // Elite Yeti, moved from (240, 207), where no monster can spawn
        new(242, 20, 238, 238, 184, 184, 1), // Elite Yeti
        new(243, 20, 227, 227, 167, 167, 1), // Elite Yeti
        new(244, 20, 202, 202, 209, 209, 1), // Elite Yeti
        new(245, 20, 113, 113, 105, 105, 1), // Elite Yeti
        new(246, 20, 96, 96, 103, 103, 1), // Elite Yeti
        new(247, 20, 102, 102, 117, 117, 1), // Elite Yeti
        new(248, 20, 241, 241, 198, 198, 1), // Elite Yeti
        new(249, 20, 228, 228, 184, 184, 1), // Elite Yeti
        new(250, 20, 193, 193, 231, 231, 1), // Elite Yeti
        new(251, 20, 214, 214, 152, 152, 1), // Elite Yeti
        new(252, 20, 197, 197, 183, 183, 1), // Elite Yeti
        new(253, 24, 162, 162, 11, 11, 1), // Worm
        new(254, 24, 244, 244, 80, 80, 1), // Worm
        new(255, 24, 238, 238, 88, 88, 1), // Worm
        new(256, 24, 229, 229, 83, 83, 1), // Worm
    ];

    /// <inheritdoc/>
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        foreach (var npc in base.CreateNpcSpawns())
        {
            yield return npc;
        }

        yield return this.CreateMonsterSpawn(30, this.NpcDictionary[540], 233, 66, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(31, this.NpcDictionary[406], 181, 35, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(32, this.NpcDictionary[385], 197, 53, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(33, this.NpcDictionary[478], 191, 17, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(34, this.NpcDictionary[256], 193, 13, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(35, this.NpcDictionary[522], 229, 221, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(36, this.NpcDictionary[257], 219, 76, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(37, this.NpcDictionary[566], 204, 61, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(38, this.NpcDictionary[379], 13, 28, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(39, this.NpcDictionary[229], 197, 48, Direction.SouthEast, SpawnTrigger.Wandering); // Marlon
        yield return this.CreateMonsterSpawn(40, this.NpcDictionary[233], 217, 29, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(41, this.NpcDictionary[233], 217, 20, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(42, this.NpcDictionary[568], 225, 52, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }
}