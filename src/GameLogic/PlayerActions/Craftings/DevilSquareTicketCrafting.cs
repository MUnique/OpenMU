// <copyright file="DevilSquareTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// Crafting for Devil Square Event Tickets.
/// </summary>
public class DevilSquareTicketCrafting : BaseEventTicketCrafting
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquareTicketCrafting"/> class.
    /// </summary>
    public DevilSquareTicketCrafting()
        : base("Devil's Invitation", "Devil's Eye", "Devil's Key")
    {
    }

    /// <inheritdoc />
    protected override int GetPrice(int eventLevel)
    {
        return eventLevel switch
        {
            2 => 200000,
            3 => 400000,
            4 => 700000,
            5 => 1100000,
            6 => 1600000,
            7 => 2000000,
            _ => 100000,
        };
    }

    /// <inheritdoc />
    protected override byte GetSuccessRate(int eventLevel)
    {
        return (byte)(eventLevel < 5 ? 80 : 70); // Future to-do: There is a +10% increase if Crywolf event is beaten
    }
}