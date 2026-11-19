// <copyright file="LoginTicketRedeemStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LoginTickets;

/// <summary>
/// The status of redeeming a login ticket.
/// </summary>
public enum LoginTicketRedeemStatus
{
    /// <summary>
    /// The ticket was redeemed.
    /// </summary>
    Redeemed,

    /// <summary>
    /// No ticket has the token.
    /// </summary>
    Unknown,

    /// <summary>
    /// The ticket is expired.
    /// </summary>
    Expired,

    /// <summary>
    /// The ticket was already redeemed.
    /// </summary>
    AlreadyUsed,
}
