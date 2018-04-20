// <copyright file="NullContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    /// <summary>
    /// A context which does nothing.
    /// </summary>
    public class NullContext : IContext
    {
        /// <inheritdoc/>
        public void Attach(object item)
        {
            // do nothing
        }

        /// <inheritdoc/>
        public void Detach(object item)
        {
            // do nothing
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <inheritdoc/>
        public bool SaveChanges()
        {
            // not saving anything
            return true;
        }
    }
}