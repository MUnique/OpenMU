// <copyright file="IContextStack.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;

    /// <summary>
    /// Interface for a stack of persistence contexts.
    /// </summary>
    internal interface IContextStack
    {
        /// <summary>
        /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
        /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The disposable to end the usage.</returns>
        IDisposable UseContext(IContext context);

        /// <summary>
        /// Gets the current context of the current thread.
        /// </summary>
        /// <returns>The current context.</returns>
        IContext GetCurrentContext();
    }
}