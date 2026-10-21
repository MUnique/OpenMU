// <copyright file="DiscordSetupPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.Persistence;
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
        gameConfigurationSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Mock.Of<IContext>());

        this._toastService = new Mock<IToastService>();
        this._context = new BunitContext();
        this._context.Services.AddSingleton(new DiscordConfigurationService(this._filePath, new ConfigurationBuilder().AddJsonFile(this._filePath).Build()));
        this._context.Services.AddSingleton(gameConfigurationSource.Object);
        this._context.Services.AddSingleton(this._toastService.Object);
        this._context.Services.AddSingleton(Mock.Of<ILogger<DiscordSetup>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordToken").Count > 0);
        cut.WaitForState(() => cut.Markup.Contains(Resources.DiscordPublisherInactive));
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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordPublisher").Count > 0);
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordPublisherInactive));
        Assert.That(cut.Find("#discordPublisher").HasAttribute("checked"), Is.False);

        cut.Find("#discordPublisher").Change(true);

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordPublisherActivated, null)));
        Assert.That(this._publisherConfiguration.IsActive, Is.True);
        Assert.That(cut.Markup, Does.Contain(Resources.DiscordPublisherActive));

        cut.Find("#discordPublisher").Change(false);

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordPublisherDeactivated, null)));
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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordToken").Count > 0);

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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordPublisher").Count > 0);
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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordToken").Count > 0);

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
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordServerId").Count > 0);

        cut.Find("#discordServerId").Change("my server");
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        this._toastService.Verify(t => t.ShowError(string.Format(Resources.DiscordInvalidId, "my server"), null));
        Assert.That(File.ReadAllText(this._filePath), Is.EqualTo(content));
    }

    /// <summary>
    /// A webhook which is removed in the page is removed from the file.
    /// </summary>
    [Test]
    public void RemovesWebhook()
    {
        var cut = this._context.Render<DiscordSetup>();
        cut.WaitForState(() => cut.FindAll("#discordToken").Count > 0);

        cut.FindAll("tbody button").Single(b => b.TextContent.Contains(Resources.Remove)).Click();
        cut.FindAll("button").First(b => b.TextContent.Contains(Resources.Save)).Click();

        cut.WaitForAssertion(() => this._toastService.Verify(t => t.ShowSuccess(Resources.DiscordSaved, null)));
        var discord = JsonNode.Parse(File.ReadAllText(this._filePath))!["Discord"]!;
        Assert.That(discord["Webhooks"]!.AsObject(), Is.Empty);
    }
}
