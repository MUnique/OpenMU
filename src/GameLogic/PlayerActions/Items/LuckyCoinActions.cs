// <copyright file="LuckyCoinActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.LuckyCoins;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The actions of the lucky coin dialogs of the NPC Delgado: showing the count of the registered coins,
/// registering a coin and exchanging coins for an item.
/// </summary>
/// <remarks>
/// A registered coin is used up and counted at the character. In the original game, the registered coins took part in a lottery,
/// which isn't part of the game server.
/// </remarks>
public class LuckyCoinActions
{
    /// <summary>
    /// Shows the count of the lucky coins which the character registered.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowRegisteredCountAsync(Player player)
    {
        if (!CanUseLuckyCoinDialog(player))
        {
            return;
        }

        var count = GetRegisteredCount(player);
        await player.InvokeViewPlugInAsync<ILuckyCoinViewPlugIn>(p => p.ShowRegisteredCountAsync(count)).ConfigureAwait(false);
    }

    /// <summary>
    /// Registers one lucky coin of the inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask RegisterAsync(Player player)
    {
        if (!CanUseLuckyCoinDialog(player) || player.SelectedCharacter is not { } character)
        {
            return;
        }

        if (GetCoins(player).FirstOrDefault() is not { } coin)
        {
            await player.InvokeViewPlugInAsync<ILuckyCoinViewPlugIn>(p => p.ShowRegistrationResultAsync(LuckyCoinRegistrationResult.NotEnoughItems, GetRegisteredCount(player))).ConfigureAwait(false);
            return;
        }

        await RemoveCoinsAsync(player, [coin], 1).ConfigureAwait(false);
        var registeredCount = GetRegisteredCount(player) + 1;
        GetOrCreateRegisteredCoinsAttribute(player, character).Value = registeredCount;
        player.Logger.LogInformation("Player {player} registered a lucky coin, now {count} coins are registered.", player, registeredCount);
        await player.InvokeViewPlugInAsync<ILuckyCoinViewPlugIn>(p => p.ShowRegistrationResultAsync(LuckyCoinRegistrationResult.Registered, registeredCount)).ConfigureAwait(false);
    }

    /// <summary>
    /// Exchanges lucky coins of the inventory for a random item of the configured rewards.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="coinCount">The count of the coins which should be exchanged.</param>
    public async ValueTask ExchangeAsync(Player player, int coinCount)
    {
        if (!CanUseLuckyCoinDialog(player)
            || LuckyCoinFeaturePlugIn.GetActiveConfiguration(player.GameContext) is not { } configuration
            || player.Inventory is not { } inventory)
        {
            return;
        }

        var result = await TryExchangeAsync(player, inventory, configuration, coinCount).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ILuckyCoinViewPlugIn>(p => p.ShowExchangeResultAsync(result)).ConfigureAwait(false);
    }

    private static bool CanUseLuckyCoinDialog(Player player)
    {
        return player.OpenedNpc?.Definition.NpcWindow == NpcWindow.DelgadoLuckyCoinRegistration
               && LuckyCoinFeaturePlugIn.GetActiveConfiguration(player.GameContext) is not null;
    }

    private static List<Item> GetCoins(Player player)
    {
        return player.Inventory?.Items
            .Where(item => item.Definition is { } definition
                           && definition.Group == ItemConstants.LuckyCoin.Group
                           && definition.Number == ItemConstants.LuckyCoin.Number
                           && item.Durability > 0)
            .ToList() ?? [];
    }

    private static int GetRegisteredCount(Player player)
    {
        return (int)(player.SelectedCharacter?.Attributes.FirstOrDefault(a => a.Definition?.Id == Stats.RegisteredLuckyCoins.Id)?.Value ?? 0);
    }

    private static StatAttribute GetOrCreateRegisteredCoinsAttribute(Player player, Character character)
    {
        if (character.Attributes.FirstOrDefault(a => a.Definition?.Id == Stats.RegisteredLuckyCoins.Id) is { } attribute)
        {
            return attribute;
        }

        var definition = player.GameContext.Configuration.Attributes?.FirstOrDefault(a => a.Id == Stats.RegisteredLuckyCoins.Id) ?? Stats.RegisteredLuckyCoins;
        attribute = player.PersistenceContext.CreateNew<StatAttribute>(definition, 0);
        character.Attributes.Add(attribute);
        return attribute;
    }

    /// <summary>
    /// Removes the count of coins from the coin stacks. A stack which gets empty is removed from the inventory.
    /// </summary>
    private static async ValueTask RemoveCoinsAsync(Player player, IEnumerable<Item> coins, int count)
    {
        var remaining = count;
        foreach (var coin in coins)
        {
            if (remaining <= 0)
            {
                break;
            }

            var taken = Math.Min(remaining, (int)coin.Durability);
            remaining -= taken;
            coin.Durability -= taken;
            if (coin.Durability <= 0)
            {
                await player.DestroyInventoryItemAsync(coin).ConfigureAwait(false);
            }
            else
            {
                await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(coin, false)).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask<LuckyCoinExchangeResult> TryExchangeAsync(Player player, IInventoryStorage inventory, LuckyCoinConfiguration configuration, int coinCount)
    {
        var coins = GetCoins(player);
        if (coinCount <= 0 || coins.Sum(coin => (int)coin.Durability) < coinCount)
        {
            return LuckyCoinExchangeResult.NotEnoughItems;
        }

        var rewards = configuration.Rewards.Where(reward => reward.CoinCount == coinCount).ToList();
        if (rewards.Count == 0)
        {
            player.Logger.LogWarning("Player {player} requested to exchange {coinCount} lucky coins, but there is no reward configured for it.", player, coinCount);
            return LuckyCoinExchangeResult.NotEnoughItems;
        }

        var reward = rewards[Rand.NextInt(0, rewards.Count)];
        if (player.GameContext.Configuration.Items.FirstOrDefault(item => item.Group == reward.ItemGroup && item.Number == reward.ItemNumber) is not { } itemDefinition)
        {
            player.Logger.LogWarning("The lucky coin reward {reward} refers to an item which doesn't exist.", reward);
            return LuckyCoinExchangeResult.NotEnoughItems;
        }

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = itemDefinition;
        item.Level = Math.Min(reward.ItemLevel, itemDefinition.MaximumItemLevel);
        item.Durability = item.IsStackable() ? 1 : itemDefinition.Durability;
        if (inventory.CheckInvSpace(item) is null)
        {
            await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
            return LuckyCoinExchangeResult.InventoryFull;
        }

        // The reward is added before the coins are removed, so that the player keeps the coins when it fails.
        if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
        {
            await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
            player.Logger.LogError("The lucky coin reward {item} of player {player} couldn't be added to the inventory.", item, player);
            return LuckyCoinExchangeResult.InventoryFull;
        }

        await RemoveCoinsAsync(player, coins, coinCount).ConfigureAwait(false);

        player.Logger.LogInformation("Player {player} exchanged {coinCount} lucky coins for {item}.", player, coinCount, item);
        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
        return LuckyCoinExchangeResult.Exchanged;
    }
}
