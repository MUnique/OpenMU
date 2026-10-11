// <copyright file="PlayerShopPriceEncoding.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

/// <summary>
/// Encodes and decodes the prices of player shop items in the fields of the extended client protocol:
/// <c>MoneyPrice</c>, <c>PriceItemType</c> and <c>RequiredItemAmount</c>.
/// </summary>
/// <remarks>
/// The same encoding is used by the item list, the set price request and the allowed price types.
/// </remarks>
internal static class PlayerShopPriceEncoding
{
    /// <summary>
    /// The value of <c>PriceItemType</c> for prices in <see cref="CashShopCoinType.WCoinC"/>.
    /// The other coin types follow in the order of <see cref="CashShopCoinType"/>, as defined by the packet description.
    /// </summary>
    private const int FirstAccountCoinPriceItemType = 0xFFF0;

    /// <summary>
    /// The number of bits of the item number in <c>PriceItemType</c>; the group is in the bits above.
    /// </summary>
    private const int ItemGroupShift = 12;

    /// <summary>
    /// The mask of the item number in <c>PriceItemType</c>.
    /// </summary>
    private const int ItemNumberMask = (1 << ItemGroupShift) - 1;

    /// <summary>
    /// Gets the value of <c>PriceItemType</c> for prices which are paid with the item.
    /// </summary>
    /// <param name="priceItem">The price item.</param>
    /// <returns>The value of <c>PriceItemType</c>.</returns>
    public static ushort GetPriceItemType(ItemDefinition priceItem)
    {
        return (ushort)((priceItem.Group << ItemGroupShift) | (priceItem.Number & ItemNumberMask));
    }

    /// <summary>
    /// Gets the value of <c>PriceItemType</c> for prices which are paid with the account coin.
    /// </summary>
    /// <param name="coinType">The coin type.</param>
    /// <returns>The value of <c>PriceItemType</c>.</returns>
    public static ushort GetPriceItemType(CashShopCoinType coinType)
    {
        return (ushort)(FirstAccountCoinPriceItemType + (int)coinType);
    }

    /// <summary>
    /// Encodes the price into the fields of the protocol.
    /// </summary>
    /// <param name="price">The price.</param>
    /// <returns>The values of the fields.</returns>
    public static (uint MoneyPrice, ushort PriceItemType, ushort RequiredItemAmount) Encode(ShopPrice price)
    {
        return price.Kind switch
        {
            ShopPriceKind.Item => (0, GetPriceItemType(price.PriceItem!), (ushort)Math.Clamp(price.Amount, 0, ushort.MaxValue)),
            ShopPriceKind.AccountCoin => ((uint)Math.Max(price.Amount, 0), GetPriceItemType(price.CoinType!.Value), 0),
            _ => ((uint)Math.Max(price.Amount, 0), 0, 0),
        };
    }

    /// <summary>
    /// Decodes the price from the fields of the protocol.
    /// </summary>
    /// <param name="moneyPrice">The value of <c>MoneyPrice</c>.</param>
    /// <param name="priceItemType">The value of <c>PriceItemType</c>.</param>
    /// <param name="requiredItemAmount">The value of <c>RequiredItemAmount</c>.</param>
    /// <param name="gameConfiguration">The game configuration, which defines the items.</param>
    /// <returns>The price; <c>null</c>, if the price item is unknown.</returns>
    /// <remarks>
    /// An amount which doesn't fit into an <see cref="int"/> is decoded as negative, so that it's rejected.
    /// </remarks>
    public static ShopPrice? Decode(uint moneyPrice, ushort priceItemType, ushort requiredItemAmount, GameConfiguration gameConfiguration)
    {
        if (requiredItemAmount > 0)
        {
            var group = priceItemType >> ItemGroupShift;
            var number = priceItemType & ItemNumberMask;
            return gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number) is { } definition
                ? new ShopPrice(requiredItemAmount, definition)
                : null;
        }

        var amount = moneyPrice > int.MaxValue ? -1 : (int)moneyPrice;
        var coinType = (CashShopCoinType)(priceItemType - FirstAccountCoinPriceItemType);
        return priceItemType >= FirstAccountCoinPriceItemType && Enum.IsDefined(coinType)
            ? new ShopPrice(amount, CoinType: coinType)
            : new ShopPrice(amount);
    }
}
