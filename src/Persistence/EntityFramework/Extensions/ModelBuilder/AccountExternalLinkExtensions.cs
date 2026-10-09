// <copyright file="AccountExternalLinkExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder"/> of the <see cref="AccountExternalLink"/>.
/// </summary>
internal static class AccountExternalLinkExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="AccountExternalLink"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<AccountExternalLink> builder)
    {
        builder.Property(link => link.Provider).IsRequired();
        builder.HasIndex(link => new { link.AccountId, link.Provider }).IsUnique();
        builder.HasIndex(link => new { link.Provider, link.ExternalUserId }).IsUnique();
        builder.HasIndex(link => link.CodeHash);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(link => link.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
