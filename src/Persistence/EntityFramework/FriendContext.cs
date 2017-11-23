// <copyright file="FriendContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Context to load instances of <see cref="FriendViewItem"/>.
    /// </summary>
    public class FriendContext : DbContext
    {
        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            this.Configure(optionsBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FriendViewItem>(e =>
            {
                e.HasAlternateKey(f => new { f.CharacterId, f.FriendId });
            });
        }
    }
}
