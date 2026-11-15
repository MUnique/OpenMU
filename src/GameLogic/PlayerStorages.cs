// <copyright file="PlayerStorages.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.WaitingItems;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The item storages of a <see cref="Player"/>.
/// </summary>
internal sealed class PlayerStorages
{
    private readonly Player _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerStorages"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerStorages(Player player)
    {
        this._player = player;
    }

    /// <summary>
    /// Gets the inventory of the selected character.
    /// </summary>
    public IInventoryStorage? Inventory { get; private set; }

    /// <summary>
    /// Gets or sets the temporary storage, which is used while an NPC or trade dialog is opened.
    /// </summary>
    public IStorage? TemporaryStorage { get; set; }

    /// <summary>
    /// Gets the shop storage of the selected character.
    /// </summary>
    public IShopStorage? ShopStorage { get; private set; }

    /// <summary>
    /// Gets or sets the vault. It's set when the vault NPC is opened.
    /// </summary>
    public IStorage? Vault { get; set; }

    /// <summary>
    /// Gets or sets the backup of the inventory, which is created before a trade and while the dialog of a crafting NPC is open.
    /// </summary>
    public BackupItemStorage? BackupInventory { get; set; }

    /// <summary>
    /// Creates the storages for the selected character, when it entered the world.
    /// </summary>
    /// <param name="character">The selected character.</param>
    public void CreateForCharacter(Character character)
    {
        this.Inventory = new InventoryStorage(this._player, this._player.GameContext);
        this.ShopStorage = new ShopStorage(character, this._player.PutAsideWhenLoaded(character));
        this.TemporaryStorage = new Storage(InventoryConstants.TemporaryStorageSize, new TemporaryItemStorage());
        this.Vault = null; // vault storage is getting set when vault npc is opened.
    }

    /// <summary>
    /// Restores the temporary storage items placed in an NPC or trade dialog when player is disconnected.
    /// </summary>
    public async ValueTask RestoreTemporaryStorageItemsAsync()
    {
        try
        {
            if (this.Inventory is not { } inventory)
            {
                return;
            }

            if (this.BackupInventory is { } backupInventory)
            {
                await this.RestoreBackupInventoryAsync(inventory, backupInventory).ConfigureAwait(false);
                return;
            }

            if (this.TemporaryStorage is not { ItemStorage.Items.Count: > 0 } temporaryStorage)
            {
                // Nothing to restore.
                return;
            }

            var count = temporaryStorage.ItemStorage.Items.Count;
            this._player.Logger.LogInformation("Returning {count} items from temporary storage to inventory for player {player}", count, this._player.Name);

            if (await inventory.TryTakeAllAsync(temporaryStorage).ConfigureAwait(false))
            {
                this._player.Logger.LogInformation("Returned {count} items from temporary storage to inventory for player {player}", count, this._player.Name);
                this.TemporaryStorage = null;
                return;
            }

            // We should never get so far, since the space is checked before doing anything with the temporary storage.
            // Log this critical situation - items may be lost if fallback also fails
            var items = temporaryStorage.Items.ToList();
            this._player.Logger.LogError(
                "CRITICAL: Could not return {count} items from temporary storage to inventory due to full inventory. Attempting fallback. Items: {items}",
                items.Count,
                string.Join(", ", items.Select(i => $"{i.Definition?.Name.ValueInNeutralLanguage ?? "Unknown"}(Slot:{i.ItemSlot})")));

            // The free cells of the inventory can get taken while the dialog is open (e.g. by items moved
            // out of the vault), or they are scattered, so that a big item like wings doesn't fit anymore.
            if (await this.PlaceItemsOneByOneAsync(temporaryStorage, inventory).ConfigureAwait(false))
            {
                this.TemporaryStorage = null;
            }
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error returning items from temporary storage to inventory");
        }
    }

