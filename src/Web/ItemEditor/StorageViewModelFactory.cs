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
    /// <param name="extensions">The storage extensions.</param>
    /// <returns>The created view model.</returns>
    internal static StorageViewModel CreateViewModel(this ItemStorage storage, StorageType storageType, byte extensions = 0)
    {
        switch (storageType)
        {
            case StorageType.Inventory:
                return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.InventoryRows,
                    InventoryConstants.EquippableSlotsCount,
                    (byte)(InventoryConstants.GetInventorySize(0) - 1));
            case StorageType.InventoryExtension:
                var emptyRowsToPersonalStore = (byte)((InventoryConstants.MaximumNumberOfExtensions - extensions) * InventoryConstants.RowsOfOneExtension);
                return new StorageViewModel(
                    storage,
                    storageType,
                    extensions * InventoryConstants.RowsOfOneExtension,
                    InventoryConstants.FirstExtensionItemSlotIndex,
                    (byte)(InventoryConstants.GetInventorySize(extensions) - 1),
                    emptyRowsToPersonalStore);
            case StorageType.PersonalStore:
                var emptyRowsToInventory = (byte)((InventoryConstants.MaximumNumberOfExtensions - extensions) * InventoryConstants.RowsOfOneExtension);
                return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.StoreRows,
                    InventoryConstants.FirstStoreItemSlotIndex,
                    (byte)(InventoryConstants.FirstStoreItemSlotIndex + InventoryConstants.StoreSize),
                    null,
                    emptyRowsToInventory);
            case StorageType.VaultExtension:
                return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.WarehouseRows,
                    InventoryConstants.WarehouseSize,
                    (InventoryConstants.WarehouseSize * 2) - 1);
            case StorageType.Vault:
            case StorageType.Merchant:
                return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.WarehouseRows,
                    0,
                    InventoryConstants.WarehouseSize - 1);
            default:
                throw new NotImplementedException();
        }
    }
}