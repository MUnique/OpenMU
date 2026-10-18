// <copyright file="IMiniGameManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Hosts and tracks the mini game instances of a game context.
/// </summary>
public interface IMiniGameManager
{
    /// <summary>
    /// Occurs when a mini game map got created.
    /// </summary>
    event EventHandler<GameMap>? GameMapCreated;

    /// <summary>
    /// Occurs when a mini game map got removed.
    /// </summary>
    event EventHandler<GameMap>? GameMapRemoved;

    /// <summary>
    /// Gets the maps of the currently hosted mini game instances.
    /// </summary>
    IReadOnlyList<GameMap> Maps { get; }

    /// <summary>
    /// Gets the mini game instance which is meant to be hosted by the game, creating it
    /// if it doesn't exist yet.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player.</param>
    /// <returns>The hosted mini game instance.</returns>
    ValueTask<MiniGameContext> GetOrCreateAsync(MiniGameDefinition miniGameDefinition, Player requester);

    /// <summary>
    /// Gets the currently running mini game instance for the given definition, if one exists.
    /// Unlike <see cref="GetOrCreateAsync"/>, this never starts a game as a side effect,
    /// so it's safe to call from read-only queries such as entrance checks.
    /// </summary>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <param name="requester">The requesting player; may be <c>null</c> for shared games.</param>
    /// <returns>The running mini game instance, if one exists; otherwise, <c>null</c>.</returns>
    MiniGameContext? TryGetRunningMiniGame(MiniGameDefinition miniGameDefinition, Player? requester);

    /// <summary>
    /// Gets a snapshot of the currently running mini game instances of the given type.
    /// </summary>
    /// <param name="miniGameType">The mini game type.</param>
    /// <returns>The running mini game instances of the given type.</returns>
    IReadOnlyList<MiniGameContext> GetRunningMiniGames(MiniGameType miniGameType);

    /// <summary>
    /// Tries to bring the player back into a running mini game, which its character
    /// left because the connection was lost.
    /// </summary>
    /// <param name="player">The player which entered the world.</param>
    /// <returns>A value indicating whether the player rejoined a mini game.</returns>
    ValueTask<bool> TryRejoinAsync(Player player);

    /// <summary>
    /// Removes the mini game instance from the game.
    /// </summary>
    /// <param name="miniGameContext">The context of the mini game.</param>
    ValueTask RemoveAsync(MiniGameContext miniGameContext);
}
