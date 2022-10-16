// <copyright file="IObjectPool.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;

/// <summary>
/// Interface for an object pool which provides an async get method which allows
/// to postpone the get until the next object is available.
/// </summary>
/// <typeparam name="T">The type which should be pooled.</typeparam>
public interface IObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    /// Gets an object from the pool if one is available, otherwise creates one
    /// or waits until one is available.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <typeparamref name="T" />.</returns>
    ValueTask<T> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Return an object to the pool.
    /// </summary>
    /// <param name="obj">The object to add to the pool.</param>
    void Return(T obj);
}