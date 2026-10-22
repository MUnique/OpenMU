// <copyright file="DiscordServiceCollectionExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

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
/// Tests for the <see cref="DiscordServiceCollectionExtensions"/>.
/// </summary>
[TestFixture]
public class DiscordServiceCollectionExtensionsTest
{
    /// <summary>
    /// Tests that nothing is added without configuration.
    /// </summary>
    [Test]
    public void NothingIsAddedWithoutConfiguration()
    {
        using var provider = CreateServices(new DiscordSettings()).BuildServiceProvider();

        Assert.That(provider.GetService<DiscordBot>(), Is.Null);
        Assert.That(provider.GetServices<IGameEventListener>(), Is.Empty);
    }

    /// <summary>
    /// Tests that only the notifier is added, when only webhooks are configured.
    /// </summary>
    [Test]
    public async Task NotifierIsAddedForWebhooksAsync()
    {
        var settings = new DiscordSettings();
        settings.Webhooks[DiscordChannelCategory.Events] = "https://discord.example/api/webhooks/1/token";
        await using var provider = CreateServices(settings).BuildServiceProvider();

        Assert.That(provider.GetService<DiscordBot>(), Is.Null);
        Assert.That(provider.GetServices<IGameEventListener>().Single(), Is.InstanceOf<DiscordNotifier>());
    }

    /// <summary>
    /// Tests that the bot is added as hosted service, and that the notifier posts through it.
    /// </summary>
    [Test]
    public async Task BotIsAddedWithTokenAsync()
    {
        var settings = new DiscordSettings();
        settings.Bot.Token = "token";
        settings.Bot.Channels[DiscordChannelCategory.Events] = 42;
        await using var provider = CreateServices(settings).BuildServiceProvider();

        var bot = provider.GetRequiredService<DiscordBot>();
        Assert.That(provider.GetServices<IHostedService>(), Does.Contain(bot));
        Assert.That(provider.GetServices<IGameEventListener>().Single(), Is.InstanceOf<DiscordNotifier>());
        Assert.That(bot.ServerState, Is.EqualTo(ServerState.Stopped));
    }

    private static IServiceCollection CreateServices(DiscordSettings settings)
    {
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.SetupGet(p => p.Servers).Returns(new List<IManageableServer>());
        return new ServiceCollection()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddSingleton(serverProvider.Object)
            .AddSingleton<IPersistenceContextProvider>(new InMemoryPersistenceContextProvider())
            .AddSingleton(Mock.Of<IFriendServer>())
            .AddSingleton(Mock.Of<IGuildServer>())
            .AddSingleton(new PlugInManager([], NullLoggerFactory.Instance, null, null))
            .AddDiscord(settings, () => TimeZoneInfo.Utc);
    }
}
