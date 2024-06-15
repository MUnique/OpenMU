// <copyright file="ISummonable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// An interface for a class which can (but not must) be summoned by a player.
/// </summary>
public interface ISummonable : IIdentifiable, ILocateable, IRotatable
{
    /// <summary>
    /// Gets the player which summoned this instance.
    /// </summary>
    Player? SummonedBy { get; }

    /// <summary>
    /// Gets the definition for this instance.
    /// </summary>
    MonsterDefinition Definition { get; }
}