// <copyright file="ItemBoxDroppedPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="ItemBoxDroppedPlugIn"/>.
/// </summary>
[TestFixture]
public class ItemBoxDroppedPlugInTest
{
    /// <summary>
    /// Tests that the item which is dropped by an opened item box can only be picked up by the
    /// player who opened it, while the owner priority is active.
    /// </summary>
    [Test]
    public async Task DroppedItemBelongsToTheOpenerAsync()
    {
        var (opener, other, droppedItem) = await OpenBoxAsync(withParty: false).ConfigureAwait(false);

        Assert.That(droppedItem.IsOwnerPickupPriorityActive, Is.True);
        Assert.That(droppedItem.IsPlayerAnOwner(opener), Is.True);
        Assert.That(droppedItem.IsPlayerAnOwner(other), Is.False);

        var (success, _) = await droppedItem.TryPickUpByAsync(other).ConfigureAwait(false);
        Assert.That(success, Is.False);
    }

    /// <summary>
    /// Tests that the members of the party of the player who opened the item box are owners of the dropped item, too.
    /// </summary>
    [Test]
    public async Task DroppedItemBelongsToThePartyOfTheOpenerAsync()
    {
        var (opener, partyMember, droppedItem) = await OpenBoxAsync(withParty: true).ConfigureAwait(false);

        Assert.That(droppedItem.IsPlayerAnOwner(opener), Is.True);
        Assert.That(droppedItem.IsPlayerAnOwner(partyMember), Is.True);
    }

    private static async ValueTask<(Player Opener, Player Other, DroppedItem DroppedItem)> OpenBoxAsync(bool withParty)
    {
        var generatedItem = new BasicModel.Item { Definition = new BasicModel.ItemDefinition() };
        var dropGenerator = new Mock<IDropGenerator>();
        dropGenerator.Setup(g => g.GenerateItemDrop(It.IsAny<IEnumerable<DropItemGroup>>()))
            .Returns((generatedItem, (uint?)null, ItemDropEffect.Undefined));
        var gameContext = GameContextTestHelper.CreateGameContext(dropGenerator: dropGenerator.Object);
        var opener = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var other = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        if (withParty)
        {
            var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
            await party.AddAsync(opener).ConfigureAwait(false);
            await party.AddAsync(other).ConfigureAwait(false);
        }

        var box = new BasicModel.Item { Definition = new BasicModel.ItemDefinition(), Level = 0 };
        box.Definition!.DropItems.Add(new BasicModel.ItemDropItemGroup { SourceItemLevel = 0 });

        var dropArgs = new IItemDropPlugIn.ItemDropArguments();
        await new ItemBoxDroppedPlugIn().HandleItemDropAsync(opener, box, opener.Position, dropArgs).ConfigureAwait(false);

        Assert.That(dropArgs.WasHandled, Is.True);
        var droppedItem = opener.CurrentMap!.GetDropsInRange(opener.Position, 5).OfType<DroppedItem>().Single();
        Assert.That(droppedItem.Item, Is.SameAs(generatedItem));
        return (opener, other, droppedItem);
    }
}
