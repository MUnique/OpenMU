// <copyright file="EnterDoppelgangerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Action to enter the doppelganger event.
/// </summary>
/// <remarks>
/// The event takes place on one of several maps, each with its own <see cref="MiniGameDefinition"/>.
/// The first player of a party enters a randomly chosen one, and the other members of the party
/// follow into the same instance.
/// The event accepts two tickets, a Mirror of Dimensions and a Doppelganger Free Ticket, so the
/// definitions don't define a ticket item. Instead, the ticket is checked and consumed here.
/// </remarks>
public class EnterDoppelgangerAction
{
    private const byte UndefinedTicketSlot = 0xFF;

    private static readonly (byte Group, short Number)[] TicketItems =
    [
        (14, 111), // Mirror of Dimensions
        (13, 125), // Doppelganger Free Ticket
    ];

    private readonly EnterMiniGameAction _enterAction = new();

    /// <summary>
    /// Tries to enter the doppelganger event.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="ticketInventoryIndex">The inventory index of the ticket item, or 0xFF to search it in the inventory.</param>
    public async ValueTask TryEnterAsync(Player player, byte ticketInventoryIndex)
    {
        var definitions = player.GameContext.Configuration.MiniGameDefinitions
            .Where(definition => definition.Type == MiniGameType.Doppelganger)
            .ToList();
        if (definitions.Count == 0 || FindTicket(player, ticketInventoryIndex) is not { } ticket)
        {
            await player.InvokeViewPlugInAsync<IShowMiniGameEnterResultPlugIn>(p => p.ShowResultAsync(MiniGameType.Doppelganger, EnterResult.Failed)).ConfigureAwait(false);
            return;
        }

        var definition = definitions.FirstOrDefault(d => player.GameContext.MiniGames.TryGetRunningMiniGame(d, player) is not null)
                         ?? definitions[Rand.NextInt(0, definitions.Count)];

        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.Doppelganger, definition.GameLevel, ticketInventoryIndex).ConfigureAwait(false);
        if (player.CurrentMiniGame is DoppelgangerContext)
        {
            await ConsumeTicketAsync(player, ticket).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Finds the ticket which should be used to enter the event.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="ticketInventoryIndex">The inventory index of the ticket item, or 0xFF to search it in the inventory.</param>
    /// <returns>The ticket, or <c>null</c> if the player has none.</returns>
    internal static Item? FindTicket(Player player, byte ticketInventoryIndex)
    {
        if (player.Inventory is not { } inventory)
        {
            return null;
        }

        if (ticketInventoryIndex != UndefinedTicketSlot && inventory.GetItem(ticketInventoryIndex) is { } item && IsTicket(item))
        {
            return item;
        }

        // The Mirror of Dimensions is preferred, because the free ticket is usually bought.
        return TicketItems
            .SelectMany(ticket => inventory.Items.Where(i => i.Definition is { } definition && definition.Group == ticket.Group && definition.Number == ticket.Number))
            .FirstOrDefault(IsTicket);
    }

    private static bool IsTicket(Item item)
    {
        return item.Durability > 0
               && item.Definition is { } definition
               && TicketItems.Any(ticket => ticket.Group == definition.Group && ticket.Number == definition.Number);
    }

    private static async ValueTask ConsumeTicketAsync(Player player, Item ticket)
    {
        ticket.Durability -= 1;
        if (ticket.Durability > 0)
        {
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(ticket, false)).ConfigureAwait(false);
            return;
        }

        if (player.Inventory is { } inventory)
        {
            await inventory.RemoveItemAsync(ticket).ConfigureAwait(false);
        }

        await player.DestroyInventoryItemAsync(ticket).ConfigureAwait(false);
    }
}
