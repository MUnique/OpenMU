// <copyright file="DuelRoom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using MUnique.OpenMU.GameLogic.Views.Duel;
using Nito.AsyncEx;

/// <summary>
/// The state of the duel.
/// </summary>
public enum DuelState
{
    /// <summary>
    /// Duel state is undefined.
    /// </summary>
    Undefined,

    /// <summary>
    /// A duel was requested.
    /// </summary>
    DuelRequested,

    /// <summary>
    /// Duel request was refused.
    /// </summary>
    DuelRefused,

    /// <summary>
    /// Duel failed to start.
    /// </summary>
    DuelStartFailed,

    /// <summary>
    /// Duel request was accepted.
    /// </summary>
    DuelAccepted,

    /// <summary>
    /// Duel has started.
    /// </summary>
    DuelStarted,

    /// <summary>
    /// Dual was cancelled.
    /// </summary>
    DuelCancelled,

    /// <summary>
    /// Duel has finished.
    /// </summary>
    DuelFinished,
}

/// <summary>
/// A class that manages a duel between two players.
/// </summary>
public sealed class DuelRoom : AsyncDisposable
{
    private readonly AsyncLock _spectatorLock = new();
    private CancellationTokenSource? _cts = new();
    private byte _scoreRequester;
    private byte _scoreOpponent;
    private int _maximumScore;
    private int _maximumSpectators;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelRoom" /> class.
    /// </summary>
    /// <param name="area">The duel area; <c>null</c>, if the duel takes place on the map where the duelists are standing.</param>
    /// <param name="requester">The requester.</param>
    /// <param name="opponent">The opponent.</param>
    public DuelRoom(DuelArea? area, Player requester, Player opponent)
    {
        this.Area = area;
        this.Index = area?.Index ?? 0;
        this.Map = area?.FirstPlayerGate?.Map ?? requester.CurrentMap?.Definition;
        this.Requester = requester;
        this.Opponent = opponent;
        this.CreatedAt = DateTime.UtcNow;

        this._maximumScore = requester.GameContext.Configuration.DuelConfiguration?.MaximumScore ?? 10;
        this._maximumSpectators = requester.GameContext.Configuration.DuelConfiguration?.MaximumSpectatorsPerDuelRoom ?? 10;
    }

    /// <summary>
    /// Gets the area of the duel, or <c>null</c>, if the duel takes place on the map where the duelists are standing.
    /// </summary>
    public DuelArea? Area { get; }

    /// <summary>
    /// Gets the index of the area of the duel.
    /// </summary>
    /// <remarks>
    /// It's the number of the duel channel which the client shows in its duel watch list.
    /// A duel without an <see cref="Area"/> can't be watched, so it just uses the first index.
    /// </remarks>
    public int Index { get; }

