// <copyright file="DiscordIntegrationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="DiscordIntegration"/>.
/// </summary>
[TestFixture]
public class DiscordIntegrationTest
{
    private const string WebhookKey = "Discord:Webhooks:Events";

    /// <summary>
    /// Tests that nothing is created without configuration.
    /// </summary>
    [Test]
    public async Task NothingIsCreatedWithoutConfigurationAsync()
    {
        var (integration, _) = await StartAsync(new Dictionary<string, string?>()).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            Assert.That(integration.Bot, Is.Null);
            Assert.That(integration.Listeners, Is.Empty);
            Assert.That(integration.ServerState, Is.EqualTo(ServerState.Stopped));
        }
    }

    /// <summary>
    /// Tests that the notifier is created again when the settings change, and removed when they are removed.
    /// </summary>
    [Test]
    public async Task NotifierIsCreatedAgainWhenTheSettingsChangeAsync()
    {
        var (integration, configuration) = await StartAsync(new Dictionary<string, string?> { [WebhookKey] = "https://discord.example/api/webhooks/1/token" }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            var notifier = integration.Listeners.Single();
            Assert.That(notifier, Is.InstanceOf<DiscordNotifier>());
            Assert.That(integration.Settings.Webhooks[DiscordChannelCategory.Events], Is.EqualTo("https://discord.example/api/webhooks/1/token"));

            await integration.ApplySettingsAsync().ConfigureAwait(false);
            Assert.That(integration.Listeners.Single(), Is.SameAs(notifier), "unchanged settings keep the notifier");

            configuration[WebhookKey] = "https://discord.example/api/webhooks/2/token";
            await integration.ApplySettingsAsync().ConfigureAwait(false);
            Assert.That(integration.Listeners.Single(), Is.Not.SameAs(notifier));

            configuration[WebhookKey] = string.Empty;
            await integration.ApplySettingsAsync().ConfigureAwait(false);
            Assert.That(integration.Listeners, Is.Empty);
        }
    }

    /// <summary>
    /// Tests that a reload of the configuration applies the changed settings.
    /// </summary>
    [Test]
    public async Task ReloadOfTheConfigurationAppliesTheSettingsAsync()
    {
        var (integration, configuration) = await StartAsync(new Dictionary<string, string?>()).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            configuration[WebhookKey] = "https://discord.example/api/webhooks/1/token";
            configuration.Reload();

            var timeout = DateTime.UtcNow.AddSeconds(10);
            while (integration.Listeners.Count == 0 && DateTime.UtcNow < timeout)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            Assert.That(integration.Listeners, Has.Count.EqualTo(1));
        }
    }

    /// <summary>
    /// Tests that a layout file which can't be read is reported as error of the settings.
    /// </summary>
    [Test]
    public async Task InvalidLayoutFileIsReportedAsync()
    {
        var (integration, _) = await StartAsync(new Dictionary<string, string?>
        {
            ["Discord:Bot:Token"] = "token",
            ["Discord:Bot:LayoutFile"] = "does-not-exist.json",
        }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            Assert.That(integration.Bot, Is.Null);
            Assert.That(integration.SettingsError, Is.Not.Null);
        }
    }

    /// <summary>
    /// Tests that a category without channel and webhook isn't posted.
    /// </summary>
    [Test]
    public async Task TestMessageWithoutChannelIsNotPostedAsync()
    {
        var (integration, _) = await StartAsync(new Dictionary<string, string?>()).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            var result = await integration.SendTestMessageAsync(DiscordChannelCategory.Events).ConfigureAwait(false);

            Assert.That(result.Outcome, Is.EqualTo(DiscordTestOutcome.NoChannel));
        }
    }

    /// <summary>
    /// Tests that the test message is posted by the webhook of the category, and that the answers of Discord are told apart.
    /// </summary>
    /// <param name="statusCode">The status code which Discord answers.</param>
    /// <param name="expected">The expected outcome.</param>
    [TestCase(HttpStatusCode.NoContent, DiscordTestOutcome.SentByWebhook)]
    [TestCase(HttpStatusCode.NotFound, DiscordTestOutcome.WebhookInvalid)]
    [TestCase(HttpStatusCode.Unauthorized, DiscordTestOutcome.WebhookInvalid)]
    [TestCase(HttpStatusCode.InternalServerError, DiscordTestOutcome.Failed)]
    public async Task TestMessageIsPostedByWebhookAsync(HttpStatusCode statusCode, DiscordTestOutcome expected)
    {
        var (integration, _) = await StartAsync(new Dictionary<string, string?> { ["Discord:Webhooks:WorldNews"] = "https://discord.example/api/webhooks/1/token" }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            var handler = new RecordingHandler(statusCode);
            integration.CreateHttpClient = () => new HttpClient(handler);

            var result = await integration.SendTestMessageAsync(DiscordChannelCategory.WorldNews).ConfigureAwait(false);

            Assert.That(result.Outcome, Is.EqualTo(expected));
            Assert.That(handler.RequestUri, Is.EqualTo(new Uri("https://discord.example/api/webhooks/1/token")));
            Assert.That(handler.Content, Does.Contain("WorldNews"));
        }
    }

    /// <summary>
    /// Tests that a webhook URL which isn't a URL is reported as invalid.
    /// </summary>
    [Test]
    public async Task TestMessageWithInvalidWebhookUrlAsync()
    {
        var (integration, _) = await StartAsync(new Dictionary<string, string?> { ["Discord:Webhooks:Notices"] = "not a url" }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            var result = await integration.SendTestMessageAsync(DiscordChannelCategory.Notices).ConfigureAwait(false);

            Assert.That(result.Outcome, Is.EqualTo(DiscordTestOutcome.WebhookInvalid));
        }
    }

    /// <summary>
    /// Tests that the pending removals of the role of linked players are kept when the bot is created again with changed settings,
    /// e.g. after an unlink in the admin panel while the bot couldn't connect.
    /// </summary>
    [Test]
    public async Task PendingRoleRevocationsSurviveChangedSettingsAsync()
    {
        var (integration, configuration) = await StartAsync(new Dictionary<string, string?> { ["Discord:Bot:Token"] = "invalid" }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            var bot = integration.Bot;
            Assert.That(bot, Is.Not.Null);
            Assert.That(bot!.ServerState, Is.Not.EqualTo(ServerState.Started));

            await integration.OnGameEventAsync(new AccountUnlinkedEvent(0, DateTime.UtcNow, AccountLinkService.DiscordProvider, "42")).ConfigureAwait(false);
            Assert.That(bot.PendingRoleRevocations.Count, Is.EqualTo(1));

            configuration["Discord:Bot:Token"] = "changed";
            await integration.ApplySettingsAsync().ConfigureAwait(false);
            var newBot = integration.Bot;
            Assert.That(newBot, Is.Not.Null.And.Not.SameAs(bot));
            Assert.That(newBot!.PendingRoleRevocations, Is.SameAs(bot.PendingRoleRevocations));
            Assert.That(newBot.PendingRoleRevocations.Count, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Tests that a pending removal keeps the Discord server and the name of the role, which the user got, when the settings change.
    /// </summary>
    [Test]
    public async Task PendingRoleRevocationKeepsItsDiscordServerAsync()
    {
        var (integration, configuration) = await StartAsync(new Dictionary<string, string?>
        {
            ["Discord:Bot:Token"] = "invalid",
            ["Discord:Bot:GuildId"] = "100",
        }).ConfigureAwait(false);
        await using (integration.ConfigureAwait(false))
        {
            await integration.OnGameEventAsync(new AccountUnlinkedEvent(0, DateTime.UtcNow, AccountLinkService.DiscordProvider, "42")).ConfigureAwait(false);

            var roleName = DiscordServerLayout.LoadDefault().Roles.Single(role => role.Key == DiscordServerLayout.LinkedRoleKey).Name;

            // Another server and another bot, which isn't in the old server: the removal must not get lost.
            configuration["Discord:Bot:GuildId"] = "200";
            configuration["Discord:Bot:Token"] = "another";
            await integration.ApplySettingsAsync().ConfigureAwait(false);
            Assert.That(integration.Bot!.PendingRoleRevocations.Entries, Is.EqualTo(new[] { new DiscordPendingRoleRevocation(42, 100, roleName) }));
        }
    }

    private static async Task<(DiscordIntegration Integration, IConfigurationRoot Configuration)> StartAsync(Dictionary<string, string?> values)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.SetupGet(p => p.Servers).Returns(new List<IManageableServer>());
        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddSingleton(serverProvider.Object)
            .AddSingleton<IPersistenceContextProvider>(new InMemoryPersistenceContextProvider())
            .AddSingleton(Mock.Of<IFriendServer>())
            .AddSingleton(Mock.Of<IGuildServer>())
            .AddSingleton(new PlugInManager([], NullLoggerFactory.Instance, null, null))
            .AddDiscordIntegration(configuration.GetSection(DiscordSettings.SectionName), () => TimeZoneInfo.Utc)
            .BuildServiceProvider();
        var integration = services.GetRequiredService<DiscordIntegration>();
        Assert.That(services.GetServices<IGameEventListener>(), Does.Contain(integration));
        await ((IHostedService)integration).StartAsync(CancellationToken.None).ConfigureAwait(false);
        return (integration, configuration);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public RecordingHandler(HttpStatusCode statusCode)
        {
            this._statusCode = statusCode;
        }

        public Uri? RequestUri { get; private set; }

        public string Content { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.RequestUri = request.RequestUri;
            this.Content = request.Content is { } content ? await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) : string.Empty;
            return new HttpResponseMessage(this._statusCode);
        }
    }
}
