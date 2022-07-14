// <copyright file="ObserverToWorldViewAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Adapts the incoming calls from the <see cref="IBucketMapObserver"/> to the available view plugins.
/// </summary>
public sealed class ObserverToWorldViewAdapter : AsyncDisposable, IBucketMapObserver
{
    private readonly AsyncReaderWriterLock _observingLock = new ();
    private readonly ISet<IObservable> _observingObjects = new HashSet<IObservable>();
    private readonly IWorldObserver _adaptee;

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverToWorldViewAdapter" /> class.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    /// <param name="infoRange">The information range.</param>
    public ObserverToWorldViewAdapter(IWorldObserver adaptee, int infoRange)
    {
        this._adaptee = adaptee;
        this.InfoRange = infoRange;
    }

    /// <inheritdoc/>
    public int InfoRange { get; }

    /// <inheritdoc/>
    public IList<Bucket<ILocateable>> ObservingBuckets { get; } = new List<Bucket<ILocateable>>();

    /// <inheritdoc/>
    public async ValueTask LocateableAddedAsync(ILocateable item)
    {
        if (this._isDisposed)
        {
            return;
        }

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
            await this._adaptee.InvokeViewPlugInAsync<INewPlayersInScopePlugIn>(p => p.NewPlayersInScopeAsync(player.GetAsEnumerable())).ConfigureAwait(false);
        }
        else if (item is NonPlayerCharacter npc)
        {
            await this._adaptee.InvokeViewPlugInAsync<INewNpcsInScopePlugIn>(p => p.NewNpcsInScopeAsync(npc.GetAsEnumerable())).ConfigureAwait(false);
        }
        else if (item is DroppedItem droppedItem)
        {
            await this._adaptee.InvokeViewPlugInAsync<IShowDroppedItemsPlugIn>(p => p.ShowDroppedItemsAsync(droppedItem.GetAsEnumerable(), true));
        }
        else if (item is DroppedMoney droppedMoney)
        {
            await this._adaptee.InvokeViewPlugInAsync<IShowMoneyDropPlugIn>(p => p.ShowMoneyAsync(droppedMoney.Id, true, droppedMoney.Amount, droppedMoney.Position)).ConfigureAwait(false);
        }
        else
        {
            // no action required.
        }

