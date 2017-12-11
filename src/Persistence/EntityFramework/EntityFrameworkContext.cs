// <copyright file="EntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Implementation of <see cref="IContext"/> for the entity framework <see cref="RepositoryManager"/>.
    /// </summary>
    internal sealed class EntityFrameworkContext : IContext
    {
        private readonly bool isOwner;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isOwner">if set to <c>true</c> this instance owns the <paramref name="context" />.</param>
        public EntityFrameworkContext(DbContext context, bool isOwner = true)
        {
            this.Context = context;
            this.isOwner = isOwner;
        }

        /// <summary>
        /// Gets the entity framework context.
        /// </summary>
        public DbContext Context { get; private set; }

        /// <inheritdoc/>
        public bool SaveChanges()
        {
            this.Context.SaveChanges();
            return true;
        }

        /// <inheritdoc />
        public void Detach(object item)
        {
            var entry = this.Context.Entry(item);
            if (entry == null)
            {
                return;
            }

            entry.State = EntityState.Detached;
        }

        /// <inheritdoc />
        public void Attach(object item)
        {
            this.Context.Attach(item);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.Context != null && this.isOwner)
            {
                this.Context.Dispose();
            }

            this.Context = null;
        }
    }
}
