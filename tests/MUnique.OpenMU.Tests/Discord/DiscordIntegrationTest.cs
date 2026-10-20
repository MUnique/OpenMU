// <copyright file="DiscordIntegrationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Discord;
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
}
