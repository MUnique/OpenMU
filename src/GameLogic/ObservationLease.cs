// <copyright file="ObservationLease.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The exclusive right to change which buckets an <see cref="IBucketMapObserver"/> observes, including
/// notifying it about the objects which come into or go out of its scope.
/// </summary>
/// <remarks>
/// <para>
/// The same observer can be moved, added or removed by several threads at the same time, e.g. a monster pushing a player
/// while the player walks. Holding the lease serializes these changes. Because the methods which change the
/// observed buckets are only available on the lease, they can't be called without holding the lock.
/// </para>
/// <para>
/// The lock behind the lease is not reentrant, see <see cref="BucketAreaOfInterestManager"/> for the lock order.
/// Dispose the lease to release the lock.
/// </para>
/// </remarks>
internal sealed class ObservationLease : IDisposable
{
    private readonly IBucketMapObserver _observer;
    private readonly IDisposable _lockRelease;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservationLease"/> class.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <param name="lockRelease">The release of the lock of the observer, which is held by this lease.</param>
    public ObservationLease(IBucketMapObserver observer, IDisposable lockRelease)
    {
        this._observer = observer;
        this._lockRelease = lockRelease;
    }

    /// <summary>
    /// Makes the observer observe exactly the specified buckets. It's notified about the objects in
    /// the buckets which are not observed anymore, and about the objects in the new ones.
    /// </summary>
    /// <param name="bucketsInRange">The buckets which should be observed.</param>
    /// <remarks>
    /// Does nothing when the observer was removed from the map in the meantime, because it would never stop
    /// observing the buckets again.
    /// </remarks>
    /// <returns>The task.</returns>
    public async ValueTask UpdateAsync(IEnumerable<Bucket<ILocateable>> bucketsInRange)
    {
        if (this._observer is IHasBucketInformation { NewBucket: null })
        {
            return;
        }

        var inRange = bucketsInRange.ToList();
        List<Bucket<ILocateable>> noLongerObserved;
        List<Bucket<ILocateable>> newlyObserved;

        // The list of observed buckets is a plain list which is read by other threads, too.
        // Computing and applying the difference has to be atomic - there is no await inside, so a normal lock is fine.
        lock (this._observer.ObservingBuckets)
        {
            noLongerObserved = this._observer.ObservingBuckets.Where(bucket => !inRange.Contains(bucket)).ToList();
            newlyObserved = inRange.Where(bucket => !this._observer.ObservingBuckets.Contains(bucket)).ToList();

            foreach (var bucket in noLongerObserved)
            {
                this.Unsubscribe(bucket);
                this._observer.ObservingBuckets.Remove(bucket);
            }

            foreach (var bucket in newlyObserved)
            {
                this.Subscribe(bucket);
                this._observer.ObservingBuckets.Add(bucket);
            }
        }

        if (noLongerObserved.Any(bucket => bucket.Count > 0))
        {
            await this._observer.LocateablesOutOfScopeAsync(noLongerObserved.SelectMany(bucket => bucket)).ConfigureAwait(false);
        }

        if (newlyObserved.Any(bucket => bucket.Count > 0))
        {
            await this._observer.NewLocateablesInScopeAsync(newlyObserved.SelectMany(bucket => bucket)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Makes the observer stop observing all buckets, e.g. because it left the map.
    /// It's notified that the objects in these buckets are out of scope.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask StopObservingAsync()
    {
        Bucket<ILocateable>[] observedBuckets;
        lock (this._observer.ObservingBuckets)
        {
            observedBuckets = this._observer.ObservingBuckets.ToArray();
            foreach (var bucket in observedBuckets)
            {
                this.Unsubscribe(bucket);
            }
        }

        // The list is cleared after the notification, because the observer might still look at it while handling it.
        if (observedBuckets.Any(bucket => bucket.Count > 0))
        {
            await this._observer.LocateablesOutOfScopeAsync(observedBuckets.SelectMany(bucket => bucket)).ConfigureAwait(false);
        }

        lock (this._observer.ObservingBuckets)
        {
            this._observer.ObservingBuckets.Clear();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._lockRelease.Dispose();
    }

    private void Subscribe(Bucket<ILocateable> bucket)
    {
        bucket.ItemAdded += this._observer.LocateableAddedAsync;
        bucket.ItemRemoved += this._observer.LocateableRemovedAsync;
    }

    private void Unsubscribe(Bucket<ILocateable> bucket)
    {
        bucket.ItemAdded -= this._observer.LocateableAddedAsync;
        bucket.ItemRemoved -= this._observer.LocateableRemovedAsync;
    }
}
