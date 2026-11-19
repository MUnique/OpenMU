// <copyright file="IllusionTempleTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting for Illusion Temple Event Tickets, the Scroll of Blood, which is combined from an Old Scroll and an
/// Illusion Sorcerer Covenant of the same level.
/// </summary>
public class IllusionTempleTicketCrafting : BaseEventTicketCrafting
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleTicketCrafting"/> class.
    /// </summary>
    public IllusionTempleTicketCrafting()
        : base("Scroll of Blood", "Old Scroll", "Illusion Sorcerer Covenant")
    {
    }

    /// <inheritdoc />
    protected override byte MaximumSuccessRate => 80;

    /// <inheritdoc />
    protected override MiniGameType? EventType => MiniGameType.IllusionTemple;

    /// <inheritdoc />
    /// <remarks>
    /// The ticket which a temple of the level accepts, so the crafting and the temples stay in step when the server
    /// owner selects another ticket item. A temple without a ticket item (free entry) has no ticket to give, so it's
    /// skipped. Of the others, an available temple comes first; the ticket of an inactive one is still the one it
    /// accepts when it's activated again. Only when no temple of the level has a ticket item, the item named
    /// "Scroll of Blood".
    /// </remarks>
    protected override ItemDefinition? GetResultItemDefinition(Player player, byte eventLevel)
    {
        return player.GameContext.Configuration.MiniGameDefinitions
                   .Where(definition => definition.Type == MiniGameType.IllusionTemple && definition.TicketItemLevel == eventLevel)
                   .OrderByDescending(definition => definition.IsAvailable())
                   .ThenBy(definition => definition.GameLevel)
                   .Select(definition => definition.TicketItem)
                   .FirstOrDefault(ticket => ticket is not null)
               ?? base.GetResultItemDefinition(player, eventLevel);
    }

    /// <inheritdoc />
    protected override int GetPrice(int eventLevel)
    {
        return eventLevel switch
        {
            2 => 5000000,
            3 => 7000000,
            4 => 9000000,
            5 => 11000000,
            6 => 13000000,
            _ => 3000000,
        };
    }

    /// <inheritdoc />
    protected override byte GetSuccessRate(int eventLevel)
    {
        return 70;
    }
}