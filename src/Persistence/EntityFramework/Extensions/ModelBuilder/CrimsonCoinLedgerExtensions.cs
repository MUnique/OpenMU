// <copyright file="CrimsonCoinLedgerExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{CrimsonCoinLedger}"/>.
/// </summary>
/// <remarks>
/// Crimson Vigil custom entity. Lives in the fork because OpenMU upstream
/// does not have a coin / wallet concept. Mirrors the extension-method
/// pattern used by <c>AccountExtensions</c> and <c>CastleSiegeExtensions</c>.
/// </remarks>
internal static class CrimsonCoinLedgerExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="CrimsonCoinLedger"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CrimsonCoinLedger> builder)
    {
        builder.Property(ledger => ledger.Reason).HasMaxLength(100).IsRequired();
        builder.Property(ledger => ledger.Note).HasMaxLength(500);

        builder.HasIndex(ledger => ledger.AccountId);
        builder.HasIndex(ledger => ledger.CreatedAt);

        builder.HasOne(ledger => ledger.Account!)
            .WithMany()
            .HasForeignKey(ledger => ledger.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}