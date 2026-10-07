// <copyright file="CashShopExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for cash shop related <see cref="EntityTypeBuilder"/>s.
/// </summary>
internal static class CashShopExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="CashShopStorageItem"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CashShopStorageItem> builder)
    {
        builder.HasIndex(item => item.AccountId);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(item => item.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Applies the settings for the <see cref="CashShopCoinGrant"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CashShopCoinGrant> builder)
    {
        builder.HasIndex(grant => new { grant.AccountId, grant.AppliedAt });
        builder.HasIndex(grant => grant.Reference).IsUnique();
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(grant => grant.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
