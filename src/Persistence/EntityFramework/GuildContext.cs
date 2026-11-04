// <copyright file="GuildContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework.Extensions;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Context for the guild server which just uses <see cref="Guild"/>, <see cref="GuildMember"/> and <see cref="Character"/>.
/// </summary>
public class GuildContext : DbContext
{
    /// <summary>
    /// Configures the model, especially defines that <see cref="Guild"/> and <see cref="GuildMember"/> are created in a separate "guild" schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    internal static void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildMember>(member =>
        {
            member.Property(m => m.Id).ValueGeneratedNever();
            member.ToTable(nameof(GuildMember), SchemaNames.Guild);
        });

        modelBuilder.Entity<Guild>(e =>
        {
            e.Property(guild => guild.Name).HasMaxLength(8).IsRequired();
            e.HasIndex(guild => guild.Name).IsUnique();
            e.ToTable(nameof(Guild), SchemaNames.Guild);
        });
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        this.Configure(optionsBuilder);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Character>();
        ConfigureModel(modelBuilder);
        modelBuilder.Entity<GuildMember>().Ignore(m => m.Character);
        modelBuilder.Entity<CharacterName>().HasKey(f => f.Id);

        modelBuilder.UseGuidV7Ids();
    }
}