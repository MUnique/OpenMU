// <copyright file="DiscordCommands.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The slash commands of the Discord bot. They only show information and don't change anything in the game.
/// </summary>
public sealed class DiscordCommands
{
    /// <summary>
    /// The color of the answers (blue).
    /// </summary>
    internal const int AnswerColor = 0x3498DB;

    /// <summary>
    /// The number of characters which the ranking shows.
    /// </summary>
    /// <remarks>
    /// A message of Discord has a limited length, so the ranking can't be arbitrarily long.
    /// </remarks>
    internal const int RankingSize = 10;

    private readonly IDiscordGameDataProvider _data;
    private readonly CultureInfo _culture;
    private readonly ILogger<DiscordCommands> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordCommands"/> class.
    /// </summary>
    /// <param name="data">The provider of the data.</param>
    /// <param name="culture">The culture of the answers.</param>
    /// <param name="logger">The logger.</param>
    public DiscordCommands(IDiscordGameDataProvider data, CultureInfo culture, ILogger<DiscordCommands> logger)
    {
        this._data = data;
        this._culture = culture;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the definitions of the commands.
    /// </summary>
    public static IReadOnlyList<DiscordCommandDefinition> Definitions { get; } =
    [
        new("online", nameof(Resources.Command_Online_Description)),
        new("who", nameof(Resources.Command_Who_Description), "character", nameof(Resources.Command_Who_CharacterOption)),
        new("guild", nameof(Resources.Command_Guild_Description), "name", nameof(Resources.Command_Guild_GuildOption)),
        new("events", nameof(Resources.Command_Events_Description)),
        new("rank", nameof(Resources.Command_Rank_Description)),
    ];

    /// <summary>
    /// Gets the culture of the answers.
    /// </summary>
    public CultureInfo Culture => this._culture;

    /// <summary>
    /// Executes a command.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="argument">The value of the option of the command, if it has one.</param>
    /// <returns>The answer.</returns>
    public async ValueTask<DiscordEmbed> ExecuteAsync(string commandName, string? argument)
    {
        try
        {
            return commandName switch
            {
                "online" => this.Online(),
                "who" => await this.WhoAsync(argument ?? string.Empty).ConfigureAwait(false),
                "guild" => await this.GuildAsync(argument ?? string.Empty).ConfigureAwait(false),
                "events" => await this.EventsAsync().ConfigureAwait(false),
                "rank" => await this.RankAsync().ConfigureAwait(false),
                _ => this.Answer(commandName, this.Text(nameof(Resources.Command_Failed))),
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when executing the Discord command {command}.", commandName);
            return this.Answer(commandName, this.Text(nameof(Resources.Command_Failed)));
        }
    }

    /// <summary>
    /// Gets the text of a resource in the culture of the answers.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The text.</returns>
    internal string Text(string resourceKey, params object[] args)
    {
        var text = Resources.ResourceManager.GetString(resourceKey, this._culture) ?? resourceKey;
        return args.Length == 0 ? text : string.Format(this._culture, text, args);
    }

    private DiscordEmbed Online()
    {
        var servers = this._data.GetGameServers();
        var lines = servers.Select(server => server.IsOnline
            ? this.Text(nameof(Resources.Online_ServerLine), DiscordMessageFormatter.Escape(server.Name), server.CurrentPlayers, server.MaximumPlayers)
            : this.Text(nameof(Resources.Online_ServerOffline), DiscordMessageFormatter.Escape(server.Name)));
        var total = servers.Where(server => server.IsOnline).Sum(server => server.CurrentPlayers);
        return this.Answer(
            this.Text(nameof(Resources.Online_Title)),
            string.Join('\n', lines.Append(this.Text(nameof(Resources.Online_Total), total))));
    }

    private async ValueTask<DiscordEmbed> WhoAsync(string name)
    {
        if (await this._data.GetCharacterAsync(name).ConfigureAwait(false) is not { } character)
        {
            return this.Answer(name, this.Text(nameof(Resources.Character_NotFound), DiscordMessageFormatter.Escape(name)));
        }

        var description = new StringBuilder()
            .AppendLine(this.Translate(character.ClassName))
            .AppendLine(this.GetLevels(character))
            .Append(character.OnlineServerName is { } serverName
                ? this.Text(nameof(Resources.Character_OnlineOn), DiscordMessageFormatter.Escape(serverName))
                : this.Text(nameof(Resources.Character_Offline)));
        return this.Answer(character.Name, description.ToString());
    }

    private async ValueTask<DiscordEmbed> GuildAsync(string name)
    {
        if (await this._data.GetGuildAsync(name).ConfigureAwait(false) is not { } guild)
        {
            return this.Answer(name, this.Text(nameof(Resources.Guild_NotFound), DiscordMessageFormatter.Escape(name)));
        }

        var description = new StringBuilder();
        if (guild.MasterName is { } masterName)
        {
            description.AppendLine(this.Text(nameof(Resources.Guild_Master), DiscordMessageFormatter.Escape(masterName)));
        }

        description
            .AppendLine(this.Text(nameof(Resources.Guild_Members), guild.MemberCount))
            .Append(this.Text(nameof(Resources.Guild_OnlineMembers), guild.OnlineMemberNames.Count > 0 ? string.Join(", ", guild.OnlineMemberNames.Select(DiscordMessageFormatter.Escape)) : "-"));
        return this.Answer(guild.Name, description.ToString());
    }

    private async ValueTask<DiscordEmbed> EventsAsync()
    {
        var events = await this._data.GetUpcomingEventsAsync(this._culture).ConfigureAwait(false);
        var description = events.Count == 0
            ? this.Text(nameof(Resources.Events_None))
            : string.Join('\n', events.Select(e => $"{DiscordMessageFormatter.RelativeTime(e.StartsAtUtc)} {DiscordMessageFormatter.Escape(e.Name)}"));
        return this.Answer(this.Text(nameof(Resources.Events_Title)), description);
    }

    private async ValueTask<DiscordEmbed> RankAsync()
    {
        var ranking = await this._data.GetRankingAsync(RankingSize).ConfigureAwait(false);
        var description = ranking.Count == 0
            ? this.Text(nameof(Resources.Rank_Empty))
            : string.Join('\n', ranking.Select((character, index) => $"{index + 1}. **{DiscordMessageFormatter.Escape(character.Name)}** ({this.Translate(character.ClassName)}) – {this.GetLevels(character)}"));
        return this.Answer(this.Text(nameof(Resources.Rank_Title)), description);
    }

    private string GetLevels(CharacterInfo character)
    {
        var parts = new List<string> { this.Text(nameof(Resources.Character_Level), character.Level) };
        if (character.MasterLevel > 0)
        {
            parts.Add(this.Text(nameof(Resources.Character_MasterLevel), character.MasterLevel));
        }

        if (character.Resets > 0)
        {
            parts.Add(this.Text(nameof(Resources.Character_Resets), character.Resets));
        }

        return string.Join(", ", parts);
    }

    private string Translate(string localizedValue)
    {
        return DiscordMessageFormatter.Escape(new LocalizedString(localizedValue).GetTranslation(this._culture) ?? localizedValue);
    }

    private DiscordEmbed Answer(string title, string description) => new(title, description, AnswerColor, DateTime.UtcNow, null);
}
