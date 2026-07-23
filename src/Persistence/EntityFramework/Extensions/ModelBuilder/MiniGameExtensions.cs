// <copyright file="MiniGameExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="MiniGameDefinition"/>-related <see cref="EntityTypeBuilder"/>.
/// </summary>
internal static class MiniGameExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="MiniGameChangeEvent"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MiniGameChangeEvent> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
        builder.Property(p => p.Message).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="MiniGameSpawnWave"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MiniGameSpawnWave> builder)
    {
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
        builder.Property(p => p.Message).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="MiniGameDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MiniGameDefinition> builder)
    {
        builder.Property(p => p.Name).HasConversion(LocalizedStringConverter.Instance);
        builder.Property(p => p.Description).HasConversion(LocalizedStringConverter.Instance);
    }

    /// <summary>
    /// Applies the settings for the <see cref="MiniGameRankingEntry"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <remarks>
    /// A ranking entry is a member of neither the character's nor the mini game definition's
    /// aggregate. The model generator only emits a cascading delete for navigations marked with
    /// <see cref="MUnique.OpenMU.DataModel.Composition.MemberOfAggregateAttribute"/>, so these two
    /// references are left without a delete behavior; Entity Framework then defaults to no action
    /// for them, and the database refuses to delete a character which has played a mini game.
    /// </remarks>
    public static void Apply(this EntityTypeBuilder<MiniGameRankingEntry> builder)
    {
        builder.HasOne(entry => entry.RawCharacter).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(entry => entry.RawMiniGame).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}