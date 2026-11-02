// <copyright file="FreeTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// The free tickets of the cash shop, which allow to enter a mini game of any level instead of its regular ticket.
/// Their durability is the number of entries.
/// </summary>
public static class FreeTicketItems
{
    private const byte UndefinedTicketSlot = 0xFF;

    private static readonly IReadOnlyDictionary<MiniGameType, (byte Group, short Number)> Tickets = new Dictionary<MiniGameType, (byte Group, short Number)>
    {
        { MiniGameType.DevilSquare, (13, 46) }, // Devil Square Ticket
        { MiniGameType.BloodCastle, (13, 47) }, // Blood Castle Ticket
        { MiniGameType.IllusionTemple, (13, 61) }, // Illusion Temple Ticket
        { MiniGameType.ChaosCastle, (13, 121) }, // Open Access Ticket to Chaos Castle
    };

    /// <summary>
    /// Finds a free ticket of the player for the mini game.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="miniGameType">The type of the mini game.</param>
    /// <param name="ticketInventoryIndex">The inventory index of the ticket item, or 0xFF to search it in the inventory.</param>
    /// <returns>The free ticket, or <c>null</c> if the player has none.</returns>
    public static Item? Find(Player player, MiniGameType miniGameType, byte ticketInventoryIndex)
    {
        if (player.Inventory is not { } inventory
            || !Tickets.TryGetValue(miniGameType, out var ticket))
        {
            return null;
        }

        if (ticketInventoryIndex != UndefinedTicketSlot
            && inventory.GetItem(ticketInventoryIndex) is { } item
            && IsTicket(item, ticket))
        {
            return item;
        }

        return inventory.Items.FirstOrDefault(i => IsTicket(i, ticket));
    }

    private static bool IsTicket(Item item, (byte Group, short Number) ticket)
    {
        return item.Durability > 0
               && item.Definition is { } definition
               && definition.Group == ticket.Group
               && definition.Number == ticket.Number;
    }
}
