// <copyright file="IDiscordGameDataProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;

/// <summary>
/// Provides the data of the game which the Discord bot shows.
/// </summary>
public interface IDiscordGameDataProvider
{
    /// <summary>
    /// Gets the status of the game servers.
    /// </summary>
    /// <returns>The status of the game servers.</returns>
    IReadOnlyList<GameServerStatus> GetGameServers();

    /// <summary>
    /// Gets information about a character.
    /// </summary>
    /// <param name="name">The name of the character.</param>
    /// <returns>The information; or <see langword="null"/>, if the character doesn't exist.</returns>
    ValueTask<CharacterInfo?> GetCharacterAsync(string name);

    /// <summary>
    /// Gets information about a guild.
    /// </summary>
    /// <param name="name">The name of the guild.</param>
    /// <returns>The information; or <see langword="null"/>, if the guild doesn't exist.</returns>
    ValueTask<GuildInfo?> GetGuildAsync(string name);

    /// <summary>
    /// Gets the events which start in the next hours.
    /// </summary>
    /// <param name="culture">The culture of the names.</param>
    /// <returns>The events.</returns>
    ValueTask<IReadOnlyList<UpcomingEventInfo>> GetUpcomingEventsAsync(CultureInfo culture);

    /// <summary>
    /// Gets the best characters.
    /// </summary>
    /// <param name="count">The maximum number of characters.</param>
    /// <returns>The best characters.</returns>
    ValueTask<IReadOnlyList<CharacterInfo>> GetRankingAsync(int count);
}
