// <copyright file="DiscordNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Posts <see cref="GameEvent"/>s as messages to Discord channels, through webhooks or the bot.
/// </summary>
public sealed class DiscordNotifier : IGameEventListener, IAsyncDisposable
{
    /// <summary>
    /// The time after which the remembered mini game openings are forgotten.
    /// </summary>
    private static readonly TimeSpan AnnouncedOpeningRetention = TimeSpan.FromHours(1);

    private readonly DiscordSettings _settings;
    private readonly DiscordMessageFormatter _formatter;
    private readonly IReadOnlyDictionary<DiscordChannelCategory, DiscordMessageQueue> _senders;
    private readonly ILogger<DiscordNotifier> _logger;
    private readonly ConcurrentDictionary<(byte ServerId, string MiniGameType, DateTime EnterEndsAt), DateTime> _announcedOpenings = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordNotifier"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="httpClient">The HTTP client for the webhooks.</param>
    /// <param name="botMessenger">The messenger of the bot, if the bot posts into channels.</param>
    /// <param name="getServerName">The function which gets the name of a game server by its identifier.</param>
    /// <param name="getGuildName">The function which gets the name of a guild by its persistent identifier.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="timeProvider">The time provider.</param>
    public DiscordNotifier(
        DiscordSettings settings,
        HttpClient httpClient,
        IDiscordMessenger? botMessenger,
        Func<byte, string?> getServerName,
        Func<Guid, ValueTask<string?>> getGuildName,
        ILoggerFactory loggerFactory,
        TimeProvider? timeProvider = null)
    {
        this._settings = settings;
        this._logger = loggerFactory.CreateLogger<DiscordNotifier>();
        this._formatter = new DiscordMessageFormatter(settings.GetCulture(), getServerName, getGuildName);

        var senders = new Dictionary<DiscordChannelCategory, DiscordMessageQueue>();
        if (botMessenger is not null)
        {
            var botLogger = loggerFactory.CreateLogger<DiscordBotChannelSender>();
            foreach (var (category, channelId) in settings.Bot.Channels)
            {
                senders[category] = new DiscordBotChannelSender(channelId, botMessenger, botLogger, settings.MaximumQueuedMessages);
            }
        }

        var senderLogger = loggerFactory.CreateLogger<DiscordWebhookSender>();
        foreach (var (category, url) in settings.Webhooks)
        {
            // A category which the bot posts doesn't need the webhook.
            if (string.IsNullOrWhiteSpace(url) || senders.ContainsKey(category))
            {
                continue;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                this._logger.LogError("The Discord webhook URL of the category {category} is invalid.", category);
                continue;
            }

            senders[category] = new DiscordWebhookSender(uri, httpClient, senderLogger, settings.MaximumQueuedMessages, timeProvider);
        }

        this._senders = senders;
    }

    /// <inheritdoc />
    public async ValueTask OnGameEventAsync(GameEvent gameEvent)
    {
        if (!this.ShouldAnnounce(gameEvent)
            || await this._formatter.FormatAsync(gameEvent).ConfigureAwait(false) is not { } message
            || !this._senders.TryGetValue(message.Category, out var sender))
        {
            return;
        }

        sender.Enqueue(message.Embed);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        foreach (var sender in this._senders.Values)
        {
            await sender.DisposeAsync().ConfigureAwait(false);
        }
    }

    private bool ShouldAnnounce(GameEvent gameEvent)
    {
        if (gameEvent is MiniGameEntranceOpenedEvent or MiniGameStartedEvent or MiniGameEndedEvent or InvasionStartedEvent or InvasionEndedEvent
            && this._settings.EventAnnouncingServerIds is { Count: > 0 } serverIds
            && !serverIds.Contains(gameEvent.ServerId))
        {
            return false;
        }

        if (gameEvent is MiniGameEntranceOpenedEvent opened)
        {
            // Mini games like Blood Castle open all their levels at the same time, but we announce it just once.
            this.ForgetOldOpenings(opened.TimestampUtc);
            var enterEndsAt = new DateTime(opened.EnterEndsAtUtc.Ticks - (opened.EnterEndsAtUtc.Ticks % TimeSpan.TicksPerMinute), DateTimeKind.Utc);
            return this._announcedOpenings.TryAdd((opened.ServerId, opened.MiniGameType, enterEndsAt), opened.TimestampUtc);
        }

        return true;
    }

    private void ForgetOldOpenings(DateTime now)
    {
        foreach (var (key, announcedAt) in this._announcedOpenings)
        {
            if (now - announcedAt > AnnouncedOpeningRetention)
            {
                this._announcedOpenings.TryRemove(key, out _);
            }
        }
    }
}
