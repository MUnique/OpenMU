// <copyright file="SoccerGameMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.ComponentModel;
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
        this.ObjectAdded += this.OnObjectAdded;
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
    public void StartBattle(GuildWarScore score)
    {
        this._score = score;
        this._score.PropertyChanged += this.OnScoreChanged;

        this.Ball.Initialize();
        this.Respawn(this.Ball);
        this.Ball.Moved += this.OnSoccerBallMoved;
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

    private void FinalizeBattle()
    {
        if (this._score is { } finishedScore)
        {
            finishedScore.PropertyChanged -= this.OnScoreChanged;
        }

        this._score = null;
        this.Ball.Moved -= this.OnSoccerBallMoved;
        this.Ball.Initialize();
        this.Respawn(this.Ball);

        this.IsBattleOngoing = false;
    }

    private void OnSoccerBallMoved(object? sender, (Point From, Point To) e)
    {
        if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone!.LeftGoal!)
            && e.To.IsWithinBoundsOf(this.Definition.BattleZone.LeftGoal!))
        {
            this._score!.IncreaseSecondGuildScore(20);
            this.Ball.Initialize();
            this.Respawn(this.Ball);
        }
        else if (!e.From.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!)
                 && e.To.IsWithinBoundsOf(this.Definition.BattleZone.RightGoal!))
        {
            this._score!.IncreaseFirstGuildScore(20);
            this.Ball.Initialize();
            this.Respawn(this.Ball);
        }
    }

    private void OnScoreChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(this._score.HasEnded))
        {
            this.FinalizeBattle();
        }
    }

    private void OnObjectAdded(object? sender, (GameMap Map, ILocateable Object) e)
    {
        if (e.Object is SoccerBall soccerBall)
        {
            this.Ball = soccerBall;
        }
    }
}