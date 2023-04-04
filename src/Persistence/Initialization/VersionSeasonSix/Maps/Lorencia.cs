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
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        foreach (var npc in base.CreateNpcSpawns())
        {
            yield return npc;
        }

        yield return this.CreateMonsterSpawn(16, this.NpcDictionary[230], 62, 130, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(17, this.NpcDictionary[226], 122, 110, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(18, this.NpcDictionary[257], 96, 129, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(19, this.NpcDictionary[257], 174, 129, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(20, this.NpcDictionary[257], 130, 128, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[257], 132, 165, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(22, this.NpcDictionary[229], 136, 88, Direction.SouthWest, SpawnTrigger.Wandering); // Marlon
        yield return this.CreateMonsterSpawn(23, this.NpcDictionary[375], 132, 161, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(24, this.NpcDictionary[543], 141, 143, Direction.South);
        yield return this.CreateMonsterSpawn(25, this.NpcDictionary[371], 130, 126, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(26, this.NpcDictionary[568], 131, 139, Direction.South, SpawnTrigger.Wandering); // Wandering Merchant Zyro
    }
}