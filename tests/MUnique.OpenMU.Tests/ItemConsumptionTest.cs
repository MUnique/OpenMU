// <copyright file="ItemConsumptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
    using MUnique.OpenMU.Persistence;
    using NUnit.Framework;
    using Rhino.Mocks;

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
        public void JewelOfBless()
        {
            Assert.That(true, Is.False);
        }

        /// <summary>
        /// Tests the jewel of soul consume.
        /// </summary>
        public void JewelOfSoul()
        {
            Assert.That(true, Is.False);
        }

        /// <summary>
        /// Tests the jewel of life consumption.
        /// </summary>
        /// <param name="numberOfOptions">The number of options.</param>
        /// <param name="expectation">If set to <c>true</c>, the item upgrade is expected; Otherwise, not.</param>
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        public void JewelOfLife(int numberOfOptions, bool expectation)
        {
            var repositoryManager = new BaseRepositoryManager();
            var consumeHandler = new LifeJewelConsumeHandler(repositoryManager);
            consumeHandler.Configuration.SuccessChance = 1;
            var player = this.GetPlayer();
            var upgradeableItem = this.GetItemWithPossibleOption();
            var upgradableItemSlot = (byte)(ItemSlot + 1);
            player.Inventory.AddItem(upgradableItemSlot, upgradeableItem);
            bool success = false;
            for (int i = 0; i < numberOfOptions; i++)
            {
                var item = this.GetItem();
                player.Inventory.AddItem(ItemSlot, item);
                item.Durability = 1;

                success = consumeHandler.ConsumeItem(player, ItemSlot, upgradableItemSlot);
            }

            Assert.That(success, Is.EqualTo(expectation));
            if (success)
            {
                Assert.That(upgradeableItem.ItemOptions.Count, Is.EqualTo(numberOfOptions));
            }
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
            Assert.That(true, Is.False);
        }

        /// <summary>
        /// Tests the complex potion consume.
        /// </summary>
        public void ComplexPotion()
        {
            Assert.That(true, Is.False);
        }

        /// <summary>
        /// Tests the shielt potion consume.
        /// </summary>
        public void ShieldPotion()
        {
            Assert.That(true, Is.False);
        }

        /// <summary>
        /// Tests if the consume fails because of the player state.
        /// </summary>
        [Test]
        public void FailByWrongPlayerState()
        {
            var consumeHandler = new BaseConsumeHandler();
            var player = this.GetPlayer();
            player.PlayerState.TryAdvanceTo(PlayerState.TradeRequested);
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);
            Assert.That(success, Is.False);
        }

        /// <summary>
        /// Tests if the consume of the item decreases its durability by one.
        /// </summary>
        [Test]
        public void ItemDurabilityDecrease()
        {
            var consumeHandler = new BaseConsumeHandler();
            var player = this.GetPlayer();
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            item.Durability = 3;
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);
            Assert.That(success, Is.True);
            Assert.That(item.Durability, Is.EqualTo(2));
            Assert.That(player.Inventory.Items.Any(), Is.True);
        }

        /// <summary>
        /// Tests if the consume of the item causes the removal of the item, when the durability reaches 0.
        /// </summary>
        [Test]
        public void ItemRemoval()
        {
            var consumeHandler = new BaseConsumeHandler();
            var player = this.GetPlayer();
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);
            Assert.That(success, Is.True);
            Assert.That(item.Durability, Is.EqualTo(0));
            Assert.That(player.Inventory.Items.Any(), Is.False);
        }

        /// <summary>
        /// Tests if the consume of the alcohol fails when the item is not in the inventory.
        /// </summary>
        [Test]
        public void DrinkAlcoholFail()
        {
            var consumeHandler = new AlcoholConsumeHandler();
            var player = this.GetPlayer();
            player.PlayerView.Expect(view => view.DrinkAlcohol()).Repeat.Never();
            var success = consumeHandler.ConsumeItem(player, 12, 0);

            Assert.That(success, Is.False);
            player.PlayerView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if the consume of alcohol works and is forwarded to the player view.
        /// </summary>
        [Test]
        public void DrinkAlcoholSuccess()
        {
            var consumeHandler = new AlcoholConsumeHandler();
            var player = this.GetPlayer();
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            player.PlayerView.Expect(view => view.DrinkAlcohol()).Repeat.Once();
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);

            Assert.That(success, Is.True);
            Assert.That(player.Inventory.Items.Any(), Is.False);
            player.PlayerView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests the health recover by drinking a health potion.
        /// </summary>
        [Test]
        public void HealthRecover()
        {
            var player = this.GetPlayer();
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            var consumeHandler = new BigHealthPotionConsumeHandler();
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);
            Assert.That(success, Is.True);
            Assert.That(player.Attributes.GetValueOfAttribute(Stats.CurrentHealth), Is.GreaterThan(0.0f));
        }

        /// <summary>
        /// Tests the mana recover by drinking a mana potion.
        /// </summary>
        [Test]
        public void ManaRecover()
        {
            var player = this.GetPlayer();
            var item = this.GetItem();
            player.Inventory.AddItem(ItemSlot, item);
            var consumeHandler = new BigManaPotionConsumeHandler();
            var success = consumeHandler.ConsumeItem(player, ItemSlot, 0);
            Assert.That(success, Is.True);
            Assert.That(player.Attributes.GetValueOfAttribute(Stats.CurrentMana), Is.GreaterThan(0.0f));
        }

        private Item GetItem()
        {
            return new Item()
            {
                Definition = new DataModel.Configuration.Items.ItemDefinition() { Width = 1, Height = 1 },
                Durability = 1
            };
        }

        private Player GetPlayer()
        {
            var player = TestHelper.GetPlayer();

            player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.Level, 100));
            player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.CurrentHealth, 0));
            player.SelectedCharacter.Attributes.Add(new StatAttribute(Stats.CurrentMana, 0));

            return player;
        }

        private Item GetItemWithPossibleOption()
        {
            var item = MockRepository.GenerateStub<Item>();
            item.Stub(i => i.ItemOptions).Return(new List<ItemOptionLink>());
            item.Definition = MockRepository.GenerateStub<ItemDefinition>();
            item.Definition.Stub(d => d.PossibleItemOptions).Return(new List<ItemOptionDefinition>());
            item.Definition.Stub(d => d.BasePowerUpAttributes).Return(new List<ItemBasePowerUpDefinition>());
            item.Durability = 1;
            item.Definition.Width = 1;
            item.Definition.Height = 2;
            var option = MockRepository.GenerateStub<ItemOptionDefinition>();
            option.Stub(o => o.PossibleOptions).Return(new List<IncreasableItemOption>());
            option.MaximumOptionsPerItem = 4;
            option.AddsRandomly = true;
            option.Name = "Damage Option";

            var possibleOption = MockRepository.GenerateStub<IncreasableItemOption>();
            possibleOption.OptionType = ItemOptionTypes.Option;
            possibleOption.Stub(o => o.LevelDependentOptions).Return(new List<ItemOptionOfLevel>());
            option.PossibleOptions.Add(possibleOption);
            item.Definition.PossibleItemOptions.Add(option);
            return item;
        }
    }
}
