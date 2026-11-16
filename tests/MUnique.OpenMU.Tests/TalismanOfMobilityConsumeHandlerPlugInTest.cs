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
