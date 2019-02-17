// <copyright file="IPlugInPointProxy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    /// <summary>
    /// A interface for the proxy object which implements <typeparamref name="TPlugIn"/>.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
    public interface IPlugInPointProxy<in TPlugIn>
    {
        /// <summary>
        /// Adds the plug in to the plugin point.
        /// </summary>
        /// <param name="plugIn">The plug in.</param>
        /// <param name="isActive">If set to <c>true</c>, it's added as an active plugin; Otherwise, not.</param>
        void AddPlugIn(TPlugIn plugIn, bool isActive);
    }
}