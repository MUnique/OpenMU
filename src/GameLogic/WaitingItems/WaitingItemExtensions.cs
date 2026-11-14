// <copyright file="WaitingItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.WaitingItems;

using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Extensions for the <see cref="Character.WaitingItems"/>, the items which wait for space in the inventory.
/// </summary>
public static class WaitingItemExtensions
{
    /// <summary>
    /// Adds the item to the waiting items of the character.
    /// The item must not belong to an item storage anymore.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="context">The persistence context of the character.</param>
    /// <param name="item">The item.</param>
    /// <param name="reason">The reason why the item waits.</param>
    public static void AddWaitingItem(this Character character, IContext context, Item item, WaitingItemReason reason)
    {
        var waitingItem = context.CreateNew<WaitingItem>();
        waitingItem.Item = item;
        waitingItem.Reason = reason;
        waitingItem.AddedAt = DateTime.UtcNow;
        character.WaitingItems.Add(waitingItem);
    }

    /// <summary>
    /// Creates the function which puts an item of the character, which finds no place when its storage is loaded,
    /// into the waiting items. It's passed to the constructor of a <see cref="Storage"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="character">The character which gets the waiting items.</param>
    /// <returns>The function; it always takes the item.</returns>
    public static Func<Item, bool> PutAsideWhenLoaded(this Player player, Character character)
    {
        return item =>
        {
            player.Logger.LogWarning("Item {Item} of character {Character} finds no place in its storage, so it waits for space in the inventory.", item, character.Name);
            character.AddWaitingItem(player.PersistenceContext, item, WaitingItemReason.NoSpaceWhenLoaded);
            return true;
        };
    }

    /// <summary>
    /// Determines whether the selected character of the player has waiting items which can be put into the inventory
    /// when they fit, which are the ones with an item definition.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>True</c>, if the character has such waiting items; Otherwise, <c>false</c>.</returns>
    public static bool HasDeliverableWaitingItems(this Player player)
    {
        return player.SelectedCharacter?.HasDeliverableWaitingItems() ?? false;
    }

    /// <summary>
    /// Determines whether the character has waiting items which can be put into the inventory when they fit,
    /// which are the ones with an item definition.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns><c>True</c>, if the character has such waiting items; Otherwise, <c>false</c>.</returns>
    public static bool HasDeliverableWaitingItems(this Character character)
    {
        return character.WaitingItems?.Any(w => w.Item?.Definition is not null) ?? false;
    }

    /// <summary>
    /// Puts the waiting items of the selected character into its inventory, as far as they fit there.
    /// The oldest items come first; an item which doesn't fit doesn't stop a later, smaller one.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The number of items which were put into the inventory.</returns>
    public static async ValueTask<int> DeliverWaitingItemsAsync(this Player player)
    {
        if (player.SelectedCharacter is not { WaitingItems: { Count: > 0 } } character
            || player.Inventory is not { } inventory)
        {
            return 0;
        }

        var delivered = 0;
        foreach (var waitingItem in character.WaitingItems.OrderBy(w => w.AddedAt).ToList())
        {
            if (waitingItem.Item is not { } item)
            {
                await player.RemoveWaitingItemAsync(character, waitingItem).ConfigureAwait(false);
                continue;
            }

            if (item.Definition is null)
            {
                // Like an inventory item without a definition, it's kept as it is, until the definition is back.
                player.Logger.LogWarning("Waiting item {Item} of character {Character} has no item definition, so it can't be put into the inventory.", item, character.Name);
                continue;
            }

            if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
            {
                continue;
            }

            // The item belongs to the inventory now, so deleting the waiting item must not delete it.
            waitingItem.Item = null;
            await player.RemoveWaitingItemAsync(character, waitingItem).ConfigureAwait(false);

            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.WaitingItemDelivered), player.GetWaitingItemName(item)).ConfigureAwait(false);
            delivered++;
        }

        return delivered;
    }

    /// <summary>
    /// Tells the player which items wait for space in the inventory: one message with their number,
    /// and one message per item, up to <paramref name="maximumNamedItems"/>, so that a long list
    /// doesn't end up in one long message.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="maximumNamedItems">The maximum number of items which are named.</param>
    /// <param name="evenWhenNone">If set to <c>true</c>, the player is told when no item waits, too.</param>
    /// <param name="pointToCommand">If set to <c>true</c>, the player is pointed to the chat command for the items which aren't named.</param>
    public static async ValueTask ShowWaitingItemsAsync(this Player player, int maximumNamedItems, bool evenWhenNone = false, bool pointToCommand = false)
    {
        var waitingItems = player.SelectedCharacter?.WaitingItems?
            .Select(w => w.Item)
            .OfType<Item>()
            .Where(item => item.Definition is not null)
            .ToList() ?? [];
        if (waitingItems.Count == 0)
        {
            if (evenWhenNone)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoWaitingItems)).ConfigureAwait(false);
            }

            return;
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemsWaitForSpace), waitingItems.Count).ConfigureAwait(false);
        foreach (var item in waitingItems.Take(maximumNamedItems))
        {
            await player.ShowBlueMessageAsync($"- {player.GetWaitingItemName(item)}").ConfigureAwait(false);
        }

        if (waitingItems.Count > maximumNamedItems)
        {
            var message = pointToCommand ? nameof(PlayerMessage.MoreWaitingItems) : nameof(PlayerMessage.MoreWaitingItemsNotNamed);
            await player.ShowLocalizedBlueMessageAsync(message, waitingItems.Count - maximumNamedItems).ConfigureAwait(false);
        }
    }

    private static async ValueTask RemoveWaitingItemAsync(this Player player, Character character, WaitingItem waitingItem)
    {
        character.WaitingItems.Remove(waitingItem);
        await player.PersistenceContext.DeleteAsync(waitingItem).ConfigureAwait(false);
    }

    private static string GetWaitingItemName(this Player player, Item item)
    {
        var name = item.Definition?.Name.GetTranslation(player.Culture) ?? item.ToString();
        if (item.Level > 0)
        {
            name += $" +{item.Level}";
        }

        return item.Definition is { } definition
            ? $"{name} ({definition.Width}x{definition.Height})"
            : name;
    }
}
