// <copyright file="SoccerGameMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A game map with a battle soccer field, which requires a defined <see cref="GameMapDefinition.BattleZone"/>.
/// </summary>
/// <remarks>
/// Everyone on the map sees the score board (F3 23) and the remaining time (F3 22), players of the match
/// and spectators alike. When the time runs out, the guild with more goals wins; on a draw the next goal
/// decides. A match whose players all left the map is cancelled, so that the field doesn't stay blocked.
/// </remarks>
public class SoccerGameMap : GameMap
{
    /// <summary>
    /// The points a guild gets for a goal.
    /// </summary>
    public static byte PointsPerGoal => 20;

    private GuildWarScore? _score;
    private SoccerBall? _ball;
    private CancellationTokenSource? _clockCts;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoccerGameMap"/> class.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    /// <param name="itemDropDuration">Duration of the item drop.</param>
    /// <param name="chunkSize">Size of the chunk.</param>
    public SoccerGameMap(GameMapDefinition mapDefinition, TimeSpan itemDropDuration, byte chunkSize)
        : base(mapDefinition, itemDropDuration, chunkSize)
    {
        this.ObjectAdded += this.OnObjectAddedAsync;
    }

    /// <summary>
    /// Gets a value indicating whether a battle in ongoing at this instance.
    /// </summary>
    public bool IsBattleOngoing { get; private set; }

    /// <summary>
    /// Gets or sets the duration of a match.
    /// </summary>
    public TimeSpan MatchDuration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets how long a match goes on without any of its players on the map, before it's cancelled.
    /// </summary>
    public TimeSpan AbandonedTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the interval of the clock updates.
    /// </summary>
    public TimeSpan ClockInterval { get; set; } = TimeSpan.FromSeconds(1);

    private SoccerBall Ball
    {
        get => this._ball ?? throw Error.NotInitializedProperty(this);
        set => this._ball = value;
    }

    /// <summary>
    /// Starts the battle.
    /// </summary>
    /// <param name="score">The score which will be updated.</param>
    public async ValueTask StartBattleAsync(GuildWarScore score)
    {
        this._score = score;
        this._score.PropertyChanged += this.OnScoreChanged;

        this.Ball.Initialize();
        await this.RespawnAsync(this.Ball).ConfigureAwait(false);
        this.Ball.Moved += this.OnSoccerBallMovedAsync;

        await this.ShowScoreToAllAsync(score).ConfigureAwait(false);

        var clockCts = new CancellationTokenSource();
        this._clockCts = clockCts;
        _ = Task.Run(() => this.RunClockAsync(score, clockCts.Token));
    }

    /// <summary>
    /// Initializes the battle.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Soccer Ball not found.
    /// or
    /// Goals not found.
    /// </exception>
    public void InitializeBattle()
    {
        if (this.Ball is null)
        {
            throw new InvalidOperationException("Soccer Ball not found.");
        }

        if (this.Definition.BattleZone?.LeftGoal is null || this.Definition.BattleZone?.RightGoal is null)
        {
            throw new InvalidOperationException("Goals not found.");
        }

        this.IsBattleOngoing = true;
    }

