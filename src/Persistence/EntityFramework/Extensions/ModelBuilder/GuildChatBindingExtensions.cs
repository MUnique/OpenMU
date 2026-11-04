// <copyright file="GuildChatBindingExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder"/> of the <see cref="GuildChatBinding"/>.
/// </summary>
internal static class GuildChatBindingExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="GuildChatBinding"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<GuildChatBinding> builder)
    {
        builder.Property(binding => binding.Provider).IsRequired();
        builder.Property(binding => binding.ExternalServerId).IsRequired();
        builder.Property(binding => binding.ExternalChannelId).IsRequired();
        builder.HasIndex(binding => new { binding.GuildId, binding.Scope, binding.Provider }).IsUnique();
        builder.HasIndex(binding => new { binding.Provider, binding.ExternalChannelId }).IsUnique();
        builder.HasOne<Guild>()
            .WithMany()
            .HasForeignKey(binding => binding.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
