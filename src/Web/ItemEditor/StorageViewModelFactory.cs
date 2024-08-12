// <copyright file="StorageViewModelFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using MUnique.OpenMU.DataModel;
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
    /// <param name="extensionIndex">The storage extension index.</param>
    /// <returns>The created view model.</returns>
    internal static StorageViewModel CreateViewModel(this ItemStorage storage, StorageType storageType, byte extensions = 0, byte extensionIndex = 0)
    {
        switch (storageType)
        {
            case StorageType.Inventory:
            {
                var emptyRowsToPersonalStore = (byte)((InventoryConstants.MaximumNumberOfExtensions - extensions) * InventoryConstants.RowsOfOneExtension);
                var emptyRowsToNextStorage = extensions == 0 ? emptyRowsToPersonalStore : (byte)0;
                    return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.InventoryRows,
                    InventoryConstants.EquippableSlotsCount,
                    (byte)(InventoryConstants.GetInventorySize(0) - 1),
                    emptyRowsToNextStorage);
            }
            case StorageType.InventoryExtension:
            {
                var emptyRowsToPersonalStore = (byte)((InventoryConstants.MaximumNumberOfExtensions - extensions) * InventoryConstants.RowsOfOneExtension);
                var emptyRowsToNextStorage = extensions > (extensionIndex + 1) ? (byte)0 : emptyRowsToPersonalStore;
                var startIndex = InventoryConstants.FirstExtensionItemSlotIndex + (extensionIndex * InventoryConstants.RowsOfOneExtension * InventoryConstants.RowSize);
                return new StorageViewModel(
                    storage,
                    storageType,
                    InventoryConstants.RowsOfOneExtension,
                    startIndex,
                    (byte)(InventoryConstants.GetInventorySize(extensionIndex + 1) - 1),
                    emptyRowsToNextStorage);
            }
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