// <copyright file="MixesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="Mixes"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MixesTests
{
    private BunitContext _context = null!;

    private CultureInfo _previousUiCulture = null!;

    private BasicModel.SimpleCraftingSettings _settings = null!;

    private Mock<IContext> _petTrainerContext = null!;

    private Mock<IContext> _testNpcContext = null!;

    /// <summary>
    /// Sets up the page with two NPCs which offer mixes; one of them has two mixes in reverse order.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        var jewelOfChaos = new BasicModel.ItemDefinition { Id = Guid.NewGuid(), Name = "Jewel of Chaos" };
        var requiredItem = new BasicModel.ItemCraftingRequiredItem
        {
            MinimumAmount = 1,
            MaximumAmount = 1,
            AddPercentage = 5,
            SuccessResult = MixResult.Disappear,
            FailResult = MixResult.StaysAsIs,
        };
        requiredItem.PossibleItems.Add(jewelOfChaos);

        this._settings = new BasicModel.SimpleCraftingSettings
        {
            SuccessPercent = 50,
            MaximumSuccessPercent = 100,
            Money = 1000,
            ResultItemLuckOptionChance = 25,
        };
        this._settings.RequiredItems.Add(requiredItem);

        var chaosWeapon = new BasicModel.ItemCrafting
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Name = "Chaos Weapon",
            SimpleCraftingSettings = this._settings,
        };
        var darkHorse = new BasicModel.ItemCrafting
        {
            Id = Guid.NewGuid(),
            Number = 2,
            Name = "Dark Horse",
            SimpleCraftingSettings = new BasicModel.SimpleCraftingSettings { SuccessPercent = 60 },
        };
        var petMix = new BasicModel.ItemCrafting
        {
            Id = Guid.NewGuid(),
            Number = 5,
            Name = "Pet Mix",
            SimpleCraftingSettings = new BasicModel.SimpleCraftingSettings { SuccessPercent = 70 },
        };

        var firstMonster = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 238, Designation = "Test NPC" };
        firstMonster.ItemCraftings.Add(darkHorse);
        firstMonster.ItemCraftings.Add(chaosWeapon);
        var secondMonster = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 226, Designation = "Pet Trainer" };
        secondMonster.ItemCraftings.Add(petMix);

        var configuration = new BasicModel.GameConfiguration();
        configuration.Monsters.Add(firstMonster);
        configuration.Monsters.Add(secondMonster);
        configuration.Items.Add(jewelOfChaos);

        var firstCraftings = new Dictionary<Guid, ItemCrafting>
        {
            [chaosWeapon.Id] = chaosWeapon,
            [darkHorse.Id] = darkHorse,
        };
        var secondCraftings = new Dictionary<Guid, ItemCrafting>
        {
            [petMix.Id] = petMix,
        };

        this._petTrainerContext = CreateContext(secondCraftings);
        this._testNpcContext = CreateContext(firstCraftings);

        // The page processes the NPCs ordered by their number: 226 (Pet Trainer) first, 238 (Test NPC) second.
        var contexts = new Queue<IContext>([this._petTrainerContext.Object, this._testNpcContext.Object]);
        var persistenceContextProvider = new Mock<IPersistenceContextProvider>();
        persistenceContextProvider
            .Setup(p => p.CreateNewTypedContext(It.IsAny<Type>(), It.IsAny<bool>(), It.IsAny<GameConfiguration?>()))
            .Returns(() => contexts.Dequeue());

        var dataSource = new Mock<IDataSource<GameConfiguration>>();
        dataSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(configuration);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddLogging();
        this._context.Services.Configure<RequestLocalizationOptions>(options => options.AddSupportedUICultures("en"));
        this._context.Services.AddSingleton(dataSource.Object);
        this._context.Services.AddSingleton(persistenceContextProvider.Object);
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<IModalService>());
        this._context.Services.AddSingleton(Mock.Of<ILookupController>());
        this._context.Services.AddSingleton(Mock.Of<IChangeNotificationService>());
        this._context.Services.AddSingleton(new LoadingOverlayService());
        this._context.Services.AddSingleton(Mock.Of<ILogger<Mixes>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// All NPC categories are collapsed by default, so no mix is loaded into a form.
    /// </summary>
    [Test]
    public void ShowsAllCategoriesCollapsedByDefault()
    {
        var cut = this.RenderPage();

        Assert.That(cut.Markup, Does.Contain("238 - Test NPC"));
        Assert.That(cut.Markup, Does.Contain("226 - Pet Trainer"));
        Assert.That(cut.FindAll("form"), Is.Empty, "No mix should be expanded by default.");
    }

    /// <summary>
    /// The mixes of a category are sorted by their number and are collapsed as well.
    /// </summary>
    [Test]
    public void ShowsMixesSortedByNumberAndCollapsed()
    {
        var cut = this.RenderPage();

        this.FindSectionHeader(cut, "238 - Test NPC").Click();

        var mixHeaders = this.GetSectionHeaders(cut)
            .Where(header => header.TextContent.Contains("Chaos Weapon") || header.TextContent.Contains("Dark Horse"))
            .ToList();
        Assert.That(mixHeaders[0].TextContent.Trim(), Does.StartWith("1 - Chaos Weapon"));
        Assert.That(mixHeaders[1].TextContent.Trim(), Does.StartWith("2 - Dark Horse"));
        Assert.That(cut.FindAll("form"), Is.Empty, "The mixes should be collapsed by default.");
    }

    /// <summary>
    /// Expanding a mix shows its form with the success rate.
    /// </summary>
    [Test]
    public void ShowsFormOfExpandedMix()
    {
        var cut = this.RenderPage();
        this.ExpandAll(cut);

        var inputValues = cut.FindAll("input[type=number]").Select(input => input.GetAttribute("value")).ToList();
        Assert.That(inputValues, Has.Some.EqualTo("50"), "The success rate should be shown.");
        Assert.That(cut.FindAll("button").Count(button => button.TextContent.Trim() == "Save"), Is.EqualTo(3), "Every mix should have its own save button.");
    }

    /// <summary>
    /// The success rate of a mix is editable and updates the configuration.
    /// </summary>
    [Test]
    public void ChangingSuccessRateUpdatesConfiguration()
    {
        var cut = this.RenderPage();
        this.ExpandAll(cut);

        var input = cut.FindAll("input[type=number]").First(element => element.GetAttribute("value") == "50");
        input.Change("75");

        Assert.That(this._settings.SuccessPercent, Is.EqualTo(75));
    }

    /// <summary>
    /// Saving a mix saves only the editing context of its own category.
    /// </summary>
    [Test]
    public void SavePersistsOnlyTheCategoryOfTheMix()
    {
        var cut = this.RenderPage();
        this.ExpandAll(cut);

        var firstForm = cut.FindAll("form").First();
        firstForm.Submit();

        this._petTrainerContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        this._testNpcContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// The required items of a mix are shown in its expanded form.
    /// </summary>
    [Test]
    public void ShowsRequiredItemsOfTheMix()
    {
        var cut = this.RenderPage();
        this.ExpandAll(cut);

        Assert.That(cut.Markup, Does.Contain("Jewel of Chaos"), "The possible items should be shown.");
        Assert.That(
            cut.FindAll("button").Any(button => button.TextContent.Contains("Remove")),
            Is.True,
            "The required items should be editable.");
    }

    private static Mock<IContext> CreateContext(IReadOnlyDictionary<Guid, ItemCrafting> craftingsById)
    {
        var context = new Mock<IContext>();
        context.Setup(c => c.GetByIdAsync(It.IsAny<Guid>(), typeof(ItemCrafting), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, Type _, CancellationToken _) => craftingsById.GetValueOrDefault(id));
        context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        return context;
    }

    private IRenderedComponent<Mixes> RenderPage()
    {
        var cut = this._context.Render<Mixes>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("238 - Test NPC")), TimeSpan.FromSeconds(10));
        return cut;
    }

    private IReadOnlyList<AngleSharp.Dom.IElement> GetSectionHeaders(IRenderedComponent<Mixes> cut)
        => cut.FindAll("div.collapsible-section__header");

    private AngleSharp.Dom.IElement FindSectionHeader(IRenderedComponent<Mixes> cut, string title)
        => this.GetSectionHeaders(cut).First(header => header.TextContent.Contains(title));

    private void ExpandAll(IRenderedComponent<Mixes> cut)
    {
        // Expand one collapsed section at a time; expanding a category reveals the headers of its mixes.
        for (var i = 0; i < 10; i++)
        {
            // The section shows a right chevron while it's collapsed and a bottom chevron when expanded.
            var collapsed = this.GetSectionHeaders(cut)
                .FirstOrDefault(header => header.QuerySelector("span.oi-chevron-right") is not null);
            if (collapsed is null)
            {
                return;
            }

            collapsed.Click();
        }

        Assert.Fail("The sections didn't collapse or expand as expected.");
    }
}
