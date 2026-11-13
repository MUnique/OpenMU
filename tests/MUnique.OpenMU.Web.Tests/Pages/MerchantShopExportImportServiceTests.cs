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
        Assert.That(result.SkippedItemCount, Is.Zero);
        Assert.That(result.SkippedOptionCount, Is.Zero);
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
    /// Items with an unknown definition are skipped and reported.
    /// </summary>
    [Test]
    public async Task Import_SkipsUnknownItemDefinitionAsync()
    {
        var (merchant, store) = CreateMerchant();

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(new ShopItemExport { ItemGroup = 9, ItemNumber = 9 }), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.Zero);
        Assert.That(result.SkippedItemCount, Is.EqualTo(1));
        Assert.That(store.Items, Is.Empty);
    }

    /// <summary>
    /// Options which the item definition doesn't know are skipped and reported.
    /// </summary>
    [Test]
    public async Task Import_SkipsUnknownOptionAsync()
    {
        var definition = this.AddItemDefinition(0, 1);
        var (merchant, store) = CreateMerchant();
        var itemExport = new ShopItemExport { ItemGroup = 0, ItemNumber = 1 };
        itemExport.Options.Add(new ShopItemOptionExport { OptionTypeId = Guid.NewGuid(), Number = 1 });

        var result = await this._service
            .TryApplyImportAsync(merchant, this.CreateExport(itemExport), this._persistenceContext.Object)
            .ConfigureAwait(false);

        Assert.That(result!.ImportedItemCount, Is.EqualTo(1));
        Assert.That(result.SkippedOptionCount, Is.EqualTo(1));
        Assert.That(store.Items.Single().ItemOptions, Is.Empty);
        Assert.That(store.Items.Single().Definition, Is.SameAs(definition));
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

        Assert.That(result!.SkippedOptionCount, Is.Zero);
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

    private BasicModel.ItemDefinition AddItemDefinition(byte group, short number)
    {
        var definition = new BasicModel.ItemDefinition { Group = group, Number = number };
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
