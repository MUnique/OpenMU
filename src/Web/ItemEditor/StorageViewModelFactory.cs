// <copyright file="StorageViewModelFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Factory for <see cref="StorageViewModel"/>s.
/// </summary>
internal static class StorageViewModelFactory
{
    /// <summary>
    /// Creates the view model.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <param name="storageType">Type of the storage.</param>
    /// <param name="inventoryExtensions">The inventory extensions.</param>
    /// <returns>The created view model.</returns>
    internal static StorageViewModel CreateViewModel(this ItemStorage storage, StorageType storageType, int inventoryExtensions = 0)
    {
        return storageType switch
        {
            StorageType.Inventory => new StorageViewModel(storage, InventoryConstants.InventoryRows, InventoryConstants.EquippableSlotsCount, (byte)(InventoryConstants.EquippableSlotsCount + InventoryConstants.GetInventorySize(inventoryExtensions))),
            StorageType.PersonalStore => new StorageViewModel(storage, InventoryConstants.StoreRows, InventoryConstants.FirstStoreItemSlotIndex, (byte)(InventoryConstants.FirstStoreItemSlotIndex + InventoryConstants.StoreSize)),
            StorageType.Vault => new StorageViewModel(storage, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize),
            StorageType.Merchant => new StorageViewModel(storage, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize),
            _ => throw new NotImplementedException()
        };
    }
}