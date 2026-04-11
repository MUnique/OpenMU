// <copyright file="RepairHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using Item = MUnique.OpenMU.Persistence.BasicModel.Item;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemSlotType = MUnique.OpenMU.Persistence.BasicModel.ItemSlotType;

/// <summary>
/// Tests for <see cref="RepairHandler"/>.
/// </summary>
[TestFixture]
public class RepairHandlerTests
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
    /// Tests that auto-repair restores item durability when the player has enough Zen.
    /// </summary>
    [Test]
    public async ValueTask RepairsItemWhenSufficientZenAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(item.Durability, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that auto-repair does nothing when disabled in the configuration.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenDisabledAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var config = new MuHelperSettings { RepairItem = false };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(item.Durability, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that auto-repair does not repair when the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask DoesNotRepairWhenInsufficientZenAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 10);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(item.Durability, Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that fully durable items are skipped by the repair handler.
    /// </summary>
    [Test]
    public async ValueTask SkipsFullyDurableItemsAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 100);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        var initialMoney = 1_000_000;
        player.TryAddMoney(initialMoney);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.Money, Is.EqualTo(initialMoney));
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

    private Item CreateDamagedItem(byte maxDurability, byte currentDurability)
    {
        return new Item
        {
            Definition = new ItemDefinition
            {
                Durability = maxDurability,
                ItemSlot = new ItemSlotType(),
                Width = 1,
                Height = 1,
                Value = 1000,
            },
            Durability = currentDurability,
        };
    }

}
