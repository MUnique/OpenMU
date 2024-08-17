// <copyright file="PowerUpFactoryTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests the power up factory which is creating power ups based on the items a player has equipped.
/// </summary>
[TestFixture]
public class PowerUpFactoryTest
{
    private const byte UnwearableSlot = 0xFF;

    private const int PowerUpStrength = 16;

    private readonly float[] _levelBonus = new float[] { 0, 1, 3, 7, 14 };

    /// <summary>
    /// Tests if the item option results in an corresponding power up.
    /// </summary>
    [Test]
    public async ValueTask ItemOptionsAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItemWithOption();
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength));
    }

    /// <summary>
    /// Tests if the item option of level 0 results in the corresponding power up.
    /// </summary>
    [Test]
    public async ValueTask ItemBasePowerUpLevel0Async()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItemWithBasePowerUp();
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength));
    }

    /// <summary>
    /// Tests if the item option of level 3 results in the corresponding power up.
    /// </summary>
    [Test]
    public async ValueTask ItemBasePowerUpLevel3Async()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItemWithBasePowerUp();
        item.Level = 3;
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength + this._levelBonus[3]));
    }

    /// <summary>
    /// Tests if the power ups don't get created if the item has no more durability.
    /// </summary>
    [Test]
    public async ValueTask NoPowerUpsWhenItemBrokenAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItemWithBasePowerUp();
        item.Durability = 0;
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if the power ups don't get created if the item is not equipped at the player.
    /// </summary>
    [Test]
    public async ValueTask NoPowerUpsWhenItemUnwearableAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItemWithBasePowerUp();
        item.ItemSlot = UnwearableSlot;
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if the power ups don't get created when the item has no options.
    /// </summary>
    [Test]
    public async ValueTask NoPowerUpsInItemAsync()
    {
        var player = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var factory = this.GetPowerUpFactory();
        var item = this.GetItem();
        var result = factory.GetPowerUps(item, player.Attributes!);
        Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if a complete set of level 11, gives the power up defined for level 11.
    /// </summary>
    [Test]
    public void SetCompleteGivesPowerUp()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(10, 11, 11, 11, 11);
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.Not.EqualTo(0));
    }

    /// <summary>
    /// Tests if a complete set of level 11, gives the power up defined for level 11 even if one item has a higher level.
    /// </summary>
    [Test]
    public void SetCompleteGivesPowerUpWhenItemIsHigherLevel()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(10, 11, 12, 11, 11);
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.Not.EqualTo(0));
    }

    /// <summary>
    /// Tests if an incomplete set of level 11 gives no power up, because at least one item has not the required level.
    /// </summary>
    [Test]
    public void SetIncompleteDueLowerLevelItemsGivesNoPowerUp()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(10, 11, 11, 15, 9);
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if no items give no power ups.
    /// </summary>
    [Test]
    public void NoItemsGiveNoSetPowerUps()
    {
        var factory = this.GetPowerUpFactory();
        var items = Enumerable.Empty<Item>();
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if an incomplete set (=not all required items equipped) gives also no power up.
    /// </summary>
    [Test]
    public void SetIncompleteGivesNoPowerUp()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(30, 15, 15, 15);
        var result = factory.GetSetPowerUps(items.Skip(1), this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if the correct value is set in the power up, as defined.
    /// </summary>
    [Test]
    public void SetBonusDefSetValueCorrect()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(5, 10, 10, 15, 10);
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration()).ToList();
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().Value, Is.EqualTo(5.0));
    }

    /// <summary>
    /// Tests if no power up is given for a specific level, when all of the items are of a higher level.
    /// This is the expected behavior, because otherwise, multiple level-dependent set bonuses would take effect.
    /// </summary>
    [Test]
    public void SetBonusNotAppliedWhenFullSetOfHigherLevel()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetDefenseBonusSet(5, 10, 15, 15, 15);
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if one ancient item gives just the bonus option.
    /// </summary>
    [Test]
    public void OneAncientItemGivesJustBonusOption()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetAncientSet(5, 4).ToList();

        var bonusOptions = factory.GetPowerUps(items.First(), this.GetAttributeSystem());
        Assert.That(bonusOptions.Count(), Is.EqualTo(1));

        var result = factory.GetSetPowerUps(items.Take(1), this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Tests if a partially complete ancient set gives just the bonus option plus the number of unlocked options (item count - 1).
    /// </summary>
    /// <param name="setSize">The item count.</param>
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void AncientSetPartiallyComplete(int setSize)
    {
        var factory = this.GetPowerUpFactory();
        var setItems = this.GetAncientSet(5, setSize).ToList();
        var items = setItems.SkipLast(1).ToList();

        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(items.Count - 1));

        var bonusOptions = items.SelectMany(item => factory.GetPowerUps(item, this.GetAttributeSystem()));
        Assert.That(bonusOptions.Count(), Is.EqualTo(items.Count));
    }

    /// <summary>
    /// Tests if a complete ancient set unlocks all options.
    /// In case of 5 items in a set, there are 9 power ups (4 unlocked options + 5 bonus options).
    /// </summary>
    [Test]
    public void AncientSetComplete()
    {
        var factory = this.GetPowerUpFactory();
        var items = this.GetAncientSet(6, 5).ToList();
        var result = factory.GetSetPowerUps(items, this.GetAttributeSystem(), new GameConfiguration());
        Assert.That(result.Count(), Is.EqualTo(6));
        var bonusOptions = items.SelectMany(item => factory.GetPowerUps(item, this.GetAttributeSystem()));
        Assert.That(bonusOptions.Count(), Is.EqualTo(5));
    }

    private IItemPowerUpFactory GetPowerUpFactory()
    {
        return new ItemPowerUpFactory(new NullLogger<ItemPowerUpFactory>());
    }

    private ItemDefinition GetItemDefinition()
    {
        var itemDefinition = new Mock<ItemDefinition>();
        itemDefinition.Object.Durability = 100;
        itemDefinition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        itemDefinition.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        return itemDefinition.Object;
    }

    private Item GetItem()
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Object.Definition = this.GetItemDefinition();
        item.Object.ItemSlot = 0;
        item.Object.Durability = item.Object.Definition.Durability;
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        return item.Object;
    }

    private Item GetItemWithOption()
    {
        var item = this.GetItem();
        item.ItemOptions.Add(this.GetOption(Stats.MaximumPhysBaseDmg, PowerUpStrength));
        return item;
    }

    private Item GetItemWithBasePowerUp()
    {
        var item = this.GetItem();
        item.Definition!.BasePowerUpAttributes.Add(this.GetBasePowerUpDefinition());
        return item;
    }

    private ItemBasePowerUpDefinition GetBasePowerUpDefinition()
    {
        var resultMock = new Mock<ItemBasePowerUpDefinition>();
        resultMock.SetupAllProperties();
        var bonusTableMock = new Mock<ItemLevelBonusTable>();
        bonusTableMock.Setup(r => r.BonusPerLevel).Returns(new List<LevelBonus>());
        var result = resultMock.Object;
        result.BaseValue = PowerUpStrength;
        result.TargetAttribute = Stats.MaximumPhysBaseDmg;
        var bonusTable = bonusTableMock.Object;
        result.BonusPerLevelTable = bonusTable;
        bonusTable.BonusPerLevel.Add(new LevelBonus(1, this._levelBonus[1]));
        bonusTable.BonusPerLevel.Add(new LevelBonus(2, this._levelBonus[2]));
        bonusTable.BonusPerLevel.Add(new LevelBonus(3, this._levelBonus[3]));
        bonusTable.BonusPerLevel.Add(new LevelBonus(4, this._levelBonus[4]));
        return result;
    }

    private ItemOptionLink GetOption(AttributeDefinition targetAttribute, float value)
    {
        var option = new IncreasableItemOption
        {
            OptionType = ItemOptionTypes.Option,
            PowerUpDefinition = new PowerUpDefinition
            {
                TargetAttribute = targetAttribute,
                Boost = new TestPowerUpDefinitionValue(new SimpleElement { Value = value }),
            },
        };
        return new ItemOptionLink { ItemOption = option, Level = 1 };
    }

    private IEnumerable<Item> GetDefenseBonusSet(float setBonusDefense, byte minimumLevel, params byte[] levels)
    {
        var itemOptionDef = new Mock<ItemOptionDefinition>();
        itemOptionDef.Setup(a => a.PossibleOptions).Returns(new List<IncreasableItemOption>());
        itemOptionDef.Object.PossibleOptions.Add(this.GetOption(Stats.DefenseBase, setBonusDefense).ItemOption!);

        var armorSet = new Mock<ItemSetGroup>();
        armorSet.Setup(a => a.Items).Returns(new List<ItemOfItemSet>());
        armorSet.Setup(a => a.Options).Returns(itemOptionDef.Object);

        armorSet.Object.MinimumItemCount = levels.Length;
        armorSet.Object.SetLevel = minimumLevel;
        
        foreach (var level in levels)
        {
            var item = this.GetItem();
            var itemOfItemSet = new ItemOfItemSet { ItemDefinition = item.Definition, ItemSetGroup = armorSet.Object };
            item.Definition!.PossibleItemSetGroups.Add(armorSet.Object);
            item.ItemSetGroups.Add(itemOfItemSet);
            armorSet.Object.Items.Add(itemOfItemSet);
            item.Level = level;
            yield return item;
        }
    }

    private IEnumerable<Item> GetAncientSet(int ancientOptionCount, int itemCount)
    {
        var itemOptionDef = new Mock<ItemOptionDefinition>();
        itemOptionDef.Setup(a => a.PossibleOptions).Returns(new List<IncreasableItemOption>());
        var ancientSet = new Mock<ItemSetGroup>();
        ancientSet.Setup(a => a.Items).Returns(new List<ItemOfItemSet>());
        ancientSet.Setup(a => a.Options).Returns(itemOptionDef.Object);

        ancientSet.Object.MinimumItemCount = 2;
        for (int i = 0; i < ancientOptionCount; i++)
        {
            var setOption = this.GetOption(Stats.DefenseBase, i + 10).ItemOption;
            setOption!.Number = i + 1;
            itemOptionDef.Object.PossibleOptions.Add(setOption);
        }

        var bonusOption = this.GetOption(Stats.TotalStrength, 5).ItemOption;
        for (int i = 0; i < itemCount; i++)
        {
            var item = this.GetItem();
            var itemOfSet = new ItemOfItemSet { BonusOption = bonusOption, ItemDefinition = item.Definition, ItemSetGroup = ancientSet.Object};
            item.Definition!.PossibleItemSetGroups.Add(ancientSet.Object);
            item.ItemSetGroups.Add(itemOfSet);
            item.ItemOptions.Add(new ItemOptionLink { ItemOption = bonusOption, Level = 1 });

            ancientSet.Object.Items.Add(itemOfSet);
            yield return item;
        }
    }

    private AttributeSystem GetAttributeSystem() => new (Enumerable.Empty<IAttribute>(), Enumerable.Empty<IAttribute>(), Enumerable.Empty<AttributeRelationship>());

    private class TestPowerUpDefinitionValue : PowerUpDefinitionValue
    {
        public TestPowerUpDefinitionValue(SimpleElement constantValue)
        {
            this.ConstantValue = constantValue;
        }
    }
}