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

    /// <inheritdoc/>
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        foreach (var npc in base.CreateNpcSpawns())
        {
            yield return npc;
        }

        yield return this.CreateMonsterSpawn(33, this.NpcDictionary[478], 191, 17, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(34, this.NpcDictionary[256], 193, 13, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(35, this.NpcDictionary[522], 229, 221, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(36, this.NpcDictionary[257], 219, 73, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(37, this.NpcDictionary[566], 204, 61, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(38, this.NpcDictionary[379], 32, 31, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(39, this.NpcDictionary[229], 197, 48, Direction.SouthEast, SpawnTrigger.Wandering); // Marlon
        yield return this.CreateMonsterSpawn(40, this.NpcDictionary[233], 209, 30, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(41, this.NpcDictionary[233], 219, 9, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(42, this.NpcDictionary[568], 225, 52, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
        yield return this.CreateMonsterSpawn(43, this.NpcDictionary[257], 172, 44, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(44, this.NpcDictionary[236], 226, 66, Direction.SouthWest); // Golden Archer
    }
}