// <copyright file="ItemSerializerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.InMemory;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="ItemSerializer"/>.
    /// </summary>
    [TestFixture]
    public class ItemSerializerTests
    {
        private GameConfiguration gameConfiguration;
        private IPersistenceContextProvider contextProvider;
        private IItemSerializer itemSerializer;

        /// <summary>
        /// Sets up the test environment by initializing configuration data and a <see cref="IPersistenceContextProvider"/>.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            this.contextProvider = new InMemoryPersistenceContextProvider();
            new MUnique.OpenMU.Persistence.Initialization.DataInitialization(this.contextProvider).CreateInitialData();
            this.gameConfiguration = this.contextProvider.CreateNewConfigurationContext().Get<GameConfiguration>().First();
            this.itemSerializer = new ItemSerializer();
        }

        /// <summary>
        /// Tests if <see cref="Item.Definition"/> is correctly (de)serialized.
        /// </summary>
        [Test]
        public void Definition()
        {
            var tuple = this.SerializeAndDeserializeBlade();
            var item = tuple.Item1;
            var deserializedItem = tuple.Item2;
            Assert.That(deserializedItem.Definition, Is.EqualTo(item.Definition));
        }

        /// <summary>
        /// Tests if <see cref="Item.Level"/> is correctly (de)serialized.
        /// </summary>
        [Test]
        public void Level()
        {
            var tuple = this.SerializeAndDeserializeBlade();
            var item = tuple.Item1;
            var deserializedItem = tuple.Item2;
            Assert.That(deserializedItem.Level, Is.EqualTo(item.Level));
        }

        /// <summary>
        /// Tests if <see cref="Item.Durability"/> is correctly (de)serialized.
        /// </summary>
        [Test]
        public void Durability()
        {
            var tuple = this.SerializeAndDeserializeBlade();
            var item = tuple.Item1;
            var deserializedItem = tuple.Item2;
            Assert.That(deserializedItem.Durability, Is.EqualTo(item.Durability));
        }

        /// <summary>
        /// Tests if <see cref="Item.HasSkill" /> is correctly (de)serialized.
        /// </summary>
        /// <param name="hasSkill">If set to <c>true</c>, the tested item has skill.</param>
        [TestCase(true)]
        [TestCase(false)]
        public void Skill(bool hasSkill)
        {
            var tuple = this.SerializeAndDeserializeBlade();
            var item = tuple.Item1;
            var deserializedItem = tuple.Item2;
            Assert.That(deserializedItem.HasSkill, Is.EqualTo(item.HasSkill));
        }

        /// <summary>
        /// Tests if <see cref="Item.ItemOptions"/> are correctly (de)serialized.
        /// </summary>
        /// <remarks>
        /// This test could be done in more detail, for each item option type.
        /// </remarks>
        [Test]
        public void Options()
        {
            var tuple = this.SerializeAndDeserializeBlade();
            var item = tuple.Item1;
            var deserializedItem = tuple.Item2;
            Assert.That(deserializedItem.ItemOptions.Count, Is.EqualTo(item.ItemOptions.Count));
            foreach (var optionLink in item.ItemOptions)
            {
                var deserializedOptionLink = deserializedItem.ItemOptions
                    .FirstOrDefault(link => link.Level == optionLink.Level
                                   && link.ItemOption.OptionType == optionLink.ItemOption.OptionType
                                   && link.ItemOption.Number == optionLink.ItemOption.Number);
                Assert.That(deserializedOptionLink, Is.Not.Null, () => $"Option Link not found: {optionLink.ItemOption.OptionType.Name}, {optionLink.ItemOption.PowerUpDefinition}, Level: {optionLink.Level}");
            }
        }

        private Tuple<Item, Item> SerializeAndDeserializeBlade(bool hasSkill = true)
        {
            using (var context = this.contextProvider.CreateNewContext(this.gameConfiguration))
            {
                var item = context.CreateNew<Item>();
                item.Definition = this.gameConfiguration.Items.First(i => i.Name == "Blade");
                item.Level = 15;
                item.Durability = 23;
                item.HasSkill = hasSkill;
                var option = context.CreateNew<ItemOptionLink>();
                option.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
                    def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Option)).First();
                option.Level = 2;
                item.ItemOptions.Add(option);

                var luck = context.CreateNew<ItemOptionLink>();
                luck.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
                    def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Luck)).First();
                item.ItemOptions.Add(luck);

                var excellent1 = context.CreateNew<ItemOptionLink>();
                excellent1.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
                    def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Excellent && p.PowerUpDefinition.TargetAttribute == Stats.ExcellentDamageChance)).First();
                item.ItemOptions.Add(excellent1);
                var excellent2 = context.CreateNew<ItemOptionLink>();
                excellent2.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
                    def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Excellent && p.PowerUpDefinition.TargetAttribute == Stats.AttackSpeed)).First();
                item.ItemOptions.Add(excellent2);

                var array = new byte[this.itemSerializer.NeededSpace];
                this.itemSerializer.SerializeItem(array, item);

                var deserializedItem = this.itemSerializer.DeserializeItem(array, this.gameConfiguration, context);
                return new Tuple<Item, Item>(item, deserializedItem);
            }
        }
    }
}