    /// <summary>
    /// Returns the items which are left in the temporary storage (e.g. the tray of a crafting NPC whose
    /// dialog was closed with items in it) back to the inventory.
    /// </summary>
    /// <returns><c>true</c>, if items have been moved; <c>false</c> if there was nothing to move or the inventory has no space for all of them.</returns>
    public async ValueTask<bool> TryReturnTemporaryStorageItemsAsync()
    {
        if (this.Inventory is not { } inventory
            || this.TemporaryStorage is not { ItemStorage.Items.Count: > 0 } temporaryStorage)
        {
            return false;
        }

        var count = temporaryStorage.ItemStorage.Items.Count;
        if (await inventory.TryTakeAllAsync(temporaryStorage).ConfigureAwait(false))
        {
            this._player.Logger.LogInformation("Returned {count} items from the closed crafting dialog's temporary storage to the inventory of player {player}", count, this._player.Name);
            return true;
        }

        // Not all of them fit together: place them one by one, the vault is used as overflow.
        if (await this.PlaceItemsOneByOneAsync(temporaryStorage, inventory).ConfigureAwait(false))
        {
            return true;
        }

        this._player.Logger.LogWarning("Could not return all {count} items from the temporary storage to the inventory or vault of player {player}: not enough space.", count, this._player.Name);
        return false;
    }

    /// <summary>
    /// Places the items of the temporary storage one by one, the biggest first: into the inventory, and if that
    /// is full into the account vault. An item is only removed from the temporary storage when it has a new place.
    /// </summary>
    /// <returns><c>true</c>, if all items have been placed; <c>false</c> if at least one item stayed in the temporary storage.</returns>
    private async ValueTask<bool> PlaceItemsOneByOneAsync(IStorage temporaryStorage, IInventoryStorage inventory)
    {
        var vault = this.GetVaultForOverflow();
        var allPlaced = true;
        foreach (var item in temporaryStorage.Items.OrderByDescending(i => i.Definition?.Width * i.Definition?.Height ?? 0).ToList())
        {
            var slot = item.ItemSlot;
            await temporaryStorage.RemoveItemAsync(item).ConfigureAwait(false);
            if (await inventory.AddItemAsync(item).ConfigureAwait(false))
            {
                continue;
            }

            if (vault is not null && await vault.AddItemAsync(item).ConfigureAwait(false))
            {
                this._player.Logger.LogWarning(
                    "Inventory of player {player} was full, the item {item} was moved to the vault. id: {itemid}",
                    this._player.Name,
                    item,
                    item.GetId());
                continue;
            }

            // Neither inventory nor vault have space: don't lose it, keep it in the temporary storage.
            await temporaryStorage.AddItemAsync(slot, item).ConfigureAwait(false);
            allPlaced = false;
            this._player.Logger.LogError(
                "No space in inventory and vault of player {player} for item {item}. It stays in the temporary storage and can be recovered with its id: {itemid}",
                this._player.Name,
                item,
                item.GetId());
        }

        return allPlaced;
    }

    /// <summary>
    /// Gets the storage of the account vault, which is used when the inventory has no more space for items
    /// which are returned from the temporary storage.
    /// </summary>
    /// <returns>The vault storage, or <c>null</c> if the player has no account.</returns>
    private IStorage? GetVaultForOverflow()
    {
        if (this.Vault is { } openedVault)
        {
            return openedVault;
        }

        if (this._player.Account is not { } account)
        {
            return null;
        }

        account.Vault ??= this._player.PersistenceContext.CreateNew<ItemStorage>();
        var size = account.IsVaultExtended ? InventoryConstants.WarehouseSize * 2 : InventoryConstants.WarehouseSize;
        return new Storage(size, account.Vault);
    }

    private async ValueTask RestoreBackupInventoryAsync(IInventoryStorage inventory, BackupItemStorage backupInventory)
    {
        inventory.Clear();
        backupInventory.RestoreItemStates();
        foreach (var item in backupInventory.Items)
        {
            try
            {
                if (!await inventory.AddItemAsync(item.ItemSlot, item).ConfigureAwait(false)
                    && !await inventory.AddItemAsync(item).ConfigureAwait(false))
                {
                    this._player.Logger.LogError("Failed to restore item {item} from backup inventory of player {player}.", item, this._player.Name);
                }
            }
            catch (Exception ex)
            {
                this._player.Logger.LogError(ex, "Error restoring item {item} from backup inventory of player {player}.", item, this._player.Name);
            }
        }

        inventory.ItemStorage.Money = backupInventory.Money;
        this.BackupInventory = null;
        this.TemporaryStorage = null;
    }
}
