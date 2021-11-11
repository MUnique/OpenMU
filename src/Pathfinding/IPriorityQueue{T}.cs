// <copyright file="IPriorityQueue{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Interface for a priority queue.
/// </summary>
/// <typeparam name="T">Type which should be contained in the queue.</typeparam>
public interface IPriorityQueue<T>
{
    /// <summary>
    /// Gets the number of elements in the queue.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Pushes the specified item into the queue, and puts them at the right place, based on it's priority.
    /// </summary>
    /// <param name="item">The item.</param>
    void Push(T item);

    /// <summary>
    /// Retrieves the instance with the highest priority, and removes it from the queue.
    /// </summary>
    /// <returns>The instance with the highest priority.</returns>
    T Pop();

    /// <summary>
    /// Retrieves the instance with the highest priority, without removing it from the queue.
    /// </summary>
    /// <returns>The instance with the highest priority.</returns>
    T Peek();

    /// <summary>
    /// Clears this instance.
    /// </summary>
    void Clear();
}