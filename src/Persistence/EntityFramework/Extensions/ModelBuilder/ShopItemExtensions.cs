// <copyright file="ShopItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{ShopItem}"/>.
/// </summary>
/// <remarks>
/// Crimson Vigil custom entity. Stock and availability live in the same
/// Postgres as the ledger so the purchase flow can debit balance + decrement
/// stock + write the ledger row in a single transaction (atomicity beats
/// splitting this across two systems like Slacks or Strapi).
/// </remarks>
internal static class ShopItemExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="ShopItem"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ShopItem> builder)
    {
        builder.Property(item => item.Name).HasMaxLength(120).IsRequired();
        builder.Property(item => item.Slug).HasMaxLength(120).IsRequired();
        builder.Property(item => item.ShortDescription).HasMaxLength(200).IsRequired();
        builder.Property(item => item.Category).HasMaxLength(40).IsRequired();
        builder.Property(item => item.GameItemId).HasMaxLength(80);

        builder.HasIndex(item => item.Slug).IsUnique();
        builder.HasIndex(item => item.Category);
        builder.HasIndex(item => new { item.Available, item.Featured, item.SortOrder });

        builder.HasOne(item => item.ItemDefinition!)
            .WithMany()
            .HasForeignKey(item => item.ItemDefinitionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}