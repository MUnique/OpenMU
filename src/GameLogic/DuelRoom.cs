namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.Duel;
using Nito.AsyncEx;
using System.Threading;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

public enum DuelState
{
    Undefined,
    DuelRequested,
    DuelAccepted,
    DuelRefused,
    DuelStartFailed,

    DuelStarted,
    DuelCancelled,
    DuelFinished,
}

public sealed class DuelRoom : AsyncDisposable
{
    private readonly AsyncLock _spectatorLock = new();
    private readonly CancellationTokenSource _cts = new();
    private byte _scoreRequester;
    private byte _scoreOpponent;
    private int _maximumScore;
    private int _maximumSpectators;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelRoom" /> class.
    /// </summary>
    /// <param name="area">The duel area.</param>
    /// <param name="requester">The requester.</param>
    /// <param name="opponent">The opponent.</param>
    public DuelRoom(DuelArea area, Player requester, Player opponent)
    {
        this.Area = area;
        this.Index = area.Index;
        this.Requester = requester;
        this.Opponent = opponent;
        this.CreatedAt = DateTime.UtcNow;

        this._maximumScore = requester.GameContext.Configuration.DuelConfiguration?.MaximumScore ?? 10;
        this._maximumSpectators = requester.GameContext.Configuration.DuelConfiguration?.MaximumSpectatorsPerDuelRoom ?? 10;
    }

    public DuelArea Area { get; }

    public int Index { get; }

    public DateTime CreatedAt { get; set; }

    public Player Requester { get; }

    public Player Opponent { get; }

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

    public byte ScoreOpponent
    {
        get => _scoreOpponent;
        set
        {
            _scoreOpponent = value;
            if (value >= this._maximumScore)
            {
                this.State = DuelState.DuelFinished;
            }
        }
    }

    //public bool IsAccepted { get; set; }

    //public bool IsFinished { get; set; }

    public DuelState State { get; set; }

    public AsyncLock Lock { get; } = new();

    public List<Player> Spectators { get; } = new ();

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

    public bool IsOpen => this.Spectators.Count < this._maximumSpectators;

    public bool IsDuelist(Player player)
    {
        return this.Requester == player || this.Opponent == player;
    }

    /// <summary>
    /// Removes the spectator from the room.
    /// </summary>
    /// <param name="spectator">The spectator which should be removed.</param>
    public async ValueTask RemoveSpectatorAsync(Player spectator)
    {
        using (await this._spectatorLock.LockAsync())
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

    public async Task RunDuelAsync()
    {
        try
        {
            var cancellationToken = this._cts.Token;

            // We first wait until both players are on the map
            while (this.Requester.Id == default || this.Opponent.Id == default)
            {
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }

            if (this.Requester.CurrentMap?.Definition != this.Area.FirstPlayerGate?.Map
                || this.Opponent.CurrentMap?.Definition != this.Area.SecondPlayerGate?.Map)
            {
                throw new InvalidOperationException("Duel cannot start when any of the players left the duel map");
            }

            cancellationToken.ThrowIfCancellationRequested();
            this.State = DuelState.DuelAccepted;

            for (int i = 5; i > 0; i--)
            {
                var message = $"The battle begins in {i} seconds";
                await this.AllPlayers.ForEachAsync(p => p.InvokeViewPlugInAsync<IShowMessagePlugIn>(m => m.ShowMessageAsync(message, MessageType.GoldenCenter))).ConfigureAwait(false);
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
        catch (OperationCanceledException)
        {
            // We expect that, when it's cancelled from outside.
            // So we just do nothing in this case.
        }
        catch (Exception ex)
        {
            await this.CancelDuelAsync().ConfigureAwait(false);
        }
        finally
        {
            await this.DisposeAsync().ConfigureAwait(false);
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

    private async ValueTask FinishDuelAsync()
    {
        await this.Opponent.ResetPetBehaviorAsync().ConfigureAwait(false);
        await this.Requester.ResetPetBehaviorAsync().ConfigureAwait(false);

        await this.NotifyDuelFinishedAsync().ConfigureAwait(false);
        await Task.Delay(10000, default).ConfigureAwait(false);
        await this.MovePlayersToExit().ConfigureAwait(false);
    }

    public async ValueTask StopDuelAsync()
    {
        await this.Opponent.ResetPetBehaviorAsync().ConfigureAwait(false);
        await this.Requester.ResetPetBehaviorAsync().ConfigureAwait(false);

        await this.ResetAndDisposeAsync(DuelStartResult.Refused).ConfigureAwait(false);
        await this.AllPlayers.ForEachAsync(player => player.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync())).ConfigureAwait(false);
    }

    public async ValueTask CancelDuelAsync()
    {
        await this._cts.CancelAsync().ConfigureAwait(false);

        await this.StopDuelAsync().ConfigureAwait(false);
    }

    public ExitGate? GetSpawnGate(Player player)
    {
        if (this.Opponent == player)
        {
            return this.Area.SecondPlayerGate;
        }

        if (this.Requester == player)
        {
            return this.Area.FirstPlayerGate;
        }

        return this.Area.SpectatorsGate;
    }

    public async ValueTask<bool> TryAddSpectatorAsync(Player player)
    {
        if (this.Area.SpectatorsGate is not { } spectatorsGate)
        {
            return false;
        }

        Player[] spectators;
        using (await this._spectatorLock.LockAsync())
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
        else
        {
            this.State = DuelState.DuelStartFailed;
            await this.Requester.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IDuelEndedPlugIn>(p => p.DuelEndedAsync()).ConfigureAwait(false);

            await this.Requester.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(startResult, this.Opponent)).ConfigureAwait(false);
            await this.Opponent.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(startResult, this.Requester)).ConfigureAwait(false);
        }

        await this.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._cts.CancelAsync().ConfigureAwait(false);
        if (this.State >= DuelState.DuelStarted)
        {
            await this.MovePlayersToExit().ConfigureAwait(false);

            await this.Requester.GameContext.DuelRoomManager.GiveBackDuelRoomAsync(this).ConfigureAwait(false);
        }

        this.AllPlayers.ForEach(p => p.DuelRoom = null);
        this._cts.Dispose();

        await base.DisposeAsyncCore();
    }

    private async ValueTask MovePlayersToExit()
    {
        var duelConfig = this.Requester.GameContext.Configuration.DuelConfiguration;
        var exitGate = duelConfig?.Exit;
        var duelArenaMapNumber = this.Area.FirstPlayerGate?.Map?.Number;

        var players = this.AllPlayers
            .Where(p => p.CurrentMap?.MapId == duelArenaMapNumber);

        foreach (var player in players)
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
    }




}