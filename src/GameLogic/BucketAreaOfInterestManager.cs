// <copyright file="BucketAreaOfInterestManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;

/// <summary>
/// A area of interest manager which works with a bucket map.
/// </summary>
/// <remarks>
/// <para>
/// Locking: Several threads can move, add or remove the same observer at the same time, e.g. a monster pushing a player
/// while the player walks. The changes of the observed buckets (and the resulting notifications of the observer)
/// are serialized per observer by an <see cref="ObservationLease"/>, so that they are applied in the same order
/// as the moves were applied to the map.
/// </para>
/// <para>
/// Lock order: <c>moveLock</c> (of the moved object) -> observer lock. A move takes the observer lock while it still holds the
/// move lock, and only then releases the move lock ("hand over hand"). Both locks are not reentrant. Therefore nothing which
/// runs under the observer lock - notifications, view plugins, observer registrations - may wait for the move lock or the observer
/// lock of the same observer, e.g. by moving it.
/// </para>
/// </remarks>
internal class BucketAreaOfInterestManager : IAreaOfInterestManager
{
    private readonly ConditionalWeakTable<IBucketMapObserver, AsyncLock> _observerLocks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BucketAreaOfInterestManager" /> class.
    /// </summary>
    /// <param name="chunkSize">Size of the chunk.</param>
    public BucketAreaOfInterestManager(int chunkSize)
    {
        this.Map = new BucketMap<ILocateable>(0x100, true, chunkSize / 2, chunkSize);
    }

    /// <summary>
    /// Gets the underlying bucket map.
    /// </summary>
    protected BucketMap<ILocateable> Map { get; }

