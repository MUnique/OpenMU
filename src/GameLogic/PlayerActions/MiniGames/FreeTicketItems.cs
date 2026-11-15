// <copyright file="FreeTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Finds the <see cref="MiniGameDefinition.FreeTicketItem"/> of a mini game, which allows to enter it at any level instead of its regular ticket.
/// Its durability is the number of entries.
/// </summary>
public static class FreeTicketItems
{
    private const byte UndefinedTicketSlot = 0xFF;

    /// <summary>
    /// Finds a free ticket of the player for the mini game.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="miniGameDefinition">The definition of the mini game.</param>
    /// <param name="ticketInventoryIndex">The inventory index of the ticket item, or 0xFF to search it in the inventory.</param>
    /// <returns>The free ticket, or <c>null</c> if the player has none, or the mini game has no free ticket.</returns>
    public static Item? Find(Player player, MiniGameDefinition miniGameDefinition, byte ticketInventoryIndex)
    {
        if (player.Inventory is not { } inventory
            || miniGameDefinition.FreeTicketItem is not { } freeTicket)
        {
            return null;
        }

        if (ticketInventoryIndex != UndefinedTicketSlot
            && inventory.GetItem(ticketInventoryIndex) is { } item
            && IsTicket(item, freeTicket))
        {
            return item;
        }

        return inventory.FindItemsByDefinition(freeTicket).FirstOrDefault(i => IsTicket(i, freeTicket));
    }

    private static bool IsTicket(Item item, ItemDefinition freeTicket)
    {
        return item.Durability > 0 && freeTicket.Equals(item.Definition);
    }
}
