// <copyright file="FakeGameDataProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Globalization;
using MUnique.OpenMU.Discord;

/// <summary>
/// A <see cref="IDiscordGameDataProvider"/> with fixed data.
/// </summary>
internal sealed class FakeGameDataProvider : IDiscordGameDataProvider
{
    /// <summary>
    /// Gets the game servers.
    /// </summary>
    public List<GameServerStatus> Servers { get; } = new();

    /// <summary>
    /// Gets the characters.
    /// </summary>
    public List<CharacterInfo> Characters { get; } = new();

    /// <summary>
    /// Gets the guilds.
    /// </summary>
    public List<GuildInfo> Guilds { get; } = new();

    /// <summary>
    /// Gets the upcoming events.
    /// </summary>
    public List<UpcomingEventInfo> Events { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the provider fails.
    /// </summary>
    public bool Fails { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<GameServerStatus> GetGameServers() => this.Servers;

    /// <inheritdoc />
    public ValueTask<CharacterInfo?> GetCharacterAsync(string name)
        => this.Fails ? throw new InvalidOperationException("Test") : ValueTask.FromResult(this.Characters.FirstOrDefault(c => c.Name == name));

    /// <inheritdoc />
    public ValueTask<GuildInfo?> GetGuildAsync(string name) => ValueTask.FromResult(this.Guilds.FirstOrDefault(g => g.Name == name));

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<UpcomingEventInfo>> GetUpcomingEventsAsync(CultureInfo culture) => ValueTask.FromResult<IReadOnlyList<UpcomingEventInfo>>(this.Events);

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<CharacterInfo>> GetRankingAsync(int count) => ValueTask.FromResult<IReadOnlyList<CharacterInfo>>(this.Characters.Take(count).ToList());
}
