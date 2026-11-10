// <copyright file="ILuckyCoinViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The result of a lucky coin registration at the NPC Delgado.
/// </summary>
public enum LuckyCoinRegistrationResult
{
    /// <summary>
    /// The player has no lucky coin in the inventory.
    /// </summary>
    NotEnoughItems,

    /// <summary>
    /// The lucky coin was registered.
    /// </summary>
    Registered,
}

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

/// <summary>
/// A view plugin which shows the results of the lucky coin dialogs of the NPC Delgado.
/// </summary>
public interface ILuckyCoinViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the count of the lucky coins which the character registered.
    /// </summary>
    /// <param name="count">The count of the registered lucky coins.</param>
    ValueTask ShowRegisteredCountAsync(int count);

    /// <summary>
    /// Shows the result of a lucky coin registration.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="count">The count of the registered lucky coins after the registration.</param>
    ValueTask ShowRegistrationResultAsync(LuckyCoinRegistrationResult result, int count);

    /// <summary>
    /// Shows the result of a lucky coin exchange.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowExchangeResultAsync(LuckyCoinExchangeResult result);
}
