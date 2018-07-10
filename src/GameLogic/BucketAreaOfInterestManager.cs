// <copyright file="BucketAreaOfInterestManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Views;

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
        public void AddObject(ILocateable obj)
        {
            this.Map[obj.X, obj.Y].Add(obj);
            if (obj is IHasBucketInformation bucketInfo)
            {
                bucketInfo.OldBucket = null;
                bucketInfo.NewBucket = this.Map[obj.X, obj.Y];
            }

            if (obj is IBucketMapObserver observingPlayer)
            {
                this.UpdateObservingBuckets(obj.X, obj.Y, observingPlayer);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This needs to take into account, that the <see cref="Walker"/> might change the <see cref="ILocateable.X"/> and <see cref="ILocateable.Y"/>,
        /// but not updating the buckets. So accessing the bucket of the current coordinates might not be the current bucket!
        /// </remarks>
        public void RemoveObject(ILocateable obj)
        {
            if (obj is IHasBucketInformation bucketInfo)
            {
                bucketInfo.NewBucket?.Remove(obj);
                bucketInfo.OldBucket?.Remove(obj);
                bucketInfo.OldBucket = bucketInfo.NewBucket;
                bucketInfo.NewBucket = null;
            }
            else
            {
                this.Map[obj.X, obj.Y].Remove(obj);
            }

            if (obj is IBucketMapObserver observingPlayer)
            {
                foreach (var bucket in observingPlayer.ObservingBuckets)
                {
                    bucket.ItemAdded -= observingPlayer.LocateableAdded;
                    bucket.ItemRemoved -= observingPlayer.LocateableRemoved;
                }

                if (observingPlayer.ObservingBuckets.Any(b => b.Count > 0))
                {
                    observingPlayer.LocateablesOutOfScope(observingPlayer.ObservingBuckets.SelectMany(o => o));
                }

                observingPlayer.ObservingBuckets.Clear();
            }
        }

        /// <inheritdoc/>
        public void MoveObject(ILocateable obj, byte newX, byte newY, object moveLock, MoveType moveType)
        {
            var differentBucket = this.MoveObjectOnMap(obj, newX, newY, moveLock, moveType);

            if (obj is IObservable observable)
            {
                observable.ObserverLock.EnterReadLock();
                try
                {
                    observable.Observers.ForEach(o => o.WorldView.ObjectMoved(obj, moveType));
                }
                finally
                {
                    observable.ObserverLock.ExitReadLock();
                }
            }

            var observingPlayer = obj as IBucketMapObserver;
            if (differentBucket && observingPlayer != null)
            {
                this.UpdateObservingBuckets(newX, newY, observingPlayer);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ILocateable> GetInRange(int x, int y, int range, RangeType rangeType)
        {
            return this.Map.GetInRange(x, y, range, rangeType);
        }

        /// <summary>
        /// Updates the observing buckets, and notifies the <paramref name="player"/> about new objects which are in scope, and old objects which are out of scope.
        /// </summary>
        /// <param name="newx">The new x coordinate on the map.</param>
        /// <param name="newy">The new y coordinate on the map.</param>
        /// <param name="player">The player.</param>
        protected void UpdateObservingBuckets(int newx, int newy, IBucketMapObserver player)
        {
            var curbuckets = this.Map.GetBucketsInRange(newx, newy, player.InfoRange, RangeType.Quadratic).ToList(); // All buckets in range
            var oldbuckets = player.ObservingBuckets.Where(i => !curbuckets.Contains(i)).ToList(); // Buckets which are not meant to be observed anymore
            var newbuckets = curbuckets.Where(i => !player.ObservingBuckets.Contains(i)).ToList(); // New buckets for observation

            oldbuckets.ForEach(i =>
            {
                i.ItemAdded -= player.LocateableAdded;
                i.ItemRemoved -= player.LocateableRemoved;
            });
            newbuckets.ForEach(i =>
            {
                i.ItemAdded += player.LocateableAdded;
                i.ItemRemoved += player.LocateableRemoved;
            });

            if (oldbuckets.Any(b => b.Count > 0))
            {
                player.LocateablesOutOfScope(oldbuckets.SelectMany(o => o));
            }

            if (newbuckets.Any(b => b.Count > 0))
            {
                player.NewLocateablesInScope(newbuckets.SelectMany(o => o));
            }

            newbuckets.ForEach(player.ObservingBuckets.Add);
            oldbuckets.ForEach(b => player.ObservingBuckets.Remove(b));
        }

        private bool MoveObjectOnMap(ILocateable obj, byte newX, byte newY, object moveLock, MoveType moveType)
        {
            if (moveType == MoveType.Walk)
            {
                if (obj is ISupportWalk supportWalk)
                {
                    newX = supportWalk.WalkTarget.X;
                    newY = supportWalk.WalkTarget.Y;
                }
            }

            var differentBucket = obj.X / this.Map.BucketSideLength != newX / this.Map.BucketSideLength
                                  || obj.Y / this.Map.BucketSideLength != newY / this.Map.BucketSideLength;

            if (!differentBucket)
            {
                return false;
            }

            lock (moveLock)
            {
                var oldX = obj.X;
                var oldY = obj.Y;
                Bucket<ILocateable> oldBucket;
                Bucket<ILocateable> newBucket = this.Map[newX, newY];
                if (obj is IHasBucketInformation bucketInfo)
                {
                    oldBucket = bucketInfo.NewBucket;
                    bucketInfo.NewBucket = newBucket;
                    bucketInfo.OldBucket = oldBucket;
                }
                else
                {
                    oldBucket = this.Map[oldX, oldY];
                }

                obj.X = newX;
                obj.Y = newY;
                oldBucket?.Remove(obj);
                newBucket.Add(obj);
            }

            return true;
        }
    }
}