// <copyright file="ItemPriceCalculatorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests the <see cref="ItemPriceCalculator"/> with some exemplary data.
/// </summary>
/// <remarks>
/// The most price values here are directly taken from stores on GMO.
/// However, I guess they are calculated and shown by the client, if you just show such an item in the merchant store.
/// </remarks>
[TestFixture]
public class ItemPriceCalculatorTest
{
    /// <summary>
    /// The calculator which is tested.
    /// </summary>
    private readonly ItemPriceCalculator _calculator = new ();

    /// <summary>
    /// Tests if the apple price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 20)]
    [TestCase(1, 40)]
    public void Apple(byte level, int price)
    {
        this.CheckPrice(0, 1, 1, 1, 1, 14, 5, level, price);
    }

    /// <summary>
    /// Tests if the small heal potion price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 80)]
    [TestCase(1, 160)]
    public void SmallHealPotion(byte level, int price)
    {
        this.CheckPrice(1, 40, 1, 1, 1, 14, 10, level, price);
    }

    /// <summary>
    /// Tests if the heal potion price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 330)]
    [TestCase(1, 660)]
    public void HealPotion(byte level, int price)
    {
        this.CheckPrice(2, 40, 1, 1, 1, 14, 20, level, price);
    }

    /// <summary>
    /// Tests if the large heal potion price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 1500)]
    [TestCase(1, 3000)]
    public void LargeHealPotion(byte level, int price)
    {
        this.CheckPrice(3, 40, 1, 1, 1, 14, 30, level, price);
    }

    /// <summary>
    /// Tests if the small shield potion price is calculated correctly.
    /// </summary>
    [Test]
    public void SmallShieldPotion()
    {
        this.CheckPrice(35, 40, 1, 1, 1, 14, 50, 0, 2000);
    }

    /// <summary>
    /// Tests if the shield potion price is calculated correctly.
    /// </summary>
    [Test]
    public void ShieldPotion()
    {
        this.CheckPrice(36, 40, 1, 1, 1, 14, 80, 0, 4000);
    }

    /// <summary>
    /// Tests if the shield potion price is calculated correctly.
    /// </summary>
    [Test]
    public void LargeShieldPotion()
    {
        this.CheckPrice(37, 40, 1, 1, 1, 14, 100, 0, 6000);
    }

    /// <summary>
    /// Tests if the bolt price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 100)]
    [TestCase(1, 1400)]
    [TestCase(2, 2200)]
    public void Bolts(byte level, int price)
    {
        this.CheckPrice(7, 0, 255, 1, 1, 4, 0, level, price);
    }

    /// <summary>
    /// Tests if the arrow price is calculated correctly.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 70)]
    [TestCase(1, 1200)]
    [TestCase(2, 2000)]
    public void Arrows(byte level, int price)
    {
        this.CheckPrice(15, 0, 255, 1, 1, 4, 0, level, price);
    }

    /// <summary>
    /// Tests if the price of the fireball scroll is calculated as 300.
    /// </summary>
    [Test]
    public void FireballScroll()
    {
        this.CheckPrice(3, 0, 1, 1, 1, 15, 300, 0, 300);
    }

    /// <summary>
    /// Tests if the price of the powerwave scroll is calculated as 1100.
    /// </summary>
    [Test]
    public void PowerwaveScroll()
    {
        this.CheckPrice(10, 0, 1, 1, 1, 15, 1100, 0, 1100);
    }

    /// <summary>
    /// Tests if the price of the lightning scroll is calculated as 3000.
    /// </summary>
    [Test]
    public void LightningScroll()
    {
        this.CheckPrice(2, 0, 1, 1, 1, 15, 3000, 0, 3000);
    }

    /// <summary>
    /// Tests if the price of the meteorite scroll is calculated as 11000.
    /// </summary>
    [Test]
    public void MeteoriteScroll()
    {
        this.CheckPrice(1, 0, 1, 1, 1, 15, 11000, 0, 11000);
    }

    /// <summary>
    /// Tests if the price of the teleport scroll is calculated as 5000.
    /// </summary>
    [Test]
    public void TeleportScroll()
    {
        this.CheckPrice(5, 0, 1, 1, 1, 15, 5000, 0, 5000);
    }

    /// <summary>
    /// Tests if the price of the ice scroll is calculated as 14000.
    /// </summary>
    [Test]
    public void IceScroll()
    {
        this.CheckPrice(6, 0, 1, 1, 1, 15, 14000, 0, 14000);
    }

    /// <summary>
    /// Tests if the price of the poison scroll is calculated as 17000.
    /// </summary>
    [Test]
    public void PoisonScroll()
    {
        this.CheckPrice(0, 0, 1, 1, 1, 15, 17000, 0, 17000);
    }

