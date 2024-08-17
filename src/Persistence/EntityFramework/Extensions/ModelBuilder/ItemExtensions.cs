// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the item-related <see cref="EntityTypeBuilder"/>.
/// </summary>
internal static class ItemExtensions
{
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
        builder.HasMany(isg => isg.RawItems).WithOne(item => item.RawItemSetGroup!);
    }

    /// <summary>
    /// Applies the settings for the <see cref="ItemBasePowerUpDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ItemBasePowerUpDefinition> builder)
    {
        builder.Ignore(d => d.BaseValueElement);
    }

    /// <summary>
    /// Applies the settings for the <see cref="LevelBonus"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<LevelBonus> builder)
    {
    }
}