// <copyright file="CashShopBuyResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a cash shop purchase attempt.
/// </summary>
public enum CashShopBuyResult
{
    /// <summary>
    /// The purchase was successful.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The purchase failed due to insufficient funds.
    /// </summary>
    InsufficientFunds = 1,

    /// <summary>
    /// The purchase failed because the product was not found.
    /// </summary>
    ProductNotFound = 2,

    /// <summary>
    /// The purchase failed because the storage is full.
    /// </summary>
    StorageFull = 3,

    /// <summary>
    /// The purchase failed for an unknown reason.
    /// </summary>
    Failed = 4,
}
