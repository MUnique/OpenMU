// <copyright file="SetupPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Tests for the <see cref="Setup"/> page and its <see cref="Install"/> component.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SetupPageTests
{
    private const string FailureMessage = "The database is not reachable.";

    private BunitContext _context = null!;

    private Mock<IMigratableDatabaseContextProvider> _contextProvider = null!;

    private ServiceProvider _plugInServices = null!;

    private TestInitialization _initialization = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context with a database whose initialization and update fail.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        this._contextProvider = new Mock<IMigratableDatabaseContextProvider>();
        this._contextProvider.Setup(p => p.CanConnectToDatabaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        this._contextProvider.Setup(p => p.DatabaseExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        this._contextProvider.Setup(p => p.ReCreateDatabaseAsync(It.IsAny<bool>())).ReturnsAsync(Mock.Of<IDisposable>());
        this._contextProvider.Setup(p => p.ApplyAllPendingUpdatesAsync()).ThrowsAsync(new InvalidOperationException(FailureMessage));

        this._plugInServices = new ServiceCollection().AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance).BuildServiceProvider();
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, this._plugInServices, null);
        this._initialization = new TestInitialization();
        plugInManager.RegisterPlugInAtPlugInPoint<IDataInitializationPlugIn>(this._initialization);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        this._context.Services.AddSingleton(new SetupService(this._contextProvider.Object, plugInManager));
        this._context.Services.AddSingleton(Mock.Of<IServerProvider>(p => p.Servers == new List<IManageableServer>()));
        this._context.Services.AddSingleton(Mock.Of<IBackupService>());
        this._context.Services.AddSingleton(Mock.Of<IDatabaseSnapshotService>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        this._plugInServices.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// A failed installation is reported instead of being shown as finished. The form keeps
    /// the selected options, and the installation can be started again.
    /// </summary>
    [Test]
    public void FailedInstallationIsReportedAndCanBeRetried()
    {
        this._initialization.ShouldFail = true;
        var cut = this._context.Render<Install>();
        cut.Find("#testAccountsCheck").Input(true);

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.That(cut.Find(".alert-danger").TextContent, Does.Contain(FailureMessage)));
        Assert.That(cut.Markup, Does.Not.Contain(Resources.FinishedHaveFun));
        Assert.That(cut.Find("#testAccountsCheck").HasAttribute("checked"), Is.True);
        Assert.That(cut.Find($"#{this._initialization.Key}").HasAttribute("checked"), Is.True);

        this._initialization.ShouldFail = false;
        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(Resources.FinishedHaveFun)));
        Assert.That(this._initialization.CreatedTestAccounts, Is.True);
    }

    /// <summary>
    /// A failed update of the database schema is reported, instead of throwing in the event handler.
    /// </summary>
    [Test]
    public void FailedUpdateIsReported()
    {
        this._contextProvider.Setup(p => p.IsDatabaseUpToDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var cut = this._context.Render<Setup>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.That(cut.Find(".alert-danger").TextContent, Does.Contain(FailureMessage)));
        Assert.That(cut.Find("button.btn-primary").TextContent, Is.EqualTo(Resources.Update));
    }

    /// <summary>
    /// A failed update remains visible when the database disconnects during the update.
    /// </summary>
    [Test]
    public void FailedUpdateIsReportedWhenDatabaseDisconnects()
    {
        this._contextProvider.Setup(p => p.IsDatabaseUpToDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
        this._contextProvider.Setup(p => p.ApplyAllPendingUpdatesAsync()).Returns(() =>
        {
            this._contextProvider.Setup(p => p.CanConnectToDatabaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
            return Task.FromException(new InvalidOperationException(FailureMessage));
        });
        var cut = this._context.Render<Setup>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.That(cut.Find(".alert-danger").TextContent, Does.Contain(FailureMessage)));
    }

    /// <summary>
    /// A data initialization which fails on demand.
    /// </summary>
    [Guid("5D1E7C2B-3F4A-4E8B-9C6D-7A2B1E0F3C59")]
    private sealed class TestInitialization : IDataInitializationPlugIn
    {
        /// <summary>
        /// Gets or sets a value indicating whether the initialization should fail.
        /// </summary>
        public bool ShouldFail { get; set; }

        /// <summary>
        /// Gets a value indicating whether the last successful initialization created test accounts.
        /// </summary>
        public bool? CreatedTestAccounts { get; private set; }

        /// <inheritdoc />
        public string Key => "setup-page-test";

        /// <inheritdoc />
        public string Caption => "Setup page test";

        /// <inheritdoc />
        public Task CreateInitialDataAsync(byte numberOfGameServers, bool createTestAccounts)
        {
            if (this.ShouldFail)
            {
                throw new InvalidOperationException(FailureMessage);
            }

            this.CreatedTestAccounts = createTestAccounts;
            return Task.CompletedTask;
        }
    }
}
