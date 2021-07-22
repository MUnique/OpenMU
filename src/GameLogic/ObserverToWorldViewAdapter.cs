// <copyright file="ObserverToWorldViewAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// Adapts the incoming calls from the <see cref="IBucketMapObserver"/> to the available view plugins.
    /// </summary>
    public sealed class ObserverToWorldViewAdapter : IBucketMapObserver, IDisposable
    {
        private readonly ReaderWriterLockSlim observingLock = new ();
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
        public void LocateableAdded(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
        {
            if (this.isDisposed)
            {
                return;
            }

            var item = eventArgs.Item;
            if (item is IHasBucketInformation { OldBucket: { } oldBucket } && this.ObservingBuckets.Contains(oldBucket))
            {
                // we already observe the bucket where the object came from
                return;
            }

            if (!item.IsActive())
            {
                return;
            }

            if (item is Player player)
            {
                this.adaptee.ViewPlugIns.GetPlugIn<INewPlayersInScopePlugIn>()?.NewPlayersInScope(player.GetAsEnumerable());
            }
            else if (item is NonPlayerCharacter npc)
            {
                this.adaptee.ViewPlugIns.GetPlugIn<INewNpcsInScopePlugIn>()?.NewNpcsInScope(npc.GetAsEnumerable());
            }
            else if (item is DroppedItem droppedItem)
            {
                this.adaptee.ViewPlugIns.GetPlugIn<IShowDroppedItemsPlugIn>()?.ShowDroppedItems(droppedItem.GetAsEnumerable(), sender != this);
            }
            else if (item is DroppedMoney droppedMoney)
            {
                this.adaptee.ViewPlugIns.GetPlugIn<IShowMoneyDropPlugIn>()?.ShowMoney(droppedMoney.Id, sender != this, droppedMoney.Amount, droppedMoney.Position);
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
        public void LocateableRemoved(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
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
            if (hasBucketInfo?.NewBucket != null && this.ObservingBuckets.Contains(hasBucketInfo.NewBucket))
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

            if (item is DroppedItem || item is DroppedMoney)
            {
                this.adaptee.ViewPlugIns.GetPlugIn<IDroppedItemsDisappearedPlugIn>()?.DroppedItemsDisappeared(item.GetAsEnumerable().Select(i => i.Id));
            }
            else
            {
                if (item.IsActive())
                {
                    this.adaptee.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()?.ObjectsOutOfScope(item.GetAsEnumerable());
                }
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
                oldItems = oldObjects.OfType<IObservable>().Where(item => this.observingObjects.Contains(item) && this.ObjectWillBeOutOfScope(item)).ToList();
                oldItems.ForEach(item => this.observingObjects.Remove(item));
            }
            finally
            {
                this.observingLock.ExitWriteLock();
            }

            oldItems.ForEach(item => item.RemoveObserver(this.adaptee));

            if (this.adaptee is IHasBucketInformation { NewBucket: null })
            {
                // adaptee (player) left the map or disconnected; it's not required to update the view
            }
            else
            {
                var droppedItems = oldItems.OfType<ILocateable>().Where(item => item is DroppedItem || item is DroppedMoney);
                var nonItems = oldItems.OfType<ILocateable>().Except(droppedItems).WhereActive();
                if (nonItems.Any())
                {
                    this.adaptee.ViewPlugIns.GetPlugIn<IObjectsOutOfScopePlugIn>()?.ObjectsOutOfScope(nonItems);
                }

                if (droppedItems.Any())
                {
                    this.adaptee.ViewPlugIns.GetPlugIn<IDroppedItemsDisappearedPlugIn>()?.DroppedItemsDisappeared(droppedItems.Select(item => item.Id));
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

            var players = newItems.OfType<Player>().WhereActive();
            if (players.Any())
            {
                this.adaptee.ViewPlugIns.GetPlugIn<INewPlayersInScopePlugIn>()?.NewPlayersInScope(players, false);
            }

            var npcs = newItems.OfType<NonPlayerCharacter>().WhereActive();
            if (npcs.Any())
            {
                this.adaptee.ViewPlugIns.GetPlugIn<INewNpcsInScopePlugIn>()?.NewNpcsInScope(npcs, false);
            }

            var droppedItems = newObjects.OfType<DroppedItem>();
            if (droppedItems.Any())
            {
                this.adaptee.ViewPlugIns.GetPlugIn<IShowDroppedItemsPlugIn>()?.ShowDroppedItems(droppedItems, false);
            }

            var droppedMoney = newObjects.OfType<DroppedMoney>();
            droppedMoney.ForEach(money => this.adaptee.ViewPlugIns.GetPlugIn<IShowMoneyDropPlugIn>()?.ShowMoney(money.Id, false, money.Amount, money.Position));

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
                Debug.Fail($"ObservingBuckets not empty when this instance is disposed. Count: {this.ObservingBuckets.Count}. Maybe observer wasn't correctly removed from the game map.");
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

        private bool ObjectWillBeOutOfScope(IObservable observable)
        {
            var myBucketInformations = this.adaptee as IHasBucketInformation;
            if (myBucketInformations is null)
            {
                return true;
            }

            if (myBucketInformations.NewBucket is null)
            {
                // I'll leave, so will be out of scope
                return true;
            }

            var locateableBucketInformations = observable as IHasBucketInformation;
            if (locateableBucketInformations is null)
            {
                // We have to assume that this observable will be out of scope since it can't tell us where it goes
                return true;
            }

            if (locateableBucketInformations.NewBucket is null)
            {
                // It's leaving the map, so will be out of scope
                return true;
            }

            // If we observe the target bucket of the observable, it will be in our range
            return !this.ObservingBuckets.Contains(locateableBucketInformations.NewBucket);
        }
    }
}
