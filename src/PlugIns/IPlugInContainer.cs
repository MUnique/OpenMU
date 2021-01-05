// <copyright file="IPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    using System.Collections.Generic;

    /// <summary>
    /// A interface a plugin container (e.g. a proxy object) which manages <typeparamref name="TPlugIn"/>s.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
    public interface IPlugInContainer<TPlugIn>
    {
        /// <summary>
        /// Gets the active plug ins.
        /// </summary>
        /// <value>
        /// The active plug ins.
        /// </value>
        IEnumerable<TPlugIn> ActivePlugIns { get; }

        /// <summary>
        /// Adds the plug in to the plugin point.
        /// </summary>
        /// <param name="plugIn">The plug in.</param>
        /// <param name="isActive">If set to <c>true</c>, it's added as an active plugin; Otherwise, not.</param>
        void AddPlugIn(TPlugIn plugIn, bool isActive);
    }
}