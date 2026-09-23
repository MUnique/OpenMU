// <copyright file="DoppelgangerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The context of a doppelganger event game.
/// </summary>
/// <remarks>
/// The players defend a magic circle against monsters which walk along a path towards it.
/// The event fails for everyone when <see cref="DoppelgangerEventDefinition.MaximumGoalCount"/>
/// monsters reached the magic circle. It fails for a single player when the character
/// dies or leaves the event map, e.g. by warping or disconnecting.
/// When the game time is over and the defense didn't fail, the remaining players succeeded.
/// </remarks>
public sealed class DoppelgangerContext : MiniGameContext
{
    private readonly DoppelgangerEventDefinition _definition;

    /// <summary>
    /// The players for which the event already ended with a result, e.g. because they died.
    /// </summary>
    private readonly ConcurrentDictionary<Player, DoppelgangerResult> _playerResults = new();

    private DateTime _gameEndsAtUtc;
    private int _goalCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public DoppelgangerContext(
        MiniGameMapKey key,
        MiniGameDefinition definition,
        IGameContext gameContext,
        IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._definition = DoppelgangerEventDefinition.CreateDefault();
    }

    /// <summary>
    /// Gets a value indicating whether the defense failed, because too many monsters reached the magic circle.
    /// </summary>
    public bool IsDefenseFailed => Volatile.Read(ref this._goalCount) >= this._definition.MaximumGoalCount;

    /// <inheritdoc />
    protected override TimeSpan RemainingTime
    {
        get
        {
            var remaining = this._gameEndsAtUtc - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Registers a monster which reached the magic circle. When too many monsters
    /// reached it, the defense fails and the event ends.
    /// </summary>
    public async ValueTask RegisterMonsterReachedMagicCircleAsync()
    {
        var goalCount = Interlocked.Increment(ref this._goalCount);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowMonsterGoalAsync(goalCount, this._definition.MaximumGoalCount)).AsTask()).ConfigureAwait(false);

        if (goalCount == this._definition.MaximumGoalCount)
        {
            this.Logger.LogInformation("{context}: The defense failed, because {goalCount} monsters reached the magic circle.", this, goalCount);
            this.FinishEvent();
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        this._gameEndsAtUtc = DateTime.UtcNow.Add(this.Definition.GameDuration);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(async p =>
        {
            await p.ShowStateAsync(DoppelgangerState.Playing).ConfigureAwait(false);
            await p.ShowMonsterGoalAsync(0, this._definition.MaximumGoalCount).ConfigureAwait(false);
        }).AsTask()).ConfigureAwait(false);

        _ = Task.Run(() => this.RunPlayInfoLoopAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc />
#pragma warning disable VSTHRD100 // Avoid async void methods
    protected override async void OnPlayerDied(object? sender, DeathInformation e)
#pragma warning restore VSTHRD100
    {
        try
        {
            base.OnPlayerDied(sender, e);

            if (sender is Player player)
            {
                await this.FailPlayerAsync(player).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in OnPlayerDied.", this);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            // Leaving the map during the game, e.g. by warping or disconnecting, fails the event for the player.
            await this.FailPlayerAsync(player).ConfigureAwait(false);
        }

        await base.OnObjectRemovedFromMapAsync(args).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        var result = this.IsDefenseFailed ? DoppelgangerResult.MonstersReachedMagicCircle : DoppelgangerResult.Success;
        foreach (var player in finishers)
        {
            if (!this._playerResults.TryAdd(player, result))
            {
                // The player already got its result, e.g. because it died.
                continue;
            }

            await player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(async p =>
            {
                await p.ShowResultAsync(result).ConfigureAwait(false);
                await p.ShowStateAsync(DoppelgangerState.Ended).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    /// <summary>
    /// Fails the event for the player, if the game is running and it didn't get a result yet.
    /// </summary>
    /// <param name="player">The player.</param>
    private async ValueTask FailPlayerAsync(Player player)
    {
        if (this.State != MiniGameState.Playing
            || !this._playerResults.TryAdd(player, DoppelgangerResult.Failed))
        {
            return;
        }

        this.Logger.LogDebug("{context}: The event failed for player {player}.", this, player);
        await player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.ShowResultAsync(DoppelgangerResult.Failed)).ConfigureAwait(false);
    }

    private async Task RunPlayInfoLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(this._definition.PlayInfoInterval);
            do
            {
                await this.ShowPlayInfoAsync().ConfigureAwait(false);
            }
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in the play info loop.", this);
        }
    }

    private async ValueTask ShowPlayInfoAsync()
    {
        var mapNumber = (short)this.Map.MapId;
        var playerPositions = new List<(Player Player, int Position)>();

        // The synchronous part of the action is executed sequentially for all players.
        await this.ForEachPlayerAsync(player =>
        {
            if (player.IsAlive && !this._playerResults.ContainsKey(player))
            {
                playerPositions.Add((player, this._definition.GetPathPosition(mapNumber, player.Position)));
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);

        var remainingTime = this.RemainingTime;
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowPlayInfoAsync(remainingTime, playerPositions)).AsTask()).ConfigureAwait(false);
    }
}
