// <copyright file="ShopPrices.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;

/// <summary>
/// Functions about the prices of items in player shops: which prices are allowed, how they're paid, shown and
/// named in chat commands.
/// </summary>
public static class ShopPrices
{
    /// <summary>
    /// The maximum amount of item pieces which can be transferred to the client.
    /// </summary>
    /// <remarks>
    /// The extended client protocol transfers the amount of required items in a 16-bit field. A higher price
    /// would be shown differently than it's charged.
    /// </remarks>
    internal const int MaximumTransferableItemAmount = ushort.MaxValue;

    /// <summary>
    /// The separator between group and number of a price item in chat commands, e.g. "14/13".
    /// </summary>
    private const char ItemIdentifierSeparator = '/';

    private static readonly ZenShopCurrencyExchange ZenExchange = new();
    private static readonly ItemShopCurrencyExchange ItemExchange = new();
    private static readonly AccountCoinShopCurrencyExchange CoinExchange = new();

    private static readonly IReadOnlyList<(CashShopCoinType CoinType, string NameResourceKey, string[] ChatAliases, Func<PlayerShopCurrenciesConfiguration, bool> IsAllowed)> Coins =
    [
        (CashShopCoinType.WCoinC, nameof(PlayerMessage.ShopCurrencyWCoinC), ["wcoinc", "wcc"], c => c.IsWCoinCAllowed),
        (CashShopCoinType.WCoinP, nameof(PlayerMessage.ShopCurrencyWCoinP), ["wcoinp", "wcp"], c => c.IsWCoinPAllowed),
        (CashShopCoinType.GoblinPoints, nameof(PlayerMessage.ShopCurrencyGoblinPoints), ["goblin", "gp"], c => c.IsGoblinPointsAllowed),
    ];

    /// <summary>
    /// Short chat names of the jewels, which are commonly used as price items.
    /// Other items are named by group and number, e.g. "14/42".
    /// </summary>
    private static readonly IReadOnlyList<(ItemIdentifier Item, string[] ChatAliases)> JewelChatAliases =
    [
        (ItemConstants.JewelOfBless, ["bless", "job"]),
        (ItemConstants.JewelOfSoul, ["soul", "jos"]),
        (ItemConstants.JewelOfChaos, ["chaos", "joc"]),
        (ItemConstants.JewelOfCreation, ["creation", "jocr"]),
        (ItemConstants.JewelOfLife, ["life", "jol"]),
    ];

    /// <summary>
    /// Determines whether the price can currently be used in player shops.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="price">The price.</param>
    /// <returns><c>true</c>, if the price can be used.</returns>
    public static bool IsAllowed(IGameContext gameContext, ShopPrice price)
    {
        if (price.Kind == ShopPriceKind.Zen)
        {
            return true;
        }

        return PlayerShopCurrenciesFeaturePlugIn.GetActiveConfiguration(gameContext) is { } configuration
               && (price.Kind == ShopPriceKind.Item
                   ? configuration.PriceItems.Any(item => ShopPrice.IsSameItem(item, price.PriceItem))
                   : Coins.Any(coin => coin.CoinType == price.CoinType && coin.IsAllowed(configuration)));
    }

    /// <summary>
    /// Gets the items with which prices can currently be paid.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The item definitions; empty, if the feature is deactivated.</returns>
    public static IReadOnlyList<ItemDefinition> GetAllowedPriceItems(IGameContext gameContext)
    {
        return PlayerShopCurrenciesFeaturePlugIn.GetActiveConfiguration(gameContext)?.PriceItems.ToList() ?? [];
    }

    /// <summary>
    /// Gets the account coin types with which prices can currently be paid.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The coin types; empty, if the feature is deactivated.</returns>
    public static IReadOnlyList<CashShopCoinType> GetAllowedCoinTypes(IGameContext gameContext)
    {
        if (PlayerShopCurrenciesFeaturePlugIn.GetActiveConfiguration(gameContext) is not { } configuration)
        {
            return [];
        }

        return Coins.Where(coin => coin.IsAllowed(configuration)).Select(coin => coin.CoinType).ToList();
    }

    /// <summary>
    /// Gets the maximum amount of a price of the specified kind.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="kind">The kind of the price.</param>
    /// <returns>The maximum amount.</returns>
    public static int GetMaximumAmount(IGameContext gameContext, ShopPriceKind kind)
    {
        var configuration = PlayerShopCurrenciesFeaturePlugIn.GetActiveConfiguration(gameContext) ?? new PlayerShopCurrenciesConfiguration();
        return kind switch
        {
            ShopPriceKind.Item => Math.Min(
                configuration.MaximumItemAmount > 0 ? configuration.MaximumItemAmount : PlayerShopCurrenciesConfiguration.DefaultMaximumItemAmount,
                MaximumTransferableItemAmount),
            ShopPriceKind.AccountCoin => configuration.MaximumCoinAmount > 0 ? configuration.MaximumCoinAmount : PlayerShopCurrenciesConfiguration.DefaultMaximumCoinAmount,
            _ => int.MaxValue,
        };
    }

