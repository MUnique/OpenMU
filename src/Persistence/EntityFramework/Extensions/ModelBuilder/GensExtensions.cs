// <copyright file="GensExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{TEntity}"/> of the gens entities.
/// </summary>
internal static class GensExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="GensMember"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<GensMember> builder)
    {
        builder.HasIndex(member => member.CharacterId).IsUnique();
        builder.HasOne<Character>()
            .WithMany()
            .HasForeignKey(member => member.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
