// <copyright file="SignInTicketService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;

/// <summary>
/// Hands out short living, single use tickets which the sign in endpoint exchanges for an authentication cookie.
/// </summary>
/// <remarks>
/// An interactive Blazor component can't set a cookie, because the response of the request which
/// started the circuit has been sent long ago. The component therefore validates the credentials,
/// gets a ticket from here and posts it to <see cref="AdminAuthenticationDefaults.SignInEndpointPath"/>,
/// which is a normal http request and can set the cookie. Since that request is done in the
/// background, the user stays on the same page - no reload, no lost state.
/// </remarks>
public class SignInTicketService
{
    private static readonly TimeSpan TicketLifetime = TimeSpan.FromMinutes(2);

    private readonly ConcurrentDictionary<string, Ticket> _tickets = new(StringComparer.Ordinal);

    /// <summary>
    /// Issues a new ticket for the specified claims.
    /// </summary>
    /// <param name="claims">The claims of the authenticated user.</param>
    /// <param name="isPersistent">If set to <c>true</c>, the resulting cookie survives a browser restart.</param>
    /// <returns>The ticket value, which has to be posted to the sign in endpoint.</returns>
    public string Issue(IEnumerable<Claim> claims, bool isPersistent)
    {
        this.RemoveExpiredTickets();
        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        this._tickets[value] = new Ticket(claims.ToList(), isPersistent, DateTime.UtcNow + TicketLifetime);
        return value;
    }

    /// <summary>
    /// Redeems the ticket with the specified value. Each ticket can only be redeemed once.
    /// </summary>
    /// <param name="value">The ticket value.</param>
    /// <param name="ticket">The redeemed ticket.</param>
    /// <returns><c>true</c>, if the ticket was valid and could be redeemed; otherwise, <c>false</c>.</returns>
    public bool TryRedeem(string? value, out Ticket? ticket)
    {
        ticket = null;
        if (string.IsNullOrEmpty(value) || !this._tickets.TryRemove(value, out var found))
        {
            return false;
        }

        if (found.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        ticket = found;
        return true;
    }

    private void RemoveExpiredTickets()
    {
        var now = DateTime.UtcNow;
        foreach (var expired in this._tickets.Where(pair => pair.Value.ExpiresAt < now).Select(pair => pair.Key).ToList())
        {
            this._tickets.TryRemove(expired, out _);
        }
    }

    /// <summary>
    /// A ticket which can be exchanged for an authentication cookie.
    /// </summary>
    /// <param name="Claims">The claims of the authenticated user.</param>
    /// <param name="IsPersistent">A value indicating whether the resulting cookie survives a browser restart.</param>
    /// <param name="ExpiresAt">The point in time at which this ticket expires.</param>
    public record Ticket(IReadOnlyList<Claim> Claims, bool IsPersistent, DateTime ExpiresAt);
}
