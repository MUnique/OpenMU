// <copyright file="IllusionTempleTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting for Illusion Temple Event Tickets.
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