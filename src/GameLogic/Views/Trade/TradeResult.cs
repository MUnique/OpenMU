// <copyright file="TradeResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// The result of a closed trade (both players pressed 'OK').
/// </summary>
public enum TradeResult
{
    /// <summary>
    /// The trade was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// The trade was successful.
    /// </summary>
    Success,

    /// <summary>
    /// The trade failed because of a full inventory.
    /// </summary>
    FailedByFullInventory,

    /// <summary>
    /// The trade failed because the request timed out.
    /// </summary>
    TimedOut,

    /// <summary>
    /// The trade failed because one or more items were not allowed to trade.
    /// </summary>
    FailedByItemsNotAllowedToTrade,
}