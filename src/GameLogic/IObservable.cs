// <copyright file="IObservable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;

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
    ReaderWriterLockSlim ObserverLock { get; }

    /// <summary>
    /// Adds the observer.
    /// </summary>
    /// <param name="observer">The observer.</param>
    void AddObserver(IWorldObserver observer);

    /// <summary>
    /// Removes the observer.
    /// </summary>
    /// <param name="observer">The observer.</param>
    void RemoveObserver(IWorldObserver observer);
}