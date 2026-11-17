// <copyright file="IssuedLoginTicket.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LoginTickets;

/// <summary>
/// A login ticket which was just issued.
/// </summary>
/// <param name="Token">The token, which the game client sends to log in. It's not stored, so it's only available here.</param>
/// <param name="ExpiresAt">The timestamp until which the ticket can be redeemed.</param>
public sealed record IssuedLoginTicket(string Token, DateTime ExpiresAt)
{
    /// <summary>
    /// Returns a string without the token, so that logging the ticket doesn't leak it.
    /// </summary>
    /// <returns>The string.</returns>
    public override string ToString() => $"{nameof(IssuedLoginTicket)} {{ {nameof(this.ExpiresAt)} = {this.ExpiresAt:O} }}";
}
