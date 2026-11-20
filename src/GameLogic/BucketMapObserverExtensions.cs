// <copyright file="BucketMapObserverExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for <see cref="IBucketMapObserver"/>.
/// </summary>
public static class BucketMapObserverExtensions
{
    /// <summary>
    /// Gets a snapshot of all objects in the buckets which are currently observed by the <paramref name="observer"/>.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <returns>The objects in the observed buckets.</returns>
    /// <remarks>
    /// <see cref="IBucketMapObserver.ObservingBuckets"/> is a plain list which is changed whenever the observer moves,
    /// which can happen on any thread (e.g. a monster pushing a player). Enumerating it directly can fail or
    /// return inconsistent results, so use this method instead.
    /// </remarks>
    public static List<ILocateable> GetObservedLocateables(this IBucketMapObserver observer)
    {
        lock (observer.ObservingBuckets)
        {
            return observer.ObservingBuckets.SelectMany(bucket => bucket).ToList();
        }
    }
}
