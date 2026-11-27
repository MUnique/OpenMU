// <copyright file="IPlayerSurrogate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Surrogate of a player.
/// </summary>
public interface IPlayerSurrogate
{
    /// <summary>
    /// Gets the owner of this instance.
    /// </summary>
    Player Owner { get; }

    /// <summary>
    /// Is called after the surrogate successfully hit a Player.
    /// </summary>
    /// <param name="damage">The damage dealt.</param>
    /// <param name="target">The target.</param>
    public ValueTask AfterHitTargetAsync(float damage, Player? target = null);
}