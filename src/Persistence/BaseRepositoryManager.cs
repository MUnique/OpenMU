// <copyright file="BaseRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The base repository manager.
    /// </summary>
    public class BaseRepositoryManager : IRepositoryManager, IDisposable
    {
        private readonly IDictionary<Thread, Stack<IContext>> contextsPerThread = new Dictionary<Thread, Stack<IContext>>();

        private readonly ReaderWriterLockSlim contextLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Field to detect redundant calls.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Gets the repositories for each entity type.
        /// </summary>
        protected IDictionary<Type, object> Repositories { get; } = new Dictionary<Type, object>();

        /// <inheritdoc/>
        public virtual IRepository<T> GetRepository<T>()
        {
            return this.GetRepository(typeof(T)) as IRepository<T>;
        }

        /// <inheritdoc/>
        public virtual IRepository GetRepository(Type objectType)
        {
            var repository = this.InternalGetRepository(objectType);
            if (repository == null)
            {
                throw new RepositoryNotFoundException(objectType);
            }

            return repository;
        }

        /// <inheritdoc/>
        public virtual TRepository GetRepository<T, TRepository>()
            where TRepository : class
        {
            var repository = this.GetRepository<T>() as TRepository;
            if (repository == null)
            {
                throw new RepositoryNotFoundException(typeof(T), typeof(TRepository));
            }

            return repository;
        }

        /// <inheritdoc/>
        public virtual IContext CreateNewContext()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual IContext CreateNewContext(GameConfiguration gameConfiguration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual IContext CreateNewAccountContext(GameConfiguration gameConfiguration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual IContext CreateNewConfigurationContext()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IDisposable UseContext(IContext context)
        {
            Stack<IContext> contextsOfCurrentThread;
            this.contextLock.EnterWriteLock();
            try
            {
                if (!this.contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread))
                {
                    contextsOfCurrentThread = new Stack<IContext>();
                    this.contextsPerThread.Add(Thread.CurrentThread, contextsOfCurrentThread);
                }
            }
            finally
            {
                this.contextLock.ExitWriteLock();
            }

            contextsOfCurrentThread.Push(context);
            return new ContextPop(contextsOfCurrentThread, this.AfterPop);
        }

        /// <inheritdoc/>
        public IContext GetCurrentContext()
        {
            Stack<IContext> contextsOfCurrentThread;
            this.contextLock.EnterReadLock();
            try
            {
                this.contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread);
            }
            finally
            {
                this.contextLock.ExitReadLock();
            }

            if (contextsOfCurrentThread != null && contextsOfCurrentThread.Count > 0)
            {
                return contextsOfCurrentThread.Peek();
            }

            return null;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        public virtual T CreateNew<T>(params object[] args)
            where T : class
        {
            if (args.Length == 0)
            {
                Activator.CreateInstance<T>();
            }

            return this.CreateNew(typeof(T), args) as T;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                try
                {
                    this.Dispose(true);
                }
                finally
                {
                    this.isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.contextLock.Dispose();
            }
        }

        /// <summary>
        /// Gets the repository of the specified type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The repository of the specified type.</returns>
        protected IRepository InternalGetRepository(Type objectType)
        {
            Type currentSearchType = objectType;
            do
            {
                if (currentSearchType == null)
                {
                    break;
                }

                if (this.Repositories.TryGetValue(currentSearchType, out object repository))
                {
                    return repository as IRepository;
                }

                currentSearchType = currentSearchType.BaseType;
            }
            while (currentSearchType != typeof(object));

            return null;
        }

        /// <summary>
        /// Creates a new instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>A new instance of <paramref name="type"/>.</returns>
        protected object CreateNew(Type type, params object[] args)
        {
            // TODO: Optimize
            var types = args.Select(a => a.GetType()).ToArray();
            var constructor = type.GetConstructor(types);
            if (constructor != null)
            {
                return constructor.Invoke(args);
            }

            throw new ArgumentException($"No constructor for {type} could be found for the given parameters.");
        }

        /// <summary>
        /// Registers the repository.
        /// </summary>
        /// <typeparam name="T">The generic type which the repository handles.</typeparam>
        /// <param name="repository">The repository.</param>
        protected void RegisterRepository<T>(IRepository<T> repository)
        {
            this.RegisterRepository(typeof(T), repository);
        }

        /// <summary>
        /// Registers the repository.
        /// </summary>
        /// <param name="type">The generic type which the repository handles.</param>
        /// <param name="repository">The repository.</param>
        protected virtual void RegisterRepository(Type type, IRepository repository)
        {
            this.Repositories.Add(type, repository);
        }

        private void AfterPop(Stack<IContext> context)
        {
            if (context.Count > 0)
            {
                return;
            }

            this.contextLock.EnterWriteLock();
            try
            {
                this.contextsPerThread.Remove(Thread.CurrentThread);
            }
            finally
            {
                this.contextLock.ExitWriteLock();
            }
        }

        private sealed class ContextPop : IDisposable
        {
            private Stack<IContext> stack;
            private Action<Stack<IContext>> afterPopAction;

            public ContextPop(Stack<IContext> stack, Action<Stack<IContext>> afterPopAction)
            {
                this.stack = stack;
                this.afterPopAction = afterPopAction;
            }

            public void Dispose()
            {
                if (this.stack != null)
                {
                    this.stack.Pop();
                    if (this.afterPopAction != null)
                    {
                        this.afterPopAction(this.stack);
                        this.afterPopAction = null;
                    }

                    this.stack = null;
                }
            }
        }
    }
}
