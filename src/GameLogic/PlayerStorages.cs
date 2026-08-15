// <copyright file="PlayerStorages.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

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
    /// Gets or sets the backup of the inventory, which is created before a trade.
    /// </summary>
    public BackupItemStorage? BackupInventory { get; set; }

    /// <summary>
    /// Creates the storages for the selected character, when it entered the world.
    /// </summary>
    /// <param name="character">The selected character.</param>
    public void CreateForCharacter(Character character)
    {
        this.Inventory = new InventoryStorage(this._player, this._player.GameContext);
        this.ShopStorage = new ShopStorage(character);
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

            // Try one more time to force-add items individually using the captured list
            foreach (var item in items)
            {
                await temporaryStorage.RemoveItemAsync(item).ConfigureAwait(false);
                if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
                {
                    this._player.Logger.LogError("Failed to return item {item} to inventory. Item is lost. id: {itemid}", item, item.GetId());
                }
            }

            this.TemporaryStorage = null;
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error returning items from temporary storage to inventory");
        }
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
