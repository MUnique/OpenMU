// <copyright file="ObserverToWorldViewAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using log4net;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Adapts the incoming calls from the <see cref="IBucketMapObserver"/> to the <see cref="IWorldView"/>.
    /// </summary>
    public sealed class ObserverToWorldViewAdapter : IBucketMapObserver, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ObserverToWorldViewAdapter));
        private readonly ReaderWriterLockSlim observingLock = new ReaderWriterLockSlim();
        private readonly ISet<IObservable> observingObjects = new HashSet<IObservable>();
        private readonly IWorldObserver adaptee;

        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObserverToWorldViewAdapter" /> class.
        /// </summary>
        /// <param name="adaptee">The adaptee.</param>
        /// <param name="infoRange">The information range.</param>
        public ObserverToWorldViewAdapter(IWorldObserver adaptee, int infoRange)
        {
            this.adaptee = adaptee;
            this.InfoRange = infoRange;
        }

        /// <inheritdoc/>
        public int InfoRange { get; }

        /// <inheritdoc/>
        public IList<Bucket<ILocateable>> ObservingBuckets { get; } = new List<Bucket<ILocateable>>();

        /// <inheritdoc/>
        public void LocateableAdded(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            if (this.isDisposed)
            {
                return;
            }

            var item = eventArgs.Item;
            if (item is IHasBucketInformation hasBucketInfo && this.ObservingBuckets.Contains(hasBucketInfo.CurrentBucket))
            {
                // we already observe the bucket
                return;
            }

            if (item is Player)
            {
                this.adaptee.WorldView.NewPlayersInScope(((Player)item).GetAsEnumerable());
            }
            else if (item is NonPlayerCharacter)
            {
                this.adaptee.WorldView.NewNpcsInScope(((NonPlayerCharacter)item).GetAsEnumerable());
            }
            else if (item is DroppedItem)
            {
                this.adaptee.WorldView.ShowDroppedItems(((DroppedItem)item).GetAsEnumerable(), sender != this);
            }
            else
            {
                // no action required.
            }

            if (item is IObservable observable)
            {
                this.observingLock.EnterWriteLock();
                try
                {
                    if (!this.observingObjects.Contains(observable))
                    {
                        this.observingObjects.Add(observable);
                    }
                }
                finally
                {
                    this.observingLock.ExitWriteLock();
                }

                observable.AddObserver(this.adaptee);
            }
        }

        /// <inheritdoc/>
        public void LocateableRemoved(object sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            if (this.isDisposed)
            {
                return;
            }

            var item = eventArgs.Item;

            if (Equals(item, this.adaptee))
            {
                return; // we are always observing ourself
            }

            var hasBucketInfo = item as IHasBucketInformation;
            if (hasBucketInfo?.CurrentBucket != null && this.ObservingBuckets.Contains(hasBucketInfo.CurrentBucket))
            {
                // CurrentBucket contains the new bucket if the object is moving. So in this case we still observe the new bucket and don't need to remove the object from observation.
                return;
            }

            if (item is IObservable observable)
            {
                this.observingLock.EnterWriteLock();
                try
                {
                    this.observingObjects.Remove(observable);
                }
                finally
                {
                    this.observingLock.ExitWriteLock();
                }

                observable.RemoveObserver(this.adaptee);
            }

            if (item is DroppedItem)
            {
                this.adaptee.WorldView.DroppedItemsDisappeared(item.GetAsEnumerable().Select(i => i.Id));
            }
            else
            {
                this.adaptee.WorldView.ObjectsOutOfScope(item.GetAsEnumerable());
            }
        }

        /// <inheritdoc/>
        public void LocateablesOutOfScope(IEnumerable<ILocateable> oldObjects)
        {
            if (this.isDisposed)
            {
                return;
            }

            IEnumerable<IObservable> oldItems;
            this.observingLock.EnterWriteLock();
            try
            {
                oldItems = oldObjects.OfType<IObservable>().Where(item =>
                    this.observingObjects.Contains(item)
                    && ((this.adaptee as IHasBucketInformation)?.CurrentBucket == null
                        || (!(item is IHasBucketInformation) || !this.ObservingBuckets.Contains(((IHasBucketInformation)item).CurrentBucket)))).ToList();
                oldItems.ForEach(item => this.observingObjects.Remove(item));
            }
            finally
            {
                this.observingLock.ExitWriteLock();
            }

            oldItems.ForEach(item => item.RemoveObserver(this.adaptee));

            if (this.adaptee is IHasBucketInformation bucketInformation && bucketInformation.CurrentBucket == null)
            {
                // adaptee (player) left the map or disconnected; it's not required to update the view
            }
            else
            {
                var nonItems = oldItems.OfType<IIdentifiable>().Where(item => !(item is DroppedItem));
                if (nonItems.Any())
                {
                    this.adaptee.WorldView.ObjectsOutOfScope(nonItems);
                }

                var droppedItems = oldItems.OfType<DroppedItem>();
                if (droppedItems.Any())
                {
                    this.adaptee.WorldView.DroppedItemsDisappeared(droppedItems.Select(item => item.Id));
                }
            }
        }

        /// <inheritdoc/>
        public void NewLocateablesInScope(IEnumerable<ILocateable> newObjects)
        {
            if (this.isDisposed)
            {
                return;
            }

            IEnumerable<IObservable> newItems;
            this.observingLock.EnterWriteLock();
            try
            {
                newItems = newObjects.OfType<IObservable>().Where(item => !this.observingObjects.Contains(item)).ToList();
                newItems.ForEach(item => this.observingObjects.Add(item));
            }
            finally
            {
                this.observingLock.ExitWriteLock();
            }

            var players = newItems.OfType<Player>();
            if (players.Any())
            {
                this.adaptee.WorldView.NewPlayersInScope(players);
            }

            var npcs = newItems.OfType<NonPlayerCharacter>();
            if (npcs.Any())
            {
                this.adaptee.WorldView.NewNpcsInScope(npcs);
            }

            var droppedItems = newItems.OfType<DroppedItem>();
            if (droppedItems.Any())
            {
                this.adaptee.WorldView.ShowDroppedItems(droppedItems, false);
            }

            newItems.ForEach(item => item.AddObserver(this.adaptee));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.ObservingBuckets.Count > 0)
            {
                Log.Warn($"ObservingBuckets not empty when this instance is disposed. Count: {this.ObservingBuckets.Count}. Maybe observer wasn't correctly removed from the game map.");
                this.ObservingBuckets.Clear();
            }

            if (this.observingObjects.Count > 0)
            {
                this.ClearObservingObjectsList();
            }

            this.observingLock.Dispose();
            this.isDisposed = true;
        }

        /// <summary>
        /// Clears the observing object list.
        /// </summary>
        internal void ClearObservingObjectsList()
        {
            this.observingLock.EnterWriteLock();
            try
            {
                this.observingObjects.Clear();
            }
            finally
            {
                this.observingLock.ExitWriteLock();
            }
        }
    }
}