    /// <summary>
    /// Tests if the items of a pad set +0+4+Luck is calculated correctly.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// pad helm+0+4+l  480
    /// armor           1400
    /// pants           960
    /// gloves          290
    /// boots           370.
    /// </remarks>
    [TestCase(7, 5, 28, 480, Description="Pad Helm")]
    [TestCase(8, 10, 28, 1400, Description = "Pad Armor")]
    [TestCase(9, 8, 28, 960, Description = "Pad Pants")]
    [TestCase(10, 3, 28, 290, Description = "Pad Gloves")]
    [TestCase(11, 4, 28, 370, Description = "Pad Boots")]
    public void PadSetItem_0_4_Luck(byte group, byte dropLevel, byte maxDurability, long price)
    {
        this.CheckPrice(2, dropLevel, maxDurability, 2, 2, group, 0, 0, price, true, true);
    }

    /// <summary>
    /// Tests if the items of a bone set +2+4+Luck is calculated correctly.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// bone helm+2+4+l     9400
    /// armor               13500
    /// pants               11300
    /// gloves              6200
    /// boots               7700.
    /// </remarks>
    [TestCase(7, 18, 30, 9400, Description = "Bone Helm")]
    [TestCase(8, 22, 30, 13500, Description = "Bone Armor")]
    [TestCase(9, 20, 30, 11300, Description = "Bone Pants")]
    [TestCase(10, 14, 30, 6200, Description = "Bone Gloves")]
    [TestCase(11, 16, 30, 7700, Description = "Bone Boots")]
    public void BoneSetItem_2_4_Luck(byte group, byte dropLevel, byte maxDurability, long price)
    {
        this.CheckPrice(4, dropLevel, maxDurability, 2, 2, group, 0, 2, price, true, true);
    }

    /// <summary>
    /// Tests if the items of a sphinx set +3+4+Luck is calculated correctly.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// sphinx helm+3+4+l   34200
    /// armor            48200
    /// pants            38500
    /// gloves           26500
    /// boots            30200.
    /// </remarks>
    [TestCase(7, 32, 36, 34200, Description = "Sphinx Mask")]
    [TestCase(8, 38, 36, 48200, Description = "Sphinx Armor")]
    [TestCase(9, 34, 36, 38500, Description = "Sphinx Pants")]
    [TestCase(10, 28, 36, 26500, Description = "Sphinx Gloves")]
    [TestCase(11, 30, 36, 30200, Description = "Sphinx Boots")]
    public void SphinxSetItem_3_4_Luck(byte group, byte dropLevel, byte maxDurability, long price)
    {
        this.CheckPrice(7, dropLevel, maxDurability, 2, 2, group, 0, 3, price, true, true);
    }

    /// <summary>
    /// Tests the price calculations of some staffs.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="level">The level.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="width">The width.</param>
    /// <param name="heigth">The heigth.</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// skull+0+4+l     480
    /// angelic+2+4+l        9400
    /// serpent+3+4+l        30200
    /// thunder+3+4+l        59300.
    /// </remarks>
    [TestCase(0, 0, 6, 20, 1, 3, 480, Description = "skull+0+4+l")]
    [TestCase(1, 2, 18, 38, 2, 3, 9400, Description = "angelic+2+4+l")]
    [TestCase(2, 3, 30, 50, 2, 3, 30200, Description = "serpent+3+4+l")]
    [TestCase(3, 3, 42, 60, 2, 4, 59300, Description = "thunder+3+4+l")]
    public void Staffs(byte id, byte level, byte dropLevel, byte maxDurability, byte width, byte heigth, long price)
    {
        this.CheckPrice(id, dropLevel, maxDurability, heigth, width, 5, 0, level, price, true, true);
    }

    /// <summary>
    /// Tests the price calculations of some shields.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="level">The level.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="width">The width.</param>
    /// <param name="heigth">The heigth.</param>
    /// <param name="skill">if set to <c>true</c> [skill].</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// small shield+0+5+l   230
    /// buckler+1+5+s+l      2300
    /// horn+2+5+l       2600
    /// kite+3+5+l       5500
    /// skull+3+5+s+l        18800.
    /// </remarks>
    [TestCase(0, 0, 3, 22, 2, 2, false, 230, Description = "small shield+0+5+l")]
    [TestCase(4, 1, 6, 24, 2, 2, true, 2300, Description = "buckler+1+5+s+l")]
    [TestCase(1, 2, 9, 28, 2, 2, false, 2600, Description = "horn+2+5+l")]
    [TestCase(2, 3, 12, 32, 2, 2, false, 5500, Description = "kite+3+5+l")]
    [TestCase(6, 3, 15, 34, 2, 2, true, 18800, Description = "skull+3+5+s+l")]
    public void Shields(byte id, byte level, byte dropLevel, byte maxDurability, byte width, byte heigth, bool skill, long price)
    {
        this.CheckPrice(id, dropLevel, maxDurability, heigth, width, 6, 0, level, price, true, true, skill);
    }

