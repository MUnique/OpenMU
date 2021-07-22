// <copyright file="ContextStack.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// A stack for persistence contexts.
    /// </summary>
    internal sealed class ContextStack : IContextStack, IDisposable
    {
        private readonly IDictionary<Thread, Stack<IContext>> contextsPerThread = new Dictionary<Thread, Stack<IContext>>();

        private readonly ReaderWriterLockSlim contextLock = new ();

        /// <summary>
        /// Field to detect redundant calls.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                try
                {
                    this.contextLock.Dispose();
                }
                finally
                {
                    this.isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
        /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The disposable to end the usage.</returns>
        public IDisposable UseContext(IContext context)
        {
            Stack<IContext>? contextsOfCurrentThread;
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

        /// <summary>
        /// Gets the current context of the current thread.
        /// </summary>
        /// <returns>The current context.</returns>
        public IContext? GetCurrentContext()
        {
            Stack<IContext>? contextsOfCurrentThread;
            this.contextLock.EnterReadLock();
            try
            {
                this.contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread);
            }
            finally
            {
                this.contextLock.ExitReadLock();
            }

            if (contextsOfCurrentThread is { Count: > 0 })
            {
                return contextsOfCurrentThread.Peek();
            }

            return null;
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
            private Stack<IContext>? stack;
            private Action<Stack<IContext>>? afterPopAction;

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