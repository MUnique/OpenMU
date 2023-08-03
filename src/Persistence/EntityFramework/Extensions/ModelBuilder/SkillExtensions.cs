// <copyright file="SkillExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="Skill"/>-related <see cref="EntityTypeBuilder"/>.
/// </summary>
internal static class SkillExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="SkillEntry"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<SkillEntry> builder)
    {
        builder.Ignore(s => s.PowerUps);
        builder.Ignore(s => s.PowerUpDuration);
    }

    /// <summary>
    /// Applies the settings for the <see cref="MasterSkillDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MasterSkillDefinition> builder)
    {
        builder.HasOne(s => s.RawRoot);
    }
}