    /// <summary>
    /// Counts down the match time, tells it everyone on the map and ends or cancels the match when it's due.
    /// </summary>
    /// <param name="score">The score of the match.</param>
    /// <param name="cancellationToken">The cancellation token, which is cancelled when the match is over.</param>
    private async Task RunClockAsync(GuildWarScore score, CancellationToken cancellationToken)
    {
        var endOfMatch = DateTime.UtcNow + this.MatchDuration;
        var lastSeenParticipant = DateTime.UtcNow;
        try
        {
            while (!cancellationToken.IsCancellationRequested && !score.HasEnded)
            {
                var now = DateTime.UtcNow;
                var remaining = (ushort)Math.Clamp(Math.Ceiling((endOfMatch - now).TotalSeconds), 0, ushort.MaxValue);
                var players = this.GetPlayers();
                await players.ForEachAsync(p => p.InvokeViewPlugInAsync<IGuildSoccerTimeUpdatePlugIn>(v => v.UpdateSoccerTimeAsync(remaining))).ConfigureAwait(false);

                if (players.Any(p => p.GuildWarContext?.Score == score))
                {
                    lastSeenParticipant = now;
                }
                else if (now - lastSeenParticipant >= this.AbandonedTimeout)
                {
                    score.Cancel();
                    return;
                }
                else
                {
                    // Nobody of the match is on the map, but the timeout is not reached yet: keep waiting.
                }

                if (remaining == 0)
                {
                    score.EndByTime();
                }

                await Task.Delay(this.ClockInterval, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // The match is over.
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private async ValueTask ShowScoreToAllAsync(GuildWarScore score)
    {
        var redGoals = (byte)(score.FirstGuildScore / PointsPerGoal);
        var blueGoals = (byte)(score.SecondGuildScore / PointsPerGoal);
        await this.GetPlayers().ForEachAsync(p => p.InvokeViewPlugInAsync<IGuildSoccerScoreUpdatePlugIn>(
            v => v.UpdateSoccerScoreAsync(score.FirstGuildName, redGoals, score.SecondGuildName, blueGoals))).ConfigureAwait(false);
    }

    private async ValueTask FinalizeBattleAsync()
    {
        if (Interlocked.Exchange(ref this._clockCts, null) is { } clockCts)
        {
            await clockCts.CancelAsync().ConfigureAwait(false);
            clockCts.Dispose();
        }

        if (this._score is { } finishedScore)
        {
            finishedScore.PropertyChanged -= this.OnScoreChanged;
        }

        this._score = null;
        this.Ball.Moved -= this.OnSoccerBallMovedAsync;
        this.Ball.Initialize();
        await this.RespawnAsync(this.Ball).ConfigureAwait(false);

        this.IsBattleOngoing = false;
        await this.GetPlayers().ForEachAsync(p => p.InvokeViewPlugInAsync<IGuildSoccerScoreUpdatePlugIn>(v => v.HideSoccerScoreAsync())).ConfigureAwait(false);
    }

    private async ValueTask OnSoccerBallMovedAsync((Point From, Point To) e)
    {
        if (this._score is not { } score)
        {
            return;
        }

        if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone!.LeftGoal!)
            && e.To.IsWithinBoundsOf(this.Definition.BattleZone.LeftGoal!))
        {
            score.IncreaseSecondGuildScore(PointsPerGoal);
        }
        else if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!)
                 && e.To.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!))
        {
            score.IncreaseFirstGuildScore(PointsPerGoal);
        }
        else
        {
            return;
        }

        if (!score.HasEnded)
        {
            await this.ShowScoreToAllAsync(score).ConfigureAwait(false);
        }

        this.Ball.Initialize();
        await this.RespawnAsync(this.Ball).ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnScoreChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(this._score.HasEnded))
            {
                await this.FinalizeBattleAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private async ValueTask OnObjectAddedAsync((GameMap Map, ILocateable Object) e)
    {
        if (e.Object is SoccerBall soccerBall)
        {
            this.Ball = soccerBall;
        }
        else if (e.Object is Player player && this._score is { } score && !score.HasEnded)
        {
            // A spectator (or a player of the match coming back) sees the board right away.
            var redGoals = (byte)(score.FirstGuildScore / PointsPerGoal);
            var blueGoals = (byte)(score.SecondGuildScore / PointsPerGoal);
            await player.InvokeViewPlugInAsync<IGuildSoccerScoreUpdatePlugIn>(
                v => v.UpdateSoccerScoreAsync(score.FirstGuildName, redGoals, score.SecondGuildName, blueGoals)).ConfigureAwait(false);
        }
        else
        {
            // Other objects, and players entering without a running match, need no handling.
        }
    }
}
