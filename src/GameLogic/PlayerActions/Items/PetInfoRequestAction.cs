// <copyright file="PetInfoRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Pet;

/// <summary>
/// Action to request the pet information about a pet item.
/// </summary>
public class PetInfoRequestAction
{
    /// <summary>
    /// Requests the pet information.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="petStorageLocation">The pet storage location.</param>
    public async ValueTask RequestPetInfoAsync(Player player, byte slot, PetStorageLocation petStorageLocation)
    {
        var targetStorage = this.DetermineStorage(player, petStorageLocation);
        if (targetStorage is null)
        {
            player.Logger.LogDebug($"Requested pet info for item at slot {slot}, storage type {petStorageLocation}, but storage is not found.");
            return;
        }

        if (targetStorage.GetItem(slot) is not { } pet)
        {
            player.Logger.LogDebug($"Requested pet info for item at slot {slot}, storage type {petStorageLocation}, but item is not found.");
            return;
        }

        if (!pet.IsTrainablePet())
        {
            player.Logger.LogDebug($"Requested pet info for item at slot {slot}, storage type {petStorageLocation}, but item is not a pet. Item: {pet}");
            return;
        }

        await player.InvokeViewPlugInAsync<IPetInfoViewPlugIn>(p => p.ShowPetInfoAsync(pet, slot, petStorageLocation)).ConfigureAwait(false);
    }

    private IStorage? DetermineStorage(Player player, PetStorageLocation petStorageLocation)
    {
        return petStorageLocation switch
        {
            PetStorageLocation.Inventory => player.Inventory,
            PetStorageLocation.InventoryPetSlot => player.Inventory,
            PetStorageLocation.Crafting => player.TemporaryStorage,
            PetStorageLocation.PersonalShop =>
                player.LastRequestedPlayerStore?.TryGetTarget(out var targetPlayer) ?? false
                    ? targetPlayer.ShopStorage
                    : null,
            PetStorageLocation.TradeOwn => player.TemporaryStorage,
            PetStorageLocation.TradeOther => player.TradingPartner?.TemporaryStorage,
            PetStorageLocation.Vault => player.Vault,
            _ => null,
        };
    }
}