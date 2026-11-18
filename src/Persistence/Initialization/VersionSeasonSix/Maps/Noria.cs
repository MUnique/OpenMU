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
    /// Gets the automatic monster spawns of Noria.
    /// </summary>
    /// <remarks>
    /// The layout of <see cref="Version075.Maps.Noria.MonsterSpawnLayout"/> plus the spiders, which Season 6
    /// added for its first hunting quest.
    /// It's shared with <see cref="Updates.FieldMonsterSpawnsUpdatePlugInSeason6"/>, which brings this layout to existing databases.
    /// </remarks>
    internal static new IReadOnlyList<FieldMonsterSpawn> MonsterSpawnLayout { get; } =
    [
        new(200, 3, 139, 247, 63, 164, 60), // Spider
        .. Version075.Maps.Noria.MonsterSpawnLayout,
    ];

    /// <inheritdoc/>
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc/>
    /// <remarks>
    /// Doesn't take the NPCs of the older versions as they are: Season 6 has no Potion Girls in Noria
    /// (spawns 1 and 2 of 0.75), so the other NPCs of the older versions are listed here with their numbers.
    /// </remarks>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[242], 173, 125, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[243], 195, 124, Direction.South);
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[240], 172, 96, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[238], 180, 103, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[237], 171, 105, Direction.SouthEast); // Charon (Devil Square)
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[544], 167, 100, Direction.South);
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[257], 167, 118, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[450], 179, 126, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[451], 179, 129, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[229], 169, 88, Direction.SouthEast, SpawnTrigger.Wandering); // Marlon
        yield return this.CreateMonsterSpawn(15, this.NpcDictionary[257], 166, 85, Direction.South);
        yield return this.CreateMonsterSpawn(16, this.NpcDictionary[257], 201, 129, Direction.South);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return MonsterSpawnLayout.Select(spawn => this.CreateMonsterSpawn(spawn.Number, this.NpcDictionary[spawn.MonsterNumber], spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
    }
}