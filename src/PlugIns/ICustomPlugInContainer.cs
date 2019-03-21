// <copyright file="ICustomPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    /// <summary>
    /// A interface for a custom proxy object which manages <typeparamref name="TPlugIn"/>s in a custom way.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plug in.</typeparam>
    public interface ICustomPlugInContainer<in TPlugIn>
    {
        /// <summary>
        /// Gets the plug in of the specified plugin interface type.
        /// </summary>
        /// <typeparam name="T">The requested plug in type.</typeparam>
        /// <returns>The plug in, if available; Otherwise, <c>null</c>.</returns>
        T GetPlugIn<T>()
            where T : class, TPlugIn;
    }
}