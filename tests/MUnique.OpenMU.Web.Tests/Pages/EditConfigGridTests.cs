// <copyright file="EditConfigGridTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

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
/// Tests for the <see cref="EditConfigGrid"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class EditConfigGridTests
{
    private BunitContext _context = null!;

    /// <summary>
    /// Sets up the test context with three monsters which are not in the order of their numbers.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        List<BasicModel.MonsterDefinition> monsters =
        [
            new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 3, Designation = "Zeta" },
            new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 1, Designation = "Alpha" },
            new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Number = 2, Designation = "Beta" },
        ];
        var gameConfiguration = new BasicModel.GameConfiguration();

        var dataSource = new Mock<IDataSource<GameConfiguration>>();
        dataSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameConfiguration);
        dataSource.Setup(s => s.IsSupporting(It.IsAny<Type>())).Returns(true);
        dataSource.Setup(s => s.GetAll(It.IsAny<Type>())).Returns(monsters);

        var modalService = Mock.Of<IModalService>();
        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.Configure<RequestLocalizationOptions>(options => options.AddSupportedUICultures("en"));
        this._context.Services.AddSingleton(dataSource.Object);
        this._context.Services.AddSingleton(Mock.Of<IPersistenceContextProvider>());
        this._context.Services.AddSingleton(modalService);
        this._context.Services.AddSingleton(new CreationPanelService(modalService));
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<EditConfigGrid>>());
        this._context.Services.AddSingleton(new LoadingOverlayService());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// The grid shows the number of the objects and orders them by it, not by their name.
    /// </summary>
    [Test]
    public void MonstersAreOrderedByNumber()
    {
        var cut = this._context.Render<EditConfigGrid>(parameters => parameters
            .Add(p => p.TypeString, typeof(MonsterDefinition).FullName!));

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Alpha")));

        var numbers = cut.FindAll("table.quickgrid tbody tr")
            .Select(row => row.QuerySelector("td:first-child")?.TextContent.Trim())
            .Where(text => !string.IsNullOrEmpty(text))
            .ToList();
        Assert.That(numbers, Is.EqualTo(new[] { "1", "2", "3" }));
    }

    /// <summary>
    /// The number is shown as its own sortable column.
    /// </summary>
    [Test]
    public void GridShowsNumberColumn()
    {
        var cut = this._context.Render<EditConfigGrid>(parameters => parameters
            .Add(p => p.TypeString, typeof(MonsterDefinition).FullName!));

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Alpha")));

        Assert.That(cut.Find("table.quickgrid thead").TextContent, Does.Contain("Number"));
    }
}
