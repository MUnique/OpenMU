﻿// <copyright file="EnumExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Extension methods to convert enum values.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts the enum value into the enum type used in the packets.
    /// </summary>
    /// <param name="storage">The enum value.</param>
    /// <returns>The converted value.</returns>
    public static ItemStorageKind Convert(this Storages storage)
    {
        return storage switch
        {
            Storages.Inventory => ItemStorageKind.Inventory,
            Storages.ChaosMachine => ItemStorageKind.ChaosMachine,
            Storages.PersonalStore => ItemStorageKind.PlayerShop,
            Storages.Trade => ItemStorageKind.Trade,
            Storages.Vault => ItemStorageKind.Vault,
            Storages.PetTrainer => ItemStorageKind.PetTrainer,
            Storages.Refinery => ItemStorageKind.Refinery,
            Storages.Smelting => ItemStorageKind.Smelting,
            Storages.ItemRestore => ItemStorageKind.ItemRestore,
            Storages.ChaosCardMaster => ItemStorageKind.ChaosCardMaster,
            Storages.CherryBlossomSpirit => ItemStorageKind.CherryBlossomSpirit,
            Storages.SeedCrafting => ItemStorageKind.SeedCrafting,
            Storages.SeedSphereCrafting => ItemStorageKind.SeedSphereCrafting,
            Storages.SeedMountCrafting => ItemStorageKind.SeedMountCrafting,
            Storages.SeedUnmountCrafting => ItemStorageKind.SeedUnmountCrafting,
            _ => throw new NotImplementedException($"Unhandled case {storage}."),
        };
    }

    /// <summary>
    /// Converts the enum value into the enum type used in the packets.
    /// </summary>
    /// <param name="storage">The enum value.</param>
    /// <returns>The converted value.</returns>
    public static Storages Convert(this ItemStorageKind storage)
    {
        return storage switch
        {
            ItemStorageKind.Inventory => Storages.Inventory,
            ItemStorageKind.ChaosMachine => Storages.ChaosMachine,
            ItemStorageKind.PlayerShop => Storages.PersonalStore,
            ItemStorageKind.Trade => Storages.Trade,
            ItemStorageKind.Vault => Storages.Vault,
            ItemStorageKind.PetTrainer => Storages.PetTrainer,
            ItemStorageKind.Refinery => Storages.Refinery,
            ItemStorageKind.Smelting => Storages.Smelting,
            ItemStorageKind.ItemRestore => Storages.ItemRestore,
            ItemStorageKind.ChaosCardMaster => Storages.ChaosCardMaster,
            ItemStorageKind.CherryBlossomSpirit => Storages.CherryBlossomSpirit,
            ItemStorageKind.SeedCrafting => Storages.SeedCrafting,
            ItemStorageKind.SeedSphereCrafting => Storages.SeedSphereCrafting,
            ItemStorageKind.SeedMountCrafting => Storages.SeedMountCrafting,
            ItemStorageKind.SeedUnmountCrafting => Storages.SeedUnmountCrafting,
            _ => throw new NotImplementedException($"Unhandled case {storage}."),
        };
    }

    /// <summary>
    /// Converts the enum value into the enum type used in the packets.
    /// </summary>
    /// <param name="reason">The enum value.</param>
    /// <returns>The converted value.</returns>
    public static ItemPickUpRequestFailed.ItemPickUpFailReason Convert(this ItemPickFailReason reason)
    {
        return reason switch
        {
            ItemPickFailReason.General => ItemPickUpRequestFailed.ItemPickUpFailReason.General,
            ItemPickFailReason.ItemStacked => ItemPickUpRequestFailed.ItemPickUpFailReason.ItemStacked,
            ItemPickFailReason.MaximumInventoryMoneyReached => ItemPickUpRequestFailed.ItemPickUpFailReason.__MaximumInventoryMoneyReached,
            _ => throw new NotImplementedException($"Unhandled case {reason}"),
        };
    }

    /// <summary>
    /// Converts the enum value into the enum type used in the packets.
    /// </summary>
    /// <param name="result">The enum value.</param>
    /// <returns>The converted value.</returns>
    public static PlayerShopSetItemPriceResponse.ItemPriceSetResult Convert(this ItemPriceResult result)
    {
        return result switch
        {
            ItemPriceResult.Failed => PlayerShopSetItemPriceResponse.ItemPriceSetResult.Failed,
            ItemPriceResult.Success => PlayerShopSetItemPriceResponse.ItemPriceSetResult.Success,
            ItemPriceResult.ItemSlotOutOfRange => PlayerShopSetItemPriceResponse.ItemPriceSetResult.ItemSlotOutOfRange,
            ItemPriceResult.ItemNotFound => PlayerShopSetItemPriceResponse.ItemPriceSetResult.ItemNotFound,
            ItemPriceResult.PriceNegative => PlayerShopSetItemPriceResponse.ItemPriceSetResult.PriceNegative,
            ItemPriceResult.ItemIsBlocked => PlayerShopSetItemPriceResponse.ItemPriceSetResult.ItemIsBlocked,
            ItemPriceResult.CharacterLevelTooLow => PlayerShopSetItemPriceResponse.ItemPriceSetResult.CharacterLevelTooLow,
            _ => throw new NotImplementedException($"Unhandled case {result}."),
        };
    }
}