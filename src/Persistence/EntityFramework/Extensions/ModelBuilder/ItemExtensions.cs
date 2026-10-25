// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the item-related <see cref="EntityTypeBuilder"/>.
/// </summary>
internal static class ItemExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="ItemDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemDefinition> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);

        // A stat profile which items still use can't be deleted.
        builder.HasOne(definition => definition.RawStatProfile)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemStatProfile"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemStatProfile> builder)
    {
        // The items and the item formulas find a profile by its name, so it's unique within a configuration.
        builder.HasIndex("GameConfigurationId", nameof(ItemStatProfile.Name)).IsUnique();
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemStatTerms"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemStatTerms> builder)
    {
        // A profile writes each stat at most once.
        builder.HasIndex("ItemStatProfileId", "AttributeId").IsUnique();
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemRequirementFormula"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemRequirementFormula> builder)
    {
        // A profile writes each stat requirement at most once.
        builder.HasIndex("ItemStatProfileId", "AttributeId").IsUnique();
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemStorage"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemStorage> builder)
    {
        builder.HasMany(storage => storage.RawItems).WithOne(item => item.RawItemStorage!);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemSetGroup"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemSetGroup> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
        builder.HasMany(isg => isg.RawItems).WithOne(item => item.RawItemSetGroup!);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemLevelBonusTable"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemLevelBonusTable> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemPriceDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemPriceDefinition> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemDropItemGroup"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemDropItemGroup> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="LevelBonus"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<LevelBonus> builder)
    {
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemSlotType"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemSlotType> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemOptionCombinationBonus"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemOptionCombinationBonus> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemOptionDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemOptionDefinition> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemOptionType"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemOptionType> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemCrafting"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemCrafting> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="DropItemGroup"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<DropItemGroup> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }
}