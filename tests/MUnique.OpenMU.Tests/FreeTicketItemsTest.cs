// <copyright file="FreeTicketItemsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Tests for the <see cref="FreeTicketItems"/>, which allow to enter a mini game instead of its regular ticket.
/// </summary>
[TestFixture]
public class FreeTicketItemsTest
{
    private const byte UndefinedTicketSlot = 0xFF;

    /// <summary>
    /// Tests that each mini game accepts its own free ticket, but not the free ticket of another mini game.
    /// </summary>
    /// <param name="miniGameType">The type of the mini game.</param>
    /// <param name="ticketNumber">The number of its free ticket.</param>
    [TestCase(MiniGameType.DevilSquare, 46)]
    [TestCase(MiniGameType.BloodCastle, 47)]
    [TestCase(MiniGameType.IllusionTemple, 61)]
    [TestCase(MiniGameType.ChaosCastle, 121)]
    public async Task MiniGameAcceptsItsFreeTicketAsync(MiniGameType miniGameType, short ticketNumber)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var ticket = await AddItemAsync(player, 12, 13, ticketNumber, 1).ConfigureAwait(false);

        Assert.That(FreeTicketItems.Find(player, miniGameType, UndefinedTicketSlot), Is.SameAs(ticket));
        Assert.That(FreeTicketItems.Find(player, miniGameType, 12), Is.SameAs(ticket));
        var otherType = miniGameType == MiniGameType.DevilSquare ? MiniGameType.BloodCastle : MiniGameType.DevilSquare;
        Assert.That(FreeTicketItems.Find(player, otherType, UndefinedTicketSlot), Is.Null);
    }

    /// <summary>
    /// Tests that a used up free ticket isn't accepted, and that the inventory is searched,
    /// if the given slot doesn't contain a free ticket.
    /// </summary>
    [Test]
    public async Task UsedUpTicketIsIgnoredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddItemAsync(player, 12, 13, 46, 0).ConfigureAwait(false);
        Assert.That(FreeTicketItems.Find(player, MiniGameType.DevilSquare, 12), Is.Null);

        var ticket = await AddItemAsync(player, 13, 13, 46, 3).ConfigureAwait(false);
        Assert.That(FreeTicketItems.Find(player, MiniGameType.DevilSquare, 12), Is.SameAs(ticket));
    }

    /// <summary>
    /// Tests that mini games without a free ticket don't accept one.
    /// </summary>
    [Test]
    public async Task MiniGameWithoutFreeTicketAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddItemAsync(player, 12, 13, 46, 1).ConfigureAwait(false);

        Assert.That(FreeTicketItems.Find(player, MiniGameType.ImperialGuardian, UndefinedTicketSlot), Is.Null);
    }

    private static async Task<Item> AddItemAsync(GameLogic.Player player, byte slot, byte group, short number, byte durability)
    {
        var item = new Item
        {
            Definition = new ItemDefinition { Group = group, Number = number, Width = 1, Height = 1, Durability = 1 },
            Durability = durability,
            ItemSlot = slot,
        };
        await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false);
        return item;
    }
}
