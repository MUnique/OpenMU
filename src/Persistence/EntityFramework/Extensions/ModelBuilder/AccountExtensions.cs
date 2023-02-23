// <copyright file="AccountExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{Account}"/>.
/// </summary>
internal static class AccountExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="Account"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<Account> builder)
    {
        builder.Property(account => account.LoginName).HasMaxLength(10).IsRequired();
        builder.HasIndex(account => account.LoginName).IsUnique();
    }
}