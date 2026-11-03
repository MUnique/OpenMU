// <copyright file="BotChaosMachineHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Lets a bot upgrade its worn gear from +9 up to +15 at the Chaos Goblin, like a player: it walks up to
/// the goblin, puts the piece and the jewels into the machine, mixes, takes the result back and wears it
/// again - all through the regular player actions, so the same success rates, prices and taxes apply.
/// </summary>
/// <remarks>
/// Which mixes exist and what they need is read from the goblin's crafting configuration: a crafting
/// which takes one item of exactly the piece's level and adds one level to it (the "+10 ... +15 item
/// combination"), together with the jewels it lists. The bot follows the habit of players:
/// <list type="bullet">
///   <item><description>Only pieces with luck go into the machine - its bonus to the success rate is
///     what makes the risk worth it. The jewel rules (see <see cref="BotJewelHandler"/>) take only lucky
///     pieces up to +9, so in effect every lucky piece ends up here.</description></item>
///   <item><description>Weapons, shields and armor pieces only; wings, pets and jewelry never.</description></item>
///   <item><description>A failure is the player's risk too: the piece is gone, and the bot goes without
///     until it finds another one.</description></item>
///   <item><description>The bot must already meet the requirements of the piece one level up, which
///     <see cref="BotGearPlanner"/> keeps for it - otherwise a success would leave it unable to wear
///     the result.</description></item>
/// </list>
/// </remarks>
internal static class BotChaosMachineHandler
{
    /// <summary>The reference number with which the upgrade craftings mark the item they upgrade.</summary>
    private const byte UpgradedItemReference = 1;

    private static readonly TalkNpcAction TalkAction = new();
    private static readonly MoveItemAction MoveAction = new();
    private static readonly ItemCraftAction CraftAction = new();
    private static readonly CloseNpcDialogAction CloseAction = new();

    /// <summary>
    /// Whether the NPC is a chaos machine.
    /// </summary>
    /// <param name="definition">The NPC definition.</param>
    /// <returns><c>True</c>, if the NPC opens the chaos machine.</returns>
    public static bool IsChaosMachine(MonsterDefinition definition)
        => definition is { NpcWindow: NpcWindow.ChaosMachine, ItemCraftings.Count: > 0 };

    /// <summary>
    /// Plans the next mix of the bot: the lowest worn lucky piece for which the goblin has an upgrade
    /// crafting, the bot carries the jewels and the Zen, and meets the requirements one level up.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="goblin">The chaos machine NPC definition.</param>
    /// <returns>The mix plan, or <c>null</c> if there is nothing to mix.</returns>
    public static MixPlan? PlanNextMix(Player player, MonsterDefinition goblin)
    {
        if (player.Inventory is not { } inventory)
        {
            return null;
        }

        var backpack = inventory.Items.Where(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount).ToList();
        return inventory.Items
            .Where(i => i.ItemSlot < InventoryConstants.EquippableSlotsCount
                        && i.Definition is { } definition
                        && BotGearPlanner.IsPlannedGear(definition, includeShields: true)
                        && i.Level < definition.MaximumItemLevel
                        && HasLuck(i))
            .OrderBy(i => i.Level)
            .Select(i => TryPlan(player, goblin, i, backpack))
            .FirstOrDefault(p => p is not null);
    }

    /// <summary>
    /// Gets the chaos machine NPC definition of the configuration, or <c>null</c>, if there is none.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>The chaos machine NPC definition.</returns>
    public static MonsterDefinition? GetChaosMachine(GameConfiguration configuration)
        => configuration.Monsters.FirstOrDefault(IsChaosMachine);

    /// <summary>
    /// Finds a chaos machine NPC on the map.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns>The position of the NPC, or <c>null</c>.</returns>
    public static Point? FindChaosMachinePosition(GameMap map)
        => map.GetNpcsInRange(new Point(128, 128), 256).FirstOrDefault(n => IsChaosMachine(n.Definition))?.Position;

    /// <summary>
    /// Mixes the next planned piece at the chaos machine standing near the given position, and puts the
    /// result back on.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="map">The map.</param>
    /// <param name="npcPosition">The position of the chaos machine NPC.</param>
    /// <returns><c>True</c>, if a mix was performed (successful or not).</returns>
    public static async ValueTask<bool> TryMixAsync(OfflinePlayer player, GameMap map, Point npcPosition)
    {
        var npc = map.GetNpcsInRange(npcPosition, 4).FirstOrDefault(n => IsChaosMachine(n.Definition));
        return npc is not null && await MixAtAsync(player, npc).ConfigureAwait(false);
    }

