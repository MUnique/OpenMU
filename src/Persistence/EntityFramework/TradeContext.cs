// <copyright file="TradeContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A context which is used for trades.
    /// </summary>
    /// <remarks>
    /// In an ideal world, it would just need to track two properties: <see cref="Item.ItemStorage"/> and <see cref="DataModel.Entities.Item.ItemSlot"/>.
    /// </remarks>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    internal class TradeContext : AccountContext
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<ItemStorage>();
        }
    }
}