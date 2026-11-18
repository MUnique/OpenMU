// <copyright file="TalismanOfMobilityConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="TalismanOfMobilityConsumeHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class TalismanOfMobilityConsumeHandlerPlugInTest
{
    private static readonly Point SavedPosition = new(100, 120);

    /// <summary>
    /// The first usage saves the place and uses up one durability, the second one warps the character back to it.
    /// </summary>
    [Test]
    public async Task SavesPlaceAndReturnsToItAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.EqualTo(1));

        player.Position = new Point(30, 40);
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.Zero);
        Assert.That((player.SelectedCharacter!.PositionX, player.SelectedCharacter.PositionY), Is.EqualTo((SavedPosition.X, SavedPosition.Y)));
    }

    /// <summary>
    /// A used talisman without a saved place isn't used up.
    /// </summary>
    [Test]
    public async Task UsedTalismanWithoutSavedPlaceIsKeptAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        talisman.Durability = 1;

        Assert.That(await new TalismanOfMobilityConsumeHandlerPlugIn().ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// While another talisman in the inventory still has to return to the saved place, an unused talisman doesn't save
    /// a new place, because the other talisman couldn't return anymore. After the other one returned, it can save again.
    /// </summary>
    [Test]
    public async Task NoNewPlaceIsSavedWhileAnotherTalismanHasToReturnAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var halfUsedTalisman = CreateTalisman();
        var unusedTalisman = new BasicModel.Item { Definition = halfUsedTalisman.Definition, Durability = 2 };
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, halfUsedTalisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(await player.Inventory!.AddItemAsync(halfUsedTalisman).ConfigureAwait(false), Is.True);
        Assert.That(await player.Inventory.AddItemAsync(unusedTalisman).ConfigureAwait(false), Is.True);

        player.Position = new Point(30, 40);
        Assert.That(await handler.ConsumeItemAsync(player, unusedTalisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(unusedTalisman.Durability, Is.EqualTo(2));

        Assert.That(await handler.ConsumeItemAsync(player, halfUsedTalisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That((player.SelectedCharacter!.PositionX, player.SelectedCharacter.PositionY), Is.EqualTo((SavedPosition.X, SavedPosition.Y)));

        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        // The used up talisman is removed by the consume action; here it stays with a durability of 0.
        Assert.That(await handler.ConsumeItemAsync(player, unusedTalisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(unusedTalisman.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// A character which doesn't fulfill the requirements of the saved map anymore, e.g. because it lost its wings
    /// after saving a place in Icarus, isn't warped and keeps the talisman and the saved place.
    /// </summary>
    [Test]
    public async Task CharacterWhichDoesNotFulfillMapRequirementsKeepsTalismanAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);

        Mock.Get(player.CurrentMap!.Definition)
            .Setup(map => map.MapRequirements)
            .Returns(new List<AttributeRequirement> { new BasicModel.AttributeRequirement { Attribute = Stats.CanFly, MinimumValue = 1 } });
        player.Position = new Point(30, 40);

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
        Assert.That(player.Position, Is.EqualTo(new Point(30, 40)));

        Mock.Get(player.CurrentMap.Definition).Setup(map => map.MapRequirements).Returns(new List<AttributeRequirement>());
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.Zero);
    }

    /// <summary>
    /// When a <see cref="IWarpGateEnteringPlugIn"/> denies the return, e.g. because an event map is closed,
    /// the character isn't warped and keeps the talisman and the saved place.
    /// </summary>
    [Test]
    public async Task DeniedReturnKeepsTalismanAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);

        var denyingPlugIn = new Mock<IWarpGateEnteringPlugIn>();
        denyingPlugIn
            .Setup(p => p.WarpGateEnteringAsync(It.IsAny<Player>(), It.IsAny<ExitGate>(), It.IsAny<CancelEventArgs>()))
            .Callback<Player, ExitGate, CancelEventArgs>((_, _, eventArgs) => eventArgs.Cancel = true)
            .Returns(ValueTask.CompletedTask);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(denyingPlugIn.Object);
        player.Position = new Point(30, 40);

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
        Assert.That(player.Position, Is.EqualTo(new Point(30, 40)));
        denyingPlugIn.Verify(p => p.WarpGateEnteringAsync(player, It.Is<ExitGate>(gate => gate.X1 == SavedPosition.X && gate.Y1 == SavedPosition.Y), It.IsAny<CancelEventArgs>()), Times.Once);
    }

    /// <summary>
    /// When the saved place is on the current map and a <see cref="IPlayerTeleportRestrictionPlugIn"/> blocks the way,
    /// e.g. by a closed castle gate, the character isn't warped and keeps the talisman and the saved place.
    /// </summary>
    [Test]
    public async Task BlockedReturnOnTheSameMapKeepsTalismanAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);

        var blockingPlugIn = new Mock<IPlayerTeleportRestrictionPlugIn>();
        blockingPlugIn
            .Setup(p => p.CheckTeleport(It.IsAny<Player>(), It.IsAny<Point>(), It.IsAny<CancelEventArgs>()))
            .Callback<Player, Point, CancelEventArgs>((_, _, eventArgs) => eventArgs.Cancel = true);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(blockingPlugIn.Object);
        player.Position = new Point(30, 40);

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
        Assert.That(player.Position, Is.EqualTo(new Point(30, 40)));
        blockingPlugIn.Verify(p => p.CheckTeleport(player, SavedPosition, It.IsAny<CancelEventArgs>()), Times.Once);
    }

    /// <summary>
    /// When no game server hosts the saved map at the moment, e.g. because it's stopped or the map got deactivated,
    /// the character isn't warped and keeps the talisman and the saved place, instead of ending at its home map.
    /// </summary>
    [Test]
    public async Task UnavailableSavedMapKeepsTalismanAsync()
    {
        const short unavailableMapNumber = 77;
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);

        player.GameContext.Configuration.Maps.Add(new BasicModel.GameMapDefinition { Number = unavailableMapNumber, IsActive = false });
        var savedMap = player.SelectedCharacter!.Attributes.Single(a => a.Definition == Stats.MobilityTalismanMap);
        savedMap.Value = unavailableMapNumber + 1;
        player.Position = new Point(30, 40);

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
        Assert.That(savedMap.Value, Is.EqualTo(unavailableMapNumber + 1));
        Assert.That(player.Position, Is.EqualTo(new Point(30, 40)));
    }

    /// <summary>
    /// Deactivating an already initialized map rejects the return without losing the charge or bookmark.
    /// Reactivating the map allows the same talisman to return to the original place.
    /// </summary>
    [Test]
    public async Task InactiveCachedSavedMapKeepsTalismanAndBookmarkAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var talisman = CreateTalisman();
        var handler = new TalismanOfMobilityConsumeHandlerPlugIn();
        player.Position = SavedPosition;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);

        var map = player.CurrentMap!;
        map.Definition.IsActive = false;
        Assert.That(await player.GameContext.GetMapAsync(map.MapId).ConfigureAwait(false), Is.SameAs(map));
        player.Position = new Point(30, 40);
        var bookmark = player.SelectedCharacter!.Attributes.Single(a => a.Definition == Stats.MobilityTalismanMap);

        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.False);
        Assert.That(talisman.Durability, Is.EqualTo(1));
        Assert.That(bookmark.Value, Is.EqualTo(map.MapId + 1));
        Assert.That(player.Position, Is.EqualTo(new Point(30, 40)));

        map.Definition.IsActive = true;
        Assert.That(await handler.ConsumeItemAsync(player, talisman, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        Assert.That(talisman.Durability, Is.Zero);
        Assert.That(bookmark.Value, Is.Zero);
        Assert.That((player.SelectedCharacter.PositionX, player.SelectedCharacter.PositionY), Is.EqualTo((SavedPosition.X, SavedPosition.Y)));
    }

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.Maps.Single().IsActive = true;
        return player;
    }

    private static Item CreateTalisman()
    {
        var definition = new BasicModel.ItemDefinition { Group = 13, Number = 70, Width = 1, Height = 1, Durability = 2 };
        return new BasicModel.Item { Definition = definition, Durability = 2 };
    }
}