    /// <summary>
    /// Mixes the next planned piece at the given chaos machine NPC, and puts the result back on.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="npc">The chaos machine NPC.</param>
    /// <returns><c>True</c>, if a mix was performed (successful or not).</returns>
    internal static async ValueTask<bool> MixAtAsync(Player player, NonPlayerCharacter npc)
    {
        if (player.Inventory is not { } inventory || PlanNextMix(player, npc.Definition) is not { } plan)
        {
            return false;
        }

        await TalkAction.TalkToNpcAsync(player, npc).ConfigureAwait(false);
        if (player.OpenedNpc is null || player.TemporaryStorage is not { } machine)
        {
            player.Logger.LogDebug("Bot '{Name}' could not open the chaos machine.", player.Name);
            return false;
        }

        var item = plan.Item;
        var equipSlot = item.ItemSlot;
        var levelBefore = item.Level;
        var mixed = false;
        try
        {
            if (await TryPutIntoMachineAsync(player, item, Storages.Inventory, machine).ConfigureAwait(false)
                && await TryPutIngredientsAsync(player, plan, machine).ConfigureAwait(false))
            {
                await CraftAction.MixItemsAsync(player, plan.Crafting.Number, 0).ConfigureAwait(false);
                mixed = true;
            }
        }
        finally
        {
            // Everything still in the machine goes back: the upgraded piece after a success, the piece
            // and the jewels when the mix didn't happen. After a failure the machine is empty.
            await TakeBackAllAsync(player, inventory, machine).ConfigureAwait(false);
            await CloseAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
        }

        var survived = inventory.Items.Contains(item);
        if (survived && item.ItemSlot != equipSlot)
        {
            await MoveAction.MoveItemAsync(player, item.ItemSlot, Storages.Inventory, equipSlot, Storages.Inventory).ConfigureAwait(false);
        }

        if (mixed)
        {
            player.Logger.LogDebug(
                "Bot '{Name}' mixed '{Item}' (+{Before}) at the chaos machine: {Outcome}.",
                player.Name,
                item.Definition?.Name.ValueInNeutralLanguage,
                levelBefore,
                !survived ? "failed, the piece is gone" : item.Level > levelBefore ? $"success, now +{item.Level}" : "no change");
        }

        return mixed;
    }

    /// <summary>
    /// Finds the crafting which takes one item of exactly the given level and returns it one level up.
    /// </summary>
    /// <param name="goblin">The chaos machine NPC definition.</param>
    /// <param name="itemLevel">The current level of the item.</param>
    /// <returns>The crafting, or <c>null</c>.</returns>
    internal static ItemCrafting? FindUpgradeCrafting(MonsterDefinition goblin, byte itemLevel)
    {
        return goblin.ItemCraftings.FirstOrDefault(c => c.SimpleCraftingSettings is { } settings
            && settings.RequiredItems.Any(r => r.Reference == UpgradedItemReference
                                               && r.MinimumAmount == 1
                                               && r.MinimumItemLevel == itemLevel
                                               && r.MaximumItemLevel == itemLevel
                                               && r.PossibleItems.Count == 0)
            && settings.ResultItems.Any(r => r.Reference == UpgradedItemReference && r.AddLevel == 1));
    }

    private static MixPlan? TryPlan(Player player, MonsterDefinition goblin, Item item, IReadOnlyList<Item> backpack)
    {
        if (FindUpgradeCrafting(goblin, item.Level) is not { SimpleCraftingSettings: { } settings } crafting
            || player.Money < settings.Money)
        {
            return null;
        }

        var upgraded = new TemporaryItem();
        upgraded.AssignValues(item);
        upgraded.Level++;
        if (!player.CompliesRequirements(upgraded))
        {
            return null;
        }

        var ingredients = new List<Item>();
        foreach (var requirement in settings.RequiredItems.Where(r => r.Reference != UpgradedItemReference))
        {
            var available = backpack
                .Where(i => i.Definition is { } definition && requirement.PossibleItems.Contains(definition) && !ingredients.Contains(i))
                .Take(requirement.MinimumAmount)
                .ToList();
            if (available.Count < requirement.MinimumAmount)
            {
                return null;
            }

            ingredients.AddRange(available);
        }

        return new MixPlan(item, crafting, ingredients);
    }

    private static async ValueTask<bool> TryPutIngredientsAsync(Player player, MixPlan plan, IStorage machine)
    {
        foreach (var ingredient in plan.Ingredients)
        {
            if (!await TryPutIntoMachineAsync(player, ingredient, Storages.Inventory, machine).ConfigureAwait(false))
            {
                return false;
            }
        }

        return true;
    }

    private static async ValueTask<bool> TryPutIntoMachineAsync(Player player, Item item, Storages from, IStorage machine)
    {
        if (machine.CheckInvSpace(item) is not { } slot)
        {
            return false;
        }

        await MoveAction.MoveItemAsync(player, item.ItemSlot, from, slot, Storages.ChaosMachine).ConfigureAwait(false);
        return machine.Items.Contains(item);
    }

    private static async ValueTask TakeBackAllAsync(Player player, IStorage inventory, IStorage machine)
    {
        foreach (var leftover in machine.Items.ToList())
        {
            if (inventory.CheckInvSpace(leftover) is not { } slot)
            {
                // Stays in the machine; the temporary storage is given back on the next logout.
                player.Logger.LogWarning("Bot '{Name}' has no room to take '{Item}' back from the chaos machine.", player.Name, leftover);
                continue;
            }

            await MoveAction.MoveItemAsync(player, leftover.ItemSlot, Storages.ChaosMachine, slot, Storages.Inventory).ConfigureAwait(false);
        }
    }

    private static bool HasLuck(Item item)
        => item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck);

    /// <summary>
    /// A planned mix: the worn piece, the crafting which upgrades it, and the jewels from the backpack it takes.
    /// </summary>
    /// <param name="Item">The worn piece.</param>
    /// <param name="Crafting">The upgrade crafting.</param>
    /// <param name="Ingredients">The jewels from the backpack.</param>
    internal sealed record MixPlan(Item Item, ItemCrafting Crafting, IReadOnlyList<Item> Ingredients);
}
