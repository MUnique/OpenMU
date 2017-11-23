// <copyright file="IContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;

    /// <summary>
    /// The context for repository actions.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Saves the changes of the context.
        /// </summary>
        /// <returns><c>True</c>, if the saving was successful; <c>false</c>, otherwise.</returns>
        bool SaveChanges();
    }
}