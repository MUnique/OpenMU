// <copyright file="ICashShopProductDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A strategy which delivers a cash shop product when a player uses it from the cash shop storage,
/// instead of putting its items into the inventory. It's selected by the item of the product, e.g.
/// a character card which unlocks a character class.
/// </summary>
/// <remarks>
/// A strategy for an item number takes precedence over a strategy for the whole item group (key
/// without number). A product whose item has no active strategy is put into the inventory, unless
/// an inactive strategy is known for it: then it can't be bought or used until the strategy is active.
/// </remarks>
[Guid("20903A72-1358-4AB7-A338-DB44E79EB656")]
[PlugInPoint("Cash shop product delivery", "Strategies which deliver cash shop products that aren't put into the inventory, selected by the item of the product.")]
public interface ICashShopProductDeliveryPlugIn : IStrategyPlugIn<ItemIdentifier>
{
    /// <summary>
    /// Determines whether this strategy can deliver the product at all, regardless of the player.
    /// A package can only be bought when all of its products can be delivered.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <returns><c>true</c>, if this strategy can deliver the product; otherwise, <c>false</c>.</returns>
    bool CanDeliver(CashShopProduct product);

    /// <summary>
    /// Delivers the product to the player.
    /// </summary>
    /// <remarks>
    /// It's called while the player's persistence is locked, before the storage item is removed and
    /// the progress is saved. Change only data which is saved with the player, and nothing at all
    /// when it doesn't succeed; the storage item is kept then.
    /// </remarks>
    /// <param name="player">The player who uses the product.</param>
    /// <param name="product">The product.</param>
    /// <returns>The result; anything but <see cref="CashShopUseResult.Success"/> keeps the storage item.</returns>
    ValueTask<CashShopUseResult> DeliverAsync(Player player, CashShopProduct product);

    /// <summary>
    /// Is called after the product was delivered and the persistence lock was released,
    /// e.g. to show a message to the player.
    /// </summary>
    /// <param name="player">The player who used the product.</param>
    /// <param name="product">The product.</param>
    /// <returns>A value task which completes when the plugin is done.</returns>
    ValueTask DeliveredAsync(Player player, CashShopProduct product) => ValueTask.CompletedTask;
}
