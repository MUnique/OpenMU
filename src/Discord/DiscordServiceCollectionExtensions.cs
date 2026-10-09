// <copyright file="DiscordServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Discord.ChatBridge;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extensions to add the Discord integration to the services of a host.
/// </summary>
/// <remarks>
/// The host has to provide the <see cref="IServerProvider"/>, <see cref="IPersistenceContextProvider"/>,
/// <see cref="IFriendServer"/>, <see cref="IGuildServer"/> and <see cref="PlugInManager"/>.
/// </remarks>
public static class DiscordServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Discord bot, if a token is configured, and the <see cref="DiscordNotifier"/> as <see cref="IGameEventListener"/>,
    /// if webhooks or the bot are configured.
    /// </summary>
    /// <param name="services">The services to which the Discord integration is added.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="getServerTimeZone">The function which gets the time zone of the server, in which the schedules of the events are defined.</param>
    /// <returns>The same services, for chaining.</returns>
    public static IServiceCollection AddDiscord(this IServiceCollection services, DiscordSettings settings, Func<TimeZoneInfo> getServerTimeZone)
    {
        var culture = settings.GetCulture();
        if (settings.Bot.IsEnabled)
        {
            services
                .AddSingleton<IDiscordGameDataProvider>(provider => new DiscordGameDataProvider(
                    provider.GetRequiredService<IServerProvider>(),
                    provider.GetRequiredService<IPersistenceContextProvider>(),
                    provider.GetRequiredService<IFriendServer>(),
                    provider.GetRequiredService<IGuildServer>(),
                    provider.GetRequiredService<PlugInManager>(),
                    getServerTimeZone))
                .AddSingleton(provider => new DiscordCommands(provider.GetRequiredService<IDiscordGameDataProvider>(), culture, provider.GetRequiredService<ILogger<DiscordCommands>>()))
                .AddSingleton(provider => new DiscordAccountCommands(
                    new AccountLinkService(() => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration())),
                    culture,
                    provider.GetRequiredService<ILogger<DiscordAccountCommands>>()))

                // The event publisher of the all-in-one deployment depends on the bot, so it's resolved when it's needed.
                .AddSingleton(provider => new DiscordChatBridge(
                    settings.Bot.ChatBridge,
                    new GuildChatBindingStore(() => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration())),
                    new AccountLinkService(() => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration())),
                    provider.GetRequiredService<IGuildServer>(),
                    () => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration()),
                    () => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewGuildContext(),
                    () => provider.GetService<IEventPublisher>(),
                    provider.GetRequiredService<ILogger<DiscordChatBridge>>()))
                .AddSingleton(new DiscordChatCommands(culture))
                .AddSingleton(provider => new DiscordGameMasterCommands(
                    provider.GetRequiredService<IServerProvider>(),
                    new AccountLinkService(() => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration())),
                    () => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration()),
                    culture,
                    provider.GetRequiredService<ILogger<DiscordGameMasterCommands>>()))
                .AddSingleton(provider => new DiscordDirectMessages(
                    new AccountLinkService(() => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration())),
                    provider.GetRequiredService<IFriendServer>(),
                    () => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(new GameConfiguration()),
                    () => provider.GetRequiredService<IPersistenceContextProvider>().CreateNewFriendServerContext(),
                    serverId => provider.GetRequiredService<IServerProvider>().Servers.OfType<IGameServer>().FirstOrDefault(server => server.Id == serverId)?.Description,
                    culture))
                .AddSingleton(new DiscordStatusFormatter(culture))
                .AddSingleton(settings.Bot.LayoutFile is { Length: > 0 } layoutFile ? DiscordServerLayout.Load(layoutFile) : DiscordServerLayout.LoadDefault())
                .AddSingleton(provider => new DiscordServerProvisioner(provider.GetRequiredService<ILogger<DiscordServerProvisioner>>()))
                .AddSingleton(provider => new DiscordBot(
                    settings.Bot,
                    provider.GetRequiredService<DiscordCommands>(),
                    provider.GetRequiredService<DiscordAccountCommands>(),
                    provider.GetRequiredService<DiscordStatusFormatter>(),
                    provider.GetRequiredService<IDiscordGameDataProvider>(),
                    provider.GetRequiredService<DiscordServerLayout>(),
                    provider.GetRequiredService<DiscordServerProvisioner>(),
                    provider.GetRequiredService<DiscordChatBridge>(),
                    provider.GetRequiredService<DiscordChatCommands>(),
                    provider.GetRequiredService<DiscordGameMasterCommands>(),
                    provider.GetRequiredService<DiscordDirectMessages>(),
                    provider.GetRequiredService<ILogger<DiscordBot>>()))
                .AddHostedService(provider => provider.GetRequiredService<DiscordBot>())
                .AddSingleton<IGameEventListener>(provider => provider.GetRequiredService<DiscordBot>());
        }

        if (settings.IsNotificationEnabled)
        {
            services.AddSingleton<IGameEventListener>(provider => new DiscordNotifier(
                settings,
                new HttpClient(),
                settings.Bot.IsEnabled ? provider.GetRequiredService<DiscordBot>() : null,
                serverId => provider.GetRequiredService<IServerProvider>().Servers.OfType<IGameServer>().FirstOrDefault(server => server.Id == serverId)?.Description,
                guildId => provider.GetRequiredService<IGuildServer>().GetPersistentGuildNameAsync(guildId),
                provider.GetRequiredService<ILoggerFactory>()));
        }

        return services;
    }
}
