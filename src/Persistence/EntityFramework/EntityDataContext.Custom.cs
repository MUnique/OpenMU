// <copyright file="EntityDataContext.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Crimson Vigil custom additions to <see cref="EntityDataContext"/>.
/// <para>
/// Owns the fork's DbSet properties and any future persistence-layer
/// extensions. Keeping them in a partial file isolates our changes from
/// upstream so <c>git pull</c> from MUnique/OpenMU no longer produces
/// conflicts on the hand-mained <c>EntityDataContext.cs</c>.
/// </para>
/// <para>
/// The two <c>modelBuilder.Entity&lt;T&gt;().Apply()</c> calls for
/// <see cref="CrimsonCoinLedger"/> and <see cref="ShopItem"/> intentionally
/// stay inside the upstream <c>OnModelCreating</c> body. Moving them out
/// would require overriding the entire method, which means re-listing every
/// upstream entity — exactly the kind of merge friction this refactor was
/// supposed to remove. Two lines, in known positions, easy to re-apply if
/// a conflict ever surfaces.
/// </para>
/// </summary>
public partial class EntityDataContext
{
    /// <summary>
    /// Gets the Crimson Coins ledger rows. Append-only; balance is
    /// <c>SUM(Amount)</c> per account, no <c>Balance</c> column.
    /// </summary>
    internal DbSet<CrimsonCoinLedger> CrimsonCoinLedger => this.Set<CrimsonCoinLedger>();

    /// <summary>
    /// Gets the shop catalog. <c>Stock = -1</c> means unlimited
    /// (digital goods, donation tiers).
    /// </summary>
    internal DbSet<ShopItem> ShopItems => this.Set<ShopItem>();
}