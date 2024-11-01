// <copyright file="PetInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Pet;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for pet info request packets.
/// </summary>
[PlugIn(nameof(PetInfoRequestHandlerPlugIn), "Handler for pet info request packets.")]
[Guid("DA535FF5-A23D-4C36-877F-73D1D811F146")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
internal class PetInfoRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly PetInfoRequestAction _requestAction = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => PetInfoRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PetInfoRequest message = packet;
        await this._requestAction.RequestPetInfoAsync(player, message.ItemSlot, ConvertStorageLocation(message.Storage)).ConfigureAwait(false);
    }

    private static PetStorageLocation ConvertStorageLocation(StorageType location)
    {
        return location switch
        {
            StorageType.Crafting => PetStorageLocation.Crafting,
            StorageType.Inventory => PetStorageLocation.Inventory,
            StorageType.InventoryPetSlot => PetStorageLocation.InventoryPetSlot,
            StorageType.PersonalShop => PetStorageLocation.PersonalShop,
            StorageType.TradeOther => PetStorageLocation.TradeOther,
            StorageType.TradeOwn => PetStorageLocation.TradeOwn,
            StorageType.Vault => PetStorageLocation.Vault,
            _ => throw new ArgumentOutOfRangeException(nameof(location)),
        };
    }
}