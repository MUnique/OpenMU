// <copyright file="MiniGamePlayerRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using Nito.AsyncEx;

/// <summary>
/// Thread-safe registry of the players which entered a mini game.
/// It owns the player collection and its lock, the entering policy
/// (the game must be open and must not be full), and the game state itself:
/// every state transition goes through <see cref="SetStateAsync"/>, so a player
/// can never be added while the entrance is closing, starting, or disposing.
/// </summary>
internal sealed class MiniGamePlayerRegistry
{
    private readonly MiniGameDefinition _definition;
    private readonly AsyncReaderWriterLock _lock = new();
    private readonly HashSet<Player> _players = new();

    // Lock-free read: correctness only needs check-and-add inside TryEnterAsync to be
    // atomic, and that stays atomic as long as every write goes through SetStateAsync.
    // Reads (e.g. every State access on hot paths) must not block on a lock that is
    // held across I/O elsewhere.
    private volatile MiniGameState _state = MiniGameState.Open;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGamePlayerRegistry"/> class.
    /// </summary>
    /// <param name="definition">The definition of the mini game.</param>
    public MiniGamePlayerRegistry(MiniGameDefinition definition)
    {
        this._definition = definition;
    }

    /// <summary>
    /// Gets the current state of the mini game.
    /// </summary>
    public MiniGameState State => this._state;

    /// <summary>
    /// Sets the state of the mini game. All state transitions (closing, starting,
    /// ending, disposing) go through here, so they are atomic with player entering.
    /// </summary>
    /// <param name="state">The new state.</param>
    public async ValueTask SetStateAsync(MiniGameState state)
    {
        using (await this._lock.WriterLockAsync().ConfigureAwait(false))
        {
            this._state = state;
        }
    }

    /// <summary>
    /// Tries to enter the mini game. It will fail, if it's full, or if it's not in an open state.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <param name="areEquippedItemsAllowedAsync">A function which checks if the equipped items of the player are allowed.</param>
    /// <returns>A value indicating whether entering had success.</returns>
    public async ValueTask<EnterResult> TryEnterAsync(Player player, Func<Player, ValueTask<bool>> areEquippedItemsAllowedAsync)
    {
        using (await this._lock.WriterLockAsync().ConfigureAwait(false))
        {
            if (this._state != MiniGameState.Open)
            {
                return EnterResult.NotOpen;
            }

            if (this._players.Count >= this._definition.MaximumPlayerCount)
            {
                return EnterResult.Full;
            }

            if (!await areEquippedItemsAllowedAsync(player).ConfigureAwait(false))
            {
                return EnterResult.Failed;
            }

            this._players.Add(player);
        }

        return EnterResult.Success;
    }

    /// <summary>
    /// Removes the player from the registry.
    /// </summary>
    /// <param name="player">The player to remove.</param>
    /// <returns>The number of remaining players.</returns>
    public async ValueTask<int> RemoveAsync(Player player)
    {
        using (await this._lock.WriterLockAsync().ConfigureAwait(false))
        {
            this._players.Remove(player);
            return this._players.Count;
        }
    }

    /// <summary>
    /// Removes all players from the registry and returns them.
    /// </summary>
    /// <returns>The players which had been registered.</returns>
    public async ValueTask<List<Player>> ClearAsync()
    {
        using (await this._lock.WriterLockAsync().ConfigureAwait(false))
        {
            var players = this._players.ToList();
            this._players.Clear();
            return players;
        }
    }

    /// <summary>
    /// Gets a snapshot of the currently registered players.
    /// </summary>
    /// <returns>The currently registered players.</returns>
    public async ValueTask<List<Player>> GetSnapshotAsync()
    {
        using (await this._lock.ReaderLockAsync().ConfigureAwait(false))
        {
            return this._players.ToList();
        }
    }

    /// <summary>
    /// Counts the registered players which are alive.
    /// </summary>
    /// <returns>The number of alive players.</returns>
    public int CountAlive()
    {
        using (this._lock.ReaderLock())
        {
            return this._players.Count(p => p.IsAlive);
        }
    }

    /// <summary>
    /// Executes the action for each registered player.
    /// </summary>
    /// <param name="playerAction">The action which should be executed for each player.</param>
    public async ValueTask ForEachAsync(Func<Player, Task> playerAction)
    {
        using (await this._lock.ReaderLockAsync().ConfigureAwait(false))
        {
            await this._players.Select(playerAction).WhenAll().ConfigureAwait(false);
        }
    }
}
