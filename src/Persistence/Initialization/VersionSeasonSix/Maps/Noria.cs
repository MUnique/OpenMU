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
}