// <copyright file="EffectTimesPageTests.cs" company="MUnique">
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
    /// Gets or sets the factory of the operations of the test, with the given defaults.
    /// </summary>
    private Func<IEffectTimeDefaultsSource, EffectTimeOperations> CreateOperations { get; set; } = null!;

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
        var iceStorm = context.CreateNew<Skill>();
        iceStorm.Number = 39;
        iceStorm.Name = "Ice Storm";
        iceStorm.MagicEffectDef = this._custom;
        config.Skills.Add(iceStorm);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var source = new Mock<IDataSource<GameConfiguration>>();
        source.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(config);
        this._audit = new InMemoryAdminAuditRepository();
        this.CreateOperations = defaults => new EffectTimeOperations(persistence, source.Object, AdminOperationTestServices.CreateExecutor(audit: this._audit), new AdminChangeConfirmation(TimeProvider.System), new AdminWriteSettings { ReadOnly = false }, defaults);
        var operations = this.CreateOperations(new TestEffectTimeDefaultsSource());
        this._modal = new TestModalService(true);

        this._context = new BunitContext();
        this._context.JSInterop.Mode = JSRuntimeMode.Loose;
        this._context.Services.AddSingleton(operations);
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

        cut.FindAll("button").Single(b => b.TextContent == string.Format(CultureInfo.CurrentCulture, Resources.DefaultsFormat, "Season 6")).Click();
        cut.FindAll("button").Single(b => b.TextContent == Resources.SaveChanges).Click();

        cut.WaitForState(() => this._infiniteArrow.SendDuration, LoadTimeout);
        Assert.That(this._greaterDamage.SendDuration, Is.False);
        Assert.That(this._custom.SendDuration, Is.True);
    }

    /// <summary>
    /// Each effect shows the skills which cause it, and the search finds an effect by one of them.
    /// </summary>
    [Test]
    public void SearchFindsAnEffectByItsSkill()
    {
        var cut = this.RenderLoaded();

        cut.Find("input[type=search]").Input("storm");

        cut.WaitForState(() => cut.FindAll("tbody tr").Count == 1, LoadTimeout);
        var row = cut.FindAll("tbody tr").Single();
        Assert.That(row.TextContent, Does.Contain("Custom effect").And.Contain("Ice Storm"));
    }

    /// <summary>
    /// Without defaults, e.g. for data of an unknown data initialization, the page says so instead of showing
    /// an empty defaults button and column.
    /// </summary>
    [Test]
    public void PageWithoutDefaultsSaysSo()
    {
        this._context.Services.AddSingleton(this.CreateOperations(new NoEffectTimeDefaultsSource()));

        var cut = this._context.Render<EffectTimes>();

        cut.WaitForState(() => cut.Markup.Contains(Resources.DefaultsNotAvailable), LoadTimeout);
        Assert.Multiple(() =>
        {
            Assert.That(cut.FindAll("button").Select(b => b.TextContent), Has.None.Contains(string.Format(CultureInfo.CurrentCulture, Resources.DefaultsFormat, string.Empty).Trim()));
            Assert.That(cut.FindAll("thead th"), Has.Count.EqualTo(4));
            Assert.That(cut.FindAll("input[disabled]"), Is.Empty);
        });
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
}
