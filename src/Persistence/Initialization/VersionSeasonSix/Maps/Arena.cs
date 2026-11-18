// <copyright file="Arena.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Arena map.
/// </summary>
/// <remarks>
/// The map is the one of 0.75, with the NPCs of Season 6: the Arena Guard at 72/164, the soccer ball at 63/160
/// and no Safety Guardian.
/// </remarks>
internal class Arena : Version075.Maps.Arena
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arena"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Arena(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[200], 63, 63, 160, 160, 1, Direction.SouthWest);
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[239], 72, 72, 164, 164, 1, Direction.SouthWest);
    }
}
