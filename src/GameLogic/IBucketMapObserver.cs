// <copyright file="IBucketMapObserver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Interface of an object which is observing other objects on its current map.
/// </summary>
public interface IBucketMapObserver
{
    /// <summary>
    /// Gets the maximum distance of which buckets in range the observer is interested in.
    /// </summary>
    int InfoRange { get; }

    /// <summary>
    /// Gets the observing buckets.
    /// </summary>
    IList<Bucket<ILocateable>> ObservingBuckets { get; }

    /// <summary>
    /// This method is called, when another locateable is moving into the observing zones.
    /// </summary>
    /// <param name="item">The item which got added.</param>
    ValueTask LocateableAddedAsync(ILocateable item);

    /// <summary>
    /// This method is called, when another locateable is moving out of the observing zones.
    /// </summary>
    /// <param name="item">The item which got removed.</param>
    ValueTask LocateableRemovedAsync(ILocateable item);

    /// <summary>
    /// This method is called, when this object is moving to another zone, and old objects are getting out of range.
    /// </summary>
    /// <param name="oldObjects">The objects which are out of range.</param>
    ValueTask LocateablesOutOfScopeAsync(IEnumerable<ILocateable> oldObjects);

    /// <summary>
    /// This method is called, when this object is moving to another zone, and new objects are getting into range.
    /// </summary>
    /// <param name="newObjects">The objects which are getting into range.</param>
    ValueTask NewLocateablesInScopeAsync(IEnumerable<ILocateable> newObjects);
}