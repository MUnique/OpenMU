// <copyright file="ItemPickupHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for <see cref="ItemPickupHandler"/>.
/// </summary>
[TestFixture]
public class ItemPickupHandlerTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that pickup does nothing when all pickup options are disabled.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenAllDisabledAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            PickAllItems = false,
            PickJewel = false,
            PickAncient = false,
            PickZen = false,
            PickExcellent = false,
        };

        var handler = new ItemPickupHandler(player, config);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that pickup does nothing when config is null.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenConfigNullAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new ItemPickupHandler(player, null);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a wanted drop out of pickup reach (but within the obtain range) is not requested from
    /// where the player stands - the server refuses it - but walked to instead.
    /// </summary>
    [Test]
    public async ValueTask WalksToDropOutOfPickupReachAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var map = player.CurrentMap!;
        var config = new MuHelperSettings
        {
            PickAllItems = true,
            ObtainRange = 6,
        };

        var handler = new ItemPickupHandler(player, config, new MovementHandler(player, config));
        var dropPosition = new Point((byte)(player.Position.X + 5), player.Position.Y);
        var drop = new DroppedItem(this.CreateItem(14, 13), dropPosition, map, player);
        await map.AddAsync(drop).ConfigureAwait(false);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(map.GetObject(drop.Id), Is.Not.Null, "The drop is out of reach and must stay on the ground for now");
        Assert.That(player.IsWalking, Is.True, "The player should walk to the drop");
    }

    /// <summary>
    /// Tests that a drop which stays on the ground after a pick up request is ignored for a while, so the
    /// player doesn't request it again on every tick. Even when it could be taken now, it's left alone.
    /// </summary>
    [Test]
    public async ValueTask IgnoresDropWhichCouldNotBePickedUpAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var map = player.CurrentMap!;
        var config = new MuHelperSettings
        {
            PickAllItems = true,
            ObtainRange = 6,
        };

        var handler = new ItemPickupHandler(player, config);
        var item = this.CreateItem(14, 13);
        item.Definition!.IsQuestItem = true; // refused: the player has no quest which requires it
        var drop = new DroppedItem(item, player.Position, map, player);
        await map.AddAsync(drop).ConfigureAwait(false);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);
        Assert.That(map.GetObject(drop.Id), Is.Not.Null, "The quest item can't be picked up without the quest");
        item.Definition.IsQuestItem = false;
        await handler.PickupItemsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(map.GetObject(drop.Id), Is.Not.Null, "The refused drop must not be requested again right away");
    }

    /// <summary>
    /// Tests that money is only walked to when the player could take it: a player who is refused the
    /// money because of the money limit leaves it alone, instead of walking to it again and again.
    /// </summary>
    /// <param name="playerMoney">The money of the player.</param>
    /// <param name="dropAmount">The money on the ground.</param>
    /// <param name="clampMoneyOnPickup">Whether a pick up which exceeds the limit adds the rest up to it.</param>
    /// <returns><c>True</c>, if the player walks to the money.</returns>
    [TestCase(0, 200, false, ExpectedResult = true)]
    [TestCase(800, 200, false, ExpectedResult = true)]
    [TestCase(900, 200, false, ExpectedResult = false)]
    [TestCase(900, 200, true, ExpectedResult = true)]
    [TestCase(1000, 200, true, ExpectedResult = false)]
    public async ValueTask<bool> WalksToMoneyOnlyWhenItCanBeTakenAsync(int playerMoney, int dropAmount, bool clampMoneyOnPickup)
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var map = player.CurrentMap!;
        this._gameContext.Configuration.MaximumInventoryMoney = 1000;
        this._gameContext.Configuration.ClampMoneyOnPickup = clampMoneyOnPickup;
        player.Money = playerMoney;

        // The test game context has no global money rate, so without it the player would get nothing.
        player.Attributes!.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.MoneyAmountRate);
        Assert.That(player.Attributes[Stats.MoneyAmountRate], Is.EqualTo(1), "test setup: the money rate was not applied");
        var config = new MuHelperSettings
        {
            PickAllItems = true,
            ObtainRange = 6,
        };

        var handler = new ItemPickupHandler(player, config, new MovementHandler(player, config));
        var dropPosition = new Point((byte)(player.Position.X + 5), player.Position.Y);
        var drop = new DroppedMoney((uint)dropAmount, dropPosition, map);
        await map.AddAsync(drop).ConfigureAwait(false);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);

        // Assert
        return player.IsWalking;
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

    private Item CreateItem(byte group, short number)
    {
        var definition = new Mock<ItemDefinition>();
        definition.SetupAllProperties();
        definition.Object.Group = group;
        definition.Object.Number = number;
        definition.Object.Width = 1;
        definition.Object.Height = 1;

        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.Definition).Returns(definition.Object);
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        return item.Object;
    }

    /// <summary>
    /// Tests that a jewel pickup option only picks up actual jewels.
    /// </summary>
    [Test]
    public async ValueTask PickJewel_PicksUpJewelsOnlyAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        Assert.That(player.CurrentMap, Is.Not.Null);

        var config = new MuHelperSettings
        {
            PickSelectItems = true,
            PickJewel = true,
            ObtainRange = 5,
        };

        var handler = new ItemPickupHandler(player, config);

        var bless = this.CreateItem(14, 13);
        var chaos = this.CreateItem(12, 15);
        var potion = this.CreateItem(14, 0);
        var eye = this.CreateItem(14, 17);

        var blessDrop = new DroppedItem(bless, player.Position, player.CurrentMap!, player);
        var chaosDrop = new DroppedItem(chaos, player.Position, player.CurrentMap!, player);
        var potionDrop = new DroppedItem(potion, player.Position, player.CurrentMap!, player);
        var eyeDrop = new DroppedItem(eye, player.Position, player.CurrentMap!, player);

        await player.CurrentMap!.AddAsync(blessDrop).ConfigureAwait(false);
        await player.CurrentMap.AddAsync(chaosDrop).ConfigureAwait(false);
        await player.CurrentMap.AddAsync(potionDrop).ConfigureAwait(false);
        await player.CurrentMap.AddAsync(eyeDrop).ConfigureAwait(false);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.CurrentMap.GetObject(blessDrop.Id), Is.Null, "Bless should be picked up");
        Assert.That(player.CurrentMap.GetObject(chaosDrop.Id), Is.Null, "Chaos should be picked up");
        Assert.That(player.CurrentMap.GetObject(potionDrop.Id), Is.Not.Null, "Potion should NOT be picked up");
        Assert.That(player.CurrentMap.GetObject(eyeDrop.Id), Is.Not.Null, "Devil's Eye should NOT be picked up");
    }
}
