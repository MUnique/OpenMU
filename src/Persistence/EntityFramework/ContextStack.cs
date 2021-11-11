// <copyright file="ContextStack.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;

/// <summary>
/// A stack for persistence contexts.
/// </summary>
internal sealed class ContextStack : IContextStack, IDisposable
{
    private readonly IDictionary<Thread, Stack<IContext>> _contextsPerThread = new Dictionary<Thread, Stack<IContext>>();

    private readonly ReaderWriterLockSlim _contextLock = new ();

    /// <summary>
    /// Field to detect redundant calls.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (!this._isDisposed)
        {
            try
            {
                this._contextLock.Dispose();
            }
            finally
            {
                this._isDisposed = true;
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
        this._contextLock.EnterWriteLock();
        try
        {
            if (!this._contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread))
            {
                contextsOfCurrentThread = new Stack<IContext>();
                this._contextsPerThread.Add(Thread.CurrentThread, contextsOfCurrentThread);
            }
        }
        finally
        {
            this._contextLock.ExitWriteLock();
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
        this._contextLock.EnterReadLock();
        try
        {
            this._contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread);
        }
        finally
        {
            this._contextLock.ExitReadLock();
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

        this._contextLock.EnterWriteLock();
        try
        {
            this._contextsPerThread.Remove(Thread.CurrentThread);
        }
        finally
        {
            this._contextLock.ExitWriteLock();
        }
    }

    private sealed class ContextPop : IDisposable
    {
        private Stack<IContext>? _stack;
        private Action<Stack<IContext>>? _afterPopAction;

        public ContextPop(Stack<IContext> stack, Action<Stack<IContext>> afterPopAction)
        {
            this._stack = stack;
            this._afterPopAction = afterPopAction;
        }

        public void Dispose()
        {
            if (this._stack != null)
            {
                this._stack.Pop();
                if (this._afterPopAction != null)
                {
                    this._afterPopAction(this._stack);
                    this._afterPopAction = null;
                }

                this._stack = null;
            }
        }
    }
}