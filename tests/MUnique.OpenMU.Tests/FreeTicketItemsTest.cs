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
    /// Tests that a mini game accepts its own free ticket, but not the free ticket of another mini game.
    /// </summary>
    [Test]
    public async Task MiniGameAcceptsItsFreeTicketAsync()
    {
        var devilSquareTicket = CreateDefinition(46);
        var bloodCastleTicket = CreateDefinition(47);
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var ticket = await AddItemAsync(player, 12, devilSquareTicket, 1).ConfigureAwait(false);

        var devilSquare = new MiniGameDefinition { FreeTicketItem = devilSquareTicket };
        Assert.That(FreeTicketItems.Find(player, devilSquare, UndefinedTicketSlot), Is.SameAs(ticket));
        Assert.That(FreeTicketItems.Find(player, devilSquare, 12), Is.SameAs(ticket));

        var bloodCastle = new MiniGameDefinition { FreeTicketItem = bloodCastleTicket };
        Assert.That(FreeTicketItems.Find(player, bloodCastle, UndefinedTicketSlot), Is.Null);
        Assert.That(FreeTicketItems.Find(player, bloodCastle, 12), Is.Null);
    }

    /// <summary>
    /// Tests that a used up free ticket isn't accepted, and that the inventory is searched,
    /// if the given slot doesn't contain a free ticket.
    /// </summary>
    [Test]
    public async Task UsedUpTicketIsIgnoredAsync()
    {
        var ticketDefinition = CreateDefinition(46);
        var devilSquare = new MiniGameDefinition { FreeTicketItem = ticketDefinition };
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddItemAsync(player, 12, ticketDefinition, 0).ConfigureAwait(false);
        Assert.That(FreeTicketItems.Find(player, devilSquare, 12), Is.Null);

        var ticket = await AddItemAsync(player, 13, ticketDefinition, 3).ConfigureAwait(false);
        Assert.That(FreeTicketItems.Find(player, devilSquare, 12), Is.SameAs(ticket));
    }

    /// <summary>
    /// Tests that mini games without a free ticket don't accept one.
    /// </summary>
    [Test]
    public async Task MiniGameWithoutFreeTicketAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddItemAsync(player, 12, CreateDefinition(46), 1).ConfigureAwait(false);

        Assert.That(FreeTicketItems.Find(player, new MiniGameDefinition(), UndefinedTicketSlot), Is.Null);
    }

    private static ItemDefinition CreateDefinition(short number)
    {
        return new ItemDefinition { Group = 13, Number = number, Width = 1, Height = 1, Durability = 10 };
    }

    private static async Task<Item> AddItemAsync(GameLogic.Player player, byte slot, ItemDefinition definition, byte durability)
    {
        var item = new Item
        {
            Definition = definition,
            Durability = durability,
            ItemSlot = slot,
        };
        await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false);
        return item;
    }
}
