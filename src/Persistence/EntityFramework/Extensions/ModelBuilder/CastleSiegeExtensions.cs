// <copyright file="CastleSiegeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for Castle Siege-related <see cref="EntityTypeBuilder"/>s.
/// </summary>
internal static class CastleSiegeExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="CastleSiegeConfiguration"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CastleSiegeConfiguration> builder)
    {
        builder.Property(configuration => configuration.CrownHoldTimeSeconds).HasDefaultValue(30);
        builder.Property(configuration => configuration.RegisterMinLevel).HasDefaultValue(200);
        builder.Property(configuration => configuration.RegisterMinMembers).HasDefaultValue(20);
        builder.Property(configuration => configuration.SignOfLordItemLevel).HasDefaultValue((byte)3);
        builder.Property(configuration => configuration.MaxAttackingGuilds).HasDefaultValue(3);

        builder.HasOne(configuration => configuration.RawCastleSiegeMapDefinition)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(configuration => configuration.RawLandOfTrialsMapDefinition)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(configuration => configuration.RawRewardItemDefinition)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(configuration => configuration.RawSignOfLordItemDefinition)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Applies the settings for the <see cref="CastleSiegeNpcDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CastleSiegeNpcDefinition> builder)
    {
        builder.HasOne(definition => definition.RawMonsterDefinition)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(definition => new { definition.MonsterDefinitionId, definition.InstanceId });
    }

    /// <summary>
    /// Applies the settings for the <see cref="CastleSiegeData"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CastleSiegeData> builder)
    {
        builder.HasOne<Guild>()
            .WithMany()
            .HasForeignKey(data => data.OwnerGuildId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Applies the settings for the <see cref="CastleSiegeNpcState"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CastleSiegeNpcState> builder)
    {
        builder.HasIndex(state => new { state.MonsterNumber, state.InstanceId }).IsUnique();
    }

    /// <summary>
    /// Applies the settings for the <see cref="CastleSiegeGuildRegistration"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<CastleSiegeGuildRegistration> builder)
    {
        builder.Property(registration => registration.GuildName).HasMaxLength(8).IsRequired();
        builder.HasIndex(registration => registration.GuildId).IsUnique();
        builder.HasOne<Guild>()
            .WithMany()
            .HasForeignKey(registration => registration.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
