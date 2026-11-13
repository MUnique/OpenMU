// <copyright file="EffectTimesPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.Mcp;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="EffectTimes"/> page, which changes whether effects send their time through the shared <see cref="EffectTimeOperations"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public sealed class EffectTimesPageTests
{
    /// <summary>
    /// The time which the page may take to load its data, which is longer for the first test of a run.
    /// </summary>
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(10);

    private BunitContext _context = null!;
    private CultureInfo _previousUiCulture = null!;
    private InMemoryAdminAuditRepository _audit = null!;
    private TestModalService _modal = null!;
    private MagicEffectDefinition _greaterDamage = null!;
    private MagicEffectDefinition _infiniteArrow = null!;
    private MagicEffectDefinition _custom = null!;

    /// <summary>
    /// Sets up the page with three effects. In the Season 6 defaults of the test, Greater Damage (1) doesn't send
    /// its time and Infinite Arrow (6) does; they don't know the custom effect (99).
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");

        var persistence = new InMemoryPersistenceContextProvider();
        using var context = persistence.CreateNewContext();
        var config = context.CreateNew<GameConfiguration>();
        this._greaterDamage = AddEffect(context, config, 1, "Greater Damage", true);
        this._infiniteArrow = AddEffect(context, config, 6, "Infinite Arrow", false);
        this._custom = AddEffect(context, config, 99, "Custom effect", true);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(config);
        this._audit = new InMemoryAdminAuditRepository();
        var operations = new EffectTimeOperations(persistence, source.Object, AdminOperationTestServices.CreateExecutor(audit: this._audit), new AdminChangeConfirmation(TimeProvider.System), new AdminWriteSettings { ReadOnly = false });
        this._modal = new TestModalService(true);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(operations);
        this._context.Services.AddSingleton<EffectTimeDefaults>(new TestDefaults());
        this._context.Services.AddSingleton<IModalService>(this._modal);
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<EffectTimes>>());
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
    /// A change of a checkbox is pending until it's saved, then it's confirmed and saved through the audited operation.
    /// </summary>
    [Test]
    public void PendingChangeIsSavedThroughTheOperation()
    {
        var cut = this.RenderLoaded();

        cut.FindAll("tbody tr").Single(row => row.TextContent.Contains("Infinite Arrow")).QuerySelector("input")!.Change(true);
        Assert.That(this._infiniteArrow.SendDuration, Is.False);

        cut.FindAll("button").Single(b => b.TextContent == Resources.SaveChanges).Click();

        cut.WaitForState(() => this._infiniteArrow.SendDuration, LoadTimeout);
        Assert.That(this._greaterDamage.SendDuration, Is.True);
        Assert.That(this._modal.ShownDialogs, Is.EqualTo(new[] { Resources.EffectTimes }));
        Assert.That(this._audit.ReadAsync(0, 100).AsTask().Result.Items.Single().Operation, Is.EqualTo("effect_times_set"));
    }

    /// <summary>
    /// "All off" switches every shown effect off.
    /// </summary>
    [Test]
    public void AllOffSwitchesEveryShownEffect()
    {
        var cut = this.RenderLoaded();

        cut.FindAll("button").Single(b => b.TextContent == Resources.AllOff).Click();
        cut.FindAll("button").Single(b => b.TextContent == Resources.SaveChanges).Click();

        cut.WaitForState(() => !this._greaterDamage.SendDuration, LoadTimeout);
        Assert.That(this._custom.SendDuration, Is.False);
        Assert.That(this._infiniteArrow.SendDuration, Is.False);
    }

    /// <summary>
    /// "Season 6 defaults" sets each effect as in the Season 6 data, and keeps effects which Season 6 doesn't know.
    /// </summary>
    [Test]
    public void SeasonSixDefaultsSetsTheKnownEffects()
    {
        var cut = this.RenderLoaded();

        cut.FindAll("button").Single(b => b.TextContent == Resources.SeasonSixDefaults).Click();
        cut.FindAll("button").Single(b => b.TextContent == Resources.SaveChanges).Click();

        cut.WaitForState(() => this._infiniteArrow.SendDuration, LoadTimeout);
        Assert.That(this._greaterDamage.SendDuration, Is.False);
        Assert.That(this._custom.SendDuration, Is.True);
    }

    /// <summary>
    /// Renders the page and waits until the effects and the Season 6 defaults are shown, so that no later render
    /// replaces the elements which the test uses.
    /// </summary>
    /// <returns>The rendered page.</returns>
    private IRenderedComponent<EffectTimes> RenderLoaded()
    {
        var cut = this._context.Render<EffectTimes>();
        cut.WaitForState(() => cut.Markup.Contains("Infinite Arrow") && cut.FindAll("input[disabled]").Count == 2, LoadTimeout);
        return cut;
    }

    private static MagicEffectDefinition AddEffect(IContext context, GameConfiguration config, short number, string name, bool sendDuration)
    {
        var effect = context.CreateNew<MagicEffectDefinition>();
        effect.Number = number;
        effect.Name = name;
        effect.SendDuration = sendDuration;
        config.MagicEffects.Add(effect);
        return effect;
    }

    /// <summary>
    /// Defaults of the test, so that the page doesn't create the real Season 6 data in memory, which is slow
    /// when many tests run at once. The real Season 6 defaults are tested with the data initialization.
    /// </summary>
    private sealed class TestDefaults() : EffectTimeDefaults(new PlugInManager(null, NullLoggerFactory.Instance, null, null), NullLoggerFactory.Instance)
    {
        /// <inheritdoc />
        public override ValueTask<IReadOnlyDictionary<short, bool>> GetAsync(string initializationKey)
        {
            IReadOnlyDictionary<short, bool> defaults = initializationKey == Persistence.Initialization.VersionSeasonSix.DataInitialization.Id
                ? new Dictionary<short, bool> { [1] = false, [6] = true }
                : new Dictionary<short, bool>();
            return ValueTask.FromResult(defaults);
        }
    }
}
