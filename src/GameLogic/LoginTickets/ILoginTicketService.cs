// <copyright file="ILoginTicketService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LoginTickets;

using System.Threading;

/// <summary>
/// Issues and redeems single-use tickets, with which game clients log into accounts without a password.
/// </summary>
/// <remarks>
/// A trusted external service, e.g. a website which signed the player in, issues a ticket.
/// The game client sends its token in the <c>LoginWithTicket</c> packet, and the game server redeems it.
/// </remarks>
public interface ILoginTicketService
{
    /// <summary>
    /// Issues a ticket for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ticket; or <c>null</c>, if the account got too many tickets recently.</returns>
    ValueTask<IssuedLoginTicket?> IssueAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Redeems a ticket. A ticket can be redeemed once.
    /// </summary>
    /// <param name="token">The token of the ticket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result, with the identifier of the account when it was redeemed.</returns>
    ValueTask<LoginTicketRedemption> RedeemAsync(string token, CancellationToken cancellationToken = default);
}
