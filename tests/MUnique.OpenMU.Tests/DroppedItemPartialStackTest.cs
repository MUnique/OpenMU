// <copyright file="DroppedItemPartialStackTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests that picking up a stackable item which only partially fits on an existing stack
/// notifies the <see cref="IItemStackedPlugIn"/>, so a completed stack gets transformed.
/// </summary>
[TestFixture]
public class DroppedItemPartialStackTest
{
    /// <summary>
    /// Tests that a symbol of kundun from the ground which completes the stack of 4 turns it into a lost map of the same level.
    /// </summary>
    [Test]
    public async Task PickingUpSymbolCompletingStackCreatesLostMapAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<IItemStackedPlugIn, SymbolOfKundunStackedPlugIn>();
        var symbolDefinition = new ItemDefinition { Group = 14, Number = 29, Width = 1, Height = 1, Durability = 5 };
        var lostMapDefinition = new ItemDefinition { Group = 14, Number = 28, Width = 1, Height = 1, Durability = 1 };
        player.GameContext.Configuration.Items.Add(symbolDefinition);
        player.GameContext.Configuration.Items.Add(lostMapDefinition);

        var stack = new Item { Definition = symbolDefinition, Durability = 4, Level = 3, ItemSlot = 12 };
        await player.Inventory!.AddItemAsync(12, stack).ConfigureAwait(false);

        // two symbols on the ground: the stack of 4 doesn't take both, so only the partial path applies.
        var dropped = new DroppedItem(
            new Item { Definition = symbolDefinition, Durability = 2, Level = 3 },
            new Point(1, 1),
            player.CurrentMap!,
            null,
            null);

        var (success, _) = await dropped.TryPickUpByAsync(player).ConfigureAwait(false);

        Assert.That(success, Is.True);
        Assert.That(stack.Definition, Is.SameAs(lostMapDefinition));
        Assert.That(stack.Durability, Is.EqualTo(1));
        Assert.That(stack.Level, Is.EqualTo(3));
    }
}
