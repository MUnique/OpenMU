// <copyright file="RepositoryManagerExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Extensions for the repository manager.
    /// </summary>
    public static class RepositoryManagerExtensions
    {
        /// <summary>
        /// Uses the a temporary context.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>A temporary context</returns>
        public static IContext UseTemporaryContext(this IRepositoryManager manager)
        {
            return new TemporaryContextWrapper(manager, manager.CreateNewContext());
        }

        /// <summary>
        /// Uses the temporary configuration context.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>The temporary configuration context.</returns>
        public static IContext UseTemporaryConfigurationContext(this IRepositoryManager manager)
        {
            return new TemporaryContextWrapper(manager, manager.CreateNewConfigurationContext());
        }

        /// <summary>
        /// Uses the a temporary context.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// A temporary context
        /// </returns>
        public static IContext UseTemporaryContext(this IRepositoryManager manager, GameConfiguration gameConfiguration)
        {
            return new TemporaryContextWrapper(manager, manager.CreateNewContext(gameConfiguration));
        }

        private class TemporaryContextWrapper : IContext
        {
            private readonly IDisposable managerUsage;
            private readonly IContext context;

            public TemporaryContextWrapper(IRepositoryManager manager, IContext context)
            {
                this.managerUsage = manager.UseContext(context);
                this.context = context;
            }

            public void Dispose()
            {
                this.managerUsage.Dispose();
                this.context.Dispose();
            }

            public bool SaveChanges()
            {
                return this.context.SaveChanges();
            }

            public void Detach(object item)
            {
                this.context.Detach(item);
            }

            public void Attach(object item)
            {
                this.context.Attach(item);
            }
        }
    }
}