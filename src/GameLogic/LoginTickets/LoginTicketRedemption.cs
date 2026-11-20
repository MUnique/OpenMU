// <copyright file="LoginTicketRedemption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LoginTickets;

/// <summary>
/// The result of redeeming a login ticket.
/// </summary>
/// <param name="Status">The status.</param>
/// <param name="AccountId">The identifier of the account of the ticket, if it was <see cref="LoginTicketRedeemStatus.Redeemed"/>.</param>
public readonly record struct LoginTicketRedemption(LoginTicketRedeemStatus Status, Guid AccountId);
