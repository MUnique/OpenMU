// <copyright file="IIndexer{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Describes an indexer for a <see cref="IndexedLinkedList{T}"/>.
/// </summary>
/// <typeparam name="T">The type for which the indexer can calculate the index value.</typeparam>
internal interface IIndexer<in T>
{
    /// <summary>
    /// Gets the index value of the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The index value.</returns>
    int GetIndexValue(T item);
}