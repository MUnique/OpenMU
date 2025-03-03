// <copyright file="ItemSerializerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Unit tests for the <see cref="ItemSerializer"/>.
/// </summary>
[TestFixture]
public class ItemSerializerTests : ItemSerializerTests<ItemSerializer>;

/// <summary>
/// Unit tests for the <see cref="ItemSerializerExtended"/>.
/// </summary>
[TestFixture]
public class ItemSerializerExtendedTests : ItemSerializerTests<ItemSerializerExtended>;

/// <summary>
/// Generic unit tests for the <see cref="IItemSerializer"/>s.
/// </summary>
[Ignore("Generic test")]
public class ItemSerializerTests<T>
    where T: IItemSerializer, new()
{
    private GameConfiguration _gameConfiguration = null!;
    private IPersistenceContextProvider _contextProvider = null!;
    private IItemSerializer _itemSerializer = null!;

    /// <summary>
    /// Sets up the test environment by initializing configuration data and a <see cref="IPersistenceContextProvider"/>.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        this._contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(this._contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(3, true).ConfigureAwait(false);
        this._gameConfiguration = (await this._contextProvider.CreateNewConfigurationContext().GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        this._itemSerializer = new T();
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
                                        && link.ItemOption!.OptionType == optionLink.ItemOption!.OptionType
                                        && link.ItemOption.Number == optionLink.ItemOption.Number);
            Assert.That(deserializedOptionLink, Is.Not.Null, () => $"Option Link not found: {optionLink.ItemOption!.OptionType!.Name}, {optionLink.ItemOption.PowerUpDefinition}, Level: {optionLink.Level}");
        }
    }

    /// <summary>
    /// Tests if ancient items are correctly (de)serialized.
    /// </summary>
    [Test]
    public void Ancient()
    {
        var tuple = this.SerializeAndDeserializeHyonLightingSword();
        var item = tuple.Item1;
        var deserializedItem = tuple.Item2;
        Assert.That(deserializedItem.ItemOptions.Count, Is.EqualTo(item.ItemOptions.Count));
        foreach (var optionLink in item.ItemOptions)
        {
            var deserializedOptionLink = deserializedItem.ItemOptions
                .FirstOrDefault(link => link.Level == optionLink.Level
                                        && link.ItemOption!.OptionType == optionLink.ItemOption!.OptionType
                                        && link.ItemOption.Number == optionLink.ItemOption.Number);
            Assert.That(deserializedOptionLink, Is.Not.Null, () => $"Option Link not found: {optionLink.ItemOption!.OptionType!.Name}, {optionLink.ItemOption.PowerUpDefinition}, Level: {optionLink.Level}");
        }

        Assert.That(deserializedItem.ItemSetGroups.Count, Is.EqualTo(item.ItemSetGroups.Count));
        foreach (var setGroup in item.ItemSetGroups)
        {
            Assert.That(deserializedItem.ItemSetGroups, Contains.Item(setGroup));
        }
    }

    /// <summary>
    /// Tests if ancient items without bonus option are correctly (de)serialized.
    /// </summary>
    [Test]
    public void AncientWithoutBonus()
    {
        var tuple = this.SerializeAndDeserializeGywenPendant();
        var item = tuple.Item1;
        var deserializedItem = tuple.Item2;
        Assert.That(deserializedItem.ItemOptions.Count, Is.EqualTo(item.ItemOptions.Count));
        foreach (var optionLink in item.ItemOptions)
        {
            var deserializedOptionLink = deserializedItem.ItemOptions
                .FirstOrDefault(link => link.Level == optionLink.Level
                                        && link.ItemOption!.OptionType == optionLink.ItemOption!.OptionType
                                        && link.ItemOption.Number == optionLink.ItemOption.Number);
            Assert.That(deserializedOptionLink, Is.Not.Null, () => $"Option Link not found: {optionLink.ItemOption!.OptionType!.Name}, {optionLink.ItemOption.PowerUpDefinition}, Level: {optionLink.Level}");
        }

        Assert.That(deserializedItem.ItemSetGroups.Count, Is.EqualTo(item.ItemSetGroups.Count));
        foreach (var setGroup in item.ItemSetGroups)
        {
            Assert.That(deserializedItem.ItemSetGroups, Contains.Item(setGroup));
        }
    }

    /// <summary>
    /// Tests if socket items are correctly (de)serialized.
    /// </summary>
    [Test]
    public void Sockets()
    {
        var tuple = this.SerializeAndDeserializeBraveHelm();
        var item = tuple.Item1;
        var deserializedItem = tuple.Item2;
        Assert.That(deserializedItem.ItemOptions.Count, Is.EqualTo(item.ItemOptions.Count));
        foreach (var optionLink in item.ItemOptions)
        {
            var deserializedOptionLink = deserializedItem.ItemOptions
                .FirstOrDefault(link => link.Level == optionLink.Level
                                        && link.ItemOption!.OptionType == optionLink.ItemOption!.OptionType
                                        && link.ItemOption.Number == optionLink.ItemOption.Number);
            Assert.That(deserializedOptionLink, Is.Not.Null, () => $"Option Link not found: {optionLink.ItemOption!.OptionType!.Name}, {optionLink.ItemOption.PowerUpDefinition}, Level: {optionLink.Level}");
        }

        Assert.That(deserializedItem.SocketCount, Is.EqualTo(item.SocketCount));
    }

    private Tuple<Item, Item> SerializeAndDeserializeBraveHelm()
    {
        using var context = this._contextProvider.CreateNewContext(this._gameConfiguration);
        var item = context.CreateNew<Item>();
        item.Definition = this._gameConfiguration.Items.First(i => i.Group == 7 && i.Number == 46);
        item.Level = 3;
        item.Durability = 100;
        item.SocketCount = 2;

        var option = context.CreateNew<ItemOptionLink>();
        option.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
            def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Option)).First();
        option.Level = 4;
        item.ItemOptions.Add(option);


        for (var i = 0; i < item.SocketCount; i++)
        {
            var socketOption = context.CreateNew<ItemOptionLink>();
            socketOption.ItemOption = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.SocketOption)
                .Skip(i)
                .First();
            socketOption.Index = i;
            socketOption.Level = 1;
            item.ItemOptions.Add(socketOption);
        }

        var bonusOption = context.CreateNew<ItemOptionLink>();
        bonusOption.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions).First(o => o.OptionType == ItemOptionTypes.SocketBonusOption);
        item.ItemOptions.Add(bonusOption);

        var array = new byte[this._itemSerializer.NeededSpace];
        this._itemSerializer.SerializeItem(array, item);

        var deserializedItem = this._itemSerializer.DeserializeItem(array, this._gameConfiguration, context);
        return new Tuple<Item, Item>(item, deserializedItem);
    }

    private Tuple<Item, Item> SerializeAndDeserializeHyonLightingSword()
    {
        using var context = this._contextProvider.CreateNewContext(this._gameConfiguration);
        var item = context.CreateNew<Item>();
        item.Definition = this._gameConfiguration.Items.First(i => i.Name == "Lighting Sword");
        item.Level = 15;
        item.Durability = 100;
        item.HasSkill = true;

        var ancientSet = this._gameConfiguration.ItemSetGroups.First(i => i.Name == "Hyon");
        var itemOfSet = ancientSet.Items.First(i => i.ItemDefinition == item.Definition);
        var ancientBonus = context.CreateNew<ItemOptionLink>();
        ancientBonus.ItemOption = itemOfSet.BonusOption;
        ancientBonus.Level = 2; // 10 Str
        item.ItemOptions.Add(ancientBonus);
        item.ItemSetGroups.Add(itemOfSet);

        var array = new byte[this._itemSerializer.NeededSpace];
        this._itemSerializer.SerializeItem(array, item);

        var deserializedItem = this._itemSerializer.DeserializeItem(array, this._gameConfiguration, context);
        return new Tuple<Item, Item>(item, deserializedItem);
    }

    private Tuple<Item, Item> SerializeAndDeserializeGywenPendant()
    {
        using var context = this._contextProvider.CreateNewContext(this._gameConfiguration);
        var item = context.CreateNew<Item>();
        item.Definition = this._gameConfiguration.Items.First(i => i.Name == "Pendant of Ability");
        item.Durability = 10;

        var ancientSet = this._gameConfiguration.ItemSetGroups.First(i => i.Name == "Gywen");
        var itemOfSet = ancientSet.Items.First(i => i.ItemDefinition == item.Definition);
        item.ItemSetGroups.Add(itemOfSet);

        var array = new byte[this._itemSerializer.NeededSpace];
        this._itemSerializer.SerializeItem(array, item);

        var deserializedItem = this._itemSerializer.DeserializeItem(array, this._gameConfiguration, context);
        return new Tuple<Item, Item>(item, deserializedItem);
    }

    private Tuple<Item, Item> SerializeAndDeserializeBlade(bool hasSkill = true)
    {
        using var context = this._contextProvider.CreateNewContext(this._gameConfiguration);
        var item = context.CreateNew<Item>();
        item.Definition = this._gameConfiguration.Items.First(i => i.Name == "Blade");
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
            def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Excellent && p.PowerUpDefinition!.TargetAttribute == Stats.ExcellentDamageChance)).First();
        item.ItemOptions.Add(excellent1);
        var excellent2 = context.CreateNew<ItemOptionLink>();
        excellent2.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def =>
            def.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.Excellent && p.PowerUpDefinition!.TargetAttribute == Stats.AttackSpeedAny)).First();
        item.ItemOptions.Add(excellent2);

        var array = new byte[this._itemSerializer.NeededSpace];
        this._itemSerializer.SerializeItem(array, item);

        var deserializedItem = this._itemSerializer.DeserializeItem(array, this._gameConfiguration, context);
        return new Tuple<Item, Item>(item, deserializedItem);
    }
}