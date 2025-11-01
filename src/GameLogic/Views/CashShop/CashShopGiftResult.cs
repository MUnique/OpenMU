// <copyright file="CashShopGiftResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a cash shop gift attempt.
/// </summary>
public enum CashShopGiftResult
{
    /// <summary>
    /// The gift was sent successfully.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The gift failed due to insufficient funds.
    /// </summary>
    InsufficientFunds = 1,

    /// <summary>
    /// The gift failed because the receiver was not found.
    /// </summary>
    ReceiverNotFound = 2,

    /// <summary>
    /// The gift failed because the receiver's storage is full.
    /// </summary>
    ReceiverStorageFull = 3,

    /// <summary>
    /// The gift failed for an unknown reason.
    /// </summary>
    Failed = 4,
}
