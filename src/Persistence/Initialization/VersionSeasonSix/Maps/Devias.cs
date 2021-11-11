﻿// <copyright file="Devias.cs" company="MUnique">
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

        yield return this.CreateMonsterSpawn(this.NpcDictionary[540], 233, 66, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[406], 181, 35, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[385], 197, 53, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[478], 191, 17, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[256], 193, 13, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[522], 229, 221, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[257], 219, 76, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[566], 204, 61, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[379], 13, 28, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[229], 183, 30, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[233], 217, 29, Direction.SouthEast);
        yield return this.CreateMonsterSpawn(this.NpcDictionary[233], 217, 20, Direction.SouthEast);
    }
}