    /// <summary>
    /// Gets the map on which the duel takes place.
    /// </summary>
    /// <remarks>
    /// For a duel in the current map, it's the map on which the duelists are when the duel is
    /// accepted - they may have moved to another map since the duel was requested.
    /// </remarks>
    public GameMapDefinition? Map { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the duel takes place on the map where the duelists were
    /// standing when the duel started, instead of in an area of the duel arena.
    /// </summary>
    /// <remarks>
    /// That's how the duel worked before the duel arena was introduced with Season 4.
    /// Such a duel can't be watched by spectators, because everyone around sees it anyway.
    /// </remarks>
    public bool IsInCurrentMap => this.Area is null;

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> of the start of the duel.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the player that requested the duel.
    /// </summary>
    public Player Requester { get; }

    /// <summary>
    /// Gets the player that accepted the duel.
    /// </summary>
    public Player Opponent { get; }

    /// <summary>
    /// Gets or sets the score of the player that requested the duel.
    /// </summary>
    public byte ScoreRequester
    {
        get => this._scoreRequester;
        set
        {
            this._scoreRequester = value;
            if (value >= this._maximumScore)
            {
                this.State = DuelState.DuelFinished;
            }
        }
    }

    /// <summary>
    /// Gets or sets the score of the player that accepted the duel.
    /// </summary>
    public byte ScoreOpponent
    {
        get => this._scoreOpponent;
        set
        {
            this._scoreOpponent = value;
            if (value >= this._maximumScore)
            {
                this.State = DuelState.DuelFinished;
            }
        }
    }

    // public bool IsAccepted { get; set; }

    // public bool IsFinished { get; set; }

    /// <summary>
    /// Gets or sets the state of the duel.
    /// </summary>
    public DuelState State { get; set; }

    /// <summary>
    /// Gets a lock object used to update the score of the duel.
    /// </summary>
    public AsyncLock Lock { get; } = new();

    /// <summary>
    /// Gets the duel room spectators list.
    /// </summary>
    public List<Player> Spectators { get; } = new();

    /// <summary>
    /// Gets all the players taking part in the duel, either as duelists or spectators.
    /// </summary>
    public IEnumerable<Player> AllPlayers
    {
        get
        {
            yield return this.Requester;

            yield return this.Opponent;

            for (var index = this.Spectators.Count - 1; index >= 0; index--)
            {
                var spectator = this.Spectators[index];
                yield return spectator;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the duel room still has spectator slots.
    /// </summary>
    public bool IsOpen => this.Spectators.Count < this._maximumSpectators;

    /// <summary>
    /// Gets a value indicating whether the player is participating in the duel as a duelist.
    /// </summary>
    /// <param name="player">The player.</param>
    public bool IsDuelist(Player player)
    {
        return this.Requester == player || this.Opponent == player;
    }

    /// <summary>
    /// Determines whether the two specified players are the two duelists of this duel.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="other">The other player.</param>
    /// <remarks>
    /// This matters for a duel which takes place in the current map: other players are around
    /// there, so a hit or kill which involves one duelist and somebody else is not part of the
    /// duel and keeps its usual consequences.
    /// </remarks>
    public bool AreDuelists(Player player, Player other)
    {
        return this.IsDuelist(player) && this.IsDuelist(other);
    }

    /// <summary>
    /// Removes the spectator from the room.
    /// </summary>
    /// <param name="spectator">The spectator which should be removed.</param>
    public async ValueTask RemoveSpectatorAsync(Player spectator)
    {
        using (await this._spectatorLock.LockAsync().ConfigureAwait(false))
        {
            if (!this.Spectators.Remove(spectator))
            {
                return;
            }
        }

        for (int j = this.Spectators.Count; j >= 0; --j)
        {
            var player = this.Spectators[j];
            await player.InvokeViewPlugInAsync<IDuelSpectatorRemovedPlugIn>(p => p.SpectatorRemovedAsync(spectator)).ConfigureAwait(false);
        }

        await spectator.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);
        await spectator.RemoveInvisibleEffectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the duel.
    /// </summary>
    /// <returns>A <see cref="Task"/>.</returns>
    /// <exception cref="InvalidOperationException">Occurs when one of the players has left the duel map.</exception>
    public async Task RunDuelAsync()
    {
        try
        {
            var cancellationToken = this._cts?.Token ?? default;

            // We first wait until both players are on the map
            while (this.Requester.Id == default || this.Opponent.Id == default)
            {
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }

            if (!this.AreDuelistsOnDuelMap())
            {
                throw new InvalidOperationException("Duel cannot start when any of the players left the duel map");
            }

            cancellationToken.ThrowIfCancellationRequested();
            this.State = DuelState.DuelAccepted;

            for (int i = 5; i > 0; i--)
            {
                var seconds = i;
                await this.AllPlayers.ForEachAsync(p => p.ShowLocalizedGoldenMessageAsync(nameof(PlayerMessage.DuelBattleBeginsInSecondsFormat), seconds)).ConfigureAwait(false);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            await this.Requester.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(DuelStartResult.Success, this.Opponent)).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(DuelStartResult.Success, this.Requester)).ConfigureAwait(false);

            await this.Requester.InvokeViewPlugInAsync<IShowDuelScoreUpdatePlugIn>(p => p.UpdateScoreAsync(this)).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IShowDuelScoreUpdatePlugIn>(p => p.UpdateScoreAsync(this)).ConfigureAwait(false);

            await this.Requester.InvokeViewPlugInAsync<IInitializeDuelPlugIn>(p => p.InitializeDuelAsync(this)).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IInitializeDuelPlugIn>(p => p.InitializeDuelAsync(this)).ConfigureAwait(false);

            this.State = DuelState.DuelStarted;

            while (this.State is not (DuelState.DuelFinished or DuelState.DuelCancelled))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                await this.SendCurrentStateToAllPlayersAsync().ConfigureAwait(false);
            }

            if (this.State is DuelState.DuelFinished)
            {
                await this.FinishDuelAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            if (this.State is not DuelState.DuelFinished)
            {
                this.State = DuelState.DuelCancelled;
                await this.StopDuelAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await this.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Cancels and stops the duel.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/>.</returns>
    public async ValueTask CancelDuelAsync()
    {
        if (this._cts is { } cts)
        {
            await cts.CancelAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the spawn gate of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The gate where the player is teleported to; <c>null</c>, if the duel doesn't define one.</returns>
    public ExitGate? GetSpawnGate(Player player)
    {
        if (this.Area is not { } area)
        {
            // A duel in the current map doesn't move the players, so they respawn where they
            // would respawn without a duel, too.
            return null;
        }

        if (this.Opponent == player)
        {
            return area.SecondPlayerGate;
        }

        if (this.Requester == player)
        {
            return area.FirstPlayerGate;
        }

        return area.SpectatorsGate;
    }

    /// <summary>
    /// Tries to add the player as a spectator to the duel.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>A <see cref="ValueTask"/> with the result.</returns>
    public async ValueTask<bool> TryAddSpectatorAsync(Player player)
    {
        if (this.Area?.SpectatorsGate is not { } spectatorsGate)
        {
            return false;
        }

        Player[] spectators;
        using (await this._spectatorLock.LockAsync().ConfigureAwait(false))
        {
            if (this.Spectators.Count >= this._maximumSpectators)
            {
                return false;
            }

            spectators = this.Spectators.ToArray();
            this.Spectators.Add(player);
        }

        await player.AddInvisibleEffectAsync().ConfigureAwait(false);
        await player.WarpToAsync(spectatorsGate).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IInitializeDuelPlugIn>(p => p.InitializeDuelAsync(this)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IDuelHealthUpdatePlugIn>(p => p.UpdateHealthAsync(this)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowDuelScoreUpdatePlugIn>(p => p.UpdateScoreAsync(this)).ConfigureAwait(false);

        // send spectator list to this player
        await player.InvokeViewPlugInAsync<IDuelSpectatorListUpdatePlugIn>(p => p.UpdateSpectatorListAsync(spectators)).ConfigureAwait(false);

        // send spectator added to all spectators of the same room
        for (int i = spectators.Length - 1; i >= 0; i--)
        {
            if (spectators[i] is { } spectator)
            {
                await spectator.InvokeViewPlugInAsync<IDuelSpectatorAddedPlugIn>(p => p.SpectatorAddedAsync(player)).ConfigureAwait(false);
            }
        }

        return true;
    }

    /// <summary>
    /// Resets and disposes of the duel room.
    /// </summary>
    /// <param name="startResult">The resuls of the duel start request.</param>
    /// <returns>A <see cref="ValueTask"/>.</returns>
    public async ValueTask ResetAndDisposeAsync(DuelStartResult startResult)
    {
        this.Requester.DuelRoom = null;
        this.Opponent.DuelRoom = null;
        await this.Requester.GameContext.DuelRoomManager.GiveBackDuelRoomAsync(this).ConfigureAwait(false);
        if (startResult == DuelStartResult.Refused)
        {
            this.State = DuelState.DuelRefused;
            await this.Requester.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(startResult, this.Opponent)).ConfigureAwait(false);
        }
        else if (startResult != DuelStartResult.Undefined)
        {
            this.State = DuelState.DuelStartFailed;
            await this.Requester.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);

            await this.Requester.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(startResult, this.Opponent)).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(startResult, this.Requester)).ConfigureAwait(false);
        }
        else
        {
            await this.Requester.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);
        }

        await this.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (Interlocked.Exchange(ref this._cts, null) is not { } cts)
        {
            return;
        }

        await cts.CancelAsync().ConfigureAwait(false);

        if (this.State >= DuelState.DuelAccepted)
        {
            await this.MovePlayersToExitAsync().ConfigureAwait(false);

            await this.Requester.GameContext.DuelRoomManager.GiveBackDuelRoomAsync(this).ConfigureAwait(false);
        }

        this.AllPlayers.ForEach(p => p.DuelRoom = null);
        cts.Dispose();

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private bool AreDuelistsOnDuelMap()
    {
        if (this.Area is { } area)
        {
            return this.Requester.CurrentMap?.Definition == area.FirstPlayerGate?.Map
                   && this.Opponent.CurrentMap?.Definition == area.SecondPlayerGate?.Map;
        }

        return this.Requester.CurrentMap?.Definition == this.Map
               && this.Opponent.CurrentMap?.Definition == this.Map;
    }

    private async ValueTask MovePlayersToExitAsync()
    {
        if (this.Area is not { } area)
        {
            // The duel took place where the players were standing, so there is nowhere to move them away from.
            return;
        }

        var duelConfig = this.Requester.GameContext.Configuration.DuelConfiguration;
        var exitGate = duelConfig?.Exit;
        var duelArenaMapNumber = area.FirstPlayerGate?.Map?.Number;

        var players = this.AllPlayers
            .Where(p => p.CurrentMap?.MapId == duelArenaMapNumber);

        foreach (var player in players)
        {
            try
            {
                if (exitGate is not null && !this.IsDuelist(player))
                {
                    await player.WarpToAsync(exitGate).ConfigureAwait(false);
                }
                else
                {
                    await player.WarpToSafezoneAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Unexpected error when moving player away from duel arena.");
            }
        }
    }

    private async ValueTask NotifyDuelFinishedAsync()
    {
        var winner = this.ScoreRequester > this.ScoreOpponent ? this.Requester : this.Opponent;
        var loser = this.Requester == winner ? this.Opponent : this.Requester;
        await this.AllPlayers.ForEachAsync(player => player.InvokeViewPlugInAsync<IDuelFinishedPlugIn>(p => p.DuelFinishedAsync(winner, loser))).ConfigureAwait(false);
    }

    private async ValueTask SendCurrentStateToAllPlayersAsync()
    {
        await this.AllPlayers.ForEachAsync(p => p.InvokeViewPlugInAsync<IShowDuelScoreUpdatePlugIn>(p => p.UpdateScoreAsync(this))).ConfigureAwait(false);

        for (var index = this.Spectators.Count - 1; index >= 0; index--)
        {
            var spectator = this.Spectators[index];
            await spectator.InvokeViewPlugInAsync<IDuelHealthUpdatePlugIn>(p => p.UpdateHealthAsync(this)).ConfigureAwait(false);
        }
    }

    private async ValueTask StopDuelAsync()
    {
        await this.Opponent.ResetPetBehaviorAsync().ConfigureAwait(false);
        await this.Requester.ResetPetBehaviorAsync().ConfigureAwait(false);

        await this.ResetAndDisposeAsync(DuelStartResult.Undefined).ConfigureAwait(false);
        await this.AllPlayers.ForEachAsync(player => player.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync())).ConfigureAwait(false);
    }

    private async ValueTask FinishDuelAsync()
    {
        await this.Opponent.ResetPetBehaviorAsync().ConfigureAwait(false);
        await this.Requester.ResetPetBehaviorAsync().ConfigureAwait(false);

        await this.NotifyDuelFinishedAsync().ConfigureAwait(false);
        await Task.Delay(10000, default).ConfigureAwait(false);
        await this.MovePlayersToExitAsync().ConfigureAwait(false);
    }
}