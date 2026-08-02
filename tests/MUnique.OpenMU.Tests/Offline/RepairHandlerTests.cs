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
    /// Items at or below the 50% durability threshold should be repaired when the player
    /// has enough Zen.  Uses 10% (well below threshold) to confirm the happy path.
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
    /// Auto-repair does nothing when disabled in the configuration, regardless of durability.
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
    /// Auto-repair does not repair when the player has insufficient Zen.
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
    /// Fully durable items are skipped and no Zen is spent.
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

    /// <summary>
    /// An item at exactly 50% durability (the threshold boundary) must be repaired.
    /// Mirrors the client check: iHealth &lt;= DEFAULT_DURABILITY_THRESHOLD (50).
    /// </summary>
    [Test]
    public async ValueTask RepairsItemAtExactlyFiftyPercentThresholdAsync()
    {
        // Arrange — 50 / 100 = 50% (ceiling-integer: (50*100 + 99) / 100 = 50)
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 50);
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
    /// An item at 51% durability is above the threshold and must NOT be repaired,
    /// so no Zen is spent.  Mirrors the client check: iHealth &lt;= 50.
    /// </summary>
    [Test]
    public async ValueTask SkipsItemAboveFiftyPercentThresholdAsync()
    {
        // Arrange — 51 / 100 = 51% (ceiling-integer: (51*100 + 99) / 100 = 51)
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 100, currentDurability: 51);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        var initialMoney = 1_000_000;
        player.TryAddMoney(initialMoney);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert — durability unchanged, no money spent
        Assert.That(item.Durability, Is.EqualTo(51));
        Assert.That(player.Money, Is.EqualTo(initialMoney));
    }

    /// <summary>
    /// Verifies that the ceiling-integer formula handles non-round max-durability values
    /// correctly: 13 / 25 = 52% (above threshold → skip), but 12 / 25 = 48% (≤ 50% → repair).
    /// </summary>
    [TestCase(13, false, TestName = "NonRoundMax_52pct_Skipped")]
    [TestCase(12, true,  TestName = "NonRoundMax_48pct_Repaired")]
    public async ValueTask ThresholdWithNonRoundMaxDurabilityAsync(byte currentDurability, bool expectRepair)
    {
        // Arrange — max = 25; ceiling(d*100/25) = ceiling(d*4)
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var item = this.CreateDamagedItem(maxDurability: 25, currentDurability: currentDurability);
        await player.Inventory!.AddItemAsync(InventoryConstants.ArmorSlot, item).ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var config = new MuHelperSettings { RepairItem = true };
        var handler = new RepairHandler(player, config);

        // Act
        await handler.PerformRepairsAsync().ConfigureAwait(false);

        // Assert
        if (expectRepair)
        {
            Assert.That(item.Durability, Is.EqualTo(25));
        }
        else
        {
            Assert.That(item.Durability, Is.EqualTo(currentDurability));
        }
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
