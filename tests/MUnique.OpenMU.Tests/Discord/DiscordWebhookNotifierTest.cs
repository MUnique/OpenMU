// <copyright file="DiscordWebhookNotifierTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Discord;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests for the <see cref="DiscordWebhookNotifier"/>.
/// </summary>
[TestFixture]
public class DiscordWebhookNotifierTest
{
    /// <summary>
    /// Tests that a mini game which opens several levels at the same time is announced just once.
    /// </summary>
    [Test]
    public async Task MiniGameOpeningIsAnnouncedOnceAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        await using var notifier = CreateNotifier(handler, new DiscordWebhookSettings());
        var closesAt = DateTime.UtcNow.AddMinutes(5);

        for (byte level = 1; level <= 7; level++)
        {
            await notifier.OnGameEventAsync(new MiniGameEntranceOpenedEvent(0, DateTime.UtcNow, "BloodCastle", "Blood Castle", level, closesAt.AddMilliseconds(level))).ConfigureAwait(false);
        }

        await handler.WaitForRequestsAsync(1).ConfigureAwait(false);
        await Task.Delay(200).ConfigureAwait(false);
        Assert.That(handler.Requests.Sum(r => r.RootElement.GetProperty("embeds").GetArrayLength()), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the events of game servers which aren't configured to announce them, aren't posted.
    /// </summary>
    [Test]
    public async Task EventsOfOtherServersAreNotAnnouncedAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        await using var notifier = CreateNotifier(handler, new DiscordWebhookSettings { EventAnnouncingServerIds = { 0 } });

        await notifier.OnGameEventAsync(new InvasionStartedEvent(1, DateTime.UtcNow, Guid.Empty, "Golden Invasion", new[] { "Lorencia" })).ConfigureAwait(false);
        await notifier.OnGameEventAsync(new InvasionStartedEvent(0, DateTime.UtcNow, Guid.Empty, "Golden Invasion", new[] { "Devias" })).ConfigureAwait(false);

        await handler.WaitForRequestsAsync(1).ConfigureAwait(false);
        await Task.Delay(200).ConfigureAwait(false);
        var descriptions = handler.Requests.SelectMany(r => r.RootElement.GetProperty("embeds").EnumerateArray()).Select(e => e.GetProperty("description").GetString());
        Assert.That(descriptions, Is.EqualTo(new[] { "Monsters invade Devias!" }));
    }

    /// <summary>
    /// Tests that events of a category without webhook aren't posted.
    /// </summary>
    [Test]
    public async Task CategoryWithoutWebhookIsNotPostedAsync()
    {
        var handler = new RecordingHttpMessageHandler();
        await using var notifier = CreateNotifier(handler, new DiscordWebhookSettings());

        await notifier.OnGameEventAsync(new GlobalNoticeEvent(0, DateTime.UtcNow, "GM", "Hello")).ConfigureAwait(false);
        await notifier.OnGameEventAsync(new BossKilledEvent(0, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7")).ConfigureAwait(false);

        await handler.WaitForRequestsAsync(1).ConfigureAwait(false);
        await Task.Delay(200).ConfigureAwait(false);
        var titles = handler.Requests.SelectMany(r => r.RootElement.GetProperty("embeds").EnumerateArray()).Select(e => e.GetProperty("title").GetString());
        Assert.That(titles, Is.EqualTo(new[] { "Boss defeated" }));
    }

    private static DiscordWebhookNotifier CreateNotifier(RecordingHttpMessageHandler handler, DiscordWebhookSettings settings)
    {
        // There is no webhook for the notices, so they aren't posted.
        settings.Webhooks[DiscordChannelCategory.Events] = "https://discord.example/api/webhooks/1/events";
        settings.Webhooks[DiscordChannelCategory.WorldNews] = "https://discord.example/api/webhooks/2/news";
        return new DiscordWebhookNotifier(
            settings,
            new HttpClient(handler),
            _ => null,
            _ => ValueTask.FromResult<string?>(null),
            NullLoggerFactory.Instance);
    }
}
