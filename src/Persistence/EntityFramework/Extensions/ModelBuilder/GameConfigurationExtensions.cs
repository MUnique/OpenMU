// <copyright file="GameConfigurationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{GameConfiguration}"/>.
/// </summary>
internal static class GameConfigurationExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="GameConfiguration"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<GameConfiguration> builder)
    {
        builder.Property(c => c.ItemDropDuration).HasDefaultValue(TimeSpan.FromSeconds(60));
        builder.Property(c => c.ExperienceFormula).HasDefaultValue("if(level == 0, 0, if(level < 256, 10 * (level + 8) * (level - 1) * (level - 1), (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256))))");
        builder.Property(c => c.MasterExperienceFormula).HasDefaultValue("(505 * level * level * level) + (35278500 * level) + (228045 * level * level)");
    }
}