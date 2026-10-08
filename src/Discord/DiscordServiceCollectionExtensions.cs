// <copyright file="DiscordServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// if webhooks or channels of the bot are configured.
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
                .AddSingleton(new DiscordStatusFormatter(culture))
                .AddSingleton(provider => new DiscordBot(
                    settings.Bot,
                    provider.GetRequiredService<DiscordCommands>(),
                    provider.GetRequiredService<DiscordStatusFormatter>(),
                    provider.GetRequiredService<IDiscordGameDataProvider>(),
                    provider.GetRequiredService<ILogger<DiscordBot>>()))
                .AddHostedService(provider => provider.GetRequiredService<DiscordBot>());
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
