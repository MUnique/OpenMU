// <copyright file="ShopPrice.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The price of an item in a player shop.
/// </summary>
/// <param name="Amount">The amount: zen, pieces of the <paramref name="PriceItem"/> or account coins.</param>
/// <param name="PriceItem">The definition of the items with which the price is paid; <c>null</c>, if it's not paid with items.</param>
/// <param name="CoinType">The account coin type with which the price is paid; <c>null</c>, if it's not paid with account coins.</param>
public readonly record struct ShopPrice(int Amount, ItemDefinition? PriceItem = null, CashShopCoinType? CoinType = null)
{
    /// <summary>
    /// Gets the kind of the price.
    /// </summary>
    public ShopPriceKind Kind => this.PriceItem is not null
        ? ShopPriceKind.Item
        : this.CoinType is not null ? ShopPriceKind.AccountCoin : ShopPriceKind.Zen;

    /// <summary>
    /// Determines whether this price is paid with the same items or coins as another one, regardless of the amount.
    /// </summary>
    /// <param name="other">The other price.</param>
    /// <returns><c>true</c>, if both prices are paid the same way.</returns>
    public bool HasSameCurrency(ShopPrice other)
    {
        return this.Kind == other.Kind
               && this.CoinType == other.CoinType
               && IsSameItem(this.PriceItem, other.PriceItem);
    }

    /// <summary>
    /// Determines whether two item definitions describe the same item, by group and number.
    /// </summary>
    /// <param name="left">The left definition.</param>
    /// <param name="right">The right definition.</param>
    /// <returns><c>true</c>, if both describe the same item.</returns>
    internal static bool IsSameItem(ItemDefinition? left, ItemDefinition? right)
    {
        if (left is null || right is null)
        {
            return left is null && right is null;
        }

        return left.Group == right.Group && left.Number == right.Number;
    }
}
