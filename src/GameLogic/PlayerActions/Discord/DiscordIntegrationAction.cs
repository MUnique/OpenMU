// <copyright file="DiscordIntegrationAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Discord;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The actions of the Discord integration of the game client: the integration info, and linking and unlinking
/// the account.
/// </summary>
public class DiscordIntegrationAction
{
    /// <summary>
    /// The last time each player got a link code. Weak, so it doesn't keep players which left alive.
    /// </summary>
    private readonly ConditionalWeakTable<Player, StrongBox<DateTimeOffset>> _lastLinkCodes = new();

    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordIntegrationAction"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider; the system time, if <c>null</c>.</param>
    public DiscordIntegrationAction(TimeProvider? timeProvider = null)
    {
        this._timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Gets the time a player has to wait between two link codes. Every code costs a database write and
    /// replaces the previous one, so a client which asks in a loop neither loads the database nor keeps
    /// invalidating the code the player is looking at.
    /// </summary>
    public static TimeSpan LinkCodeCooldown { get; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Shows the player how the game server is connected to Discord.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowIntegrationInfoAsync(Player player)
    {
        var info = await this.GetIntegrationInfoAsync(player).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IDiscordIntegrationViewPlugIn>(p => p.ShowDiscordIntegrationInfoAsync(info)).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a one-time code to link the account of the player to a Discord user, and shows it.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask RequestLinkCodeAsync(Player player)
    {
        string? code = null;
        var result = DiscordLinkCodeResult.NotAvailable;
        if (DiscordIntegrationFeaturePlugIn.GetConfiguration(player.GameContext) is not null
            && player.Account is { } account)
        {
            if (this.TryStartLinkCode(player))
            {
                code = await CreateLinkService(player).CreateCodeAsync(account.GetId(), AccountLinkService.DiscordProvider, player.SelectedCharacter?.Name).ConfigureAwait(false);
                result = DiscordLinkCodeResult.Created;
            }
            else
            {
                result = DiscordLinkCodeResult.TooSoon;
            }
        }

        await player.InvokeViewPlugInAsync<IDiscordIntegrationViewPlugIn>(p => p.ShowDiscordLinkCodeAsync(result, code, AccountLinkService.CodeValidity)).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the link of the account of the player to a Discord user, and shows the updated integration info.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask UnlinkAndShowInfoAsync(Player player)
    {
        await UnlinkAsync(player).ConfigureAwait(false);
        await this.ShowIntegrationInfoAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the link of the account of the player to a Discord user.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c>, if a link was removed.</returns>
    public static async ValueTask<bool> UnlinkAsync(Player player)
    {
        if (player.Account is not { } account
            || await CreateLinkService(player).UnlinkAccountAsync(account.GetId(), AccountLinkService.DiscordProvider).ConfigureAwait(false) is not { } userId)
        {
            return false;
        }

        if (player.GameContext is IGameServerContext gameServerContext)
        {
            // The Discord bot removes the roles of the user.
            await gameServerContext.EventPublisher.GameEventAsync(
                new AccountUnlinkedEvent(gameServerContext.Id, DateTime.UtcNow, AccountLinkService.DiscordProvider, userId)).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Gets how the game server is connected to Discord, from the point of view of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The integration info.</returns>
    public async ValueTask<DiscordIntegrationInfo> GetIntegrationInfoAsync(Player player)
    {
        if (DiscordIntegrationFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration)
        {
            return DiscordIntegrationInfo.None;
        }

        var linkedUserName = await GetLinkedUserNameAsync(player).ConfigureAwait(false);
        var (isGuildChatBridged, isAllianceChatBridged) = await GetGuildChatBindingsAsync(player).ConfigureAwait(false);
        return new DiscordIntegrationInfo(linkedUserName, isGuildChatBridged, isAllianceChatBridged, IsWorldChatPublished(player), configuration);
    }

    private static AccountLinkService CreateLinkService(Player player)
    {
        return new AccountLinkService(() => player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration));
    }

    private static async ValueTask<string?> GetLinkedUserNameAsync(Player player)
    {
        if (player.Account is not { } account)
        {
            return null;
        }

        var link = await CreateLinkService(player).GetLinkAsync(account.GetId(), AccountLinkService.DiscordProvider).ConfigureAwait(false);
        return link?.ExternalUserId is null ? null : link.ExternalUserName ?? string.Empty;
    }

    private static async ValueTask<(bool Guild, bool Alliance)> GetGuildChatBindingsAsync(Player player)
    {
        if (player.SelectedCharacter is not { } character)
        {
            return (false, false);
        }

        try
        {
            using var guildContext = player.GameContext.PersistenceContextProvider.CreateNewGuildContext();
            if (await guildContext.GetGuildMembershipAsync(character.Name).ConfigureAwait(false) is not { } membership)
            {
                return (false, false);
            }

            using var context = player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration);
            var allianceId = membership.AllianceMasterGuildId ?? membership.GuildId;
            var bindings = await context.GetGuildChatBindingsAsync(membership.GuildId, allianceId).ConfigureAwait(false);
            return (bindings.Any(b => b.Scope == GuildChatScope.Guild), bindings.Any(b => b.Scope == GuildChatScope.Alliance));
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "The bindings of the guild chat couldn't be checked.");
            return (false, false);
        }
    }

    /// <summary>
    /// The world chat leaves the game only when the game events publisher publishes the chat;
    /// the bot then mirrors it to the world chat channel of its layout.
    /// </summary>
    private static bool IsWorldChatPublished(Player player)
    {
        return player.GameContext.PlugInManager.GetActivePlugInsOf<IChatMessageSentPlugIn>()
            .OfType<GameEventPublisherPlugIn>()
            .Any(p => p.Configuration?.PublishChatMessages is true);
    }

    /// <summary>
    /// Takes the cooldown of the player, if it has passed.
    /// </summary>
    private bool TryStartLinkCode(Player player)
    {
        var now = this._timeProvider.GetUtcNow();
        var last = this._lastLinkCodes.GetValue(player, _ => new StrongBox<DateTimeOffset>(DateTimeOffset.MinValue));
        lock (last)
        {
            if (now - last.Value < LinkCodeCooldown)
            {
                return false;
            }

            last.Value = now;
            return true;
        }
    }
}
