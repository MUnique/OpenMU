// <copyright file="ItemConsumptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Tests the item consumption action.
/// </summary>
[TestFixture]
public class ItemConsumptionTest
{
    private const int ItemSlot = 12;

    /// <summary>
    /// Tests the jewel of bless consume.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="consumptionExpectation">if set to <c>true</c>, the item consumption is expected.</param>
    [TestCase(0, true)]
    [TestCase(1, true)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    [TestCase(4, true)]
    [TestCase(5, true)]
    [TestCase(6, false)]
    [TestCase(7, false)]
    public async ValueTask JewelOfBlessAsync(byte itemLevel, bool consumptionExpectation)
    {
        var consumeHandler = new BlessJewelConsumeHandlerPlugIn();

        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var upgradeableItem = this.GetItemWithPossibleOption();
        upgradeableItem.Level = itemLevel;
        var upgradableItemSlot = (byte)(ItemSlot + 1);
        await player.Inventory!.AddItemAsync(upgradableItemSlot, upgradeableItem).ConfigureAwait(false);
        var bless = this.GetItem();
        await player.Inventory.AddItemAsync(ItemSlot, bless).ConfigureAwait(false);
        bless.Durability = 1;

        var consumed = await consumeHandler.ConsumeItemAsync(player, bless, upgradeableItem, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.EqualTo(consumptionExpectation));
        Assert.That(upgradeableItem.Level, consumed ? Is.EqualTo(itemLevel + 1) : Is.EqualTo(itemLevel));
    }

    /// <summary>
    /// Tests the jewel of soul consumption.
    /// </summary>
    /// <param name="itemLevel">The item level before consuming the jewel of soul.</param>
    /// <param name="consumptionExpectation">If set to <c>true</c>, the consumption of the jewel of soul is expected.</param>
    /// <param name="success">If set to <c>true</c>, the randomizer returns <c>true</c> when asked about wether the item level should be increased. However, it doesn't have any effect if the item is already level 9 or higher.</param>
    /// <param name="expectedItemLevel">The expected item level after trying to consume the jewel of soul.</param>
    [TestCase(0, true, true, 1)]
    [TestCase(1, true, true, 2)]
    [TestCase(2, true, true, 3)]
    [TestCase(3, true, true, 4)]
    [TestCase(4, true, true, 5)]
    [TestCase(5, true, true, 6)]
    [TestCase(6, true, true, 7)]
    [TestCase(7, true, true, 8)]
    [TestCase(8, true, true, 9)]
    [TestCase(9, false, true, 9)]
    [TestCase(10, false, true, 10)]
    [TestCase(11, false, true, 11)]
    [TestCase(12, false, true, 12)]
    [TestCase(13, false, true, 13)]
    [TestCase(14, false, true, 14)]
    [TestCase(15, false, true, 15)]
    [TestCase(0, true, false, 0)]
    [TestCase(1, true, false, 0)]
    [TestCase(2, true, false, 1)]
    [TestCase(3, true, false, 2)]
    [TestCase(4, true, false, 3)]
    [TestCase(5, true, false, 4)]
    [TestCase(6, true, false, 5)]
    [TestCase(7, true, false, 0)]
    [TestCase(8, true, false, 0)]
    public async ValueTask JewelOfSoulAsync(byte itemLevel, bool consumptionExpectation, bool success, byte expectedItemLevel)
    {
        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(r => r.NextRandomBool(50)).Returns(success);
        var consumeHandler = new SoulJewelConsumeHandlerPlugIn(randomizer.Object);

        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var upgradeableItem = this.GetItemWithPossibleOption();
        upgradeableItem.Level = itemLevel;
        var upgradableItemSlot = (byte)(ItemSlot + 1);
        await player.Inventory!.AddItemAsync(upgradableItemSlot, upgradeableItem).ConfigureAwait(false);
        var soul = this.GetItem();
        await player.Inventory.AddItemAsync(ItemSlot, soul).ConfigureAwait(false);
        soul.Durability = 1;

        var consumed = await consumeHandler.ConsumeItemAsync(player, soul, upgradeableItem, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.EqualTo(consumptionExpectation));
        Assert.That(upgradeableItem.Level, Is.EqualTo(expectedItemLevel));
    }

    /// <summary>
    /// Test if the jewel of life consumption increases the item option level by 1 until the maximum level is reached.
    /// </summary>
    /// <param name="numberOfOptions">The number of options.</param>
    /// <param name="consumptionExpectation">If set to <c>true</c>, the item consumption is expected; Otherwise, not.</param>
    [TestCase(1, true)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    [TestCase(4, true)]
    [TestCase(5, false)]
    public async ValueTask JewelOfLifeAsync(int numberOfOptions, bool consumptionExpectation)
    {
        var consumeHandler = new LifeJewelConsumeHandlerPlugIn();
        consumeHandler.Configuration.SuccessChance = 1;
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var upgradeableItem = this.GetItemWithPossibleOption();
        var upgradableItemSlot = (byte)(ItemSlot + 1);
        await player.Inventory!.AddItemAsync(upgradableItemSlot, upgradeableItem).ConfigureAwait(false);
        bool jolConsumed = false;
        for (int i = 0; i < numberOfOptions; i++)
        {
            var item = this.GetItem();
            await player.Inventory.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
            item.Durability = 1;

            jolConsumed = await consumeHandler.ConsumeItemAsync(player, item, upgradeableItem, FruitUsage.Undefined).ConfigureAwait(false);
        }

        Assert.That(jolConsumed, Is.EqualTo(consumptionExpectation));
        if (jolConsumed)
        {
            Assert.That(upgradeableItem.ItemOptions.Count, Is.EqualTo(1));
            Assert.That(upgradeableItem.ItemOptions.First().Level, Is.EqualTo(numberOfOptions));
        }
    }

    /// <summary>
    /// Tests if a failed Jewel of life removes the option at any level.
    /// </summary>
    /// <param name="numberOfOptions">The number of options.</param>
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async ValueTask JewelOfLifeFailRemovesOptionAsync(int numberOfOptions)
    {
        var consumeHandler = new LifeJewelConsumeHandlerPlugIn();
        consumeHandler.Configuration.SuccessChance = 1;
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var upgradeableItem = this.GetItemWithPossibleOption();
        var upgradableItemSlot = (byte)(ItemSlot + 1);
        await player.Inventory!.AddItemAsync(upgradableItemSlot, upgradeableItem).ConfigureAwait(false);

        for (int i = 0; i < numberOfOptions; i++)
        {
            var jol1 = this.GetItem();
            await player.Inventory.AddItemAsync(ItemSlot, jol1).ConfigureAwait(false);
            jol1.Durability = 1;

            await consumeHandler.ConsumeItemAsync(player, jol1, upgradeableItem, FruitUsage.Undefined).ConfigureAwait(false);
        }
 
        Assert.That(upgradeableItem.ItemOptions.Count, Is.EqualTo(1));

        // then adding fails, so option needs to be removed
        consumeHandler.Configuration.SuccessChance = 0;
        var jol2 = this.GetItem();
        await player.Inventory.AddItemAsync(ItemSlot, jol2).ConfigureAwait(false);
        jol2.Durability = 1;
        var jolConsumed = await consumeHandler.ConsumeItemAsync(player, jol2, upgradeableItem, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(jolConsumed, Is.True);
        Assert.That(upgradeableItem.ItemOptions.Count, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests the jewel of harmony consume.
    /// </summary>
    public void JewelOfHarmony()
    {
        Assert.That(true, Is.False);
    }

    /// <summary>
    /// Tests the refine stone consume.
    /// </summary>
    public void RefineStone()
    {
        // refine stone consume handler is not implemented yet
        Assert.That(true, Is.False);
    }

    /// <summary>
    /// Tests the complex potion consume.
    /// </summary>
    public void ComplexPotion()
    {
        // complex potion consume handler is not implemented yet
        Assert.That(true, Is.False);
    }

    /// <summary>
    /// Tests the shield potion consume.
    /// </summary>
    [Test]
    public async ValueTask ShieldPotionAsync()
    {
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var consumeHandler = new LargeShieldPotionConsumeHandlerPlugIn();
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.True);
        Assert.That(player.Attributes!.GetValueOfAttribute(Stats.CurrentShield), Is.GreaterThan(0.0f));
    }

    /// <summary>
    /// Tests if the consumption fails because of the player state.
    /// </summary>
    [Test]
    public async ValueTask FailByWrongPlayerStateAsync()
    {
        var consumeHandler = new AlcoholConsumeHandlerPlugIn();
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.False);
    }

    /// <summary>
    /// Tests if the consumption of the item decreases its durability by one.
    /// </summary>
    [Test]
    public async ValueTask ItemDurabilityDecreaseAsync()
    {
        var consumeHandler = new LargeShieldPotionConsumeHandlerPlugIn();
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        item.Durability = 3;
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.True);
        Assert.That(item.Durability, Is.EqualTo(2));
        Assert.That(player.Inventory.Items.Any(), Is.True);
    }

    /// <summary>
    /// Tests if the consumption of the item not causes the removal of the item, when the durability reaches 0.
    /// The removal is handled in the <see cref="ItemConsumeAction"/>.
    /// </summary>
    [Test]
    public async ValueTask ItemRemovalAsync()
    {
        var consumeHandler = new LargeShieldPotionConsumeHandlerPlugIn();
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.True);
        Assert.That(item.Durability, Is.EqualTo(0));
        Assert.That(player.Inventory.Items.Any(), Is.True);
    }

    /// <summary>
    /// Tests if the consumption of the alcohol fails when the item has no durability anymore.
    /// </summary>
    [Test]
    public async ValueTask DrinkAlcoholFailAsync()
    {
        var consumeHandler = new AlcoholConsumeHandlerPlugIn();
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        item.Durability = 0;
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(success, Is.False);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IConsumeSpecialItemPlugIn>()!).Verify(view => view!.ConsumeSpecialItemAsync(item, 80), Times.Never);
    }

