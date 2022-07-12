// <copyright file="IObservable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using Nito.AsyncEx;

/// <summary>
/// Interface for an observable object.
/// </summary>
public interface IObservable
{
    /// <summary>
    /// Gets the observers.
    /// </summary>
    ISet<IWorldObserver> Observers { get; }

    /// <summary>
    /// Gets the lock for <see cref="Observers"/>.
    /// </summary>
    AsyncReaderWriterLock ObserverLock { get; }

    /// <summary>
    /// Adds the observer.
    /// </summary>
    /// <param name="observer">The observer.</param>
    ValueTask AddObserverAsync(IWorldObserver observer);

    /// <summary>
    /// Removes the observer.
    /// </summary>
    /// <param name="observer">The observer.</param>
    ValueTask RemoveObserverAsync(IWorldObserver observer);
}