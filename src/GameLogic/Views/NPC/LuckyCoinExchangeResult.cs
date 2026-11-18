// <copyright file="LuckyCoinExchangeResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The result of a lucky coin exchange at the NPC Delgado.
/// </summary>
public enum LuckyCoinExchangeResult
{
    /// <summary>
    /// The player doesn't have enough lucky coins in the inventory,
    /// or no reward is configured for the requested count of coins.
    /// </summary>
    NotEnoughItems,

    /// <summary>
    /// The lucky coins were exchanged for an item.
    /// </summary>
    Exchanged,

    /// <summary>
    /// The inventory has not enough space for the item.
    /// </summary>
    InventoryFull,
}
