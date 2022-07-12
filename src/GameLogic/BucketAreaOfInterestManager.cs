// <copyright file="BucketAreaOfInterestManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx;

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A area of interest manager which works with a bucket map.
/// </summary>
internal class BucketAreaOfInterestManager : IAreaOfInterestManager
{
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

        await newBucket.AddAsync(obj);

        if (obj is IBucketMapObserver observingPlayer)
        {
            await this.UpdateObservingBucketsAsync(obj.Position, observingPlayer);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This needs to take into account, that the <see cref="Walker"/> might change the <see cref="ILocateable.Position"/>,
    /// but not updating the buckets. So accessing the bucket of the current coordinates might not be the current bucket!.
    /// </remarks>
    public async ValueTask RemoveObjectAsync(ILocateable obj)
    {
        if (obj is IHasBucketInformation bucketInfo)
        {
            // we remove the object from all known buckets and set old/new bucket to null
            var oldBucket = bucketInfo.OldBucket;
            var newBucket = bucketInfo.NewBucket;

            bucketInfo.OldBucket = newBucket;
            bucketInfo.NewBucket = null;
            newBucket?.RemoveAsync(obj);

            bucketInfo.OldBucket = newBucket;
            oldBucket?.RemoveAsync(obj);

            bucketInfo.OldBucket = null;
        }
        else
        {
            await this.Map[obj.Position].RemoveAsync(obj);
        }

        if (obj is IBucketMapObserver observingPlayer)
        {
            foreach (var bucket in observingPlayer.ObservingBuckets)
            {
                bucket.ItemAdded -= observingPlayer.LocateableAddedAsync;
                bucket.ItemRemoved -= observingPlayer.LocateableRemovedAsync;
            }

            if (observingPlayer.ObservingBuckets.Any(b => b.Count > 0))
            {
                await observingPlayer.LocateablesOutOfScopeAsync(observingPlayer.ObservingBuckets.SelectMany(o => o));
            }

            observingPlayer.ObservingBuckets.Clear();
        }
    }

    /// <inheritdoc/>
    public async ValueTask MoveObjectAsync(ILocateable obj, Point target, AsyncLock moveLock, MoveType moveType)
    {
        var differentBucket = await this.MoveObjectOnMapAsync(obj, target, moveLock, moveType);

        if (obj is IObservable observable)
        {
            await observable.ForEachWorldObserverAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(obj, moveType), true).ConfigureAwait(false);
        }

        var observingPlayer = obj as IBucketMapObserver;
        if (differentBucket && observingPlayer != null)
        {
            await this.UpdateObservingBucketsAsync(target, observingPlayer);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ILocateable> GetInRange(Point point, int range)
    {
        return this.Map.GetInRange(point, range);
    }

    /// <summary>
    /// Updates the observing buckets, and notifies the <paramref name="player"/> about new objects which are in scope, and old objects which are out of scope.
    /// </summary>
    /// <param name="newPoint">The new coordinates on the map.</param>
    /// <param name="player">The player.</param>
    protected async ValueTask UpdateObservingBucketsAsync(Point newPoint, IBucketMapObserver player)
    {
        var curbuckets = this.Map.GetBucketsInRange(newPoint, player.InfoRange).ToList(); // All buckets in range
        var oldbuckets = player.ObservingBuckets.Where(i => !curbuckets.Contains(i)).ToList(); // Buckets which are not meant to be observed anymore
        var newbuckets = curbuckets.Where(i => !player.ObservingBuckets.Contains(i)).ToList(); // New buckets for observation

        oldbuckets.ForEach(i =>
        {
            i.ItemAdded -= player.LocateableAddedAsync;
            i.ItemRemoved -= player.LocateableRemovedAsync;
        });
        newbuckets.ForEach(i =>
        {
            i.ItemAdded += player.LocateableAddedAsync;
            i.ItemRemoved += player.LocateableRemovedAsync;
        });

        oldbuckets.ForEach(b => player.ObservingBuckets.Remove(b));
        newbuckets.ForEach(player.ObservingBuckets.Add);
        if (oldbuckets.Any(b => b.Count > 0))
        {
            await player.LocateablesOutOfScopeAsync(oldbuckets.SelectMany(o => o));
        }

        if (newbuckets.Any(b => b.Count > 0))
        {
            await player.NewLocateablesInScopeAsync(newbuckets.SelectMany(o => o));
        }
    }

    private async ValueTask<bool> MoveObjectOnMapAsync(ILocateable obj, Point target, AsyncLock moveLock, MoveType moveType)
    {
        if (moveType == MoveType.Walk)
        {
            if (obj is ISupportWalk supportWalk)
            {
                target = supportWalk.WalkTarget;
            }
        }

        var differentBucket = obj.Position.X / this.Map.BucketSideLength != target.X / this.Map.BucketSideLength
                              || obj.Position.Y / this.Map.BucketSideLength != target.Y / this.Map.BucketSideLength;

        if (!differentBucket)
        {
            if (moveType != MoveType.Walk)
            {
                obj.Position = target;
            }

            return false;
        }

        using (await moveLock.LockAsync())
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

            await newBucket.AddAsync(obj);
        }

        return true;
    }
}