// <copyright file="LoginTicket.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A single-use ticket, with which a game client logs into an <see cref="Account"/> without its password.
/// </summary>
/// <remarks>
/// A trusted external service, e.g. a website which signed the player in, requests the ticket and
/// hands its token to the game client, which sends it to the game server instead of a password.
/// Only the hash of the token is stored.
/// It's not part of the <see cref="Account"/> aggregate, because it's issued and redeemed while a game
/// server may hold the account in memory. That's why it refers to its account by identifier.
/// </remarks>
[AggregateRoot]
public class LoginTicket
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the account, into which the ticket logs in.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the token.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp at which the ticket was issued.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp until which the ticket can be redeemed.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the ticket was redeemed; <c>null</c>, if it wasn't redeemed yet.
    /// </summary>
    public DateTime? UsedAt { get; set; }
}
