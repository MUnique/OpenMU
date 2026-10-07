// <copyright file="CashShopCatalogPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="CashShopCatalog"/> page and the <see cref="CashShopCatalogCheck"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CashShopCatalogPageTests
{
    private BunitContext _context = null!;

    private Mock<IContext> _persistenceContext = null!;

    private BasicModel.GameConfiguration _gameConfiguration = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context with a catalog of two packages.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        var configuration = new BasicModel.CashShopConfiguration { ScriptSaleZone = 512, ScriptYear = 2012, ScriptYearId = 84 };
        configuration.Packages.Add(CreatePackage(10, "Jewel bundle", isForSale: true, new ItemDefinition(), TimeSpan.Zero));
        configuration.Packages.Add(CreatePackage(20, "Pet for a day", isForSale: false, new ItemDefinition(), TimeSpan.FromDays(1)));
        this._gameConfiguration = new BasicModel.GameConfiguration { CashShopConfiguration = configuration };

        this._persistenceContext = new Mock<IContext>();
        this._persistenceContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var dataSource = new Mock<IDataSource<GameConfiguration>>();
        dataSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(this._persistenceContext.Object);
        dataSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(this._gameConfiguration);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(dataSource.Object);
        this._context.Services.AddSingleton(Mock.Of<ILookupController>());
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<CashShopCatalog>>());
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
    /// The list shows the packages, and why a package can't be sold.
    /// </summary>
    [Test]
    public void ListShowsPackagesAndIssues()
    {
        var cut = this._context.Render<CashShopCatalog>();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Jewel bundle")));
        Assert.That(cut.Markup, Does.Contain("Pet for a day"));
        Assert.That(cut.Markup, Does.Contain(string.Format(Resources.CashShopIssueTimeLimited, 21)));
    }

    /// <summary>
    /// No cell of the package grid is empty, because the shared grid style hides a row with an empty cell
    /// (<c>.quickgrid tbody tr:has(td:empty)</c>). Packages with unset flags, without name or without
    /// products would be invisible otherwise.
    /// </summary>
    [Test]
    public void GridHasNoEmptyCells()
    {
        this._gameConfiguration.CashShopConfiguration!.Packages.Add(new BasicModel.CashShopPackage { Id = Guid.NewGuid(), PackageSequence = 30 });

        var cut = this._context.Render<CashShopCatalog>();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Jewel bundle")));

        // The grid fills the page with rows whose cells are all empty; the other rows show the packages.
        static bool IsEmpty(AngleSharp.Dom.IElement cell) => cell.Children.Length == 0 && cell.TextContent.Length == 0;
        var packageRows = cut.FindAll("table.quickgrid tbody tr")
            .Where(row => row.QuerySelectorAll("td").Any(cell => !IsEmpty(cell)))
            .ToList();
        Assert.That(packageRows, Has.Count.EqualTo(3));
        Assert.That(packageRows.SelectMany(row => row.QuerySelectorAll("td")).Where(IsEmpty), Is.Empty);
    }

    /// <summary>
    /// A package is edited and saved.
    /// </summary>
    [Test]
    public void EditAndSavePackage()
    {
        var cut = this._context.Render<CashShopCatalog>();
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Jewel bundle")));

        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Edit)).Click();
        cut.Find("#isForSale").Change(false);
        cut.Find("form").Submit();

        var package = this._gameConfiguration.CashShopConfiguration!.Packages.First(p => p.PackageSequence == 10);
        Assert.That(package.IsForSale, Is.False);
        this._persistenceContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// The check finds the problems of a package, and decides whether it can be sold.
    /// </summary>
    [Test]
    public void CheckFindsIssues()
    {
        var configuration = new BasicModel.CashShopConfiguration();
        var options = CreatePackage(1, "Options", isForSale: true, new ItemDefinition(), TimeSpan.Zero);
        var missingItem = new BasicModel.CashShopProduct { Id = Guid.NewGuid(), PriceSequence = 12 };
        options.Products.Add(missingItem);
        var duplicate = CreatePackage(1, "Duplicate", isForSale: true, null, TimeSpan.Zero);
        var empty = new BasicModel.CashShopPackage { Id = Guid.NewGuid(), PackageSequence = 2, IsForSale = true };
        configuration.Packages.Add(options);
        configuration.Packages.Add(duplicate);
        configuration.Packages.Add(empty);

        Assert.That(CashShopCatalogCheck.IsSellable(options), Is.True, "one deliverable price option is enough");
        Assert.That(CashShopCatalogCheck.GetIssues(configuration, options), Is.EquivalentTo(new[]
        {
            string.Format(Resources.CashShopIssueDuplicatePackageSequence, 1),
            string.Format(Resources.CashShopIssueMissingItem, 12),
        }));

        options.IsBundle = true;
        Assert.That(CashShopCatalogCheck.IsSellable(options), Is.False, "a bundle needs all products");
        Assert.That(CashShopCatalogCheck.IsSellable(empty), Is.False);
        Assert.That(CashShopCatalogCheck.GetIssues(configuration, empty), Is.EqualTo(new[] { Resources.CashShopIssueNoProducts }));
    }

    /// <summary>
    /// A negative price makes a package or price option unsellable, because it would add coins to the buyer,
    /// and it's reported like a quantity below 1.
    /// </summary>
    [Test]
    public void CheckFindsNegativePricesAndInvalidQuantities()
    {
        var configuration = new BasicModel.CashShopConfiguration();
        var options = CreatePackage(1, "Options", isForSale: true, new ItemDefinition(), TimeSpan.Zero);
        var option = options.Products.Single();
        option.Price = -1;
        option.Quantity = 0;
        var bundle = CreatePackage(2, "Bundle", isForSale: true, new ItemDefinition(), TimeSpan.Zero);
        bundle.IsBundle = true;
        bundle.Price = -1;
        configuration.Packages.Add(options);
        configuration.Packages.Add(bundle);

        Assert.That(CashShopCatalogCheck.IsSellable(options), Is.False);
        Assert.That(CashShopCatalogCheck.IsSellable(bundle), Is.False);
        Assert.That(CashShopCatalogCheck.GetIssues(configuration, options), Is.EquivalentTo(new[]
        {
            string.Format(Resources.CashShopIssueNegativePrice, option.PriceSequence),
            string.Format(Resources.CashShopIssueInvalidQuantity, option.PriceSequence),
        }));
        Assert.That(CashShopCatalogCheck.GetIssues(configuration, bundle), Is.EqualTo(new[] { Resources.CashShopIssueNegativeBundlePrice }));
    }

    private static CashShopPackage CreatePackage(int sequence, string name, bool isForSale, ItemDefinition? item, TimeSpan duration)
    {
        // The ids are set like the persistence context does, because the page lists the packages by them.
        var package = new BasicModel.CashShopPackage
        {
            Id = Guid.NewGuid(),
            PackageSequence = sequence,
            Name = name,
            IsForSale = isForSale,
            CoinType = CashShopCoinType.WCoinC,
        };
        package.Products.Add(new BasicModel.CashShopProduct
        {
            Id = Guid.NewGuid(),
            ProductSequence = sequence,
            PriceSequence = sequence + 1,
            Price = 100,
            ItemDefinition = item,
            Duration = duration,
        });
        return package;
    }
}
