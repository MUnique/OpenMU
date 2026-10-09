// <copyright file="DiscordBot.Extras.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Threading;
using global::Discord;
using global::Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.Properties;
using MUnique.OpenMU.Discord.Provisioning;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The part of the <see cref="DiscordBot"/> with the commands for game masters, the direct messages
/// and the scheduled events.
/// </summary>
public sealed partial class DiscordBot
{
    /// <summary>
    /// The name of the command which turns direct messages on or off.
    /// </summary>
    private const string NotifyCommandName = "notify";

    private const string NotifyTypeOptionName = "type";

    private const string NotifyEnabledOptionName = "enabled";

    private IEnumerable<ApplicationCommandProperties> CreateExtraCommands()
    {
        foreach (var definition in DiscordGameMasterCommands.Definitions)
        {
            yield return new SlashCommandBuilder()
                .WithName(definition.Name)
                .WithDescription(this._commands.Text(definition.DescriptionKey))
                .WithContextTypes(InteractionContextType.Guild)
                .AddOption(definition.OptionName, ApplicationCommandOptionType.String, this._commands.Text(definition.OptionDescriptionKey!), isRequired: true)
                .Build();
        }

        yield return new SlashCommandBuilder()
            .WithName(NotifyCommandName)
            .WithDescription(this._commands.Text(nameof(Resources.Command_Notify_Description)))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName(NotifyTypeOptionName)
                .WithDescription(this._commands.Text(nameof(Resources.Command_Notify_TypeOption)))
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
                .AddChoice(this._commands.Text(nameof(Resources.Notify_Type_Login)), AccountNotificationKeywords.LoginAttempt)
                .AddChoice(this._commands.Text(nameof(Resources.Notify_Type_Letter)), AccountNotificationKeywords.LetterReceived)
                .AddChoice(this._commands.Text(nameof(Resources.Notify_Type_Friend)), AccountNotificationKeywords.FriendOnline))
            .AddOption(NotifyEnabledOptionName, ApplicationCommandOptionType.Boolean, this._commands.Text(nameof(Resources.Command_Notify_EnabledOption)), isRequired: true)
            .Build();
    }

    private async Task ExecuteGameMasterCommandAsync(SocketSlashCommand command)
    {
        try
        {
            await command.DeferAsync(ephemeral: true).ConfigureAwait(false);
            var argument = command.Data.Options.FirstOrDefault()?.Value as string ?? string.Empty;
            var answer = await this._gameMasterCommands.ExecuteAsync(command.Data.Name, argument, command.User.Id, command.User.Username, this.HasGameMasterRole(command)).ConfigureAwait(false);
            if (answer.StaffAlert is { } alert && this._routing.GetChannelId(DiscordChannelCategory.StaffAlerts) is { } channelId)
            {
                await this.SendAsync(channelId, [alert], CancellationToken.None).ConfigureAwait(false);
            }

            await command.FollowupAsync(embed: ToDiscordEmbed(answer.Embed), ephemeral: true, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when answering the Discord command {command}.", command.Data.Name);
        }
    }

    /// <summary>
    /// Determines whether the user has the GM role of the layout on the Discord server of the game server.
    /// </summary>
    private bool HasGameMasterRole(SocketSlashCommand command)
    {
        var roleName = this._layout.Roles.FirstOrDefault(role => role.Key == DiscordServerLayout.GameMasterRoleKey)?.Name;
        return this._client is { } client
               && command.User is SocketGuildUser user
               && user.Guild.Id == this.GetRoutingGuild(client)?.Id
               && user.Roles.Any(role => string.Equals(role.Name, roleName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task ExecuteNotifyCommandAsync(SocketSlashCommand command)
    {
        try
        {
            await command.DeferAsync(ephemeral: true).ConfigureAwait(false);
            var type = command.Data.Options.FirstOrDefault(option => option.Name == NotifyTypeOptionName)?.Value as string;
            var enabled = command.Data.Options.FirstOrDefault(option => option.Name == NotifyEnabledOptionName)?.Value is true;
            var answer = await this._accountCommands.SetNotificationAsync(command.User.Id, type, enabled).ConfigureAwait(false);
            await command.FollowupAsync(embed: ToDiscordEmbed(answer), ephemeral: true, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when answering the Discord command {command}.", command.Data.Name);
        }
    }

    /// <summary>
    /// Sends the direct messages for a game event in the background, so that the game doesn't wait for the database or Discord.
    /// </summary>
    private void SendDirectMessages(GameEvent gameEvent)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                foreach (var (userId, embed) in await this._directMessages.GetMessagesAsync(gameEvent).ConfigureAwait(false))
                {
                    await this._ready.Task.WaitAsync(CancellationToken.None).ConfigureAwait(false);
                    if (this._client is { } client
                        && await client.Rest.GetUserAsync(userId).ConfigureAwait(false) is { } user)
                    {
                        var channel = await user.CreateDMChannelAsync().ConfigureAwait(false);
                        await channel.SendMessageAsync(embed: ToDiscordEmbed(embed), allowedMentions: AllowedMentions.None).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                // E.g. the user doesn't accept direct messages.
                this._logger.LogWarning(ex, "A direct message for the game event {gameEvent} couldn't be sent.", gameEvent.GetType().Name);
            }
        });
    }

    /// <summary>
    /// Creates or updates the scheduled event of the next castle siege on the Discord server of the game server.
    /// </summary>
    private void UpdateCastleSiegeEvent(CastleSiegeStateChangedEvent stateChanged)
    {
        if (!this._settings.CreateScheduledEvents
            || stateChanged.NextBattleStartUtc is not { } startUtc
            || startUtc <= DateTime.UtcNow.AddMinutes(1))
        {
            return;
        }

        var endUtc = stateChanged.NextBattleEndUtc is { } end && end > startUtc ? end : startUtc.AddHours(2);
        _ = Task.Run(async () =>
        {
            try
            {
                await this._ready.Task.WaitAsync(CancellationToken.None).ConfigureAwait(false);
                if (this._client is not { } client || this.GetRoutingGuild(client) is not { } guild)
                {
                    return;
                }

                var name = this._commands.Text(nameof(Resources.Event_CastleSiege_Name));
                var start = new DateTimeOffset(DateTime.SpecifyKind(startUtc, DateTimeKind.Utc));
                var endTime = new DateTimeOffset(DateTime.SpecifyKind(endUtc, DateTimeKind.Utc));
                var events = await guild.GetEventsAsync().ConfigureAwait(false);
                var existing = events.FirstOrDefault(e => e.Name == name && e.Type == GuildScheduledEventType.External && e.Status == GuildScheduledEventStatus.Scheduled);
                if (existing is null)
                {
                    await guild.CreateEventAsync(
                        name,
                        start,
                        GuildScheduledEventType.External,
                        GuildScheduledEventPrivacyLevel.Private,
                        this._commands.Text(nameof(Resources.Event_CastleSiege_Description)),
                        endTime,
                        location: this._commands.Text(nameof(Resources.Event_Location))).ConfigureAwait(false);
                }
                else if (existing.StartTime != start || existing.EndTime != endTime)
                {
                    await existing.ModifyAsync(properties =>
                    {
                        properties.StartTime = start;
                        properties.EndTime = endTime;
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "The scheduled event of the castle siege couldn't be updated. Has the bot the permission to create events?");
            }
        });
    }
}
