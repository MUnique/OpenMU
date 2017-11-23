// <copyright file="FriendSaveContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Context which is used to save instances of <see cref="FriendViewItem"/>.
    /// </summary>
    /// <remarks>
    /// This separate context is needed, because when saving the name properties should not be saved,
    /// because they are not present in the database and loaded from the character table.
    /// </remarks>
    public class FriendSaveContext : FriendContext
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FriendViewItem>(entity =>
            {
                entity.Ignore(f => f.CharacterName);
                entity.Ignore(f => f.FriendName);
            });
        }
    }
}
