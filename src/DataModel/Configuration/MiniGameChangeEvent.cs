// <copyright file="MiniGameChangeEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines the kind of targets for the <see cref="MiniGameChangeEvent"/>.
/// </summary>
public enum KillTarget
{
    /// <summary>
    /// Any object counts as kill.
    /// </summary>
    AnyObject,

    /// <summary>
    /// Any monster counts as kill.
    /// </summary>
    AnyMonster,

    /// <summary>
    /// A specific monster or destructible, defined by <see cref="MiniGameChangeEvent.TargetDefinition"/> counts as kill.
    /// </summary>
    Specific,
}

/// <summary>
/// An event which changes the mini game to a next phase.
/// Defines what the player needs to do to reach it,
/// and what happens when the requirements are fulfilled.
/// </summary>
[Cloneable]
public partial class MiniGameChangeEvent
{
    /// <summary>
    /// Gets or sets the index to define the order of the event.
    /// Usually, events are worked through in sequential order.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the description about the event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the (golden) message which should be shown to the player.
    /// One placeholder can be used to show the triggering player name.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the targets which need to be killed to reach the required <see cref="NumberOfKills"/>.
    /// </summary>
    public KillTarget Target { get; set; }

    /// <summary>
    /// Gets or sets the minimum level of the <see cref="Target"/>s,
    /// if no specific <see cref="TargetDefinition"/> is supplied.
    /// </summary>
    public short? MinimumTargetLevel { get; set; }

    /// <summary>
    /// Gets or sets the number of kills which the player(s) have to reach.
    /// </summary>
    public short NumberOfKills { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="NumberOfKills"/> are meant to be
    /// a multiple of the players of the game, or not.
    /// </summary>
    public bool MultiplyKillsByPlayers { get; set; }

    /// <summary>
    /// Gets or sets the target definition.
    /// </summary>
    public virtual MonsterDefinition? TargetDefinition { get; set; }

    /// <summary>
    /// Gets or sets the optional spawns which will appear when the players reach the goal.
    /// </summary>
    [MemberOfAggregate]
    public virtual MonsterSpawnArea? SpawnArea { get; set; }

    /// <summary>
    /// Gets or sets the changes which will be applied to the terrain when the player reach the goal.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MiniGameTerrainChange> TerrainChanges { get; protected set; } = null!;
}