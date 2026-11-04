// <copyright file="Atlans.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Atlans map.
/// </summary>
internal class Atlans : Version075.Maps.Atlans
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Atlans"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Atlans(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc />
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        foreach (var npc in base.CreateNpcSpawns())
        {
            yield return npc;
        }

        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[229], 17, 35, Direction.SouthEast, SpawnTrigger.Wandering); // Marlon
    }
}