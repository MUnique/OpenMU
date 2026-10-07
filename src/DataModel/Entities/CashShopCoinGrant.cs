// <copyright file="CashShopCoinGrant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A grant of cash shop coins to an account, e.g. by an administrator or a payment provider.
/// </summary>
/// <remarks>
/// The balances of an account are only changed by the game server, which may hold the account in memory.
/// That's why other services add a grant instead, which the game server applies to the balance of the
/// account and marks as applied in the same save. The applied grants remain as history.
/// Like the cash shop storage, it's not part of the <see cref="Account"/> aggregate and refers to its
/// account by identifier.
/// </remarks>
[AggregateRoot]
public class CashShopCoinGrant
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the account which gets the coins.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the type of the granted coins.
    /// </summary>
    public CashShopCoinType CoinType { get; set; }

    /// <summary>
    /// Gets or sets the granted amount. A negative amount takes coins, but not below zero.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the reason, e.g. the purchase or the event for which the coins are granted.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the name of the administrator or API client which granted the coins.
    /// </summary>
    public string? GrantedBy { get; set; }

    /// <summary>
    /// Gets or sets an optional reference of the granting system, e.g. the identifier of a payment.
    /// A reference can only be granted once, so that a repeated request doesn't grant the coins twice.
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the coins were granted.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp at which the game server applied the grant to the balance of the account;
    /// <c>null</c>, if it's not applied yet.
    /// </summary>
    public DateTime? AppliedAt { get; set; }
}
