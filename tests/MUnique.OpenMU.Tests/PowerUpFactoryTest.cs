// <copyright file="PowerUpFactoryTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using NUnit.Framework;

    /// <summary>
    /// Tests the power up factory which is creating powerups based on the items a player has equipped.
    /// </summary>
    [TestFixture]
    public class PowerUpFactoryTest
    {
        private const byte UnwearableSlot = 0xFF;

        private const int PowerUpStrength = 16;

        private readonly float[] levelBonus = new float[] { 0, 1, 3, 7, 14 };

        /// <summary>
        /// Tests if the item option results in an corresponding powerup.
        /// </summary>
        [Test]
        public void ItemOptions()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItemWithOption();
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength));
        }

        /// <summary>
        /// Tests if the item option of level 0 results in the corresponding power up.
        /// </summary>
        [Test]
        public void ItemBasePowerUpLevel0()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItemWithBasePowerUp();
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength));
        }

        /// <summary>
        /// Tests if the item option of level 3 results in the corresponding power up.
        /// </summary>
        [Test]
        public void ItemBasePowerUpLevel3()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItemWithBasePowerUp();
            item.Level = 3;
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(PowerUpStrength + this.levelBonus[3]));
        }

        /// <summary>
        /// Tests if the powerups don't get created if the item has no more durability.
        /// </summary>
        [Test]
        public void NoPowerUpsWhenItemBroken()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItemWithBasePowerUp();
            item.Durability = 0;
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
        }

        /// <summary>
        /// Tests if the powerups don't get created if the item is not equipped at the player.
        /// </summary>
        [Test]
        public void NoPowerUpsWhenItemUnwearable()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItemWithBasePowerUp();
            item.ItemSlot = UnwearableSlot;
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
        }

        /// <summary>
        /// Tests if the powerups don't get created when the item has no options.
        /// </summary>
        [Test]
        public void NoPowerUpsInItem()
        {
            var player = TestHelper.GetPlayer();
            var factory = this.GetPowerUpFactory();
            var item = this.GetItem();
            var result = factory.GetPowerUps(item, player.Attributes);
            Assert.That(result.Sum(p => p.Value), Is.EqualTo(0));
        }

        private IItemPowerUpFactory GetPowerUpFactory()
        {
            return new ItemPowerUpFactory();
        }

        private ItemDefinition GetItemDefintion()
        {
            var itemDefinition = new Mock<ItemDefinition>();
            itemDefinition.Object.Durability = 100;
            itemDefinition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
            return itemDefinition.Object;
        }

        private Item GetItem()
        {
            var item = new Mock<Item>();
            item.SetupAllProperties();
            item.Object.Definition = this.GetItemDefintion();
            item.Object.ItemSlot = 0;
            item.Object.Durability = item.Object.Definition.Durability;
            item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
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
            item.Definition.BasePowerUpAttributes.Add(this.GetBasePowerUpDefinition());
            return item;
        }

        private ItemBasePowerUpDefinition GetBasePowerUpDefinition()
        {
            var resultMock = new Mock<ItemBasePowerUpDefinition>();
            resultMock.SetupAllProperties();
            resultMock.Setup(r => r.BonusPerLevel).Returns(new List<LevelBonus>());
            var result = resultMock.Object;
            result.BaseValueElement = new ConstantElement(PowerUpStrength);
            result.TargetAttribute = Stats.MaximumPhysBaseDmg;
            result.BonusPerLevel.Add(new LevelBonus(1, this.levelBonus[1]));
            result.BonusPerLevel.Add(new LevelBonus(2, this.levelBonus[2]));
            result.BonusPerLevel.Add(new LevelBonus(3, this.levelBonus[3]));
            result.BonusPerLevel.Add(new LevelBonus(4, this.levelBonus[4]));
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

        // TODO: Sets / Dual wield
        // There is a attributesystem to be defined for the outcommented tests.
        /*
        [Test]
        public void SetBonusDefSetComplete()
        {
            var factory = GetPowerUpFactory();
            var items = GetSet(true, 10, 15, 10);
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SetBonusDefSetIncomplete()
        {
            var factory = GetPowerUpFactory();
            var items = GetSet(true, 10, 15, 9);
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SetBonusDefSetIncomplete2()
        {
            var factory = GetPowerUpFactory();
            var items = Enumerable.Empty<Item>();
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SetBonusDefSetIncomplete3()
        {
            var factory = GetPowerUpFactory();
            var items = GetSet(false, 15, 15);
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SetBonusDefSetValueCorrect1()
        {
            var factory = GetPowerUpFactory();
            var items = GetSet(true, 10, 15, 10);
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Value, Is.EqualTo(5.0));
        }

        [Test]
        public void SetBonusDefSetValueCorrect2()
        {
            var factory = GetPowerUpFactory();
            var items = GetSet(true, 15, 15, 15);
            var result = factory.GetSetPowerUps(items,);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Value, Is.EqualTo(30.0));
        }

        [Test]
        public void DualWieldBonus()
        {
            var factory = GetPowerUpFactory();
            var result = factory.GetSetPowerUps(new[]{this.GetItemWithDamage(), GetItemWithDamage()})
                .Where(powerUp => powerUp.Stage == Stage.Always)
                .Where(powerUp => powerUp.PowerUpType == PowerUpProperty.BaseDmg)
                .Where(powerUp => powerUp.DamageType == DamageType.Physical)
                .Where(powerUp => powerUp.AddStrategy == AddStrategy.Relative);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Value, Is.EqualTo(10));
        }

        private IEnumerable<Item> GetSet(bool complete, params byte[] levels)
        {
            var armorSet = new Mock<IArmorSet>();
            if (!complete)
                armorSet.Setup(a => a.NumberOfPieces).Returns(levels.Length + 1);
            else
                armorSet.Setup(a => a.NumberOfPieces).Returns(levels.Length);
            foreach(var level in levels)
            {
                var item = GetItem();
                item.Definition.Set = armorSet;
                item.Level = level;
                yield return item;
            }
        }
        /*
        /// <summary>
        /// Returns an ancient group which contains 3 different items with 5 options.
        /// </summary>
        /// <returns>the ancient group.</returns>
        private IAncientGroup GetAncientGroup()
        {
            var result = new Mock<IAncientGroup>();
            result.Setup(link => link.Items).Returns(new List<IAncientOptionLink>());
            for(int i=0;i<3;i++)
            {
                var optionLink = new Mock<IAncientOptionLink>();
                optionLink.Setup(o => o.ItemDefinition).Returns(GetItemDefintion());
                optionLink.Setup(o => o.PowerUp).Returns(new Mock<IPowerUp>()); //this is the +5/10 vit/str etc bonus
                result.Items.Add(optionLink);
            }

            //full set got 5 options:
            result.Setup(link => link.Options).Returns(new List<IPowerUp>());
            for (int i = 0; i < 5; i++)
            {
                result.Options.Add(new Mock<IPowerUp>());
            }

            return result;
        }*/
        /*
        [Test]
        public void NoAncientSet()
        {
            var factory = GetPowerUpFactory();
            var items = new List<Item>();
            items.Add(GetItem());
            items.Add(GetItem());
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void AncientSetIncomplete()
        {
            var factory = GetPowerUpFactory();
            var ancientGroup = GetAncientGroup();
            var items = new List<Item>();
            foreach (var ancientLink in ancientGroup.Items.Skip(1))
            {
                var item = GetItem();
                item.AncientGroup = ancientGroup;
                item.Setup(i => i.Definition).Returns(ancientLink.ItemDefinition);
                items.Add(item);
            }
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(items.Count - 1));
        }

        [Test]
        public void AncientSetComplete()
        {
            var factory = GetPowerUpFactory();
            var ancientGroup = GetAncientGroup();
            var items = new List<Item>();
            foreach (var ancientLink in ancientGroup.Items)
            {
                var item = GetItem();
                item.AncientGroup = ancientGroup;
                item.Setup(i => i.Definition).Returns(ancientLink.ItemDefinition);
                items.Add(item);
            }
            var result = factory.GetSetPowerUps(items);
            Assert.That(result.Count(), Is.EqualTo(ancientGroup.Options.Count));
        }*/

        private class TestPowerUpDefinitionValue : PowerUpDefinitionValue
        {
            public TestPowerUpDefinitionValue(SimpleElement constantValue)
            {
                this.ConstantValue = constantValue;
            }
        }
    }
}
