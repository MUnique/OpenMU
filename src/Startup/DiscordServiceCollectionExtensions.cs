// <copyright file="DiscordServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Extensions to add the Discord integration to the services.
/// </summary>
internal static class DiscordServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="DiscordWebhookNotifier"/> as <see cref="IGameEventListener"/>,
    /// if at least one webhook is configured in the <see cref="DiscordWebhookSettings.SectionName"/> section.
    /// </summary>
    /// <param name="services">The services to which the notifier is added.</param>
    /// <param name="configuration">The configuration, e.g. with the environment variable <c>Discord__Webhooks__Events</c>.</param>
    /// <param name="gameServers">The game servers, to get their names.</param>
    /// <returns>The same services, for chaining.</returns>
    public static IServiceCollection AddDiscordWebhookNotifier(this IServiceCollection services, IConfiguration configuration, IDictionary<int, IGameServer> gameServers)
    {
        var settings = configuration.GetSection(DiscordWebhookSettings.SectionName).Get<DiscordWebhookSettings>();
        if (settings?.IsEnabled is not true)
        {
            return services;
        }

        return services.AddSingleton<IGameEventListener>(provider => new DiscordWebhookNotifier(
            settings,
            new HttpClient(),
            serverId => gameServers.Values.FirstOrDefault(server => server.Id == serverId)?.Description,
            guildId => provider.GetRequiredService<IGuildServer>().GetPersistentGuildNameAsync(guildId),
            provider.GetRequiredService<ILoggerFactory>()));
    }
}
