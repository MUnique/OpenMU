// <copyright file="MoneyShare.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The part of a money drop which is reserved for a single player.
/// </summary>
/// <param name="Player">The player for which the part is reserved.</param>
/// <param name="Amount">The amount of money, before the <see cref="Attributes.Stats.MoneyAmountRate"/> of the player is applied.</param>
public readonly record struct MoneyShare(Player Player, uint Amount);
