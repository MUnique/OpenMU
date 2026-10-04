// <copyright file="CaptionsPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="Captions"/> page, which guides through linking and applying caption changes.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CaptionsPageTests
{
    private const string SourceName = "CaptionsPageTestResources";

    private BunitContext _context = null!;

    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    private CultureInfo _previousUiCulture = null!;

    private static string SourceKey => LocalizedStringResources.CreateSourceKey(SourceName, nameof(Resources.Captions));

    /// <summary>
    /// Sets up the test context with services which are backed by an in-memory persistence.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        if (!LocalizedStringResources.TryGetName(Resources.ResourceManager, out _))
        {
            LocalizedStringResources.Register(SourceName, Resources.ResourceManager);
        }

        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        this._context = new BunitContext();
        this._context.Services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        this._context.Services.AddSingleton(new SetupService(this._persistenceContextProvider, plugInManager));
        this._context.Services.AddSingleton(new ConfigurationCaptionService(this._persistenceContextProvider, plugInManager, NullLoggerFactory.Instance));
        this._context.Services.AddScoped<NavigationHistory>();
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
    /// If no caption is linked yet, the page only offers to link them.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task NotLinked_OnlyOffersLinkingAsync()
    {
        await this.CreateConfigurationAsync(new LocalizedString("Captions")).ConfigureAwait(false);

        var cut = this._context.Render<Captions>();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(Resources.CaptionsNotLinkedTitle)));
        Assert.That(cut.FindAll("button").Select(b => b.TextContent.Trim()), Is.EqualTo(new[] { Resources.LinkBuiltInCaptions }));
        Assert.That(cut.FindAll("table"), Is.Empty);
        Assert.That(cut.Markup, Does.Not.Contain(Resources.AllLocalizationsInPlace));
    }

    /// <summary>
    /// If all linked captions equal their sources, the page says so.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task LinkedAndEqual_AllLocalizationsInPlaceAsync()
    {
        await this.CreateConfigurationAsync(Resources.ResourceManager.GetLocalizedString(nameof(Resources.Captions))).ConfigureAwait(false);

        var cut = this._context.Render<Captions>();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(Resources.AllLocalizationsInPlace)));
        Assert.That(cut.Markup, Does.Contain(string.Format(Resources.CaptionsLinkedCount, 1)));
        Assert.That(cut.FindAll("table"), Is.Empty);
        Assert.That(cut.Markup, Does.Not.Contain(Resources.CaptionsNotLinkedTitle));
    }

    /// <summary>
    /// Linked captions with differences are listed with the recommended changes selected, and can be applied.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task LinkedWithChanges_ReviewAndApplyAsync()
    {
        var monster = await this.CreateConfigurationAsync(new LocalizedString("Captions").WithSourceKey(SourceKey)).ConfigureAwait(false);

        var cut = this._context.Render<Captions>();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(Resources.CaptionsReviewTitle)));
        var rows = cut.FindAll("tbody tr");
        var row = rows.Single(r => r.TextContent.Contains("zh-CN", StringComparison.Ordinal));
        Assert.That(row.TextContent, Does.Contain(Resources.CaptionChangeKind_Missing));
        Assert.That(row.QuerySelector("input[type=checkbox]")!.HasAttribute("checked"), Is.True);

        cut.FindAll("button").Single(b => b.TextContent.Trim() == Resources.ApplySelectedChanges).Click();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(Resources.AllLocalizationsInPlace)));
        Assert.That(cut.Markup, Does.Contain(string.Format(Resources.AppliedCaptionChanges, rows.Count)));
        using var context = this._persistenceContextProvider.CreateNewContext();
        var storedMonster = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single().Monsters.Single();
        Assert.That(storedMonster.Designation.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("配置名称"));
        Assert.That(storedMonster.Designation.IsUnchangedSinceSourceStamp, Is.True);
    }

    private async Task<MonsterDefinition> CreateConfigurationAsync(LocalizedString designation)
    {
        using var context = this._persistenceContextProvider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var monster = context.CreateNew<MonsterDefinition>();
        monster.Designation = designation;
        configuration.Monsters.Add(monster);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return monster;
    }
}
