// <copyright file="MiniGameManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Diagnostics.Metrics;
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
    private readonly Dictionary<MiniGameMapKey, MiniGameContext> _miniGames = new();
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
                if (miniGameContext.IsDisposed)
                {
                    this._miniGames.Remove(miniGameKey);
                }
                else
                {
                    return miniGameContext;
                }
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
                case MiniGameType.Kanturu:
                    miniGameContext = new KanturuContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
                default:
                    miniGameContext = new MiniGameContext(miniGameKey, miniGameDefinition, this._gameContext, this._mapInitializer);
                    break;
            }

            this._miniGames.Add(miniGameKey, miniGameContext);
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
        MiniGameCounter.Add(-1);
        miniGameContext.Dispose();
        this._miniGames.Remove(miniGameContext.Key);
        this.GameMapRemoved?.Invoke(this, miniGameContext.Map);
    }
}