    /// <inheritdoc/>
    public async ValueTask AddObjectAsync(ILocateable obj)
    {
        var newBucket = this.Map[obj.Position];
        if (obj is IHasBucketInformation bucketInfo)
        {
            bucketInfo.OldBucket = null;
            bucketInfo.NewBucket = newBucket;
        }

        await newBucket.AddAsync(obj).ConfigureAwait(false);

        if (obj is IBucketMapObserver observer)
        {
            using var observation = await this.AcquireObservationAsync(observer).ConfigureAwait(false);
            await observation.UpdateAsync(this.Map.GetBucketsInRange(obj.Position, observer.InfoRange)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This needs to take into account, that the <see cref="Walker"/> might change the <see cref="ILocateable.Position"/>,
    /// but not updating the buckets. So accessing the bucket of the current coordinates might not be the current bucket!.
    /// </remarks>
    public async ValueTask RemoveObjectAsync(ILocateable obj)
    {
        await this.RemoveFromBucketsAsync(obj).ConfigureAwait(false);

        if (obj is IBucketMapObserver observer)
        {
            // Queues behind changes which are still in progress, so that they can't subscribe again afterwards.
            using var observation = await this.AcquireObservationAsync(observer).ConfigureAwait(false);
            await observation.StopObservingAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask MoveObjectAsync(ILocateable obj, Point target, AsyncLock moveLock, MoveType moveType, bool notifyMovedObject = true, bool notifyObservers = true)
    {
        var observer = obj as IBucketMapObserver;
        var targetOnMap = GetTargetOnMap(obj, target, moveType);
        ObservationLease? observation = null;

        using (await moveLock.LockAsync().ConfigureAwait(false))
        {
            if (this.IsInSameBucket(obj.Position, targetOnMap))
            {
                if (moveType != MoveType.Walk)
                {
                    obj.Position = targetOnMap;
                }
            }
            else
            {
                await this.MoveBetweenBucketsAsync(obj, targetOnMap, moveType).ConfigureAwait(false);
            }

            if (observer is not null)
            {
                // Hand over: the lease is acquired before the move lock is released, so that queued moves update the observed buckets in the order they were applied.
                observation = await this.AcquireObservationAsync(observer).ConfigureAwait(false);
            }
        }

        using (observation)
        {
            if (notifyObservers && obj is IObservable observable)
            {
                await observable.ForEachWorldObserverAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(obj, moveType), notifyMovedObject).ConfigureAwait(false);
            }

            if (observation is not null && observer is not null)
            {
                await observation.UpdateAsync(this.Map.GetBucketsInRange(target, observer.InfoRange)).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ILocateable> GetInRange(Point point, int range)
    {
        return this.Map.GetInRange(point, range);
    }

    /// <summary>
    /// Gets the target which is used to determine the bucket of the object.
    /// </summary>
    /// <param name="obj">The moved object.</param>
    /// <param name="target">The requested target.</param>
    /// <param name="moveType">Type of the move.</param>
    /// <returns>The target of the walker when the object is walking, otherwise the requested target.</returns>
    private static Point GetTargetOnMap(ILocateable obj, Point target, MoveType moveType)
    {
        return moveType == MoveType.Walk && obj is ISupportWalk supportWalk
            ? supportWalk.WalkTarget
            : target;
    }

    private bool IsInSameBucket(Point first, Point second)
    {
        return first.X / this.Map.BucketSideLength == second.X / this.Map.BucketSideLength
               && first.Y / this.Map.BucketSideLength == second.Y / this.Map.BucketSideLength;
    }

    /// <summary>
    /// Acquires the lease which allows to change the observed buckets of the <paramref name="observer"/>.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <returns>The lease. Dispose it to release the lock of the observer.</returns>
    private async ValueTask<ObservationLease> AcquireObservationAsync(IBucketMapObserver observer)
    {
        var observerLock = this._observerLocks.GetValue(observer, static _ => new AsyncLock());
        var lockRelease = await observerLock.LockAsync().ConfigureAwait(false);
        return new ObservationLease(observer, lockRelease);
    }

    /// <summary>
    /// Moves the object from its current bucket to the bucket of the target.
    /// The caller has to hold the move lock of the object.
    /// </summary>
    /// <param name="obj">The moved object.</param>
    /// <param name="target">The target on the map.</param>
    /// <param name="moveType">Type of the move.</param>
    private async ValueTask MoveBetweenBucketsAsync(ILocateable obj, Point target, MoveType moveType)
    {
        var oldPosition = obj.Position;
        Bucket<ILocateable>? oldBucket;
        Bucket<ILocateable> newBucket = this.Map[target];
        if (obj is IHasBucketInformation bucketInfo)
        {
            oldBucket = bucketInfo.NewBucket;
            bucketInfo.NewBucket = newBucket;
            bucketInfo.OldBucket = oldBucket;
        }
        else
        {
            oldBucket = this.Map[oldPosition];
        }

        // only set x and y of the object if it's not walking - the Walker sets these coordinates!
        if (moveType != MoveType.Walk)
        {
            obj.Position = target;
        }

        if (oldBucket is not null)
        {
            await oldBucket.RemoveAsync(obj).ConfigureAwait(false);
        }

        await newBucket.AddAsync(obj).ConfigureAwait(false);
    }

    private async ValueTask RemoveFromBucketsAsync(ILocateable obj)
    {
        if (obj is not IHasBucketInformation bucketInfo)
        {
            await this.Map[obj.Position].RemoveAsync(obj).ConfigureAwait(false);
            return;
        }

        // we remove the object from all known buckets and set old/new bucket to null
        var oldBucket = bucketInfo.OldBucket;
        var newBucket = bucketInfo.NewBucket;

        bucketInfo.OldBucket = newBucket;
        bucketInfo.NewBucket = null;
        if (newBucket is not null)
        {
            await newBucket.RemoveAsync(obj).ConfigureAwait(false);
        }

        bucketInfo.OldBucket = newBucket;
        if (oldBucket is not null)
        {
            await oldBucket.RemoveAsync(obj).ConfigureAwait(false);
        }

        bucketInfo.OldBucket = null;
    }
}
