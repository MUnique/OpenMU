// <copyright file="MiniGameManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using Nito.AsyncEx;

/// <summary>
/// Hosts and tracks the mini game instances of a game context.
/// </summary>
public sealed class MiniGameManager : IMiniGameManager
{
    private static readonly Meter Meter = new(GameContext.MeterName);

    private static readonly Counter<int> MiniGameCounter = Meter.CreateCounter<int>("MiniGameCount");

    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly ConcurrentDictionary<MiniGameMapKey, MiniGameContext> _miniGames = new();
    private readonly AsyncLock _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameManager"/> class.
    /// </summary>
    /// <param name="gameContext">The game context to which the hosted games belong.</param>
    /// <param name="mapInitializer">The map initializer.</param>
    public MiniGameManager(IGameContext gameContext, IMapInitializer mapInitializer)
    {
        this._gameContext = gameContext;
        this._mapInitializer = mapInitializer;
    }

    /// <inheritdoc />
    public event EventHandler<GameMap>? GameMapCreated;

    /// <inheritdoc />
    public event EventHandler<GameMap>? GameMapRemoved;

    /// <inheritdoc />
    public IReadOnlyList<GameMap> Maps => this._miniGames.Values.Select(game => game.Map).ToList();

    /// <inheritdoc />
    public async ValueTask<MiniGameContext> GetOrCreateAsync(MiniGameDefinition miniGameDefinition, Player requester)
    {
        var miniGameKey = MiniGameMapKey.Create(miniGameDefinition, requester);

        if (this._miniGames.TryGetValue(miniGameKey, out var miniGameContext) && miniGameContext is { IsDisposed: false, IsDisposing: false })
        {
            return miniGameContext;
        }

        using (await this._lock.LockAsync().ConfigureAwait(false))
        {
            if (this._miniGames.TryGetValue(miniGameKey, out miniGameContext))
            {
                if (miniGameContext is { IsDisposed: false, IsDisposing: false })
                {
                    return miniGameContext;
                }

                // A dead entry must not be handed out, and it must not linger either:
                // RemoveAsync of the dying instance would otherwise find a different
                // (or no) entry under the key and skew the game counter.
                this._miniGames.TryRemove(miniGameKey, out _);
                MiniGameCounter.Add(-1);
            }

            switch (miniGameDefinition.Type)
            {
                case MiniGameType.ChaosCastle:
                    miniGameContext = new ChaosCastleContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                case MiniGameType.DevilSquare:
                    miniGameContext = new DevilSquareContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                case MiniGameType.BloodCastle:
                    miniGameContext = new BloodCastleContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                case MiniGameType.Doppelganger:
                    miniGameContext = new DoppelgangerContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                case MiniGameType.Kanturu:
                    miniGameContext = new KanturuContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                default:
                    miniGameContext = new MiniGameContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
            }

            this._miniGames[miniGameKey] = miniGameContext;
        }

        var createdMap = miniGameContext.Map;

        // ReSharper disable once InconsistentlySynchronizedField it's desired behavior to initialize the map outside the lock to keep locked timespan short.
        await this._mapInitializer.InitializeStateAsync(createdMap).ConfigureAwait(false);
        this.GameMapCreated?.Invoke(this, createdMap);
        MiniGameCounter.Add(1);
        return miniGameContext;
    }

    /// <inheritdoc />
    public MiniGameContext? TryGetRunningMiniGame(MiniGameDefinition miniGameDefinition, Player? requester)
    {
        if (requester is null && miniGameDefinition.MapCreationPolicy != MiniGameMapCreationPolicy.Shared)
        {
            // The key of per-player and per-party games is derived from the requester.
            return null;
        }

        var miniGameKey = MiniGameMapKey.Create(miniGameDefinition, requester!);
        if (this._miniGames.TryGetValue(miniGameKey, out var miniGameContext)
            && miniGameContext is { IsDisposed: false, IsDisposing: false })
        {
            return miniGameContext;
        }

        return null;
    }

    /// <inheritdoc />
    public IReadOnlyList<MiniGameContext> GetRunningMiniGames(MiniGameType miniGameType)
    {
        return this._miniGames.Values
            .Where(game => game is { IsDisposed: false, IsDisposing: false } && game.Definition.Type == miniGameType)
            .ToList();
    }

    /// <inheritdoc />
    public async ValueTask RemoveAsync(MiniGameContext miniGameContext)
    {
        using var l = await this._lock.LockAsync().ConfigureAwait(false);
        miniGameContext.Dispose();

        // Only unregister what is actually there: a forced restart may already
        // have replaced the dying instance, and must not lose the new one.
        // The removal event still fires unconditionally: it's keyed by the map
        // instance (GameMap.Id), so it can never hit the replacement's entry,
        // while skipping it would leave a stale map in observers.
        if (this._miniGames.TryGetValue(miniGameContext.Key, out var current)
            && ReferenceEquals(current, miniGameContext)
            && this._miniGames.TryRemove(miniGameContext.Key, out _))
        {
            MiniGameCounter.Add(-1);
        }

        this.GameMapRemoved?.Invoke(this, miniGameContext.Map);
    }
}
