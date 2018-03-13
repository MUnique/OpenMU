// <copyright file="DropGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests the drop generator.
    /// </summary>
    [TestFixture]
    public class DropGeneratorTest
    {
        /// <summary>
        /// Tests if the drop fails because the randomizer returns a number which causes a fail.
        /// </summary>
        [Test]
        public void TestDropFail()
        {
            var config = this.GetGameConfig();
            var generator = new DefaultDropGenerator(config, this.GetRandomizer(9999));
            var item = generator.GetItemDropsOrAddMoney(this.GetMonster(1), 0, TestHelper.GetPlayer()).FirstOrDefault();
            Assert.That(item, Is.Null);
        }

        /// <summary>
        /// Tests if the drop of the item succeeds.
        /// </summary>
        [Test]
        public void TestBaseItemDropItem()
        {
            var config = this.GetGameConfig();
            var generator = new DefaultDropGenerator(config, this.GetRandomizer(0));
            var item = generator.GetItemDropsOrAddMoney(this.GetMonster(1), 0, TestHelper.GetPlayer()).FirstOrDefault();

            var possibleItemDefinition = config.BaseDropItemGroups.First().PossibleItems.First();
            Assert.That(item, Is.Not.Null);

            // ReSharper disable once PossibleNullReferenceException
            Assert.That(item.Definition, Is.SameAs(possibleItemDefinition));
        }

        /// <summary>
        /// Tests if the drop of money succeeds.
        /// </summary>
        [Test]
        public void TestItemDropMoney()
        {
            const int startMoney = 10000;
            var experience = 1;
            var moneyDrop = experience + DefaultDropGenerator.BaseMoneyDrop;

            var player = TestHelper.GetPlayer();
            player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
            player.Money = startMoney;
            var config = this.GetGameConfig();
            var generator = new DefaultDropGenerator(config, this.GetRandomizer(4000));
            var item = generator.GetItemDropsOrAddMoney(this.GetMonster(1), experience, player).FirstOrDefault();

            Assert.That(player.Money, Is.EqualTo(startMoney + moneyDrop));
            Assert.That(item, Is.Null);
        }

        /// <summary>
        /// Tests the drops defined by a monster are getting considered.
        /// </summary>
        [Test]
        public void TestItemDropItemByMonster()
        {
            var config = this.GetGameConfig();
            var monster = this.GetMonster(1);
            monster.DropItemGroups.Add(this.GetDropItemGroup(3000, SpecialItemType.Ancient, true));
            var generator = new DefaultDropGenerator(config, this.GetRandomizer2(0, 0.5));
            var item = generator.GetItemDropsOrAddMoney(monster, 1, TestHelper.GetPlayer()).FirstOrDefault();

            Assert.That(item, Is.Not.Null);

            // ReSharper disable once PossibleNullReferenceException
            Assert.That(item.Definition, Is.SameAs(monster.DropItemGroups.First().PossibleItems.First()));
        }

        /// <summary>
        /// Tests the drops defined by a player are getting considered.
        /// </summary>
        public void TestItemDropItemByPlayer()
        {
            // to be implemented
        }

        /// <summary>
        /// Tests the drops defined by a map are getting considered.
        /// </summary>
        public void TestItemDropItemByMap()
        {
            // to be implemented
        }

        private MonsterDefinition GetMonster(int numberOfDrops)
        {
            var monster = MockRepository.GenerateStub<MonsterDefinition>();
            monster.Stub(m => m.DropItemGroups).Return(new List<DropItemGroup>());

            monster.NumberOfMaximumItemDrops = numberOfDrops;
            monster.Stub(m => m.Attributes).Return(new List<MonsterAttribute>());
            monster.Attributes.Add(new MonsterAttribute { AttributeDefinition = Stats.Level, Value = 0 });
            return monster;
        }

        private IRandomizer GetRandomizer(int randomValue)
        {
            var randomizer = MockRepository.GenerateMock<IRandomizer>();
            randomizer.Stub(r => r.NextInt(0, 0)).IgnoreArguments().Return(randomValue).Repeat.Any();
            randomizer.Stub(r => r.NextDouble()).Return(randomValue / 10000.0).Repeat.Any();
            return randomizer;
        }

        private IRandomizer GetRandomizer2(int integerValue, double doubleValue)
        {
            var randomizer = MockRepository.GenerateMock<IRandomizer>();
            randomizer.Stub(r => r.NextInt(0, 0)).IgnoreArguments().Return(integerValue);
            randomizer.Stub(r => r.NextDouble()).Return(doubleValue);

            return randomizer;
        }

        private GameConfiguration GetGameConfig()
        {
            var gameConfiguration = MockRepository.GenerateStub<GameConfiguration>();
            var itemGroups = new List<DropItemGroup>
            {
                this.GetDropItemGroup(1, SpecialItemType.RandomItem, true),
                this.GetDropItemGroup(1000, SpecialItemType.Excellent, true),
                this.GetDropItemGroup(3000, SpecialItemType.Money, true)
            };
            gameConfiguration.Stub(c => c.BaseDropItemGroups).Return(itemGroups);
            gameConfiguration.Stub(c => c.Items)
                .Return(gameConfiguration.BaseDropItemGroups.SelectMany(g => g.PossibleItems).ToList());
            return gameConfiguration;
        }

        private DropItemGroup GetDropItemGroup(int chance, SpecialItemType itemType, bool addItem)
        {
            var dropItemGroup = MockRepository.GenerateStub<DropItemGroup>();
            dropItemGroup.Chance = chance / 10000.0;
            dropItemGroup.ItemType = itemType;
            var itemList = new List<ItemDefinition>();
            dropItemGroup.Stub(g => g.PossibleItems).Return(itemList);
            if (addItem)
            {
                var itemDefinition = MockRepository.GenerateStub<ItemDefinition>();
                itemDefinition.DropsFromMonsters = true;
                itemDefinition.Stub(d => d.PossibleItemSetGroups).Return(new List<ItemSetGroup>());
                itemDefinition.Stub(d => d.PossibleItemOptions).Return(new List<ItemOptionDefinition>());
                itemList.Add(itemDefinition);
            }

            return dropItemGroup;
        }
    }
}
