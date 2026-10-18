// <copyright file="ShopPurchase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.Persistence;

/// <summary>
/// The purchase of an item from a player shop, for which the price is paid.
/// </summary>
/// <param name="Buyer">The buyer.</param>
/// <param name="Seller">The seller, which owns the shop.</param>
/// <param name="Price">The price.</param>
/// <param name="TradeContext">
/// The persistence context in which the sold item is moved from the seller to the buyer.
/// Items which are paid with are moved in it, too, so that everything is saved together.
/// </param>
public sealed record ShopPurchase(Player Buyer, Player Seller, ShopPrice Price, IContext TradeContext)
{
    /// <summary>
    /// Gets the items which the buyer paid with, together with their previous slots in the inventory of the buyer.
    /// They are already moved into the inventory of the seller and attached to the <see cref="TradeContext"/>.
    /// </summary>
    public IList<(Item Item, byte BuyerSlot)> PaymentItems { get; } = new List<(Item Item, byte BuyerSlot)>();
}