        if (item is IObservable observable)
        {
            using (await this._observingLock.WriterLockAsync())
            {
                if (!this._observingObjects.Contains(observable))
                {
                    this._observingObjects.Add(observable);
                }
            }

            await observable.AddObserverAsync(this._adaptee);
        }
    }

    /// <inheritdoc/>
    public async ValueTask LocateableRemovedAsync(ILocateable item)
    {
        if (this._isDisposed)
        {
            return;
        }

        if (Equals(item, this._adaptee))
        {
            return; // we are always observing ourselves
        }

        var hasBucketInfo = item as IHasBucketInformation;
        if (hasBucketInfo?.NewBucket != null && this.ObservingBuckets.Contains(hasBucketInfo.NewBucket))
        {
            // CurrentBucket contains the new bucket if the object is moving. So in this case we still observe the new bucket and don't need to remove the object from observation.
            return;
        }

        if (item is IObservable observable)
        {
            using (await this._observingLock.WriterLockAsync())
            {
                this._observingObjects.Remove(observable);
            }

            await observable.RemoveObserverAsync(this._adaptee);
        }

        if (item is DroppedItem || item is DroppedMoney)
        {
            await this._adaptee.InvokeViewPlugInAsync<IDroppedItemsDisappearedPlugIn>(p => p.DroppedItemsDisappearedAsync(item.GetAsEnumerable().Select(i => i.Id)));
        }
        else
        {
            if (item.IsActive())
            {
                await this._adaptee.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(item.GetAsEnumerable()));
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask LocateablesOutOfScopeAsync(IEnumerable<ILocateable> oldObjects)
    {
        if (this._isDisposed)
        {
            return;
        }

        IEnumerable<IObservable> oldItems;
        using (await this._observingLock.WriterLockAsync())
        {
            oldItems = oldObjects.OfType<IObservable>().Where(item => this._observingObjects.Contains(item) && this.ObjectWillBeOutOfScope(item)).ToList();
            oldItems.ForEach(item => this._observingObjects.Remove(item));
        }

        oldItems.ForEach(item => item.RemoveObserverAsync(this._adaptee));

        if (this._adaptee is IHasBucketInformation { NewBucket: null })
        {
            // adaptee (player) left the map or disconnected; it's not required to update the view
        }
        else
        {
            var droppedItems = oldItems.OfType<ILocateable>().Where(item => item is DroppedItem || item is DroppedMoney);
            var nonItems = oldItems.OfType<ILocateable>().Except(droppedItems).WhereActive();
            if (nonItems.Any())
            {
                await this._adaptee.InvokeViewPlugInAsync<IObjectsOutOfScopePlugIn>(p => p.ObjectsOutOfScopeAsync(nonItems)).ConfigureAwait(false);
            }

            if (droppedItems.Any())
            {
                await this._adaptee.InvokeViewPlugInAsync<IDroppedItemsDisappearedPlugIn>(p => p.DroppedItemsDisappearedAsync(droppedItems.Select(item => item.Id))).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask NewLocateablesInScopeAsync(IEnumerable<ILocateable> newObjects)
    {
        if (this._isDisposed)
        {
            return;
        }

        IEnumerable<IObservable> newItems;
        using (await this._observingLock.WriterLockAsync())
        {
            newItems = newObjects.OfType<IObservable>().Where(item => !this._observingObjects.Contains(item)).ToList();
            newItems.ForEach(item => this._observingObjects.Add(item));
        }

        var players = newItems.OfType<Player>().WhereActive();
        if (players.Any())
        {
            await this._adaptee.InvokeViewPlugInAsync<INewPlayersInScopePlugIn>(p => p.NewPlayersInScopeAsync(players, false)).ConfigureAwait(false);
        }

        var npcs = newItems.OfType<NonPlayerCharacter>().WhereActive();
        if (npcs.Any())
        {
            await this._adaptee.InvokeViewPlugInAsync<INewNpcsInScopePlugIn>(p => p.NewNpcsInScopeAsync(npcs, false)).ConfigureAwait(false);
        }

        var droppedItems = newObjects.OfType<DroppedItem>();
        if (droppedItems.Any())
        {
            await this._adaptee.InvokeViewPlugInAsync<IShowDroppedItemsPlugIn>(p => p.ShowDroppedItemsAsync(droppedItems, false)).ConfigureAwait(false);
        }

        var droppedMoney = newObjects.OfType<DroppedMoney>();
        foreach (var money in droppedMoney)
        {
            await this._adaptee.InvokeViewPlugInAsync<IShowMoneyDropPlugIn>(p => p.ShowMoneyAsync(money.Id, false, money.Amount, money.Position)).ConfigureAwait(false);
        }

        newItems.ForEach(item => item.AddObserverAsync(this._adaptee));
    }

    /// <summary>
    /// Clears the observing object list.
    /// </summary>
    internal async ValueTask ClearObservingObjectsListAsync()
    {
        using (await this._observingLock.WriterLockAsync())
        {
            this._observingObjects.Clear();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this.ObservingBuckets.Count > 0)
        {
            Debug.Fail($"ObservingBuckets not empty when this instance is disposed. Count: {this.ObservingBuckets.Count}. Maybe observer wasn't correctly removed from the game map.");
            this.ObservingBuckets.Clear();
        }

        if (this._observingObjects.Count > 0)
        {
            await this.ClearObservingObjectsListAsync();
        }

        await base.DisposeAsyncCore();
    }

    private bool ObjectWillBeOutOfScope(IObservable observable)
    {
        var myBucketInformations = this._adaptee as IHasBucketInformation;
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