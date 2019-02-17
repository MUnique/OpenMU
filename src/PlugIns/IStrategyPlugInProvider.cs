// <copyright file="IStrategyPlugInProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns
{
    /// <summary>
    /// Interface for a strategy plugin provider which holds/manages strategy plugins and provides them to the caller.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TStrategy">The type of the strategy.</typeparam>
    public interface IStrategyPlugInProvider<in TKey, out TStrategy>
        where TStrategy : class, IStrategyPlugIn<TKey>
    {
        /// <summary>
        /// Gets the <typeparamref name="TStrategy"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <typeparamref name="TStrategy"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The <typeparamref name="TStrategy"/> with the specified key, if available; Otherwise, <c>null</c>.</returns>
        TStrategy this[TKey key] { get; }
    }
}