    /// <summary>
    /// Tests if the consumption of alcohol works and is forwarded to the player view.
    /// </summary>
    [Test]
    public async ValueTask DrinkAlcoholSuccessAsync()
    {
        var consumeHandler = new AlcoholConsumeHandlerPlugIn();
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        item.Definition!.ConsumeEffect = new Persistence.BasicModel.MagicEffectDefinition
        {
            Duration = new Persistence.BasicModel.PowerUpDefinitionValue
            {
                ConstantValue = { Value = 80 }
            },
            PowerUpDefinitions =
            {
                new Persistence.BasicModel.PowerUpDefinition
                {
                    TargetAttribute = Stats.AttackSpeedAny,
                    Boost = new Persistence.BasicModel.PowerUpDefinitionValue
                    {
                        ConstantValue = { Value = 20 }
                    }
                }
            }
        };

        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(success, Is.True);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IConsumeSpecialItemPlugIn>()!).Verify(view => view!.ConsumeSpecialItemAsync(item, 80), Times.Once);
    }

    /// <summary>
    /// Tests the health recover by drinking a health potion.
    /// </summary>
    [Test]
    public async ValueTask HealthRecoverAsync()
    {
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var consumeHandler = new LargeHealthPotionConsumeHandlerPlugIn();
        consumeHandler.Configuration = consumeHandler.CreateDefaultConfig() as RecoverConsumeHandlerConfiguration;
        consumeHandler.Configuration!.RecoverSteps.Clear(); // When there are no steps, we recover all immediately.
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.True);
        Assert.That(player.Attributes!.GetValueOfAttribute(Stats.CurrentHealth), Is.GreaterThan(0.0f));
    }

    /// <summary>
    /// Tests the mana recover by drinking a mana potion.
    /// </summary>
    [Test]
    public async ValueTask ManaRecoverAsync()
    {
        var player = await this.GetPlayerAsync().ConfigureAwait(false);
        var item = this.GetItem();
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        var consumeHandler = new LargeManaPotionConsumeHandler();
        var success = await consumeHandler.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);
        Assert.That(success, Is.True);
        Assert.That(player.Attributes!.GetValueOfAttribute(Stats.CurrentMana), Is.GreaterThan(0.0f));
    }

    private Item GetItem()
    {
        return new ()
        {
            Definition = new DataModel.Configuration.Items.ItemDefinition { Width = 1, Height = 1 },
            Durability = 1,
        };
    }

    private async ValueTask<Player> GetPlayerAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);

        player.SelectedCharacter!.Attributes.Add(new StatAttribute(Stats.Level, 100));
        player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.CurrentHealth, 0));
        player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.CurrentMana, 0));
        player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.CurrentShield, 0));

        return player;
    }

    private Item GetItemWithPossibleOption()
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var definition = new Mock<ItemDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definition.Object.MaximumItemLevel = 15;
        var itemSlot = new Mock<ItemSlotType>();
        itemSlot.Setup(s => s.ItemSlots).Returns(new List<int> { InventoryConstants.LeftHandSlot });
        definition.Setup(d => d.ItemSlot).Returns(itemSlot.Object);
        item.Object.Definition = definition.Object;
        item.Object.Durability = 1;
        item.Object.Definition.Width = 1;
        item.Object.Definition.Height = 2;
        var option = new Mock<ItemOptionDefinition>();
        option.SetupAllProperties();
        option.Setup(o => o.PossibleOptions).Returns(new List<IncreasableItemOption>());
        option.Object.MaximumOptionsPerItem = 4;
        option.Object.AddsRandomly = true;
        option.Name = "Damage Option";

        var possibleOption = new Mock<IncreasableItemOption>();
        possibleOption.SetupAllProperties();
        possibleOption.Setup(o => o.LevelDependentOptions).Returns(new List<ItemOptionOfLevel>());
        possibleOption.Object.OptionType = ItemOptionTypes.Option;
        option.Object.PossibleOptions.Add(possibleOption.Object);
        for (int level = 1; level <= 4; level++)
        {
            var levelDependentOption = new ItemOptionOfLevel();
            levelDependentOption.Level = level;
            possibleOption.Object.LevelDependentOptions.Add(levelDependentOption);
        }

        item.Object.Definition.PossibleItemOptions.Add(option.Object);
        return item.Object;
    }
}