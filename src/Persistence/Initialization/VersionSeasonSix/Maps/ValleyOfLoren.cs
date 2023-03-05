// <copyright file="ValleyOfLoren.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Valley of Loren map.
/// </summary>
internal class ValleyOfLoren : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 30;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Valley of Loren";

    /// <summary>
    /// Initializes a new instance of the <see cref="ValleyOfLoren"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ValleyOfLoren(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[216], 176, 212, Direction.SouthWest); // Crown
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[216], 176, 212, Direction.SouthWest); // Crown
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[217], 167, 194, Direction.NorthWest); // Crown Switch1
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[218], 184, 195, Direction.NorthWest); // Crown Switch2
        yield return this.CreateMonsterSpawn(10, this.NpcDictionary[220], 139, 101, Direction.SouthEast); // Guard
        yield return this.CreateMonsterSpawn(11, this.NpcDictionary[223], 179, 214, Direction.SouthWest); // Sinior
        yield return this.CreateMonsterSpawn(12, this.NpcDictionary[224], 086, 061, Direction.SouthWest); // Guardsman
        yield return this.CreateMonsterSpawn(13, this.NpcDictionary[224], 099, 061, Direction.SouthWest); // Guardsman
        yield return this.CreateMonsterSpawn(20, this.NpcDictionary[376], 090, 043, Direction.SouthWest); // Pamela
        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[377], 090, 218, Direction.SouthEast); // Angela
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters here
    }
}