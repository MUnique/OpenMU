// <copyright file="DiscordSetupPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Json;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests for the <see cref="DiscordSetup"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DiscordSetupPageTests
{
    private BunitContext _context = null!;
    private Mock<IToastService> _toastService = null!;
    private CultureInfo _previousUiCulture = null!;
    private bool _previousIsHostingEmbedded;
    private string _filePath = string.Empty;
    private PlugInConfiguration _publisherConfiguration = null!;
    private GameConfiguration _gameConfiguration = null!;

    /// <summary>
    /// Sets up the test context with a configuration file.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._previousIsHostingEmbedded = AdminPanelEnvironment.IsHostingEmbedded;
        AdminPanelEnvironment.IsHostingEmbedded = true;

        this._filePath = Path.GetTempFileName();
        File.WriteAllText(this._filePath, """{ "Serilog": { }, "Discord": { "Webhooks": { "Events": "https://discord.example/api/webhooks/1/secret" } } }""");

        var gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        var gameConfiguration = new Mock<GameConfiguration>();
        this._publisherConfiguration = new PlugInConfiguration { TypeId = typeof(GameEventPublisherPlugIn).GUID };
        gameConfiguration.SetupGet(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration> { this._publisherConfiguration });
        gameConfigurationSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(gameConfiguration.Object);
        this._gameConfiguration = gameConfiguration.Object;
        var dataContext = new Mock<IContext>();
        dataContext.Setup(context => context.CreateNew<PlugInConfiguration>(It.IsAny<object?[]>())).Returns(() => new PlugInConfiguration());
        gameConfigurationSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(dataContext.Object);

        this._toastService = new Mock<IToastService>();
        this._context = new BunitContext();
        this._context.Services.AddSingleton(new DiscordConfigurationService(this._filePath, new ConfigurationBuilder().AddJsonFile(this._filePath).Build()));
        this._context.Services.AddSingleton(gameConfigurationSource.Object);
        this._context.Services.AddSingleton(this._toastService.Object);
        this._context.Services.AddSingleton(Mock.Of<ILogger<DiscordSetup>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
        this._context.ComponentFactories.AddStub<Microsoft.AspNetCore.Components.Routing.NavigationLock>();
    }

    /// <summary>
    /// Restores the environment and deletes the configuration file.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
        File.Delete(this._filePath);
        AdminPanelEnvironment.IsHostingEmbedded = this._previousIsHostingEmbedded;
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// The entered settings are saved to the file, and a webhook which isn't entered again is kept.
    /// </summary>
    [Test]
    public void SavesEnteredSettings()
    {
        var cut = this.RenderLoaded();
        cut.WaitForState(() => cut.Markup.Contains(Resources.DiscordPublisherInactive));
        cut.Find("#discordMode").Change("bot");
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordInviteNeedsToken));

        cut.Find("#discordToken").Input("MTIzNDU2Nzg5MDEyMzQ1Njc4.abc.def");
        Assert.That(cut.Find("a[href^='https://discord.com/oauth2/authorize']").GetAttribute("href"), Does.Contain("client_id=123456789012345678"));
        cut.Find("#discordServerId").Change("987654321098765432");
        cut.FindAll("tbody input[inputmode='numeric']")[1].Change("42");
        cut.Find("#discordReadMessages").Change(true);
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordSaved, null)));
        var discord = JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!;
        Assert.That(discord["Bot"]!["Token"]?.ToString(), Is.EqualTo("MTIzNDU2Nzg5MDEyMzQ1Njc4.abc.def"));
        Assert.That(discord["Bot"]!["GuildId"]?.ToString(), Is.EqualTo("987654321098765432"));
        Assert.That(discord["Bot"]!["Channels"]!["CastleSiege"]?.ToString(), Is.EqualTo("42"));
        Assert.That(discord["Bot"]!["ChatBridge"]!["ReadMessages"]?.GetValue<bool>(), Is.True);
        Assert.That(discord["Webhooks"]!["Events"]?.ToString(), Is.EqualTo("https://discord.example/api/webhooks/1/secret"));
    }

    /// <summary>
    /// The game event publisher is activated and deactivated with the switch of the status.
    /// </summary>
    [Test]
    public void SwitchesGameEventPublisher()
    {
        var cut = this.RenderLoaded();
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordPublisherInactive));
        Assert.That(cut.Find("#discordPublisher").HasAttribute("checked"), Is.False);

        cut.Find("#discordPublisher").Change(true);

        Assert.That(this._publisherConfiguration.IsActive, Is.False, "publisher changes are staged until Save and apply");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();
        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordSaved, null)));
        Assert.That(this._publisherConfiguration.IsActive, Is.True);
        cut.WaitForState(() => !cut.Markup.Contains(Resources.DiscordUnsavedChanges));
        Assert.That(cut.Find("#discordPublisher").HasAttribute("checked"), Is.True);

        cut.Find("#discordPublisher").Change(false);

        Assert.That(this._publisherConfiguration.IsActive, Is.True);
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();
        cut.WaitForState(() => !this._publisherConfiguration.IsActive);
        Assert.That(this._publisherConfiguration.IsActive, Is.False);
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordPublisherInactive));
    }

    /// <summary>
    /// With the bot, the webhooks aren't needed and hidden, and the channels show the ones of the layout as default.
    /// </summary>
    [Test]
    public void BotShowsChannelsOfTheLayoutInsteadOfWebhooks()
    {
        File.WriteAllText(this._filePath, """{ "Discord": { "Bot": { "Token": "token" } } }""");
        var cut = this.RenderLoaded();

        Assert.That(cut.FindAll("tbody input[type=password]"), Is.Empty);
        Assert.That(cut.Markup, Does.Not.Contain(Resources.DiscordWebhookUrl));
        var channelInputs = cut.FindAll("tbody input[inputmode='numeric']");
        Assert.That(channelInputs[0].GetAttribute("placeholder"), Is.EqualTo(string.Format(Resources.DiscordDefaultChannel, "events")));
        Assert.That(channelInputs[3].GetAttribute("placeholder"), Is.EqualTo(string.Format(Resources.DiscordDefaultChannel, "announcements")));
        Assert.That(cut.Find("#discordStatusChannel").GetAttribute("placeholder"), Is.EqualTo(string.Format(Resources.DiscordDefaultChannel, "server-status")));
    }

    /// <summary>
    /// Unsaved changes are shown, and the tests are only possible after saving, because they use the saved settings.
    /// </summary>
    [Test]
    public void TestsNeedSavedSettings()
    {
        var cut = this.RenderLoaded();
        Assert.That(cut.Markup, Does.Not.Contain(Resources.DiscordUnsavedChanges));
        Assert.That(cut.FindAll("button").First(b => b.TextContent.Contains(Resources.DiscordTestAll)).HasAttribute("disabled"), Is.False);

        cut.Find("#discordLanguage").Change("de");

        cut.WaitForState(() => cut.Markup.Contains(Resources.DiscordUnsavedChanges));
        Assert.That(cut.FindAll("button").First(b => b.TextContent.Contains(Resources.DiscordTestAll)).HasAttribute("disabled"), Is.True);

        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordSaved, null)));
        cut.WaitForState(() => !cut.Markup.Contains(Resources.DiscordUnsavedChanges));
    }

    /// <summary>
    /// The channels and roles which the setup creates are shown before.
    /// </summary>
    [Test]
    public void ShowsWhatTheSetupCreates()
    {
        var cut = this.RenderLoaded();

        cut.Find("#discordMode").Change("bot");
        cut.WaitForState(() => cut.FindAll("details").Any(d => d.TextContent.Contains(Resources.DiscordLayoutPreview)));
        var preview = cut.FindAll("details").First(d => d.TextContent.Contains(Resources.DiscordLayoutPreview)).TextContent;
        Assert.That(preview, Does.Contain(string.Format(Resources.DiscordLayoutRoles, "GM, Linked Player")));
        Assert.That(preview, Does.Contain("#events"));
        Assert.That(preview, Does.Contain(string.Format(Resources.DiscordLayoutVisibleTo, "GM")));
    }

    /// <summary>
    /// An invalid identifier is not saved.
    /// </summary>
    [Test]
    public void DoesNotSaveInvalidIdentifier()
    {
        var content = File.ReadAllText(this._filePath);
        var cut = this.RenderLoaded();

        cut.Find("#discordMode").Change("bot");
        cut.WaitForElement("#discordServerId");
        cut.Find("#discordServerId").Change("my server");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowError(string.Format(Resources.DiscordInvalidId, "my server"), null)));
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(content));
    }

    /// <summary>
    /// A webhook which is removed in the page is removed from the file.
    /// </summary>
    [Test]
    public void RemovesWebhook()
    {
        var cut = this.RenderLoaded();

        cut.FindAll("tbody button").Single(b => b.TextContent.Contains(Resources.Remove)).Click();
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordSaved, null)));
        var discord = JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!;
        Assert.That(discord["Webhooks"]!.AsObject(), Is.Empty);
    }

    /// <summary>
    /// Webhook-only setup does not present incomplete bot steps or bot-only controls.
    /// </summary>
    [Test]
    public void WebhookModeSkipsBotSetup()
    {
        var cut = this.RenderLoaded();
        Assert.That(cut.FindAll("#discordToken"), Is.Empty);
        Assert.That(cut.FindAll("#discordServerId"), Is.Empty);
        Assert.That(cut.FindAll("#discordReadMessages"), Is.Empty);
        Assert.That(cut.Markup, Does.Not.Contain(Resources.DiscordStatusNotConfigured));
    }

    /// <summary>
    /// Numeric validation does not let browser minimum attributes be bypassed by saving.
    /// </summary>
    [Test]
    public void InvalidQueueSizeIsNotSaved()
    {
        var cut = this.RenderLoaded();
        var original = File.ReadAllText(this._filePath);
        cut.Find("#discordQueuedMessages").Change("0");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();
        Assert.That(cut.FindAll(".invalid-feedback"), Is.Not.Empty);
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(original));
    }

    /// <summary>
    /// A category can be disabled without deleting its configured webhook.
    /// </summary>
    [Test]
    public void DisablingCategoryKeepsItsDestination()
    {
        var cut = this.RenderLoaded();
        cut.Find("#discordEnabled-Events").Change(false);
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();
        cut.WaitForState(() => !cut.Markup.Contains(Resources.DiscordUnsavedChanges));
        var settings = JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!;
        Assert.That(settings["DisabledNotifications"]!.AsArray().Select(node => node!.ToString()), Does.Contain("Events"));
        Assert.That(settings["Webhooks"]!["Events"], Is.Not.Null);
    }

    /// <summary>
    /// Discard requires confirmation and leaves settings untouched when cancelled.
    /// </summary>
    [Test]
    public void DiscardingRequiresConfirmation()
    {
        var cut = this.RenderLoaded();
        cut.Find("#discordLanguage").Change("de");
        cut.WaitForState(() => cut.Markup.Contains("Discard changes"));
        cut.FindAll("button").Single(button => button.TextContent.Contains("Discard changes")).Click();
        Assert.That(cut.FindAll("[role=alertdialog]"), Has.Count.EqualTo(1));
        cut.FindAll("button").Single(button => button.TextContent.Contains("Keep editing")).Click();
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordUnsavedChanges));
        Assert.That(cut.Find("#discordLanguage").GetAttribute("value"), Is.EqualTo("de"));
    }

    /// <summary>
    /// Common event toggles and filters are saved with the rest of the page.
    /// </summary>
    [Test]
    public async Task SavesEventSelectionAndFiltersAsync()
    {
        var cut = this.RenderLoaded();
        cut.Find("#discordPublish-PublishBossKills").Change(false);
        cut.Find("#discordLevels").Change("100, 400");
        await cut.FindAll("button").First(button => button.TextContent.Contains(Resources.Save)).ClickAsync(new MouseEventArgs()).ConfigureAwait(false);
        this._toastService.Verify(service => service.ShowSuccess(Resources.DiscordSaved, null));
        Assert.That(this._publisherConfiguration.CustomConfiguration, Is.Not.Null);
        var publisher = this._publisherConfiguration.GetConfiguration<GameEventPublisherConfiguration>(new IdReferenceHandler())!;
        Assert.That(publisher.PublishBossKills, Is.False);
        Assert.That(publisher.LevelMilestones, Is.EqualTo(new[] { 100, 400 }));
    }

    /// <summary>
    /// Settings initialized with reference metadata remain editable and retain their filters.
    /// </summary>
    [Test]
    public void LoadsInitializedPublisherSettings()
    {
        this._publisherConfiguration.SetConfiguration(new GameEventPublisherConfiguration
        {
            BossMonsterNumbers = new List<short> { 275 },
            LevelMilestones = new List<int> { 100, 400 },
        }, new IdReferenceHandler());
        var cut = this.RenderLoaded();
        Assert.That(cut.Find("#discordBosses").GetAttribute("value"), Is.EqualTo("275"));
        Assert.That(cut.Find("#discordLevels").GetAttribute("value"), Is.EqualTo("100, 400"));
        Assert.That(cut.FindAll("#discordPublisher"), Has.Count.EqualTo(1));
    }

    /// <summary>
    /// A fresh server can configure publishing even when it has no publisher configuration yet.
    /// </summary>
    [Test]
    public void CreatesMissingPublisherConfigurationWhenSaving()
    {
        this._gameConfiguration.PlugInConfigurations.Clear();
        var cut = this.RenderLoaded();
        Assert.That(cut.Find("#discordMode").GetAttribute("value"), Is.EqualTo("webhooks"));
        cut.Find("#discordPublisher").Change(true);
        cut.FindAll("button").First(button => button.TextContent.Contains(Resources.Save)).Click();
        cut.WaitForAssertion(() => Assert.That(this._gameConfiguration.PlugInConfigurations.Single().IsActive, Is.True));
        Assert.That(this._gameConfiguration.PlugInConfigurations.Single().TypeId, Is.EqualTo(typeof(GameEventPublisherPlugIn).GUID));
    }

    /// <summary>
    /// Publishing alone does not mark notification delivery as verified.
    /// </summary>
    [Test]
    public void ActivePublisherDoesNotVerifyDelivery()
    {
        this._publisherConfiguration.IsActive = true;
        var cut = this.RenderLoaded();
        var header = cut.FindAll(".card-header").Single(element => element.TextContent.Contains(Resources.DiscordStep4Title));
        Assert.That(header.TextContent, Does.Not.Contain(Resources.DiscordStepDone));
    }

    /// <summary>
    /// Switching to webhooks keeps the saved token for later bot use.
    /// </summary>
    [Test]
    public async Task WebhookModeKeepsSavedTokenAsync()
    {
        File.WriteAllText(this._filePath, """{ "Discord": { "Bot": { "Token": "saved-token" } } }""");
        var cut = this.RenderLoaded();
        cut.Find("#discordMode").Change("webhooks");
        await cut.FindAll("button").First(button => button.TextContent.Contains(Resources.Save)).ClickAsync(new MouseEventArgs()).ConfigureAwait(false);
        this._toastService.Verify(service => service.ShowSuccess(Resources.DiscordSaved, null));
        var settings = JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!;
        Assert.That(settings["Bot"]!["Token"]!.GetValue<string>(), Is.EqualTo("saved-token"));
        Assert.That(settings["UseBot"]!.GetValue<bool>(), Is.False);
    }

    /// <summary>
    /// Mode selection survives saving before any webhook has been entered.
    /// </summary>
    [Test]
    public void WebhookModePersistsWithoutDestinations()
    {
        File.WriteAllText(this._filePath, """{ "Discord": { } }""");
        var cut = this.RenderLoaded();
        cut.Find("#discordMode").Change("webhooks");
        cut.FindAll("button").First(button => button.TextContent.Contains(Resources.Save)).Click();
        cut.WaitForState(() => !cut.Markup.Contains(Resources.DiscordUnsavedChanges));
        Assert.That(cut.Find("#discordMode").GetAttribute("value"), Is.EqualTo("webhooks"));
        Assert.That(cut.FindAll("#discordToken"), Is.Empty);
        Assert.That(JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!["UseBot"]!.GetValue<bool>(), Is.False);
    }

    /// <summary>
    /// Renders the page and waits until it's loaded completely: the settings, then the state of the game event
    /// publisher and the bound chats. Before, an input might be changed while the page still loads.
    /// </summary>
    private IRenderedComponent<DiscordSetup> RenderLoaded()
    {
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordPublisher").Count > 0);
        return cut;
    }
}
