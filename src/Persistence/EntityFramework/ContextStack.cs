// <copyright file="ContextStack.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections.Immutable;
using System.Threading;

/// <summary>
/// A stack for persistence contexts.
/// </summary>
internal sealed class ContextStack : IContextStack
{
    /// <summary>
    /// The stack of the current asynchronous flow.
    /// It's immutable, because a flow which is forked from the current one (e.g. by starting a task)
    /// would otherwise share and modify the same stack instance.
    /// </summary>
    private readonly AsyncLocal<ImmutableStack<IContext>?> _localStack = new();

    /// <summary>
    /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
    /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The disposable to end the usage.</returns>
    public IDisposable UseContext(IContext context)
    {
        var previousStack = this._localStack.Value;
        this._localStack.Value = (previousStack ?? ImmutableStack<IContext>.Empty).Push(context);
        return new ContextPop(this, previousStack);
    }

    /// <summary>
    /// Gets the current context of the current thread.
    /// </summary>
    /// <returns>The current context.</returns>
    public IContext? GetCurrentContext()
    {
        if (this._localStack.Value is { IsEmpty: false } contextsOfCurrentFlow)
        {
            return contextsOfCurrentFlow.Peek();
        }

        return null;
    }

    private sealed class ContextPop : IDisposable
    {
        private readonly ImmutableStack<IContext>? _previousStack;

        private ContextStack? _contextStack;

        public ContextPop(ContextStack contextStack, ImmutableStack<IContext>? previousStack)
        {
            this._contextStack = contextStack;
            this._previousStack = previousStack;
        }

        public void Dispose()
        {
            if (this._contextStack != null)
            {
                this._contextStack._localStack.Value = this._previousStack;
                this._contextStack = null;
            }
        }
    }
}