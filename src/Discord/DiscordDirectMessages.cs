// <copyright file="DiscordDirectMessages.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Globalization;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Determines the direct messages which the bot sends to linked users because of game events, if they turned them on.
/// </summary>
public sealed class DiscordDirectMessages
{
    /// <summary>
    /// The color of the direct messages (blue).
    /// </summary>
    private const int Color = DiscordCommands.AnswerColor;

    private readonly AccountLinkService _linkService;
    private readonly IFriendServer _friendServer;
    private readonly Func<IPlayerContext> _createContext;
    private readonly Func<IFriendServerContext> _createFriendContext;
    private readonly Func<byte, string?> _getServerName;
    private readonly CultureInfo _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordDirectMessages"/> class.
    /// </summary>
    /// <param name="linkService">The service of the account links.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="createContext">The function which creates a player persistence context.</param>
    /// <param name="createFriendContext">The function which creates a friend persistence context.</param>
    /// <param name="getServerName">The function which gets the name of a game server.</param>
    /// <param name="culture">The culture of the messages.</param>
    public DiscordDirectMessages(AccountLinkService linkService, IFriendServer friendServer, Func<IPlayerContext> createContext, Func<IFriendServerContext> createFriendContext, Func<byte, string?> getServerName, CultureInfo culture)
    {
        this._linkService = linkService;
        this._friendServer = friendServer;
        this._createContext = createContext;
        this._createFriendContext = createFriendContext;
        this._getServerName = getServerName;
        this._culture = culture;
    }

    /// <summary>
    /// Gets the direct messages for a game event.
    /// </summary>
    /// <param name="gameEvent">The game event.</param>
    /// <returns>The identifiers of the Discord users and their messages.</returns>
    public async ValueTask<IReadOnlyList<(ulong UserId, DiscordEmbed Embed)>> GetMessagesAsync(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case AccountLoginBlockedEvent loginBlocked:
                {
                    using var context = this._createContext();
                    var accountId = await context.GetAccountIdByLoginNameAsync(loginBlocked.LoginName).ConfigureAwait(false);
                    return await this.CreateAsync(accountId, AccountNotificationTypes.LoginAttempt, nameof(Resources.Dm_LoginAttempt), this._getServerName(loginBlocked.ServerId) ?? loginBlocked.ServerId.ToString(this._culture)).ConfigureAwait(false);
                }

            case LetterReceivedEvent letter:
                {
                    using var context = this._createContext();
                    var accountId = await context.GetAccountIdByCharacterNameAsync(letter.ReceiverName).ConfigureAwait(false);
                    return await this.CreateAsync(accountId, AccountNotificationTypes.LetterReceived, nameof(Resources.Dm_LetterReceived), DiscordMessageFormatter.Escape(letter.ReceiverName), DiscordMessageFormatter.Escape(letter.SenderName), DiscordMessageFormatter.Escape(letter.Subject)).ConfigureAwait(false);
                }

            case PlayerEnteredGameEvent entered:
                return await this.GetFriendOnlineMessagesAsync(entered).ConfigureAwait(false);
            default:
                return [];
        }
    }

    private async ValueTask<IReadOnlyList<(ulong UserId, DiscordEmbed Embed)>> GetFriendOnlineMessagesAsync(PlayerEnteredGameEvent entered)
    {
        // Invisible players don't want their friends to know that they are online.
        if (await this._friendServer.GetOnlineServerIdAsync(entered.CharacterName).ConfigureAwait(false) is null)
        {
            return [];
        }

        IEnumerable<string> friendNames;
        using (var friendContext = this._createFriendContext())
        {
            friendNames = (await friendContext.GetFriendNamesAsync(entered.CharacterId).ConfigureAwait(false)).ToList();
        }

        var accountIds = new HashSet<Guid>();
        using (var context = this._createContext())
        {
            foreach (var friendName in friendNames)
            {
                if (await context.GetAccountIdByCharacterNameAsync(friendName).ConfigureAwait(false) is { } accountId)
                {
                    accountIds.Add(accountId);
                }
            }
        }

        var messages = new List<(ulong UserId, DiscordEmbed Embed)>();
        foreach (var accountId in accountIds)
        {
            messages.AddRange(await this.CreateAsync(accountId, AccountNotificationTypes.FriendOnline, nameof(Resources.Dm_FriendOnline), DiscordMessageFormatter.Escape(entered.CharacterName)).ConfigureAwait(false));
        }

        return messages;
    }

    private async ValueTask<IReadOnlyList<(ulong UserId, DiscordEmbed Embed)>> CreateAsync(Guid? accountId, AccountNotificationTypes type, string resourceKey, params object[] args)
    {
        if (accountId is not { } id
            || await this._linkService.GetLinkAsync(id, AccountLinkService.DiscordProvider).ConfigureAwait(false) is not { ExternalUserId: { } externalUserId } link
            || !link.Notifications.HasFlag(type)
            || !ulong.TryParse(externalUserId, NumberStyles.None, CultureInfo.InvariantCulture, out var userId))
        {
            return [];
        }

        var embed = new DiscordEmbed(
            DiscordCommands.GetText(this._culture, nameof(Resources.Link_Title)),
            DiscordCommands.GetText(this._culture, resourceKey, args),
            Color,
            DateTime.UtcNow,
            null);
        return [(userId, embed)];
    }
}
