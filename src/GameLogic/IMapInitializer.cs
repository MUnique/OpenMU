// <copyright file="IMapInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// An interface for a map initializer which is responsible to create new instances of <see cref="GameMap"/>s
/// and it's initialization.
/// </summary>
public interface IMapInitializer
{
    /// <summary>
    /// Creates a new game map instance of the specified game map number.
    /// </summary>
    /// <param name="mapNumber">The map number.</param>
    /// <returns>The new game map instance.</returns>
    GameMap? CreateGameMap(ushort mapNumber);

    /// <summary>
    /// Creates a new game map instance with the specified definition.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    /// <returns>The new game map instance.</returns>
    GameMap CreateGameMap(GameMapDefinition mapDefinition);

    /// <summary>
    /// Initializes the state of the previously created game map (e.g. by creating NPC instances).
    /// </summary>
    /// <param name="createdMap">The created map.</param>
    ValueTask InitializeStateAsync(GameMap createdMap);

    /// <summary>
    /// Initializes the spawn on the map.
    /// </summary>
    /// <param name="gameMap">The game map on which the spawn should be initialized.</param>
    /// <param name="spawnArea">The spawn area.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    ValueTask<NonPlayerCharacter?> InitializeSpawnAsync(int spawnIndex, GameMap gameMap, MonsterSpawnArea spawnArea, IEventStateProvider? eventStateProvider = null, IDropGenerator? dropGenerator = null);

    /// <summary>
    /// Initializes the event NPCs of the previously created game map.
    /// </summary>
    /// <param name="createdMap">The created map.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    ValueTask InitializeNpcsOnEventStartAsync(GameMap createdMap, IEventStateProvider eventStateProvider);

    /// <summary>
    /// Initializes the event NPCs of the previously created game map after the spawn waves started.
    /// </summary>
    /// <param name="createdMap">The created map.</param>
    /// <param name="eventStateProvider">The event state provider.</param>
    /// <param name="waveNumber">The number of the started spawn wave.</param>
    ValueTask InitializeNpcsOnWaveStartAsync(GameMap createdMap, IEventStateProvider eventStateProvider, byte waveNumber);
}