// <copyright file="MonsterExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="MonsterDefinition"/>-related <see cref="EntityTypeBuilder"/>s.
/// </summary>
internal static class MonsterExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="MonsterDefinition"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MonsterDefinition> builder)
    {
        builder.HasMany<QuestDefinition>().WithOne(q => q.RawQuestGiver);
    }

    /// <summary>
    /// Applies the settings for the <see cref="MonsterSpawnArea"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<MonsterSpawnArea> builder)
    {
        builder.HasOne(spawn => spawn.RawMonsterDefinition);
        builder.HasOne(spawn => spawn.RawGameMap).WithMany(map => map.RawMonsterSpawns);
    }
}
