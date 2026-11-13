// <copyright file="MerchantShopExportImportServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Text.Json;
using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="MerchantShopExportImportService"/>.
/// </summary>
[TestFixture]
public class MerchantShopExportImportServiceTests
{
    private readonly MerchantShopExportImportService _service = new();
    private readonly List<BasicModel.ItemDefinition> _itemDefinitions = new();

    private Mock<IContext> _persistenceContext = null!;

    /// <summary>
    /// Sets up the persistence context and the item definitions which the import can resolve.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._itemDefinitions.Clear();
        this._persistenceContext = new Mock<IContext>();
        this._persistenceContext.Setup(c => c.GetAsync<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>(It.IsAny<CancellationToken>()))
            .Returns(() => new ValueTask<IEnumerable<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>>(this._itemDefinitions));
        this._persistenceContext.Setup(c => c.CreateNew<Item>(It.IsAny<object?[]>()))
            .Returns(() => new BasicModel.Item());
        this._persistenceContext.Setup(c => c.CreateNew<ItemOptionLink>(It.IsAny<object?[]>()))
            .Returns(() => new BasicModel.ItemOptionLink());
        this._persistenceContext.Setup(c => c.DeleteAsync(It.IsAny<Item>()))
            .Returns(() => new ValueTask<bool>(true));
    }

    /// <summary>
    /// The export contains the complete state of the shop items.
    /// </summary>
    [Test]
    public void Export_ContainsCompleteItemState()
    {
        var definition = this.AddItemDefinition(0, 1);
        var option = this.AddOption(definition, "Excellent");
        var (merchant, _) = CreateMerchant();
        var item = new BasicModel.Item
        {
            Definition = definition,
            ItemSlot = 4,
            Level = 7,
            Durability = 123.5,
            HasSkill = true,
            SocketCount = 2,
            PetExperience = 42,
        };
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = option, Level = 3, Index = 1 });
        merchant.MerchantStore!.Items.Add(item);

        var json = this._service.BuildExport(merchant);
        using var document = JsonDocument.Parse(json);
        Assert.That(document.RootElement.TryGetProperty("Merchant", out _), Is.False, "The file must not contain the merchant name, so that it can be imported into another shop.");

        var export = Deserialize(json);

        Assert.That(export.Items, Has.Count.EqualTo(1));
        var itemExport = export.Items.Single();
        Assert.That(itemExport.Slot, Is.EqualTo(4));
        Assert.That(itemExport.ItemGroup, Is.EqualTo(0));
        Assert.That(itemExport.ItemNumber, Is.EqualTo(1));
        Assert.That(itemExport.Level, Is.EqualTo(7));
        Assert.That(itemExport.Durability, Is.EqualTo(123.5));
        Assert.That(itemExport.HasSkill, Is.True);
        Assert.That(itemExport.SocketCount, Is.EqualTo(2));
        Assert.That(itemExport.PetExperience, Is.EqualTo(42));
        Assert.That(itemExport.Options, Has.Count.EqualTo(1));
        Assert.That(itemExport.Options.Single().OptionTypeId, Is.EqualTo(option.OptionType!.Id));
        Assert.That(itemExport.Options.Single().OptionTypeName, Is.EqualTo("Excellent"));
        Assert.That(itemExport.Options.Single().Level, Is.EqualTo(3));
        Assert.That(itemExport.Options.Single().Index, Is.EqualTo(1));
    }

    /// <summary>
    /// An exported shop can be imported again and keeps its items and options.
    /// </summary>
    [Test]
    public async Task ExportAndImport_RoundTripsItemStateAsync()
    {
        var definition = this.AddItemDefinition(1, 2);
        var option = this.AddOption(definition, "Excellent");
        var (sourceMerchant, _) = CreateMerchant("Source Merchant");
        var item = new BasicModel.Item
        {
            Definition = definition,
            ItemSlot = 2,
            Level = 5,
            Durability = 80,
            HasSkill = true,
            SocketCount = 1,
            PetExperience = 12,
        };
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = option, Level = 2, Index = 0 });
        sourceMerchant.MerchantStore!.Items.Add(item);

        var json = this._service.BuildExport(sourceMerchant);
        var (targetMerchant, _) = CreateMerchant("Target Merchant");

        var result = await this._service
            .TryApplyImportAsync(targetMerchant, json, this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ImportedItemCount, Is.EqualTo(1));
        Assert.That(result.Problems, Is.Empty);
        Assert.That(targetMerchant.Designation.ValueInNeutralLanguage, Is.EqualTo("Target Merchant"));

        var importedItem = targetMerchant.MerchantStore!.Items.Single();
        Assert.That(importedItem.Definition, Is.SameAs(definition));
        Assert.That(importedItem.ItemSlot, Is.EqualTo(2));
        Assert.That(importedItem.Level, Is.EqualTo(5));
        Assert.That(importedItem.Durability, Is.EqualTo(80));
        Assert.That(importedItem.HasSkill, Is.True);
        Assert.That(importedItem.SocketCount, Is.EqualTo(1));
        Assert.That(importedItem.PetExperience, Is.EqualTo(12));
        Assert.That(importedItem.ItemOptions.Single().ItemOption, Is.SameAs(option));
        Assert.That(importedItem.ItemOptions.Single().Level, Is.EqualTo(2));
    }

    /// <summary>
    /// The import replaces the existing items of the shop instead of adding to them.
    /// </summary>
    [Test]
    public async Task Import_ReplacesExistingItemsAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        store.Money = 5000;
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(new ShopItemExport { ItemGroup = 0, ItemNumber = 1 }), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.EqualTo(1));
        Assert.That(store.Items, Has.Count.EqualTo(1));
        Assert.That(store.Items.Single(), Is.Not.SameAs(existingItem));
        Assert.That(store.Money, Is.EqualTo(5000));
        this._persistenceContext.Verify(c => c.DeleteAsync((Item)existingItem), Times.Once);
    }

    /// <summary>
    /// An item with an unknown definition rejects the whole file; nothing is imported, not even the valid items.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_UnknownItemDefinition_RejectsWholeFileAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(
                merchant,
                this.CreateExport(
                    new ShopItemExport { Slot = 0, ItemGroup = 0, ItemNumber = 1 },
                    new ShopItemExport { Slot = 1, ItemGroup = 9, ItemNumber = 9 }),
                this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.Problems, Has.Count.EqualTo(1));
        Assert.That(result.Problems.Single(), Does.Contain("Slot 1"));
        this.AssertShopUntouched(store, existingItem);
    }

    /// <summary>
    /// An option which the item definition doesn't know rejects the whole file.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_UnknownOption_RejectsWholeFileAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.Options.Add(new ShopItemOptionExport { OptionTypeId = Guid.NewGuid(), Number = 1 });

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.Problems, Has.Count.EqualTo(1));
        this.AssertShopUntouched(store, existingItem);
    }

    /// <summary>
    /// An ancient set which the item definition doesn't know rejects the whole file.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_UnknownAncientSet_RejectsWholeFileAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.AncientSets.Add("Unknown Set");

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.Problems, Has.Count.EqualTo(1));
        this.AssertShopUntouched(store, existingItem);
    }

    /// <summary>
    /// All problems of the file are reported at once, so that it can be adjusted in one go.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_MultipleProblems_AreAllReportedAsync()
    {
        this.AddItemDefinition(0, 1);
        var (merchant, _) = CreateMerchant();

        var result = await this._service
            .TryApplyImportAsync(
                merchant,
                this.CreateExport(
                    new ShopItemExport { Slot = 0, ItemGroup = 8, ItemNumber = 8 },
                    new ShopItemExport { Slot = 1, ItemGroup = 9, ItemNumber = 9 }),
                this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.Problems, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Ancient sets are matched by their neutral language name.
    /// </summary>
    [Test]
    public async Task Import_AssignsAncientSetByNameAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var itemOfSet = this.AddAncientSet(definition, "Warrior Leather");
        var (merchant, store) = CreateMerchant();
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.AncientSets.Add("Warrior Leather");

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.Problems, Is.Empty);
        Assert.That(store.Items.Single().ItemSetGroups, Is.EquivalentTo(new[] { itemOfSet }));
    }

    /// <summary>
    /// An invalid file is rejected without touching the shop.
    /// </summary>
    [Test]
    public async Task Import_InvalidFile_LeavesShopUntouchedAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(merchant, "{ not valid json", this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result, Is.Null);
        Assert.That(store.Items, Is.EquivalentTo(new[] { existingItem }));
        this._persistenceContext.Verify(c => c.DeleteAsync(It.IsAny<Item>()), Times.Never);
    }

    /// <summary>
    /// Incomplete or malformed files are rejected without touching the shop.
    /// In particular, a missing version or a missing item list must not be mistaken for an empty shop.
    /// </summary>
    /// <param name="json">The content of the file.</param>
    /// <returns>The task.</returns>
    [TestCase("{}")]
    [TestCase("{\"FormatVersion\":\"1.0\"}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":null}")]
    [TestCase("{\"Items\":[]}")]
    [TestCase("{\"FormatVersion\":null,\"Items\":[]}")]
    [TestCase("{\"FormatVersion\":\"2.0\",\"Items\":[]}")]
    [TestCase("[]")]
    [TestCase("null")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[null]}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[{\"Slot\":1},{\"Slot\":1}]}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[{\"Slot\":1,\"Durability\":-1}]}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[{\"Slot\":1,\"Options\":[null]}]}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[{\"Slot\":1,\"Options\":[{\"Level\":-1}]}]}")]
    [TestCase("{\"FormatVersion\":\"1.0\",\"Items\":[{\"Slot\":1,\"AncientSets\":[null]}]}")]
    public async Task Import_IncompleteOrMalformedFile_LeavesShopUntouchedAsync(string json)
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(merchant, json, this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result, Is.Null);
        Assert.That(store.Items, Is.EquivalentTo(new[] { existingItem }));
        this._persistenceContext.Verify(c => c.DeleteAsync(It.IsAny<Item>()), Times.Never);
        this._persistenceContext.Verify(c => c.CreateNew<Item>(It.IsAny<object?[]>()), Times.Never);
    }

    /// <summary>
    /// An explicitly empty item list is a valid file, which clears the shop.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_ExplicitlyEmptyItemList_ClearsShopAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(merchant, "{\"FormatVersion\":\"1.0\",\"Items\":[]}", this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(store.Items, Is.Empty);
        this._persistenceContext.Verify(c => c.DeleteAsync((Item)existingItem), Times.Once);
    }

    /// <summary>
    /// If the id of the option type is unknown, as in another configuration, the option type is found by its name.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_MatchesOptionTypeByNameWhenIdIsUnknownAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var option = this.AddOption(definition, "Excellent");
        var (merchant, store) = CreateMerchant();
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.Options.Add(new ShopItemOptionExport { OptionTypeId = Guid.NewGuid(), OptionTypeName = "Excellent", Number = 1 });

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.Problems, Is.Empty);
        Assert.That(store.Items.Single().ItemOptions.Single().ItemOption, Is.SameAs(option));
    }

    /// <summary>
    /// An option is a problem when neither the id nor the name of its option type is known.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_OptionWithUnknownIdAndName_RejectsWholeFileAsync()
    {
        this.AddOption(this.AddItemDefinition(0, 1), "Excellent");
        var (merchant, store) = CreateMerchant();
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.Options.Add(new ShopItemOptionExport { OptionTypeId = Guid.NewGuid(), OptionTypeName = "Harmony", Number = 1 });

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.Problems, Has.Count.EqualTo(1));
        Assert.That(result.Problems.Single(), Does.Contain("Harmony"));
        Assert.That(store.Items, Is.Empty);
    }

    /// <summary>
    /// If the removal of the old items fails, no new items are created and the exception is propagated.
    /// </summary>
    [Test]
    public void Import_FailingRemoval_DoesNotCreateItems()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        store.Items.Add(new BasicModel.Item { Definition = definition });
        this._persistenceContext.Setup(c => c.DeleteAsync(It.IsAny<Item>())).Throws<InvalidOperationException>();

        Assert.ThrowsAsync<InvalidOperationException>(async () => await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(new ShopItemExport { ItemGroup = 0, ItemNumber = 1 }), this._persistenceContext.Object)
            .ConfigureAwait(false));

        this._persistenceContext.Verify(c => c.CreateNew<Item>(It.IsAny<object?[]>()), Times.Never);
    }

    /// <summary>
    /// If the creation of the new items fails, the already created items are discarded again.
    /// </summary>
    [Test]
    public void Import_FailingCreation_DiscardsCreatedItems()
    {
        var definition = this.AddItemDefinition(0, 1);
        var option = this.AddOption(definition, "Excellent");
        var (merchant, store) = CreateMerchant();
        var createdItem = new BasicModel.Item();
        this._persistenceContext.Setup(c => c.CreateNew<Item>(It.IsAny<object?[]>())).Returns(createdItem);
        this._persistenceContext.Setup(c => c.CreateNew<ItemOptionLink>(It.IsAny<object?[]>())).Throws<InvalidOperationException>();
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.Options.Add(new ShopItemOptionExport { OptionTypeId = option.OptionType!.Id, Number = 1 });

        Assert.ThrowsAsync<InvalidOperationException>(async () => await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false));

        Assert.That(store.Items, Is.Empty);
        this._persistenceContext.Verify(c => c.Detach(createdItem), Times.Once);
    }

    /// <summary>
    /// Items which don't fit into the 8x15 grid of the shop reject the whole file.
    /// </summary>
    /// <param name="slot">The slot of the item.</param>
    /// <param name="width">The width of the item.</param>
    /// <param name="height">The height of the item.</param>
    /// <returns>The task.</returns>
    [TestCase(120, 1, 1)]
    [TestCase(200, 1, 1)]
    [TestCase(7, 2, 1)]
    [TestCase(112, 1, 2)]
    public async Task Import_ItemNotFittingIntoShop_RejectsWholeFileAsync(int slot, int width, int height)
    {
        var definition = this.AddItemDefinition(0, 1, (byte)width, (byte)height);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = definition };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(
                merchant,
                this.CreateExport(new ShopItemExport { Slot = (byte)slot, ItemGroup = 0, ItemNumber = 1 }),
                this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.Problems, Has.Count.EqualTo(1));
        this.AssertShopUntouched(store, existingItem);
    }

    /// <summary>
    /// Items which share a cell of the shop's grid reject the whole file, even if their slots differ.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_OverlappingItems_RejectsWholeFileAsync()
    {
        var large = this.AddItemDefinition(0, 1, 2, 2);
        this.AddItemDefinition(0, 2);
        var (merchant, store) = CreateMerchant();
        var existingItem = new BasicModel.Item { Definition = large };
        store.Items.Add(existingItem);

        var result = await this._service
            .TryApplyImportAsync(
                merchant,
                this.CreateExport(
                    new ShopItemExport { Slot = 0, ItemGroup = 0, ItemNumber = 1 },
                    new ShopItemExport { Slot = 9, ItemGroup = 0, ItemNumber = 2 }),
                this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.Problems.Single(), Does.Contain("Slot 9"));
        this.AssertShopUntouched(store, existingItem);
    }

    /// <summary>
    /// Multi-cell items next to each other, including the last cell of the grid, are imported.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task Import_AdjacentMultiCellItems_AreImportedAsync()
    {
        this.AddItemDefinition(0, 1, 2, 2);
        this.AddItemDefinition(0, 2);
        var (merchant, store) = CreateMerchant();

        var result = await this._service
            .TryApplyImportAsync(
                merchant,
                this.CreateExport(
                    new ShopItemExport { Slot = 0, ItemGroup = 0, ItemNumber = 1 },
                    new ShopItemExport { Slot = 2, ItemGroup = 0, ItemNumber = 2 },
                    new ShopItemExport { Slot = 119, ItemGroup = 0, ItemNumber = 2 }),
                this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.Problems, Is.Empty);
        Assert.That(store.Items, Has.Count.EqualTo(3));
    }

    private static MerchantShopExport Deserialize(string json)
    {
        return JsonSerializer.Deserialize<MerchantShopExport>(json)!;
    }

    private static (BasicModel.MonsterDefinition Merchant, ItemStorage Store) CreateMerchant(string name = "Test Merchant")
    {
        var store = new BasicModel.ItemStorage();
        var merchant = new BasicModel.MonsterDefinition
        {
            Designation = name,
            MerchantStore = store,
        };

        return (merchant, store);
    }

    private void AssertShopUntouched(ItemStorage store, Item existingItem)
    {
        Assert.That(store.Items, Is.EquivalentTo(new[] { existingItem }));
        this._persistenceContext.Verify(c => c.DeleteAsync(It.IsAny<Item>()), Times.Never);
        this._persistenceContext.Verify(c => c.CreateNew<Item>(It.IsAny<object?[]>()), Times.Never);
    }

    private BasicModel.ItemDefinition AddItemDefinition(byte group, short number, byte width = 1, byte height = 1)
    {
        var definition = new BasicModel.ItemDefinition { Group = group, Number = number, Width = width, Height = height };
        this._itemDefinitions.Add(definition);
        return definition;
    }

    private BasicModel.IncreasableItemOption AddOption(BasicModel.ItemDefinition definition, string optionTypeName)
    {
        var optionType = new BasicModel.ItemOptionType { Id = Guid.NewGuid(), Name = optionTypeName };
        var option = new BasicModel.IncreasableItemOption { OptionType = optionType, Number = 1 };
        var optionDefinition = new BasicModel.ItemOptionDefinition();
        optionDefinition.PossibleOptions.Add(option);
        definition.PossibleItemOptions.Add(optionDefinition);
        return option;
    }

    private BasicModel.ItemOfItemSet AddAncientSet(BasicModel.ItemDefinition definition, string setName)
    {
        var setGroup = new BasicModel.ItemSetGroup { Name = setName };
        var itemOfSet = new BasicModel.ItemOfItemSet
        {
            AncientSetDiscriminator = 1,
            ItemSetGroup = setGroup,
            ItemDefinition = definition,
        };
        setGroup.Items.Add(itemOfSet);
        definition.PossibleItemSetGroups.Add(setGroup);
        return itemOfSet;
    }

    private string CreateExport(params ShopItemExport[] items)
    {
        var export = new MerchantShopExport();
        foreach (var item in items)
        {
            export.Items.Add(item);
        }

        return JsonSerializer.Serialize(export);
    }
}
