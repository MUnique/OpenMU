// <copyright file="LoginTicketExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder"/> of the <see cref="LoginTicket"/>.
/// </summary>
internal static class LoginTicketExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="LoginTicket"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<LoginTicket> builder)
    {
        builder.Property(ticket => ticket.TokenHash).IsRequired();
        builder.HasIndex(ticket => ticket.TokenHash).IsUnique();
        builder.HasIndex(ticket => ticket.ExpiresAt);
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(ticket => ticket.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
