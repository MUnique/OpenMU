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
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                "Golden Archer no está configurado. Configura el ítem token y el grupo de premios en Admin Panel → Plugins.",
                npc)).ConfigureAwait(false);
            // Keep dialog open so the client shows NPC bubble; no need to change state.
            eventArgs.LeavesDialogOpen = false;
            return;
        }

        var tokenDef = cfg.TokenItemDefinition!;

        // Find token items in inventory
        var inventory = player.Inventory;
        if (inventory is null)
        {
            return;
        }

        var allTokens = inventory.Items.Where(i => ReferenceEquals(i.Definition, tokenDef)).ToList();
        var tokenCount = allTokens.Count;
        if (tokenCount <= 0)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                $"No tienes {tokenDef.Name}.", npc)).ConfigureAwait(false);
            eventArgs.LeavesDialogOpen = false;
            return;
        }

        var maxAccept = Math.Clamp(cfg.MaximumAcceptedTokens, 1, 255);
        var tokensToConsume = Math.Min(tokenCount, maxAccept);

        // Determine reward tier
        if (tokensToConsume >= cfg.TopTierExact && cfg.TopTierItemDefinition is not null)
        {
            tokensToConsume = cfg.TopTierExact; // consume exact 255 for top reward
            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
            var item = new TemporaryItem
            {
                Definition = cfg.TopTierItemDefinition,
                Level = cfg.TopTierItemLevel,
                Durability = 1,
            };
            await this.GiveOrDropAsync(player, item, $"Premio por {tokensToConsume} {tokenDef.Name}: {item.Definition?.Name} +{item.Level}").ConfigureAwait(false);
        }
        else if (tokensToConsume >= 200 && cfg.AdvancedTierDropGroup is not null)
        {
            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
            var rewardItem = player.GameContext.DropGenerator.GenerateItemDrop(cfg.AdvancedTierDropGroup);
            if (rewardItem is null)
            {
                await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                    "No has recibido premio esta vez.", npc)).ConfigureAwait(false);
            }
            else
            {
                await this.GiveOrDropAsync(player, rewardItem, $"Premio por {tokensToConsume} {tokenDef.Name}: {rewardItem.Definition?.Name}").ConfigureAwait(false);
            }
        }
        else if (tokensToConsume >= 100 && cfg.HighTierDropGroup is not null)
        {
            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
            var rewardItem = player.GameContext.DropGenerator.GenerateItemDrop(cfg.HighTierDropGroup);
            if (rewardItem is null)
            {
                await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(
                    "No has recibido premio esta vez.", npc)).ConfigureAwait(false);
            }
            else
            {
                await this.GiveOrDropAsync(player, rewardItem, $"Premio por {tokensToConsume} {tokenDef.Name}: {rewardItem.Definition?.Name}").ConfigureAwait(false);
            }
        }
        else if (tokensToConsume >= 10 && cfg.MidTierItemDefinition is not null)
        {
            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
            var item = new TemporaryItem
            {
                Definition = cfg.MidTierItemDefinition,
                Level = cfg.MidTierItemLevel,
                Durability = 1,
            };
            await this.GiveOrDropAsync(player, item, $"Premio por {tokensToConsume} {tokenDef.Name}: {item.Definition?.Name} +{item.Level}").ConfigureAwait(false);
        }
        else
        {
            await this.ConsumeTokensAsync(player, allTokens, tokensToConsume).ConfigureAwait(false);
            var money = (uint)Math.Max(0, (long)cfg.LowTierMoneyPerToken * tokensToConsume);
            if (!player.TryAddMoney((int)money))
            {
                if (player.CurrentMap is { } map)
                {
                    var dropPoint = map.Terrain.GetRandomCoordinate(player.Position, 1);
                    var dropped = new DroppedMoney(money, dropPoint, map);
                    await map.AddAsync(dropped).ConfigureAwait(false);
                }
            }

            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(
                $"Recibiste {money:N0} Zen por entregar {tokensToConsume} {tokenDef.Name}.", MessageType.BlueNormal)).ConfigureAwait(false);
        }

        eventArgs.LeavesDialogOpen = false; // finish interaction
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => CreateDefaultConfiguration();

    private static bool IsConfigurationValid(GoldenArcherNpcPlugInConfiguration cfg)
        => cfg.TokenItemDefinition is not null;

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
        for (int i = 0; i < count && i < tokens.Count; i++)
        {
            await player.DestroyInventoryItemAsync(tokens[i]).ConfigureAwait(false);
        }
    }

    private async ValueTask GiveOrDropAsync(Player player, Item item, string message)
    {
        var inventory = player.Inventory!;
        if (await inventory.AddItemAsync(item).ConfigureAwait(false))
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
            return;
        }

        if (player.CurrentMap is { } map)
        {
            var dropPoint = map.Terrain.GetRandomCoordinate(player.Position, 1);
            var dropped = new DroppedItem(item, dropPoint, map, player);
            await map.AddAsync(dropped).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(
                message + " (dejado en el suelo por inventario lleno)", MessageType.BlueNormal)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(
                "No se pudo entregar el premio.", MessageType.BlueNormal)).ConfigureAwait(false);
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

    /// <summary>
    /// Gets or sets the maximum accepted tokens per exchange (1..255).
    /// </summary>
    [Display(Name = "Max tokens per exchange")]
    public int MaximumAcceptedTokens { get; set; } = 255;

    // Low tier: 1..9 tokens => Zen
    [Display(Name = "Low tier money per token (1..9)")]
    public int LowTierMoneyPerToken { get; set; } = 10000;

    // Mid tier: 10..99 tokens => usually Box of Heaven (Box of Luck +7)
    [Display(Name = "Mid tier item definition (10..99)")]
    public ItemDefinition? MidTierItemDefinition { get; set; }

    [Display(Name = "Mid tier item level")]
    public byte MidTierItemLevel { get; set; } = 7;

    // High tier: 100..199 tokens => basic jewels drop group
    [Display(Name = "High tier drop group (100..199)")]
    public DropItemGroup? HighTierDropGroup { get; set; }

    // Advanced tier: 200..254 tokens => Packed Jewels
    [Display(Name = "Advanced tier drop group (200..254)")]
    public DropItemGroup? AdvancedTierDropGroup { get; set; }

    // Top tier: exactly 255 tokens => Box of Kundun (+5) by default
    [Display(Name = "Top tier exact tokens")]
    public int TopTierExact { get; set; } = 255;

    [Display(Name = "Top tier item definition")]
    public ItemDefinition? TopTierItemDefinition { get; set; }

    [Display(Name = "Top tier item level")]
    public byte TopTierItemLevel { get; set; } = 12;
}
