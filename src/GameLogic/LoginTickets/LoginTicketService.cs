// <copyright file="LoginTicketService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.LoginTickets;

using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Issues and redeems login tickets, which are stored with the hash of their token.
/// </summary>
/// <remarks>
/// Each method uses its own short-lived persistence context, like the <see cref="AccountLinking.AccountLinkService"/>,
/// so that it doesn't interfere with the context of a player which holds the account.
/// </remarks>
public sealed class LoginTicketService : ILoginTicketService
{
    /// <summary>
    /// The length of a token: 256 random bits as base64url without padding.
    /// </summary>
    public static int TokenLength { get; } = 43;

    private const int TokenBytes = 32;

    private readonly Func<IPlayerContext> _createContext;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginTicketService"/> class.
    /// </summary>
    /// <param name="createContext">The function which creates a new persistence context.</param>
    /// <param name="lifetime">The time in which a ticket has to be redeemed; <see cref="DefaultLifetime"/>, if it's <c>null</c>.</param>
    /// <param name="timeProvider">The time provider.</param>
    public LoginTicketService(Func<IPlayerContext> createContext, TimeSpan? lifetime = null, TimeProvider? timeProvider = null)
    {
        if (lifetime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(lifetime), "The lifetime of a ticket must be positive.");
        }

        this._createContext = createContext;
        this.Lifetime = lifetime ?? DefaultLifetime;
        this._timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Gets the default time in which a ticket has to be redeemed.
    /// </summary>
    public static TimeSpan DefaultLifetime { get; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// Gets the maximum number of tickets which an account can get within one <see cref="Lifetime"/>.
    /// </summary>
    public static int MaximumTicketsPerLifetime => 5;

    /// <summary>
    /// Gets the time in which a ticket has to be redeemed.
    /// </summary>
    public TimeSpan Lifetime { get; }

    /// <inheritdoc />
    public async ValueTask<IssuedLoginTicket?> IssueAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var now = this._timeProvider.GetUtcNow().UtcDateTime;
        using var context = this._createContext.Invoke();

        // Cleaned up while issuing, so the table only holds the tickets of the last lifetime.
        await context.DeleteExpiredLoginTicketsAsync(now, cancellationToken).ConfigureAwait(false);
        if (await context.CountUnexpiredLoginTicketsAsync(accountId, now, cancellationToken).ConfigureAwait(false) >= MaximumTicketsPerLifetime)
        {
            return null;
        }

        var token = System.Buffers.Text.Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenBytes));
        var ticket = context.CreateNew<LoginTicket>();
        ticket.AccountId = accountId;
        ticket.TokenHash = HashToken(token);
        ticket.CreatedAt = now;
        ticket.ExpiresAt = now.Add(this.Lifetime);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new IssuedLoginTicket(token, ticket.ExpiresAt);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The ticket is looked up by the hash of the token, so the duration of the lookup doesn't tell an attacker
    /// how much of a guessed token was right, like at the <c>ApiKeyRegistry</c>.
    /// </remarks>
    public async ValueTask<LoginTicketRedemption> RedeemAsync(string token, CancellationToken cancellationToken = default)
    {
        if (!IsWellFormed(token))
        {
            return new LoginTicketRedemption(LoginTicketRedeemStatus.Unknown, Guid.Empty);
        }

        var now = this._timeProvider.GetUtcNow().UtcDateTime;
        using var context = this._createContext.Invoke();
        if (await context.GetLoginTicketByHashAsync(HashToken(token), cancellationToken).ConfigureAwait(false) is not { } ticket)
        {
            return new LoginTicketRedemption(LoginTicketRedeemStatus.Unknown, Guid.Empty);
        }

        if (ticket.UsedAt is not null)
        {
            return new LoginTicketRedemption(LoginTicketRedeemStatus.AlreadyUsed, Guid.Empty);
        }

        if (ticket.ExpiresAt <= now)
        {
            return new LoginTicketRedemption(LoginTicketRedeemStatus.Expired, Guid.Empty);
        }

        // Lost against a concurrent redeemer of the same ticket.
        if (!await context.TryUseLoginTicketAsync(ticket.Id, now, cancellationToken).ConfigureAwait(false))
        {
            return new LoginTicketRedemption(LoginTicketRedeemStatus.AlreadyUsed, Guid.Empty);
        }

        return new LoginTicketRedemption(LoginTicketRedeemStatus.Redeemed, ticket.AccountId);
    }

    private static bool IsWellFormed(string token)
    {
        return token.Length == TokenLength && System.Buffers.Text.Base64Url.IsValid(token);
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.ASCII.GetBytes(token)));
    }
}
