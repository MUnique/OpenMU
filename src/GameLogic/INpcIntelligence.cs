// <copyright file="INpcIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Interface of a non-player-character artificial intelligence.
/// </summary>
public interface INpcIntelligence
{
    /// <summary>
    /// Gets or sets the monster which this AI is controlling.
    /// </summary>
    NonPlayerCharacter Npc { get; set; }

    /// <summary>
    /// Registers a hit from an attacker.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    void RegisterHit(IAttacker attacker);

    /// <summary>
    /// Starts the actions.
    /// </summary>
    void Start();

    /// <summary>
    /// Pauses the actions.
    /// </summary>
    void Pause();

    /// <summary>
    /// Determines whether the monster can walk on the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>
    ///   <c>true</c> if this instance can walk on the specified target; otherwise, <c>false</c>.
    /// </returns>
    bool CanWalkOn(Point target);
}