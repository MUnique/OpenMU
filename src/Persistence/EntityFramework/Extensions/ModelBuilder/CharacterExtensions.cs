// <copyright file="CharacterExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{Account}"/>.
/// </summary>
internal static class CharacterExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="Character"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<Character> builder)
    {
        builder.Property(character => character.Name).HasMaxLength(10).IsRequired();
        builder.HasIndex(character => character.Name).IsUnique();

        if (builder.Metadata.FindNavigation(nameof(Character.RawCharacterClass)) is { } navigation)
        {
            navigation.ForeignKey.IsRequired = true;
        }

        builder.Property(character => character.CharacterSlot).IsRequired();
        builder.HasMany(character => character.RawLetters).WithOne(letter => letter.Receiver!).OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Applies the settings for the <see cref="CharacterClass"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CharacterClass> builder)
    {
        builder.HasMany(c => c.RawBaseAttributeValues)
            .WithOne(c => c.CharacterClass!);
    }
}