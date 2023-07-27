// <copyright file="PetInfoViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Pet;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.Pet;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IPetInfoViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(PetInfoViewPlugIn), "The default implementation of the IPetInfoViewPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("9346B880-7B4E-4FEF-A342-419E88392250")]
public class PetInfoViewPlugIn : IPetInfoViewPlugIn
{
    private const byte DarkHorseNumber = 4;
    private const byte DarkRavenNumber = 5;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetInfoViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PetInfoViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowPetInfoAsync(Item petItem, byte slot, PetStorageLocation petStorageLocation)
    {
        await this._player.Connection.SendPetInfoResponseAsync(
                DeterminePetType(petItem),
                ConvertStorageLocation(petStorageLocation),
                slot,
                petItem.Level,
                (uint)petItem.PetExperience,
                petItem.Durability())
            .ConfigureAwait(false);
    }

    private static PetType DeterminePetType(Item item)
    {
        var itemDefinition = item.Definition;
        if (itemDefinition?.Group != 13
            || itemDefinition.Number is not (DarkHorseNumber or DarkRavenNumber))
        {
            throw new ArgumentException($"Item {item} is not a known pet");
        }

        return itemDefinition.Number == DarkHorseNumber ? PetType.DarkHorse : PetType.DarkRaven;
    }

    private static StorageType ConvertStorageLocation(PetStorageLocation location)
    {
        return location switch
        {
            PetStorageLocation.Crafting => StorageType.Crafting,
            PetStorageLocation.Inventory => StorageType.Inventory,
            PetStorageLocation.InventoryPetSlot => StorageType.InventoryPetSlot,
            PetStorageLocation.PersonalShop => StorageType.PersonalShop,
            PetStorageLocation.TradeOther => StorageType.TradeOther,
            PetStorageLocation.TradeOwn => StorageType.TradeOwn,
            PetStorageLocation.Vault => StorageType.Vault,
            _ => throw new ArgumentOutOfRangeException(nameof(location)),
        };
    }
}