    /// <summary>
    /// Tries to parse the currency of a price of a chat command: "zen", the name of an account coin or a jewel,
    /// or the group and number of an item, e.g. "14/13".
    /// </summary>
    /// <param name="gameContext">The game context, which defines the items.</param>
    /// <param name="text">The text.</param>
    /// <param name="amount">The amount of the price.</param>
    /// <param name="price">The parsed price.</param>
    /// <returns><c>true</c>, if the text describes a currency.</returns>
    public static bool TryParse(IGameContext gameContext, string? text, int amount, out ShopPrice price)
    {
        price = new ShopPrice(amount);
        if (string.IsNullOrWhiteSpace(text) || string.Equals(text.Trim(), "zen", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var trimmed = text.Trim();
        if (Coins.FirstOrDefault(coin => coin.ChatAliases.Contains(trimmed, StringComparer.OrdinalIgnoreCase)) is { NameResourceKey: not null } namedCoin)
        {
            price = new ShopPrice(amount, CoinType: namedCoin.CoinType);
            return true;
        }

        if (GetItemIdentifier(trimmed) is not { } identifier
            || gameContext.Configuration.Items.FirstOrDefault(item => item.Group == identifier.Group && item.Number == identifier.Number) is not { } definition)
        {
            return false;
        }

        price = new ShopPrice(amount, definition);
        return true;
    }

    /// <summary>
    /// Gets the chat names of the currencies which are currently allowed, e.g. to show them to the player.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The chat names.</returns>
    public static IEnumerable<string> GetAllowedChatNames(IGameContext gameContext)
    {
        yield return "zen";
        foreach (var item in GetAllowedPriceItems(gameContext))
        {
            yield return JewelChatAliases.FirstOrDefault(jewel => jewel.Item == new ItemIdentifier(item.Number, item.Group)).ChatAliases?[0]
                         ?? FormattableString.Invariant($"{item.Group}{ItemIdentifierSeparator}{item.Number}");
        }

        foreach (var coinType in GetAllowedCoinTypes(gameContext))
        {
            yield return Coins.First(coin => coin.CoinType == coinType).ChatAliases[0];
        }
    }

    /// <summary>
    /// Gets the localized name of the currency of a price, e.g. "Jewel of Bless" or "Goblin Points".
    /// </summary>
    /// <param name="player">The player to which the name is shown.</param>
    /// <param name="price">The price.</param>
    /// <returns>The localized name.</returns>
    public static string GetCurrencyName(this Player player, ShopPrice price)
    {
        switch (price.Kind)
        {
            case ShopPriceKind.Item:
                using (CultureHelper.SetTemporaryCulture(player.Culture))
                {
                    return price.PriceItem!.GetNameForLevel(0);
                }

            case ShopPriceKind.AccountCoin:
                return player.GetLocalizedMessage(Coins.First(coin => coin.CoinType == price.CoinType).NameResourceKey);
            default:
                return player.GetLocalizedMessage(nameof(PlayerMessage.ShopCurrencyZen));
        }
    }

    /// <summary>
    /// Gets the exchange which pays prices of the specified kind.
    /// </summary>
    /// <param name="kind">The kind of the price.</param>
    /// <returns>The exchange.</returns>
    public static IShopCurrencyExchange GetExchange(ShopPriceKind kind)
    {
        return kind switch
        {
            ShopPriceKind.Item => ItemExchange,
            ShopPriceKind.AccountCoin => CoinExchange,
            _ => ZenExchange,
        };
    }

    /// <summary>
    /// Shows the allowed prices to the player, if the feature is activated.
    /// </summary>
    /// <remarks>
    /// Nothing is sent when the feature is deactivated, so that the protocol stays the same as without it.
    /// </remarks>
    /// <param name="player">The player.</param>
    /// <returns>The value task.</returns>
    public static async ValueTask ShowAllowedPricesAsync(this Player player)
    {
        if (PlayerShopCurrenciesFeaturePlugIn.GetActiveConfiguration(player.GameContext) is null)
        {
            return;
        }

        var priceItems = GetAllowedPriceItems(player.GameContext);
        var coinTypes = GetAllowedCoinTypes(player.GameContext);
        await player.InvokeViewPlugInAsync<IShowShopCurrenciesPlugIn>(p => p.ShowShopCurrenciesAsync(priceItems, coinTypes)).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether the client of the player can show and buy items with other prices than zen.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c>, if the client supports other prices.</returns>
    public static bool SupportsShopCurrencies(this Player player)
    {
        return player.ViewPlugIns.GetPlugIn<IShowShopCurrenciesPlugIn>() is not null;
    }

    private static ItemIdentifier? GetItemIdentifier(string text)
    {
        if (JewelChatAliases.FirstOrDefault(jewel => jewel.ChatAliases.Contains(text, StringComparer.OrdinalIgnoreCase)) is { ChatAliases: not null } jewel)
        {
            return jewel.Item;
        }

        var parts = text.Split(ItemIdentifierSeparator);
        if (parts.Length == 2
            && byte.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var group)
            && short.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var number))
        {
            return new ItemIdentifier(number, group);
        }

        return null;
    }
}
