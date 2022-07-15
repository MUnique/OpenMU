// <copyright file="SoccerGameMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.ComponentModel;
using System.Diagnostics;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A game map with a battle soccer field, which requires a defined <see cref="GameMapDefinition.BattleZone"/>.
/// </summary>
public class SoccerGameMap : GameMap
{
    private GuildWarScore? _score;
    private SoccerBall? _ball;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoccerGameMap"/> class.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    /// <param name="itemDropDuration">Duration of the item drop.</param>
    /// <param name="chunkSize">Size of the chunk.</param>
    public SoccerGameMap(GameMapDefinition mapDefinition, int itemDropDuration, byte chunkSize)
        : base(mapDefinition, itemDropDuration, chunkSize)
    {
        this.ObjectAdded += this.OnObjectAddedAsync;
    }

    /// <summary>
    /// Gets a value indicating whether a battle in ongoing at this instance.
    /// </summary>
    public bool IsBattleOngoing { get; private set; }

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

    private async ValueTask FinalizeBattleAsync()
    {
        if (this._score is { } finishedScore)
        {
            finishedScore.PropertyChanged -= this.OnScoreChanged;
        }

        this._score = null;
        this.Ball.Moved -= this.OnSoccerBallMovedAsync;
        this.Ball.Initialize();
        await this.RespawnAsync(this.Ball).ConfigureAwait(false);

        this.IsBattleOngoing = false;
    }

    private async ValueTask OnSoccerBallMovedAsync((Point From, Point To) e)
    {
        if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone!.LeftGoal!)
            && e.To.IsWithinBoundsOf(this.Definition.BattleZone.LeftGoal!))
        {
            this._score!.IncreaseSecondGuildScore(20);
            this.Ball.Initialize();
            await this.RespawnAsync(this.Ball).ConfigureAwait(false);
        }
        else if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!)
                 && e.To.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!))
        {
            this._score!.IncreaseFirstGuildScore(20);
            this.Ball.Initialize();
            await this.RespawnAsync(this.Ball).ConfigureAwait(false);
        }
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
    }
}