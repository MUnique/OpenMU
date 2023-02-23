// <copyright file="GameMapDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{GameMapDefinition}"/>.
/// </summary>
internal static class GameMapDefinitionExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="GameMapDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<GameMapDefinition> builder)
    {
        builder.HasMany(map => map.RawEnterGates);
        builder.HasMany(map => map.RawExitGates).WithOne(g => g.RawMap);
        builder.HasOne(map => map.RawSafezoneMap);
        builder.HasMany(map => map.RawMonsterSpawns);
    }
}