// <copyright file="BattleZoneDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a battle zone.
/// </summary>
[Cloneable]
public partial class BattleZoneDefinition
{
    /// <summary>
    /// Gets or sets the battle type.
    /// </summary>
    public BattleType Type { get; set; }

    /// <summary>
    /// Gets or sets the x-coordinate of the upper spawn point for the left team.
    /// </summary>
    public byte? LeftTeamSpawnPointX { get; set; }

    /// <summary>
    /// Gets or sets the y-coordinate of the upper spawn point for the left team.
    /// </summary>
    public byte LeftTeamSpawnPointY { get; set; }

    /// <summary>
    /// Gets or sets the x-coordinate of the upper spawn point for the right team.
    /// </summary>
    public byte? RightTeamSpawnPointX { get; set; }

    /// <summary>
    /// Gets or sets the y-coordinate of the upper spawn point for the right team.
    /// </summary>
    public byte RightTeamSpawnPointY { get; set; }

    /// <summary>
    /// Gets or sets the battle ground.
    /// </summary>
    [MemberOfAggregate]
    public virtual Rectangle? Ground { get; set; }

    /// <summary>
    /// Gets or sets the first goal zone.
    /// </summary>
    [MemberOfAggregate]
    public virtual Rectangle? LeftGoal { get; set; }

    /// <summary>
    /// Gets or sets the second goal zone.
    /// </summary>
    [MemberOfAggregate]
    public virtual Rectangle? RightGoal { get; set; }
}