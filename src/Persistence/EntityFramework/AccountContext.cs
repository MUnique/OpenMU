// <copyright file="AccountContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A context which is used by each account. This context does not track and save configuration data.
    /// </summary>
    internal class AccountContext : EntityDataContext
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var types = modelBuilder.Model.GetEntityTypes().ToList();
            var configTypes = types.Where(t => t.ClrType.BaseType?.Namespace?.Contains("Configuration") ?? (t.ClrType.BaseType?.Namespace?.Contains("AttributeSystem") ?? false)).ToList();
            foreach (var type in configTypes)
            {
                modelBuilder.Ignore(type.ClrType);
            }

            modelBuilder.Ignore(typeof(AttributeDefinition));

            modelBuilder.Ignore<Guild>();
            modelBuilder.Ignore<GuildMember>();
            modelBuilder.Entity<Item>(b => b.Ignore(e => e.RawItemStorage));
        }
    }
}
