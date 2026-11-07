// <copyright file="EditItemDropsTests.cs" company="MUnique">
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
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="EditItemDrops"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class EditItemDropsTests
{
    private BunitContext _context = null!;

    private CultureInfo _previousUiCulture = null!;

    private BasicModel.GameMapDefinition _map = null!;

    /// <summary>
    /// Sets up the page with one map.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        var configuration = new BasicModel.GameConfiguration();
        this._map = new BasicModel.GameMapDefinition { Id = Guid.NewGuid(), Number = 0, Name = "Lorencia" };
        configuration.Maps.Add(this._map);

        var persistenceContext = new Mock<IContext>();
        persistenceContext.Setup(c => c.CreateNew<DropItemGroup>(It.IsAny<object?[]>()))
            .Returns(() => new BasicModel.DropItemGroup());
        var dataSource = new Mock<IDataSource<GameConfiguration>>();
        dataSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(persistenceContext.Object);
        dataSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(configuration);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.Configure<RequestLocalizationOptions>(options => options.AddSupportedUICultures("en"));
        this._context.Services.AddSingleton(dataSource.Object);
        this._context.Services.AddSingleton(Mock.Of<IPersistenceContextProvider>());
        this._context.Services.AddSingleton(Mock.Of<IModalService>());
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<EditItemDrops>>());
        this._context.Services.AddSingleton(new LoadingOverlayService());
        this._context.Services.AddSingleton(Mock.Of<ILookupController>());
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
    /// A new map drop group is assigned to all maps, so that it drops without further configuration.
    /// </summary>
    [Test]
    public void AddedMapGroupIsAssignedToAllMaps()
    {
        var cut = this._context.Render<EditItemDrops>();

        cut.WaitForAssertion(
            () => Assert.That(cut.FindAll("button").Any(button => button.TextContent.Contains("Add drop item group")), Is.True),
            TimeSpan.FromSeconds(10));

        var addButton = cut.FindAll("button").First(button => button.TextContent.Contains("Add drop item group"));
        addButton.Click();

        Assert.That(this._map.DropItemGroups, Has.Count.EqualTo(1));
    }
}
