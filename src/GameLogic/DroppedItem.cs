﻿// <copyright file="DroppedItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// An item which got dropped on the ground of a map.
/// </summary>
public sealed class DroppedItem : IDisposable, ILocateable
{
    private static readonly TimeSpan TimeUntilDropIsFree = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets the pickup lock. Used to synchronize pick up requests from the players.
    /// </summary>
    private readonly object _pickupLock;

    private readonly DateTime _dropTimestamp = DateTime.UtcNow;

    private Player? _dropper;

    private IEnumerable<object>? _owners;

    private Timer? _removeTimer;

    private bool _availableToPick = true;

    /// <summary>
    /// Indicates if the item was persistent when it was dropped.
    /// If it wasn't and we clean it up, then we don't need to delete it.
    /// </summary>
    private bool _itemIsPersistent;

    /// <summary>
    /// Initializes a new instance of the <see cref="DroppedItem" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="position">The position where the item was dropped on the map.</param>
    /// <param name="map">The map.</param>
    /// <param name="dropper">The dropper.</param>
    public DroppedItem(Item item, Point position, GameMap map, Player dropper)
        : this(item, position, map, dropper, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DroppedItem" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="position">The position where the item was dropped on the map.</param>
    /// <param name="map">The map.</param>
    /// <param name="dropper">The dropper.</param>
    /// <param name="owners">The owners.</param>
    public DroppedItem(Item item, Point position, GameMap map, Player? dropper, IEnumerable<object>? owners)
    {
        this.Item = item;
        this._pickupLock = new object();
        this.Position = position;
        this.CurrentMap = map;
        this._dropper = dropper;
        this._owners = owners;
        this._removeTimer = new Timer(this.DisposeAndDelete, null, map.ItemDropDuration * 1000, Timeout.Infinite);
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    public Item Item { get; }

    /// <inheritdoc />
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public ushort Id { get; set; }

    /// <inheritdoc/>
    public GameMap CurrentMap { get; }

    /// <summary>
    /// Tries to pick the item up by the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="stackTarget">If the success is <c>true</c>, and this is not <c>null</c>, this dropped item was stacked on an existing item of the players inventory.</param>
    /// <returns>
    /// The success.
    /// </returns>
    /// <remarks>
    /// Can be overwritten, for example for quest items which dropped only for a specific player.
    /// </remarks>
    public bool TryPickUpBy(Player player, out Item? stackTarget)
    {
        stackTarget = null;
        if (!this._availableToPick)
        {
            return false;
        }

        if (this.Item.IsStackable())
        {
            stackTarget = player.Inventory?.Items.FirstOrDefault(i => i.CanCompletelyStackOn(this.Item));
        }

        if (stackTarget != null)
        {
            if (this.TryStackOnItem(player, stackTarget))
            {
                return true;
            }

            return false;
        }

        if (this.Item.Definition!.IsBoundToCharacter && !this.IsPlayerAnOwner(player))
        {
            return false;
        }

        if (!this.IsPlayerAnOwner(player)
            && DateTime.UtcNow < this._dropTimestamp.Add(TimeUntilDropIsFree))
        {
            return false;
        }

        return this.TryPickUp(player);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Id}: {this.Item} at {this.CurrentMap.Definition.Name} ({this.Position})";
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        var timer = this._removeTimer;
        if (timer != null)
        {
            this._removeTimer = null;
            timer.Dispose();
            this.CurrentMap.Remove(this);
            this._dropper = null;
            this._owners = null;
        }
    }

    private void DisposeAndDelete(object? state)
    {
        var player = this._dropper;
        try
        {
            this.Dispose();
            if (player != null)
            {
                this.DeleteItem(player);
            }
        }
        catch (Exception ex)
        {
            // we have to catch all errors, because it runs under a pooled thread without an additional safety net ;-)
            player?.Logger.LogError(ex, "Error during DroppedItem.DisposeAndDelete");
        }
    }

    private bool IsPlayerAnOwner(Player player)
    {
        return this._owners?.Contains(player) ?? true;
    }

    private bool TryPickUp(Player player)
    {
        player.Logger.LogDebug("Player {0} tries to pick up {1}", player, this);
        var slot = player.Inventory?.CheckInvSpace(this.Item);
        if (!slot.HasValue || slot < InventoryConstants.LastEquippableItemSlotIndex)
        {
            player.Logger.LogDebug("Inventory full, Player {0}, Item {1}", player, this);
            return false;
        }

        var itemWasTemporary = this.Item is TemporaryItem;
        lock (this._pickupLock)
        {
            if (!this._availableToPick)
            {
                player.Logger.LogDebug("Picked up by another player in the mean time, Player {0}, Item {1}", player, this);
                return false;
            }

            if (!player.Inventory!.AddItem((byte)slot, this.Item))
            {
                player.Logger.LogDebug("Item could not be added to the inventory, Player {0}, Item {1}", player, this);
                return false;
            }

            if (itemWasTemporary)
            {
                // We set the item slot in a temporary item manually, so the further logic can report to the client where the item has been added.
                // If it's temporary, it has been converted into a persistent item before it was added to the inventory. So it's not the same instance.
                // If it's not temporary, this step is not required, because the inventory already set the ItemSlot in the same instance we're holding here.
                this.Item.ItemSlot = (byte)slot;
            }

            this._availableToPick = false;
        }

        player.Logger.LogInformation("Item '{0}' was picked up by player '{1}' and added to his inventory.", this, player);
        this.Dispose();
        if (this._dropper != null && !this._dropper.PlayerState.Finished)
        {
            this._dropper.PersistenceContext.SaveChanges(); // Otherwise, if the item got modified since last save point by the dropper, changes would not be saved by the picking up player!
            this._itemIsPersistent = this._dropper.PersistenceContext.Detach(this.Item);
        }

        if (!itemWasTemporary)
        {
            player.PersistenceContext.Attach(this.Item);
        }

        return true;
    }

    private bool TryStackOnItem(Player player, Item stackTarget)
    {
        player.Logger.LogDebug("Player {0} tries to pick up {1}, trying to add to an existing item at slot {2}", player, this, stackTarget.ItemSlot);
        lock (this._pickupLock)
        {
            if (!this._availableToPick)
            {
                player.Logger.LogDebug("Picked up by another player in the mean time, Player {0}, Item {1}", player, this);
                return false;
            }

            stackTarget.Durability += this.Item.Durability;
            this._availableToPick = false;
        }

        player.Logger.LogInformation("Item '{0}' got picked up by player '{1}'. Durability of available stack {2} increased to {3}", this, player, stackTarget, stackTarget.Durability);
        this.DisposeAndDelete(null);
        return true;
    }

    private void DeleteItem(Player player)
    {
        player.Logger.LogInformation("Item '{0}' which was dropped by player '{1}' is getting deleted.", this, player);
        if (!player.PlayerState.Finished && this.Item is not TemporaryItem)
        {
            this._itemIsPersistent = player.PersistenceContext.Detach(this.Item);
        }

        if (!this._itemIsPersistent)
        {
            return;
        }

        try
        {
            var repositoryManager = player.GameContext.PersistenceContextProvider;

            // We could use here the persistence context of the dropper - but if it logged out and is not saving anymore, the deletion would not be saved.
            // So we use a new temporary persistence context instead.
            // We use a trade-context as it just focuses on the items. Otherwise, we would track a lot more items.
            using var context = repositoryManager.CreateNewTradeContext();
            context.Delete(this.Item);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            player.Logger.LogWarning("Exception during deleting of the item {0}: {1}\n{2}", this, e.Message, e.StackTrace);
        }

        player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(this.Item);
    }
}