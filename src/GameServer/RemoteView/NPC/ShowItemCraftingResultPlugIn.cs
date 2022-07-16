// <copyright file="ShowItemCraftingResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowItemCraftingResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowItemCraftingResultPlugIn), "The default implementation of the IShowItemCraftingResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("D4339CC0-3E44-4F51-9186-9C3CB02F99F6")]
public class ShowItemCraftingResultPlugIn : IShowItemCraftingResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowItemCraftingResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowItemCraftingResultPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowResultAsync(CraftingResult result, Item? createdItem)
    {
        var itemData = new byte[this._player.ItemSerializer.NeededSpace];
        if (createdItem is { })
        {
            this._player.ItemSerializer.SerializeItem(itemData, createdItem);
        }

        await this._player.Connection.SendItemCraftingResultAsync(Convert(result), itemData).ConfigureAwait(false);
    }

    private static ItemCraftingResult.CraftingResult Convert(CraftingResult result)
    {
        return result switch
        {
            CraftingResult.Success => ItemCraftingResult.CraftingResult.Success,
            CraftingResult.Failed => ItemCraftingResult.CraftingResult.Failed,
            CraftingResult.NotEnoughMoney => ItemCraftingResult.CraftingResult.NotEnoughMoney,
            CraftingResult.CharacterClassTooLow => ItemCraftingResult.CraftingResult.CharacterClassTooLow,
            CraftingResult.CharacterLevelTooLow => ItemCraftingResult.CraftingResult.CharacterLevelTooLow,
            CraftingResult.IncorrectBloodCastleItems => ItemCraftingResult.CraftingResult.IncorrectBloodCastleItems,
            CraftingResult.NotEnoughMoneyForBloodCastle => ItemCraftingResult.CraftingResult.NotEnoughMoneyForBloodCastle,
            CraftingResult.IncorrectMixItems => ItemCraftingResult.CraftingResult.IncorrectMixItems,
            CraftingResult.InvalidItemLevel => ItemCraftingResult.CraftingResult.InvalidItemLevel,
            CraftingResult.LackingMixItems => ItemCraftingResult.CraftingResult.LackingMixItems,
            CraftingResult.TooManyItems => ItemCraftingResult.CraftingResult.TooManyItems,
            _ => throw new ArgumentException($"Unknown crafting result {result}.", nameof(result)),
        };
    }
}