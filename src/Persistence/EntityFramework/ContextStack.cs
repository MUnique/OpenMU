// <copyright file="ContextStack.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;

/// <summary>
/// A stack for persistence contexts.
/// </summary>
internal sealed class ContextStack : IContextStack
{
    private readonly AsyncLocal<Stack<IContext>> _localStack = new();

    /// <summary>
    /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
    /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The disposable to end the usage.</returns>
    public IDisposable UseContext(IContext context)
    {
        var contextsOfCurrentThread = this._localStack.Value ??= new Stack<IContext>();
        contextsOfCurrentThread.Push(context);
        return new ContextPop(contextsOfCurrentThread);
    }

    /// <summary>
    /// Gets the current context of the current thread.
    /// </summary>
    /// <returns>The current context.</returns>
    public IContext? GetCurrentContext()
    {
        var contextsOfCurrentThread = this._localStack.Value ??= new Stack<IContext>();
        if (contextsOfCurrentThread is { Count: > 0 })
        {
            return contextsOfCurrentThread.Peek();
        }

        return null;
    }

    private sealed class ContextPop : IDisposable
    {
        private Stack<IContext>? _stack;

        public ContextPop(Stack<IContext> stack)
        {
            this._stack = stack;
        }

        public void Dispose()
        {
            if (this._stack != null)
            {
                this._stack.Pop();
                this._stack = null;
            }
        }
    }
}