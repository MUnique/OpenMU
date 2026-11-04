// <copyright file="FruitUsage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Defines how the fruit is used. Only applies, if the the item is a fruit.
/// </summary>
public enum FruitUsage
{
    /// <summary>
    /// The undefined usage. Used when it doesn't apply.
    /// </summary>
    Undefined,

    /// <summary>
    /// Adds 1~3 stat points to the character.
    /// </summary>
    AddPoints,

    /// <summary>
    /// Removes 1~9 stat points from the character.
    /// </summary>
    RemovePoints,
}