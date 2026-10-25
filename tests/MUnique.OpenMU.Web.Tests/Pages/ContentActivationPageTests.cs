// <copyright file="ContentActivationPageTests.cs" company="MUnique">
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
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.Mcp;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="ContentActivation"/> page, which changes the content through the shared <see cref="ContentActivationOperations"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public sealed class ContentActivationPageTests
{
    /// <summary>
    /// The time which the page may take to load its data, which is longer for the first test of a run.
    /// </summary>
    /// <remarks>
    /// The tests wait for a state instead of an assertion, because NUnit records a failed assertion
    /// even when bUnit retries it later.
    /// </remarks>
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(10);

    private BunitContext _context = null!;
    private CultureInfo _previousUiCulture = null!;
    private InMemoryAdminAuditRepository _audit = null!;
    private TestModalService _modal = null!;
    private GameMapDefinition _lorencia = null!;
    private GameMapDefinition _karutan = null!;
    private GameConfiguration _config = null!;
    private Mock<IDataSource<GameConfiguration>> _source = null!;

    /// <summary>
    /// Sets up the page with a configuration of two maps.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        var persistence = new InMemoryPersistenceContextProvider();
        using var context = persistence.CreateNewContext();
        var config = this._config = context.CreateNew<GameConfiguration>();
        this._lorencia = context.CreateNew<GameMapDefinition>();
        this._lorencia.Number = 0;
        this._lorencia.Name = "Lorencia";
        this._lorencia.IntroducedIn = GameVersion.Version029;
        config.Maps.Add(this._lorencia);
        this._karutan = context.CreateNew<GameMapDefinition>();
        this._karutan.Number = 80;
        this._karutan.Name = "Karutan 1";
        this._karutan.IntroducedIn = GameVersion.Season6Episode1;
        config.Maps.Add(this._karutan);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var source = this._source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(config);
        this._audit = new InMemoryAdminAuditRepository();
        var operations = new ContentActivationOperations(persistence, source.Object, AdminOperationTestServices.CreateExecutor(audit: this._audit), new AdminChangeConfirmation(TimeProvider.System), new AdminWriteSettings { ReadOnly = false });
        this._modal = new TestModalService(true);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(operations);
        this._context.Services.AddSingleton<IModalService>(this._modal);
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<ContentActivation>>());
        this._context.Services.AddSingleton<LoadingOverlayService>();
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// A change of a checkbox is pending until it's saved, then it's saved through the audited operation.
    /// </summary>
    [Test]
    public void PendingChangeIsSavedThroughTheOperation()
    {
        var cut = this._context.Render<ContentActivation>();
        cut.WaitForState(() => cut.Markup.Contains("Karutan 1"), LoadTimeout);

        cut.FindAll("tbody tr").Single(row => row.TextContent.Contains("Karutan 1")).QuerySelector("input")!.Change(false);
        Assert.That(this._karutan.IsActive, Is.True);

        cut.FindAll("button").Single(b => b.TextContent == Resources.SaveChanges).Click();

        cut.WaitForState(() => !this._karutan.IsActive, LoadTimeout);
        Assert.That(this._lorencia.IsActive, Is.True);
        Assert.That(this._modal.ShownDialogs, Has.Count.EqualTo(1));
        Assert.That(this._audit.ReadAsync(0, 100).AsTask().Result.Items.Single().Operation, Is.EqualTo("content_set_active"));
    }

    /// <summary>
    /// Restricting to a version is previewed, confirmed and applied by the operation.
    /// </summary>
    [Test]
    public void RestrictToGameVersion()
    {
        var cut = this._context.Render<ContentActivation>();
        cut.WaitForState(() => cut.Markup.Contains("Karutan 1"), LoadTimeout);

        cut.Find("select").Change(GameVersion.Season2.ToString());
        cut.FindAll("button").Single(b => b.TextContent == Resources.RestrictToGameVersion).Click();

        cut.WaitForState(() => !this._karutan.IsActive, LoadTimeout);
        Assert.That(this._lorencia.IsActive, Is.True);
        Assert.That(this._audit.ReadAsync(0, 100).AsTask().Result.Items.Single().Operation, Is.EqualTo("content_restrict_to_version"));
    }

    /// <summary>
    /// Setting the parameters again doesn't start another load, which could complete
    /// out of order and replace the data with an older snapshot.
    /// </summary>
    [Test]
    public void ParametersSetAgainDuringLoadDoesNotLoadAgain()
    {
        var owner = new TaskCompletionSource<GameConfiguration>(TaskCreationOptions.RunContinuationsAsynchronously);
        this._source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(() => new ValueTask<GameConfiguration>(owner.Task));
        var cut = this._context.Render<ContentActivation>();

        cut.Render();
        owner.SetResult(this._config);

        cut.WaitForState(() => cut.Markup.Contains("Karutan 1"), LoadTimeout);
        this._source.Verify(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
