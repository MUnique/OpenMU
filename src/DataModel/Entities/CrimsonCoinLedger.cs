// <copyright file="CrimsonCoinLedger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Append-only ledger entry for the Crimson Coins wallet.
///
/// The current balance for an account is <c>SUM(Amount)</c> over all
/// ledger rows for that account — there is no <c>Balance</c> column by
/// design. Append-only lets us:
///   * reconstruct any historical balance at any point in time
///   * audit every credit and debit (who, why, when)
///   * detect fraud (impossible amounts, impossible timing)
///   * swap the funding model (admin manual → Mercado Pago → PayPal)
///     without migrating the schema — new funding sources just write
///     entries with new <see cref="Reason"/> prefixes.
///
/// Reason taxonomy (colon-separated, scoped by source):
///   <c>admin:discord:credit</c>        — operator manually credited, paid via Discord.
///   <c>admin:topup:adjustment</c>      — operator adjustment (fraud reversal, goodwill).
///   <c>shop:purchase:{itemSlug}</c>    — user bought an item.
///   <c>topup:mp:{mpTxnId}</c>          — Mercado Pago top-up (v2).
///   <c>topup:paypal:{paypalTxnId}</c>  — PayPal top-up (v2).
/// </summary>
[AggregateRoot]
public class CrimsonCoinLedger
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to <see cref="Account"/>.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the navigation back to <see cref="Account"/>.
    /// Marked <c>[MemberOfAggregate]</c> so EF treats the relationship
    /// as a join column rather than loading the aggregate. This is what
    /// lets the migration emit a FK constraint instead of an owned type.
    /// </summary>
    [MemberOfAggregate]
    public virtual Account? Account { get; set; }

    /// <summary>
    /// Gets or sets the signed coin delta. Positive = credit, negative = debit.
    /// </summary>
    [Required]
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the structured reason for the entry. See class-level XML doc
    /// for the taxonomy. Always a colon-separated token, never free-form prose.
    /// </summary>
    [Required]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional human-readable note (operator commentary, external
    /// reference id, etc.). Free-form. Never used for control flow.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp (UTC). Indexable for
    /// "recent activity" queries.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
