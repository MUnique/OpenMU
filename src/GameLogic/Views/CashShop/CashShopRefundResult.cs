// <copyright file="CashShopRefundResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a cash shop refund attempt.
/// </summary>
public enum CashShopRefundResult
{
    /// <summary>
    /// The refund was successful.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The refund failed because the item was not found.
    /// </summary>
    ItemNotFound = 1,

    /// <summary>
    /// The refund failed because the time limit for refunds has been exceeded.
    /// </summary>
    TimeLimitExceeded = 2,

    /// <summary>
    /// The refund failed for an unknown reason.
    /// </summary>
    Failed = 3,
}