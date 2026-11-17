// <copyright file="Lorencia.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Lorencia map.
/// </summary>
internal class Lorencia : Version095d.Maps.Lorencia
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Lorencia"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Lorencia(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc/>
    /// <remarks>
    /// Doesn't take the NPCs of the older versions as they are: Season 6 has no guard 247 at 114/125 (spawn 9 of 0.75)
    /// and has the Golden Archer in Devias instead of Lorencia (spawn 15 of 0.95d), so the other NPCs of the older
    /// versions are listed here with their numbers.
    /// </remarks>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[248], 6, 145, Direction.SouthEast, SpawnTrigger.Wandering);
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[240], 146, 110, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[240], 147, 145, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[249], 131, 88, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[249], 173, 125, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[249], 94, 125, Direction.NorthWest);
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[249], 94, 130, Direction.NorthWest);
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[249], 131, 148, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[250], 183, 137, Direction.South, SpawnTrigger.Wandering);
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[251], 116, 141, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[253], 127, 86, Direction.South);
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[254], 118, 113, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(14, this.NpcDictionary[255], 123, 135, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(16, this.NpcDictionary[230], 62, 130, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(17, this.NpcDictionary[226], 122, 110, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(18, this.NpcDictionary[257], 96, 129, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(19, this.NpcDictionary[257], 170, 129, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(20, this.NpcDictionary[257], 132, 90, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[257], 132, 165, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(22, this.NpcDictionary[229], 136, 88, Direction.SouthWest, SpawnTrigger.Wandering); // Marlon
        yield return this.CreateMonsterSpawn(23, this.NpcDictionary[375], 132, 161, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(24, this.NpcDictionary[543], 144, 127, Direction.South);
        yield return this.CreateMonsterSpawn(25, this.NpcDictionary[371], 130, 126, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(26, this.NpcDictionary[568], 131, 139, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
        yield return this.CreateMonsterSpawn(27, this.NpcDictionary[547], 139, 138, Direction.SouthEast); // Market Union Member Julia (warps to the Loren Market)
    }
}