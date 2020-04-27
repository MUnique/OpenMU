// <copyright file="EntityFrameworkContextBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Abstract base class for an <see cref="IContext"/> which uses an <see cref="DbContext"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.IContext" />
    public class EntityFrameworkContextBase : IContext
    {
        private readonly bool isOwner;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkContextBase" /> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="isOwner">If set to <c>true</c>, this instance owns the <see cref="Context"/>. That means it will be disposed when this instance will be disposed.</param>
        protected EntityFrameworkContextBase(DbContext context, RepositoryManager repositoryManager, bool isOwner)
        {
            this.Context = context;
            this.RepositoryManager = repositoryManager;
            this.isOwner = isOwner;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="EntityFrameworkContextBase"/> class.
        /// </summary>
        ~EntityFrameworkContextBase() => this.Dispose(false);

        /// <summary>
        /// Gets the entity framework context.
        /// </summary>
        internal DbContext Context { get; }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        protected RepositoryManager RepositoryManager { get; }

        /// <inheritdoc/>
        public bool SaveChanges()
        {
            this.Context.SaveChanges();
            return true;
        }

        /// <inheritdoc />
        public bool Detach(object item)
        {
            var entry = this.Context.Entry(item);
            if (entry == null)
            {
                return false;
            }

            var previousState = entry.State;
            entry.State = EntityState.Detached;
            this.ForEachAggregate(item, obj => this.Detach(obj));

            return previousState != EntityState.Added;
        }

        /// <inheritdoc />
        public void Attach(object item)
        {
            this.Context.Attach(item);
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments which are handed 1-to-1 to the constructor. If no arguments are given, the default constructor will be called.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        public T CreateNew<T>(params object[] args)
            where T : class
        {
            var instance = typeof(CachingEntityFrameworkContext).Assembly.CreateNew<T>(args);
            if (instance != null)
            {
                this.Context.Add(instance);
            }

            return instance;
        }

        /// <inheritdoc/>
        public bool Delete<T>(T obj)
            where T : class
        {
            var result = this.Context.Remove(obj) is { };
            if (result)
            {
                this.ForEachAggregate(obj, a => this.Context.Remove(a));
            }

            return result;
        }

        /// <inheritdoc/>
        public T GetById<T>(Guid id)
            where T : class
        {
            using var context = this.RepositoryManager.ContextStack.UseContext(this);
            return this.RepositoryManager.GetRepository<T>().GetById(id);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Get<T>()
            where T : class
        {
            using var context = this.RepositoryManager.ContextStack.UseContext(this);
            return this.RepositoryManager.GetRepository<T>().GetAll();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.Dispose(true);
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="dispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool dispose)
        {
            if (dispose && this.isOwner)
            {
                this.Context.Dispose();
            }
        }

        private void ForEachAggregate(object obj, Action<object> action)
        {
            var aggregateProperties = obj.GetType()
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<MemberOfAggregateAttribute>() is { }
                            || p.Name.StartsWith("Joined"));
            foreach (var propertyInfo in aggregateProperties)
            {
                var propertyValue = propertyInfo.GetMethod.Invoke(obj, Array.Empty<object>());
                if (propertyValue is IEnumerable enumerable)
                {
                    foreach (var value in enumerable)
                    {
                        action(value);
                    }
                }
                else if (propertyValue is { })
                {
                    action(propertyValue);
                }
            }
        }
    }
}