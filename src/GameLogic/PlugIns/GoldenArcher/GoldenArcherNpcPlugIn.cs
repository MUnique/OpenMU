// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.PlugIns.GoldenArcher;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the Golden Archer NPC (classic Rena exchange event).
/// Minimal implementation: on talk, if configured token items are available,
/// consumes the configured amount and rewards one random item from the configured drop group.
/// If configuration is missing or not enough tokens are available, shows an informative message.
/// </summary>
[Guid("E2B0D9E2-3FCD-4F39-BA2E-8D2B35D20A11")]
[PlugIn(nameof(GoldenArcherNpcPlugIn), "Golden Archer event: exchanges tokens (e.g. Rena) for rewards.")]
public class GoldenArcherNpcPlugIn : IPlayerTalkToNpcPlugIn,
    ISupportCustomConfiguration<GoldenArcherNpcPlugInConfiguration>,
    ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Gets the NPC number of Golden Archer.
    /// </summary>
    public static short GoldenArcherNpcNumber => 236; // defined in Version095d.NpcInitialization

    /// <inheritdoc />
    public GoldenArcherNpcPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != GoldenArcherNpcNumber)
        {
            return;
        }

        eventArgs.HasBeenHandled = true; // we take over handling; do not show default not-implemented message
        var cfg = this.Configuration ??= CreateDefaultConfiguration();

        if (!IsConfigurationValid(cfg))
        {
            var message = player.GetLocalizedMessage(
                "GoldenArcher_Message_NotConfigured",
                "Golden Archer is not configured. Configure the token item and reward group in the Admin Panel → Plugins.");
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
            // Keep dialog open so the client shows NPC bubble; no need to change state.
            eventArgs.LeavesDialogOpen = false;
            return;
        }

        var tokenDef = cfg.TokenItemDefinition;

        // Find token items in inventory
        var inventory = player.Inventory;
        if (inventory is null)
        {
            return;
        }

        bool MatchesToken(Item? item)
        {
            if (item?.Definition is null)
            {
                return false;
            }

            if (tokenDef is not null)
            {
                // Compare by group/number to avoid reference identity mismatches.
                return item.Definition.Group == tokenDef.Group && item.Definition.Number == tokenDef.Number;
            }

            return item.Definition.Group == cfg.TokenItemGroup && item.Definition.Number == cfg.TokenItemNumber;
        }

        var allTokens = inventory.Items.Where(MatchesToken).ToList();
        // Count pieces: for stackables, durability is the amount; otherwise, each item counts as 1.
        var tokenCount = allTokens.Sum(i => i.IsStackable() ? (int)i.Durability : 1);
        var tokenName = tokenDef?.Name ?? player.GetLocalizedMessage("GoldenArcher_Token_DefaultName", "token");
        if (tokenCount <= 0)
        {
            var message = player.GetLocalizedMessage("GoldenArcher_Message_NoTokens", "You don't have any {0}.", tokenName);
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
            eventArgs.LeavesDialogOpen = false;
            return;
        }

        var maxAccept = Math.Clamp(cfg.MaximumAcceptedTokens, 1, 255);
        var tokensToConsume = Math.Min(tokenCount, maxAccept);

        // Determine reward tier and validate before consuming tokens
        Func<ValueTask>? applyReward = null;

        if (tokensToConsume >= cfg.TopTierExact && (cfg.TopTierItemGroup > 0 || cfg.TopTierItemNumber > 0))
        {
            var topDef = player.GameContext.Configuration.Items.FirstOrDefault(i => i.Group == cfg.TopTierItemGroup && i.Number == cfg.TopTierItemNumber);
            if (topDef is null)
            {
                var message = player.GetLocalizedMessage("GoldenArcher_Message_InvalidTopRewardConfig", "Invalid configuration for the 255-token reward.");
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
                return;
            }

            tokensToConsume = cfg.TopTierExact; // consume exact 255 for top reward
            applyReward = async () =>
            {
                var item = new TemporaryItem { Definition = topDef, Level = cfg.TopTierItemLevel, Durability = 1 };
                await this.GiveOrDropAsync(
                    player,
                    item,
                    "GoldenArcher_Message_ItemRewardWithLevel",
                    "Reward for delivering {0} {1}: {2} +{3}",
                    tokensToConsume,
                    tokenName,
                    item.Definition?.Name ?? string.Empty,
                    item.Level).ConfigureAwait(false);
            };
        }
        else if (tokensToConsume >= 200 && !string.IsNullOrWhiteSpace(cfg.AdvancedTierDropGroupDescription))
        {
            var group = player.GameContext.Configuration.DropItemGroups.FirstOrDefault(g => string.Equals(g.Description, cfg.AdvancedTierDropGroupDescription, StringComparison.OrdinalIgnoreCase));
            var rewardItem = group is null ? null : player.GameContext.DropGenerator.GenerateItemDrop(group);
            if (rewardItem is null)
            {
                var message = player.GetLocalizedMessage("GoldenArcher_Message_NoReward", "You didn't receive a reward this time.");
                await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
                return;
            }

            tokensToConsume = 200;
            applyReward = async () => await this.GiveOrDropAsync(
                player,
                rewardItem,
                "GoldenArcher_Message_ItemReward",
                "Reward for delivering {0} {1}: {2}",
                tokensToConsume,
                tokenName,
                rewardItem.Definition?.Name ?? string.Empty).ConfigureAwait(false);
        }
        else if (tokensToConsume >= 100 && !string.IsNullOrWhiteSpace(cfg.HighTierDropGroupDescription))
        {
            var group = player.GameContext.Configuration.DropItemGroups.FirstOrDefault(g => string.Equals(g.Description, cfg.HighTierDropGroupDescription, StringComparison.OrdinalIgnoreCase));
            var rewardItem = group is null ? null : player.GameContext.DropGenerator.GenerateItemDrop(group);
            if (rewardItem is null)
            {
                var message = player.GetLocalizedMessage("GoldenArcher_Message_NoReward", "You didn't receive a reward this time.");
                await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
                return;
            }

            tokensToConsume = 100;
            applyReward = async () => await this.GiveOrDropAsync(
                player,
                rewardItem,
                "GoldenArcher_Message_ItemReward",
                "Reward for delivering {0} {1}: {2}",
                tokensToConsume,
                tokenName,
                rewardItem.Definition?.Name ?? string.Empty).ConfigureAwait(false);
        }
        else if (tokensToConsume >= 10 && (cfg.MidTierItemGroup > 0 || cfg.MidTierItemNumber > 0))
        {
            var midDef = player.GameContext.Configuration.Items.FirstOrDefault(i => i.Group == cfg.MidTierItemGroup && i.Number == cfg.MidTierItemNumber);
            if (midDef is null)
            {
                var message = player.GetLocalizedMessage("GoldenArcher_Message_InvalidMidRewardConfig", "Invalid configuration for the 10-99 token reward.");
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
                return;
            }

            applyReward = async () =>
            {
                var item = new TemporaryItem { Definition = midDef, Level = cfg.MidTierItemLevel, Durability = 1 };
                await this.GiveOrDropAsync(
                    player,
                    item,
                    "GoldenArcher_Message_ItemRewardWithLevel",
                    "Reward for delivering {0} {1}: {2} +{3}",
                    tokensToConsume,
                    tokenName,
                    item.Definition?.Name ?? string.Empty,
                    item.Level).ConfigureAwait(false);
            };
            tokensToConsume = 10;
        }
        else
        {
            var money = (uint)Math.Max(0, (long)cfg.LowTierMoneyPerToken * tokensToConsume);
            applyReward = async () =>
            {
                if (!player.TryAddMoney((int)money))
                {
                    if (player.CurrentMap is { } map)
                    {
                        var dropPoint = map.Terrain.GetRandomCoordinate(player.Position, 1);
                        var dropped = new DroppedMoney(money, dropPoint, map);
                        await map.AddAsync(dropped).ConfigureAwait(false);
                    }
                }

                var message = player.GetLocalizedMessage(
                    "GoldenArcher_Message_MoneyReward",
                    "You received {0:N0} zen for delivering {1} {2}.",
                    money,
                    tokensToConsume,
                    tokenName);
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            };
        }

        // Apply reward first; consume tokens afterwards so players don't lose
        // items in case of unexpected errors while granting the reward.
        // In normal cases, granting the reward either adds to inventory or
        // drops it on the ground and shows a message.
        try
        {
            if (applyReward is not null)
            {
                await applyReward().ConfigureAwait(false);
            }
            else
            {
                var message = player.GetLocalizedMessage("GoldenArcher_Message_NoReward", "You didn't receive a reward this time.");
                await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
                return;
            }

            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "GoldenArcher reward failed for player {player}", player);
            var message = player.GetLocalizedMessage("GoldenArcher_Message_Error", "An error occurred while delivering the reward. Please try again.");
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
            return;
        }

        eventArgs.LeavesDialogOpen = false; // finish interaction
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => CreateDefaultConfiguration();

    private static bool IsConfigurationValid(GoldenArcherNpcPlugInConfiguration cfg)
        => cfg.TokenItemDefinition is not null || (cfg.TokenItemGroup > 0 || cfg.TokenItemNumber > 0);

    private static GoldenArcherNpcPlugInConfiguration CreateDefaultConfiguration()
    {
        // By default, keep references null so server admins must configure
        // the token and reward group explicitly from the Admin Panel.
        return new GoldenArcherNpcPlugInConfiguration
        {
            MaximumAcceptedTokens = 255,
            LowTierMoneyPerToken = 10000,
            MidTierItemLevel = 7,
            TopTierExact = 255,
            TopTierItemLevel = 12,
        };
    }

    private async ValueTask ConsumeTokensAsync(Player player, IList<Item> tokens, int count)
    {
        var remaining = count;
        foreach (var t in tokens)
        {
            if (remaining <= 0)
            {
                break;
            }

            if (t.IsStackable())
            {
                var take = (int)Math.Min(remaining, (int)t.Durability);
                if (take <= 0)
                {
                    continue;
                }

                if (take == (int)t.Durability)
                {
                    remaining -= take;
                    await player.DestroyInventoryItemAsync(t).ConfigureAwait(false);
                }
                else
                {
                    t.Durability -= take;
                    remaining -= take;
                    await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(t, true)).ConfigureAwait(false);
                }
            }
            else
            {
                remaining--;
                await player.DestroyInventoryItemAsync(t).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask GiveOrDropAsync(Player player, Item item, string messageKey, string fallback, params object?[] arguments)
    {
        var localizedMessage = player.GetLocalizedMessage(messageKey, fallback, arguments);
        var inventory = player.Inventory!;
        // Determine the target slot up-front to be able to fetch the persisted item afterwards.
        var targetSlot = inventory.CheckInvSpace(item);
        if (targetSlot is not null && await inventory.AddItemAsync(targetSlot.Value, item).ConfigureAwait(false))
        {
            var finalItem = inventory.GetItem(targetSlot.Value) ?? item;
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(localizedMessage, MessageType.BlueNormal)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(finalItem)).ConfigureAwait(false);
            return;
        }

        if (player.CurrentMap is { } map)
        {
            var dropPoint = map.Terrain.GetRandomCoordinate(player.Position, 1);
            var dropped = new DroppedItem(item, dropPoint, map, player);
            await map.AddAsync(dropped).ConfigureAwait(false);
            var dropMessage = player.GetLocalizedMessage("GoldenArcher_Message_ItemDropped", "{0} (dropped on the ground because the inventory is full)", localizedMessage);
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(dropMessage, MessageType.BlueNormal)).ConfigureAwait(false);
        }
        else
        {
            var failure = player.GetLocalizedMessage("GoldenArcher_Message_DeliveryFailed", "The reward could not be delivered.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(failure, MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Configuration for <see cref="GoldenArcherNpcPlugIn"/>.
/// </summary>
    public class GoldenArcherNpcPlugInConfiguration
    {
    /// <summary>
    /// Gets or sets the token item definition (e.g., Rena) which will be consumed.
    /// Must be configured in Admin Panel.
    /// </summary>
    [Display(Name = "Token Item (e.g. Rena)")]
    public ItemDefinition? TokenItemDefinition { get; set; }

    [Display(Name = "Token item group (fallback)")]
    public byte TokenItemGroup { get; set; }

    [Display(Name = "Token item number (fallback)")]
    public short TokenItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the maximum accepted tokens per exchange (1..255).
    /// </summary>
    [Display(Name = "Max tokens per exchange")]
    public int MaximumAcceptedTokens { get; set; } = 255;

    // Low tier: 1..9 tokens => Zen
    [Display(Name = "Low tier money per token (1..9)")]
    public int LowTierMoneyPerToken { get; set; } = 10000;

    // Mid tier: 10..99 tokens => usually Box of Heaven (Box of Luck +7)
    [Display(Name = "Mid tier item group (10..99)")]
    public byte MidTierItemGroup { get; set; }

    [Display(Name = "Mid tier item number (10..99)")]
    public short MidTierItemNumber { get; set; }

    [Display(Name = "Mid tier item level")]
    public byte MidTierItemLevel { get; set; } = 7;

    // High tier: 100..199 tokens => basic jewels drop group
    [Display(Name = "High tier drop group description (100..199)")]
    public string? HighTierDropGroupDescription { get; set; }

    // Advanced tier: 200..254 tokens => Packed Jewels
    [Display(Name = "Advanced tier drop group description (200..254)")]
    public string? AdvancedTierDropGroupDescription { get; set; }

    // Top tier: exactly 255 tokens => Box of Kundun (+5) by default
    [Display(Name = "Top tier exact tokens")]
    public int TopTierExact { get; set; } = 255;

    [Display(Name = "Top tier item group")]
    public byte TopTierItemGroup { get; set; }

    [Display(Name = "Top tier item number")]
    public short TopTierItemNumber { get; set; }

    [Display(Name = "Top tier item level")]
    public byte TopTierItemLevel { get; set; } = 12;
}
