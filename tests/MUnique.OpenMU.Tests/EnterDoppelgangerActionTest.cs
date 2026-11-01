// <copyright file="EnterDoppelgangerActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Tests for the ticket search of the <see cref="EnterDoppelgangerAction"/>.
/// </summary>
[TestFixture]
public class EnterDoppelgangerActionTest
{
    private const byte UndefinedTicketSlot = 0xFF;

    /// <summary>
    /// Tests that the mirror of dimensions is preferred over the free ticket, when no slot is given.
    /// </summary>
    [Test]
    public async Task MirrorIsPreferredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var freeTicket = await AddItemAsync(player, 12, 13, 125, 1).ConfigureAwait(false);
        var mirror = await AddItemAsync(player, 13, 14, 111, 1).ConfigureAwait(false);

        Assert.That(EnterDoppelgangerAction.FindTicket(player, UndefinedTicketSlot), Is.SameAs(mirror));
        Assert.That(EnterDoppelgangerAction.FindTicket(player, 12), Is.SameAs(freeTicket), "The given slot is used, if it contains a ticket.");
    }

    /// <summary>
    /// Tests that a used up ticket and other items aren't accepted.
    /// </summary>
    [Test]
    public async Task UsedUpTicketIsIgnoredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddItemAsync(player, 12, 13, 125, 0).ConfigureAwait(false);
        await AddItemAsync(player, 13, 14, 110, 5).ConfigureAwait(false);

        Assert.That(EnterDoppelgangerAction.FindTicket(player, UndefinedTicketSlot), Is.Null);
        Assert.That(EnterDoppelgangerAction.FindTicket(player, 13), Is.Null, "A sign of dimensions isn't a ticket.");
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
