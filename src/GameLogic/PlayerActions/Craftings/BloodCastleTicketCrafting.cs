// <copyright file="BloodCastleTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting for Blood Castle Tickets.
/// </summary>
public class BloodCastleTicketCrafting : BaseEventTicketCrafting
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleTicketCrafting"/> class.
    /// </summary>
    public BloodCastleTicketCrafting()
        : base("Invisibility Cloak", "Scroll of Archangel", "Blood Bone")
    {
    }

    /// <inheritdoc />
    protected override CraftingResult IncorrectMixItemsResult => CraftingResult.IncorrectBloodCastleItems;

    /// <inheritdoc />
    protected override int GetPrice(int eventLevel)
    {
        return eventLevel switch
        {
            2 => 80_000,
            3 => 150_000,
            4 => 250_000,
            5 => 400_000,
            6 => 600_000,
            7 => 850_000,
            8 => 1_050_000,
            _ => 50_000,
        };
    }

    /// <inheritdoc />
    protected override byte GetSuccessRate(int eventLevel)
    {
        return 80;
    }
}