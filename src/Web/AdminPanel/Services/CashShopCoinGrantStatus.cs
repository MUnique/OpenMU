// <copyright file="CashShopCoinGrantStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The status of a request to grant cash shop coins.
/// </summary>
public enum CashShopCoinGrantStatus
{
    /// <summary>
    /// The grant was created. The game server applies it the next time the player opens the cash shop.
    /// </summary>
    Created,

    /// <summary>
    /// The account already got a grant with the same reference, so no further grant was created.
    /// </summary>
    AlreadyGranted,

    /// <summary>
    /// The account doesn't exist.
    /// </summary>
    AccountNotFound,

    /// <summary>
    /// Another account already got a grant with the same reference.
    /// </summary>
    ReferenceUsedByOtherAccount,
}