    /// <summary>
    /// Tests the price calculation of a small shield.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="price">The price.</param>
    [TestCase(0, 110)]
    [TestCase(1, 240)]
    [TestCase(2, 470)]
    [TestCase(3, 820)]
    [TestCase(4, 1300)]
    [TestCase(5, 3000)]
    [TestCase(6, 6900)]
    [TestCase(7, 21400)]
    [TestCase(8, 58100)]
    [TestCase(9, 121900)]
    [TestCase(10, 275300)]
    [TestCase(11, 617000)]
    [TestCase(12, 1324700)]
    [TestCase(13, 2693500)]
    [TestCase(14, 4777500)]
    [TestCase(15, 7726800)]
    public void SmallShield(byte level, long price)
    {
        this.CheckPrice(0, 3, 22, 2, 2, 6, 0, level, price);
    }

    /// <summary>
    /// Tests the price calculations of some swords.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="level">The level.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="maxDurability">The maximum durability.</param>
    /// <param name="width">The width.</param>
    /// <param name="heigth">The heigth.</param>
    /// <param name="skill">if set to <c>true</c> [skill].</param>
    /// <param name="price">The price.</param>
    /// <remarks>
    /// short sword+0+4+l   230
    /// hand axe+1+4+l       610
    /// kris+2+4+l       1600
    /// mace+2+4+l       1900
    /// rapier+2+4+l     2600
    /// double+2+4+s+l       12400
    /// blade+3+4+s+l        86400.
    /// </remarks>
    [TestCase(1, 0, 3, 22, 1, 2, false, 230, Description = "short sword+0+4+l")]
    [TestCase(0, 2, 6, 20, 1, 2, false, 1600, Description = "kris+2+4+l")]
    [TestCase(2, 2, 9, 23, 1, 3, false, 2600, Description = "rapier+2+4+l")]
    [TestCase(5, 3, 36, 39, 1, 3, true, 86400, Description = "blade+3+4+s+l")]
    public void Swords(byte id, byte level, byte dropLevel, byte maxDurability, byte width, byte heigth, bool skill, long price)
    {
        this.CheckPrice(id, dropLevel, maxDurability, heigth, width, 0, 0, level, price, true, true, skill);
    }

    private void CheckPrice(byte id, byte dropLevel, byte maxDurability, byte height, byte width, byte group, int value, byte level, long price, bool luck = false, bool option = false, bool skill = false)
    {
        var itemDefinitionMock = new Mock<ItemDefinition>();
        itemDefinitionMock.SetupAllProperties();
        itemDefinitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());

        var itemDefinition = itemDefinitionMock.Object;

        itemDefinition.DropLevel = dropLevel;
        itemDefinition.Durability = maxDurability;
        itemDefinition.Height = height;
        itemDefinition.Width = width;
        itemDefinition.Group = group;
        itemDefinition.Value = value;
        itemDefinition.Number = id;
        if (group <= 11)
        {
            itemDefinition.ItemSlot = new ItemSlotType();
        }

        if (group < 6)
        {
            // weapons should have an attack speed attribute
            itemDefinition.BasePowerUpAttributes.Add(new ItemBasePowerUpDefinition { TargetAttribute = Stats.AttackSpeedByWeapon });
        }

        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = itemDefinition;
        item.Level = level;
        item.Durability = Math.Max(item.GetMaximumDurabilityOfOnePiece(), maxDurability);

        if (luck)
        {
            var optionLink = new ItemOptionLink
            {
                ItemOption = new IncreasableItemOption
                {
                    OptionType = ItemOptionTypes.Luck,
                },
            };
            item.ItemOptions.Add(optionLink);
        }

        if (option)
        {
            var optionLink = new ItemOptionLink
            {
                ItemOption = new IncreasableItemOption
                {
                    OptionType = ItemOptionTypes.Option,
                },
                Level = 1,
            };
            item.ItemOptions.Add(optionLink);
        }

        if (skill)
        {
            item.HasSkill = true;
        }

        var buyingPrice = this._calculator.CalculateFinalBuyingPrice(item);
        Assert.That(buyingPrice, Is.EqualTo(price));
    }
}