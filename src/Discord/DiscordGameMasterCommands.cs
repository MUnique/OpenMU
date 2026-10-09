// <copyright file="DiscordGameMasterCommands.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The slash commands for game masters, which change something in the game.
/// </summary>
/// <remarks>
/// They are allowed for Discord users who have the GM role of the layout, and who are linked to an account
/// whose selected character is a game master. Every use is reported to the staff.
/// </remarks>
public sealed class DiscordGameMasterCommands
{
    /// <summary>
    /// The color of the alerts for the staff (orange).
    /// </summary>
    private const int StaffAlertColor = 0xE67E22;

    private readonly IServerProvider _serverProvider;
    private readonly AccountLinkService _linkService;
    private readonly Func<IPlayerContext> _createContext;
    private readonly CultureInfo _culture;
    private readonly ILogger<DiscordGameMasterCommands> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordGameMasterCommands"/> class.
    /// </summary>
    /// <param name="serverProvider">The provider of the game servers.</param>
    /// <param name="linkService">The service of the account links.</param>
    /// <param name="createContext">The function which creates a persistence context.</param>
    /// <param name="culture">The culture of the answers.</param>
    /// <param name="logger">The logger.</param>
    public DiscordGameMasterCommands(IServerProvider serverProvider, AccountLinkService linkService, Func<IPlayerContext> createContext, CultureInfo culture, ILogger<DiscordGameMasterCommands> logger)
    {
        this._serverProvider = serverProvider;
        this._linkService = linkService;
        this._createContext = createContext;
        this._culture = culture;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the definitions of the commands.
    /// </summary>
    public static IReadOnlyList<DiscordCommandDefinition> Definitions { get; } =
    [
        new("announce", nameof(Resources.Command_Announce_Description), "text", nameof(Resources.Command_Announce_TextOption)),
        new("kick", nameof(Resources.Command_Kick_Description), "character", nameof(Resources.Command_Who_CharacterOption)),
        new("ban", nameof(Resources.Command_Ban_Description), "character", nameof(Resources.Command_Who_CharacterOption)),
    ];

    /// <summary>
    /// Determines whether the command is one of these commands.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns><c>true</c>, if the command is one of these commands.</returns>
    public static bool IsGameMasterCommand(string commandName) => Definitions.Any(definition => definition.Name == commandName);

    /// <summary>
    /// Executes a command, if the user is allowed to.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="argument">The value of the option of the command.</param>
    /// <param name="userId">The identifier of the Discord user.</param>
    /// <param name="userName">The name of the Discord user.</param>
    /// <param name="hasGameMasterRole">A value indicating whether the user has the GM role.</param>
    /// <returns>The answer.</returns>
    public async ValueTask<DiscordGameMasterAnswer> ExecuteAsync(string commandName, string argument, ulong userId, string userName, bool hasGameMasterRole)
    {
        if (!hasGameMasterRole || await this.GetGameMasterCharacterAsync(userId).ConfigureAwait(false) is not { } characterName)
        {
            return new(this.Answer(nameof(Resources.Gm_NotAllowed)), null);
        }

        argument = argument.Trim();
        var gameServers = this._serverProvider.Servers.OfType<IGameServer>().ToList();
        string resultKey;
        switch (commandName)
        {
            case "announce":
                foreach (var gameServer in gameServers)
                {
                    await gameServer.SendGlobalMessageAsync(argument, MessageType.GoldenCenter).ConfigureAwait(false);
                }

                resultKey = nameof(Resources.Gm_Announced);
                break;
            case "kick":
                resultKey = await AnyAsync(gameServers, server => server.DisconnectPlayerAsync(argument)).ConfigureAwait(false)
                    ? nameof(Resources.Gm_Kicked)
                    : nameof(Resources.Gm_NotOnline);
                break;
            default:
                resultKey = await AnyAsync(gameServers, server => server.BanPlayerAsync(argument)).ConfigureAwait(false)
                    ? nameof(Resources.Gm_Banned)
                    : nameof(Resources.Gm_NotOnline);
                break;
        }

        this._logger.LogInformation("The Discord user {user} ({userId}) as {character} used the command {command} {argument}.", userName, userId, characterName, commandName, argument);
        var alert = new DiscordEmbed(
            DiscordCommands.GetText(this._culture, nameof(Resources.StaffAlert_Title)),
            DiscordCommands.GetText(this._culture, nameof(Resources.StaffAlert_Text), DiscordMessageFormatter.Escape(userName), DiscordMessageFormatter.Escape(characterName), commandName, DiscordMessageFormatter.Escape(argument)),
            StaffAlertColor,
            DateTime.UtcNow,
            null);
        return new(this.Answer(resultKey, DiscordMessageFormatter.Escape(argument)), alert);
    }

    private static async ValueTask<bool> AnyAsync(IEnumerable<IGameServer> gameServers, Func<IGameServer, ValueTask<bool>> action)
    {
        foreach (var gameServer in gameServers)
        {
            if (await action(gameServer).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the name of the selected character of the linked account, if it's a game master.
    /// </summary>
    private async ValueTask<string?> GetGameMasterCharacterAsync(ulong userId)
    {
        if (await this._linkService.GetLinkByUserAsync(AccountLinkService.DiscordProvider, userId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false) is not { ExternalUserId: not null, CharacterName: { } characterName } link)
        {
            return null;
        }

        using var context = this._createContext();
        return await context.GetAccountIdByCharacterNameAsync(characterName).ConfigureAwait(false) == link.AccountId
               && await context.GetCharacterStatusAsync(characterName).ConfigureAwait(false) == CharacterStatus.GameMaster
            ? characterName
            : null;
    }

    private DiscordEmbed Answer(string resourceKey, params object[] args)
    {
        return new DiscordEmbed(
            DiscordCommands.GetText(this._culture, nameof(Resources.Gm_Title)),
            DiscordCommands.GetText(this._culture, resourceKey, args),
            DiscordCommands.AnswerColor,
            DateTime.UtcNow,
            null);
    }
}
