// <copyright file="HealingHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for <see cref="HealingHandler"/>.
/// </summary>
[TestFixture]
public class HealingHandlerTests
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
    /// Tests that <see cref="HealingHandler.PerformHealthRecoveryAsync"/> does nothing
    /// when config is null.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenConfigNullAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new HealingHandler(player, null);

        // Act
        await handler.PerformHealthRecoveryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that <see cref="HealingHandler.PerformHealthRecoveryAsync"/> does not consume
    /// a potion when the player's HP is above the threshold.
    /// </summary>
    [Test]
    public async ValueTask DoesNotUsePotionAboveThresholdAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var maxHp = player.Attributes![Stats.MaximumHealth];
        player.Attributes[Stats.CurrentHealth] = maxHp * 0.9f;

        var potion = this.CreateHealthPotion();
        await player.Inventory!.AddItemAsync((byte)(InventoryConstants.FirstEquippableItemSlotIndex + 12), potion).ConfigureAwait(false);

        var config = new MuHelperSettings
        {
            UseHealPotion = true,
            PotionThresholdPercent = 50,
        };

        var handler = new HealingHandler(player, config);

        // Act
        await handler.PerformHealthRecoveryAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.Inventory?.GetItem(potion.ItemSlot), Is.Not.Null);
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

    private MUnique.OpenMU.DataModel.Entities.Item CreateHealthPotion()
    {
        var definition = new Mock<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definition.Object.Number = ItemConstants.SmallHealingPotion.Number!.Value;
        definition.Object.Group = ItemConstants.SmallHealingPotion.Group;

        var item = new Mock<MUnique.OpenMU.DataModel.Entities.Item>();
        item.SetupAllProperties();
        item.Setup(i => i.Definition).Returns(definition.Object);
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Durability = 1;

        return item.Object;
    }

}
