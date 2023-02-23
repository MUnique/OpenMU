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

        // TODO:
        builder.Ignore(c => c.ExperienceTable)
            .Ignore(c => c.MasterExperienceTable);
    }
}