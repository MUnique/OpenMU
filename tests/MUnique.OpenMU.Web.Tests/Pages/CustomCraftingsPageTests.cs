// <copyright file="CustomCraftingsPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>Tests the custom crafting editor against in-memory configuration persistence.</summary>
[TestFixture]
[NonParallelizable]
public sealed class CustomCraftingsPageTests
{
    private BunitContext _context = null!;
    private CultureInfo _previousUiCulture = null!;
    private Mock<IDataSource<GameConfiguration>> _dataSource = null!;
    private Mock<IContext> _persistenceContext = null!;
    private Mock<IToastService> _toastService = null!;
    private GameConfiguration _configuration = null!;
    private MonsterDefinition _chaosGoblin = null!;
    private ItemDefinition _requiredItem = null!;
    private ItemDefinition _resultItem = null!;
    private ItemDefinition _wearableResultItem = null!;

    /// <summary>Creates the NPCs and items used by the page.</summary>
    [SetUp]
    public void SetUp()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._configuration = new BasicModel.GameConfiguration();
        this._chaosGoblin = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 246, Designation = new LocalizedString("Chaos Goblin"), NpcWindow = NpcWindow.ChaosMachine };
        this._configuration.Monsters.Add(this._chaosGoblin);
        this._configuration.Monsters.Add(new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 1, Designation = new LocalizedString("Merchant"), NpcWindow = NpcWindow.Merchant });
        this._requiredItem = new BasicModel.ItemDefinition { Id = Guid.NewGuid(), Name = new LocalizedString("Jewel of Chaos"), Group = 14, Number = 12 };
        this._configuration.Items.Add(this._requiredItem);
        this._resultItem = new BasicModel.ItemDefinition { Id = Guid.NewGuid(), Name = new LocalizedString("Arrow stack"), Group = 4, Number = 15, Durability = byte.MaxValue };
        this._configuration.Items.Add(this._resultItem);
        var itemSlot = new BasicModel.ItemSlotType { Id = Guid.NewGuid() };
        itemSlot.ItemSlots.Add(0);
        this._configuration.ItemSlotTypes.Add(itemSlot);
        this._wearableResultItem = new BasicModel.ItemDefinition { Id = Guid.NewGuid(), Name = new LocalizedString("Chaos Weapon"), Group = 0, Number = 31, Durability = 40, ItemSlot = itemSlot };
        this._configuration.Items.Add(this._wearableResultItem);
        this._persistenceContext = new Mock<IContext>();
        this._persistenceContext.SetupGet(context => context.HasChanges).Returns(false);
        this._persistenceContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        this._persistenceContext.Setup(context => context.DeleteAsync(It.IsAny<ItemCrafting>())).Returns(ValueTask.FromResult(true));
        this._persistenceContext.Setup(context => context.CreateNew<ItemCrafting>()).Returns(() => new BasicModel.ItemCrafting { Id = Guid.NewGuid() });
        this._persistenceContext.Setup(context => context.CreateNew<SimpleCraftingSettings>()).Returns(() => new BasicModel.SimpleCraftingSettings { Id = Guid.NewGuid() });
        this._persistenceContext.Setup(context => context.CreateNew<ItemCraftingRequiredItem>()).Returns(() => new BasicModel.ItemCraftingRequiredItem { Id = Guid.NewGuid() });
        this._persistenceContext.Setup(context => context.CreateNew<ItemCraftingResultItem>()).Returns(() => new BasicModel.ItemCraftingResultItem { Id = Guid.NewGuid() });
        this._dataSource = new Mock<IDataSource<GameConfiguration>>();
        this._dataSource.Setup(source => source.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(this._persistenceContext.Object);
        this._dataSource.Setup(source => source.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(this._configuration);
        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(this._dataSource.Object);
        this._toastService = new Mock<IToastService>();
        this._context.Services.AddSingleton(this._toastService.Object);
        this._context.Services.AddSingleton(NullLogger<CustomCraftings>.Instance);
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>Releases the rendered component and restores the UI culture.</summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
        this._dataSource.Object.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>The NPC selector excludes merchants, and the all-recipe tab lists recipes across NPCs.</summary>
    [Test]
    public void RecipeCatalogFiltersNpcChoicesAndSearchesAcrossNpcs()
    {
        var recipe = CreateRecipe("Wing recipe", 5);
        this._chaosGoblin.ItemCraftings.Add(recipe);
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));

        Assert.That(cut.Find("#availableRecipesNpc").TextContent, Does.Contain("Chaos Goblin"));
        Assert.That(cut.Find("#availableRecipesNpc").TextContent, Does.Not.Contain("Merchant"));
        cut.Find("#tab-all-recipes").Click();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Wing recipe")));
        Assert.That(cut.Markup, Does.Contain("Chaos Goblin (246)"));

        cut.Find("input[type=search]").Input("Wing recipe");
        cut.WaitForAssertion(() => Assert.That(cut.FindAll("table.quickgrid tbody tr").Any(row => row.TextContent.Contains("Wing recipe")), Is.True));
        cut.Find("input[type=search]").Input("no matching recipe");
        cut.WaitForAssertion(() => Assert.That(cut.FindAll("table.quickgrid tbody tr").Any(row => row.TextContent.Contains("Wing recipe")), Is.False));
    }

    /// <summary>Creating a recipe persists the configured requirement and result with a free NPC recipe number.</summary>
    [Test]
    public void CreateRecipePersistsRequirementAndResult()
    {
        this._chaosGoblin.ItemCraftings.Add(CreateRecipe("Existing recipe", 1));
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#tab-create-recipe").Click();
        cut.Find("#craftingNpc").Change(this._chaosGoblin.GetId().ToString());
        cut.Find("#recipeName").Change("Custom wing");
        cut.Find("#requiredItem").Change(this._requiredItem.GetId().ToString());
        cut.Find("#requiredMinimumAmount").Change("2");
        cut.Find("#requiredMaximumAmount").Change("3");
        cut.Find("#requiredMinimumLevel").Change("1");
        cut.Find("#requiredMaximumLevel").Change("4");
        cut.FindAll("button").Single(button => button.TextContent.Contains("Add required item", StringComparison.OrdinalIgnoreCase)).Click();
        cut.Find("#resultItem").Change(this._resultItem.GetId().ToString());
        Assert.That(cut.Find("label[for=resultQuantityOrDurability]").TextContent, Is.EqualTo("Stack quantity"));
        cut.Find("#resultMinimumLevel").Change("2");
        cut.Find("#resultMaximumLevel").Change("6");
        cut.Find("#resultQuantityOrDurability").Change("80");
        cut.FindAll("button").Single(button => button.TextContent.Contains("Add result", StringComparison.OrdinalIgnoreCase)).Click();
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.That(this._chaosGoblin.ItemCraftings.Any(crafting => crafting.Name.ValueInNeutralLanguage == "Custom wing"), Is.True));
        var created = this._chaosGoblin.ItemCraftings.Single(crafting => crafting.Name.ValueInNeutralLanguage == "Custom wing");
        Assert.That(created.Number, Is.EqualTo(2));
        Assert.That(created.ItemCraftingHandlerClassName, Does.Contain("SimpleItemCraftingHandler"));
        Assert.That(created.SimpleCraftingSettings!.RequiredItems.Single().PossibleItems.Single(), Is.SameAs(this._requiredItem));
        Assert.That(created.SimpleCraftingSettings.RequiredItems.Single().MinimumAmount, Is.EqualTo(2));
        Assert.That(created.SimpleCraftingSettings.RequiredItems.Single().MaximumItemLevel, Is.EqualTo(4));
        Assert.That(created.SimpleCraftingSettings.ResultItems.Single().ItemDefinition, Is.SameAs(this._resultItem));
        Assert.That(created.SimpleCraftingSettings.ResultItems.Single().RandomMinimumLevel, Is.EqualTo(2));
        Assert.That(created.SimpleCraftingSettings.ResultItems.Single().Durability, Is.EqualTo(80));
    }

    /// <summary>The result value changes its caption and range for stackable and wearable items.</summary>
    [Test]
    public void ResultInputDistinguishesStackQuantityFromWearableDurability()
    {
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#tab-create-recipe").Click();

        cut.Find("#resultItem").Change(this._resultItem.GetId().ToString());
        Assert.That(cut.Find("label[for=resultQuantityOrDurability]").TextContent, Is.EqualTo("Stack quantity"));
        Assert.That(cut.Find("#resultQuantityOrDurability").GetAttribute("min"), Is.EqualTo("1"));
        Assert.That(cut.Find("#resultQuantityOrDurability").GetAttribute("max"), Is.EqualTo("255"));

        cut.Find("#resultItem").Change(this._wearableResultItem.GetId().ToString());
        Assert.That(cut.Find("label[for=resultQuantityOrDurability]").TextContent, Is.EqualTo("Durability"));
        Assert.That(cut.Find("#resultQuantityOrDurability").GetAttribute("min"), Is.EqualTo("0"));
        Assert.That(cut.Find("#resultQuantityOrDurability").GetAttribute("max"), Is.EqualTo("255"));
        Assert.That(cut.Markup, Does.Contain("Remaining durability of the result item"));
        Assert.That(cut.Markup, Does.Contain("Counts separate matching items."));
    }

    /// <summary>A result stack cannot exceed its item definition's configured capacity.</summary>
    [Test]
    public void ResultStackQuantityCannotExceedConfiguredMaximum()
    {
        this._resultItem.Durability = 10;
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#tab-create-recipe").Click();
        cut.Find("#resultItem").Change(this._resultItem.GetId().ToString());
        cut.Find("#resultQuantityOrDurability").Change("11");
        cut.FindAll("button").Single(button => button.TextContent.Contains("Add result", StringComparison.OrdinalIgnoreCase)).Click();

        Assert.That(cut.FindAll("table tbody tr").Any(row => row.TextContent.Contains("Arrow stack")), Is.False);
        this._toastService.Verify(toast => toast.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    /// <summary>Invalid numeric input is reported through a toast instead of an inline validation summary.</summary>
    [Test]
    public void InvalidNumericInputShowsToastWithoutInlineValidationSummary()
    {
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#tab-create-recipe").Click();
        cut.Find("#zenCost").Change("200000000000000000000");
        cut.Find("form").Submit();

        this._toastService.Verify(toast => toast.ShowError("One or more values are invalid. Check the highlighted fields and try again.", It.IsAny<string>()), Times.Once);
        Assert.That(cut.Markup, Does.Not.Contain("The Money field must be a number."));
    }

    /// <summary>The all-recipes catalog paginates and search narrows its rows.</summary>
    [Test]
    public void AllRecipesCatalogPaginatesAndSearchResetsPage()
    {
        for (var index = 0; index < 22; index++)
        {
            this._chaosGoblin.ItemCraftings.Add(CreateRecipe($"Recipe {index:00}", (byte)index));
        }

        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#tab-all-recipes").Click();
        cut.WaitForAssertion(() => Assert.That(cut.FindAll("table.quickgrid tbody tr").Count, Is.EqualTo(20)));
        Assert.That(cut.Markup, Does.Contain("Recipe 00"));
        Assert.That(cut.Markup, Does.Not.Contain("Recipe 21"));

        cut.FindAll("button").Single(button => button.TextContent == "Next").Click();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Recipe 21")));
        Assert.That(cut.FindAll("table.quickgrid tbody tr").Any(row => row.TextContent.Contains("Recipe 00")), Is.False);

        cut.Find("input[type=search]").Input("Recipe 00");
        cut.WaitForAssertion(() => Assert.That(cut.FindAll("table.quickgrid tbody tr").Count(row => row.TextContent.Contains("Recipe")), Is.EqualTo(1)));
        Assert.That(cut.Markup, Does.Contain("Recipe 00"));
    }

    /// <summary>Deleting a custom recipe confirms and persists removal from the NPC recipe collection.</summary>
    [Test]
    public void DeleteCustomRecipeRemovesItAfterConfirmation()
    {
        var recipe = CreateRecipe("Custom recipe", 7);
        recipe.ItemCraftingHandlerClassName = typeof(SimpleItemCraftingHandler).FullName!;
        this._chaosGoblin.ItemCraftings.Add(recipe);
        this._context.JSInterop.Setup<bool>("window.confirm", _ => true).SetResult(true);
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#availableRecipesNpc").Change(this._chaosGoblin.GetId().ToString());
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Custom recipe")));
        cut.FindAll("button").Single(button => button.TextContent == "Delete").Click();

        cut.WaitForAssertion(() => Assert.That(this._chaosGoblin.ItemCraftings, Does.Not.Contain(recipe)));
        this._persistenceContext.Verify(context => context.DeleteAsync(recipe), Times.Once);
        this._persistenceContext.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        this._toastService.Verify(toast => toast.ShowSuccess(It.IsAny<string>()), Times.Once);
    }

    /// <summary>Recipes with a custom handler are displayed without the custom-recipe delete action.</summary>
    [Test]
    public void RecipeWithExternalHandlerCannotBeDeletedFromTheEditor()
    {
        this._chaosGoblin.ItemCraftings.Add(CreateRecipe("Built-in recipe", 8));
        var cut = this._context.Render<CustomCraftings>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Chaos Goblin")));
        cut.Find("#availableRecipesNpc").Change(this._chaosGoblin.GetId().ToString());
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Built-in recipe")));

        Assert.That(cut.FindAll("button").Any(button => button.TextContent == "Delete"), Is.False);
    }

    private static ItemCrafting CreateRecipe(string name, byte number)
        => new BasicModel.ItemCrafting
        {
            Id = Guid.NewGuid(),
            Name = new LocalizedString(name),
            Number = number,
            ItemCraftingHandlerClassName = "external.handler",
        };
}
