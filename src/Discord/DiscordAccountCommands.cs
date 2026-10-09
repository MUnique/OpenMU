// <copyright file="DiscordAccountCommands.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.GameLogic.AccountLinking;

/// <summary>
/// The slash commands with which Discord users link themselves to their game account.
/// Their answers are only shown to the user who used them.
/// </summary>
public sealed class DiscordAccountCommands
{
    private const string Provider = AccountLinkService.DiscordProvider;

    private readonly AccountLinkService _linkService;
    private readonly CultureInfo _culture;
    private readonly ILogger<DiscordAccountCommands> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordAccountCommands"/> class.
    /// </summary>
    /// <param name="linkService">The service which links the accounts.</param>
    /// <param name="culture">The culture of the answers.</param>
    /// <param name="logger">The logger.</param>
    public DiscordAccountCommands(AccountLinkService linkService, CultureInfo culture, ILogger<DiscordAccountCommands> logger)
    {
        this._linkService = linkService;
        this._culture = culture;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the definitions of the commands.
    /// </summary>
    public static IReadOnlyList<DiscordCommandDefinition> Definitions { get; } =
    [
        new("link", nameof(Resources.Command_Link_Description), "code", nameof(Resources.Command_Link_CodeOption)),
        new("unlink", nameof(Resources.Command_Unlink_Description)),
        new("character", nameof(Resources.Command_Character_Description), "name", nameof(Resources.Command_Character_NameOption)),
    ];

    /// <summary>
    /// Determines whether the command is one of these commands.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns><c>true</c>, if the command is one of these commands.</returns>
    public static bool IsAccountCommand(string commandName) => Definitions.Any(definition => definition.Name == commandName);

    /// <summary>
    /// Executes a command.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="argument">The value of the option of the command, if it has one.</param>
    /// <param name="userId">The identifier of the Discord user who used the command.</param>
    /// <param name="userName">The name of the Discord user who used the command.</param>
    /// <returns>The answer.</returns>
    public async ValueTask<DiscordCommandAnswer> ExecuteAsync(string commandName, string? argument, ulong userId, string userName)
    {
        try
        {
            return commandName switch
            {
                "link" => await this.LinkAsync(argument ?? string.Empty, userId, userName).ConfigureAwait(false),
                "unlink" => await this.UnlinkAsync(userId).ConfigureAwait(false),
                "character" => await this.SelectCharacterAsync(argument ?? string.Empty, userId).ConfigureAwait(false),
                _ => this.Answer(nameof(Resources.Command_Failed)),
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when executing the Discord command {command}.", commandName);
            return this.Answer(nameof(Resources.Command_Failed));
        }
    }

    private async ValueTask<DiscordCommandAnswer> LinkAsync(string code, ulong userId, string userName)
    {
        var userIdText = userId.ToString(CultureInfo.InvariantCulture);
        if (await this._linkService.LinkAsync(Provider, code, userIdText, userName).ConfigureAwait(false) is not { } result)
        {
            return this.Answer(nameof(Resources.Link_InvalidCode));
        }

        var roleChanges = new List<DiscordRoleChange> { new(userId, true) };
        if (ulong.TryParse(result.ReplacedExternalUserId, NumberStyles.None, CultureInfo.InvariantCulture, out var replacedUserId))
        {
            roleChanges.Add(new(replacedUserId, false));
        }

        return this.Answer(nameof(Resources.Link_Linked), roleChanges, DiscordMessageFormatter.Escape(result.CharacterName ?? "-"));
    }

    private async ValueTask<DiscordCommandAnswer> UnlinkAsync(ulong userId)
    {
        var wasLinked = await this._linkService.UnlinkUserAsync(Provider, userId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        return wasLinked
            ? this.Answer(nameof(Resources.Link_Unlinked), [new(userId, false)])
            : this.Answer(nameof(Resources.Link_NotLinked));
    }

    private async ValueTask<DiscordCommandAnswer> SelectCharacterAsync(string characterName, ulong userId)
    {
        var result = await this._linkService.SelectCharacterAsync(Provider, userId.ToString(CultureInfo.InvariantCulture), characterName.Trim()).ConfigureAwait(false);
        return result switch
        {
            CharacterSelectionResult.Selected => this.Answer(nameof(Resources.Character_Selected), [], DiscordMessageFormatter.Escape(characterName.Trim())),
            CharacterSelectionResult.CharacterNotFound => this.Answer(nameof(Resources.Character_NotOfAccount), [], DiscordMessageFormatter.Escape(characterName.Trim())),
            _ => this.Answer(nameof(Resources.Link_NotLinked)),
        };
    }

    private DiscordCommandAnswer Answer(string resourceKey) => this.Answer(resourceKey, []);

    private DiscordCommandAnswer Answer(string resourceKey, IReadOnlyList<DiscordRoleChange> roleChanges, params object[] args)
    {
        var text = DiscordCommands.GetText(this._culture, resourceKey, args);
        return new DiscordCommandAnswer(
            new DiscordEmbed(DiscordCommands.GetText(this._culture, nameof(Resources.Link_Title)), text, DiscordCommands.AnswerColor, DateTime.UtcNow, null),
            roleChanges);
    }
}
