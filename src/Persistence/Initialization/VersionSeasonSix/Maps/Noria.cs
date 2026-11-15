// <copyright file="Noria.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Noria map.
/// </summary>
internal class Noria : Version095d.Maps.Noria
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Noria"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Noria(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the automatic monster spawns of the Season 6 layout of Noria.
    /// </summary>
    /// <remarks>
    /// The layout places the monsters in smaller areas instead of spreading them over quarters of the map
    /// like the data of version 0.75. It includes the spiders, which the first hunting quest of Season 6 needs.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInSeason6"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(200, 3, 139, 247, 63, 164, 60), // Spider
        new(201, 26, 109, 154, 46, 181, 10), // Goblin
        new(202, 26, 136, 247, 156, 204, 10), // Goblin
        new(203, 26, 135, 247, 42, 73, 10), // Goblin
        new(204, 27, 109, 154, 46, 181, 20), // Chain Scorpion
        new(205, 27, 135, 247, 42, 73, 20), // Chain Scorpion
        new(206, 27, 136, 247, 156, 204, 20), // Chain Scorpion
        new(207, 33, 106, 247, 3, 40, 20), // Elite Goblin
        new(208, 33, 59, 128, 6, 106, 20), // Elite Goblin
        new(209, 28, 106, 247, 3, 40, 20), // Beetle Monster
        new(210, 28, 59, 128, 6, 106, 20), // Beetle Monster
        new(211, 29, 106, 247, 3, 40, 20), // Hunter
        new(212, 29, 59, 128, 6, 106, 20), // Hunter
        new(213, 30, 7, 63, 4, 176, 10), // Forest Monster
        new(214, 30, 41, 107, 107, 181, 10), // Forest Monster
        new(215, 30, 105, 247, 184, 246, 10), // Forest Monster
        new(216, 31, 7, 63, 4, 176, 20), // Agon
        new(217, 31, 41, 107, 107, 181, 20), // Agon
        new(218, 31, 105, 247, 184, 246, 20), // Agon
        new(219, 32, 4, 115, 168, 248, 40), // Stone Golem
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

        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[544], 187, 125, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[257], 167, 118, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[450], 179, 126, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[451], 179, 129, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[229], 169, 88, Direction.SouthEast, SpawnTrigger.Wandering); // Marlon
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }
}