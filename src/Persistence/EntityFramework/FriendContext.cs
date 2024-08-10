// <copyright file="FriendContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework.Extensions;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Context to load instances of <see cref="Friend"/>s.
/// </summary>
public class FriendContext : DbContext
{
    /// <summary>
    /// Configures the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    internal static void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friend>().ToTable("Friend", SchemaNames.Friend);
        modelBuilder.Entity<Friend>(e =>
        {
            e.HasAlternateKey(f => new { f.CharacterId, f.FriendId });
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
        ConfigureModel(modelBuilder);
        modelBuilder.Entity<CharacterName>().HasKey(f => f.Id);
        modelBuilder.UseGuidV7Ids();
    }
}