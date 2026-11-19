// <copyright file="GameEventWebhookPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the <see cref="GameEventWebhookPlugIn"/>, its <see cref="GameEventWebhookSender"/> and the <see cref="GameEventPlugInListener"/>.
/// </summary>
[TestFixture]
public class GameEventWebhookPlugInTest
{
    private static readonly Uri WebhookUrl = new("https://example.com/api/realm-events");

    /// <summary>
    /// Tests that the private events are never forwarded, even when everything else is.
    /// </summary>
    [Test]
    public void PrivateEventsAreNeverForwarded()
    {
        var configuration = new GameEventWebhookConfiguration { ForwardMiniGameOpenings = true };
        var privateEvents = new GameEvent[]
        {
            new ChatMessageEvent(1, DateTime.UtcNow, GameChatChannel.World, 0, "Hero", "Hello"),
            new LetterReceivedEvent(1, DateTime.UtcNow, "Hero", "Elf", "Hello"),
            new AccountLoginBlockedEvent(1, DateTime.UtcNow, "hero"),
            new AccountUnlinkedEvent(1, DateTime.UtcNow, "discord", "42"),
            new PlayerEnteredGameEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Hero"),
        };

        Assert.That(privateEvents.Where(e => GameEventWebhookPlugIn.ShouldForward(configuration, e)), Is.Empty);
    }

    /// <summary>
    /// Tests that each kind of event follows its switch.
    /// </summary>
    [Test]
    public void EventKindsFollowTheirSwitches()
    {
        var on = new GameEventWebhookConfiguration { ForwardMiniGameOpenings = true };
        var off = new GameEventWebhookConfiguration
        {
            ForwardBossKills = false,
            ForwardLevelMilestones = false,
            ForwardItemDrops = false,
            ForwardMiniGameResults = false,
            ForwardInvasions = false,
            ForwardCastleSiege = false,
            ForwardGlobalNotices = false,
        };
        var events = new GameEvent[]
        {
            new BossKilledEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7"),
            new CharacterLevelMilestoneEvent(1, DateTime.UtcNow, "Hero", "Blade Knight", 400, false),
            new MonsterItemDroppedEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7", "Sword of Destruction", 13, true, false),
            new MiniGameEndedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, "Hero", ["Hero"]),
            new MiniGameStartedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, 5),
            new MiniGameEntranceOpenedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, DateTime.UtcNow),
            new InvasionStartedEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Golden Invasion", ["Lorencia"]),
            new InvasionEndedEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Golden Invasion", ["Lorencia"]),
            new CastleSiegeStateChangedEvent(1, DateTime.UtcNow, "Start", "End", DateTime.UtcNow, Guid.NewGuid()),
            new GlobalNoticeEvent(1, DateTime.UtcNow, "GM", "Hello"),
        };

        Assert.That(events.Where(e => !GameEventWebhookPlugIn.ShouldForward(on, e)), Is.Empty);
        Assert.That(events.Where(e => GameEventWebhookPlugIn.ShouldForward(off, e)), Is.Empty);
        Assert.That(GameEventWebhookPlugIn.ShouldForward(new GameEventWebhookConfiguration(), events[4]), Is.False, "openings are off by default");
    }

    /// <summary>
    /// Tests the filter of the dropped items, by name and level.
    /// </summary>
    [Test]
    public void DropsAreFilteredByNameAndLevel()
    {
        var configuration = new GameEventWebhookConfiguration { DropItemNames = ["Sword of Destruction"], DropMinimumItemLevel = 9 };

        Assert.That(GameEventWebhookPlugIn.ShouldForward(configuration, Drop("sword of destruction", 9)), Is.True);
        Assert.That(GameEventWebhookPlugIn.ShouldForward(configuration, Drop("Sword of Destruction", 8)), Is.False);
        Assert.That(GameEventWebhookPlugIn.ShouldForward(configuration, Drop("Kris", 13)), Is.False);
        Assert.That(GameEventWebhookPlugIn.ShouldForward(new GameEventWebhookConfiguration(), Drop("Kris", 0)), Is.True, "an empty list forwards every drop");

        static MonsterItemDroppedEvent Drop(string item, byte level) => new(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7", item, level, true, false);
    }

    /// <summary>
    /// Tests the signature against a value which was computed independently.
    /// </summary>
    [Test]
    public void SignatureIsHmacSha256OfTimestampAndBody()
    {
        Assert.That(
            GameEventWebhookSender.Sign("secret", 1700000000, "{\"a\":1}"),
            Is.EqualTo("sha256=49f24e537407743fa4a0242bb63b94b9a47ee99cbbe071ccd8a22550ae411686"));
    }

    /// <summary>
    /// Tests the posted request: the body with the id and the typed event, and the signature of exactly that body.
    /// </summary>
    [Test]
    public async Task RequestIsSignedJsonAsync()
    {
        var handler = new RecordingHandler(_ => HttpStatusCode.OK);
        var sender = CreateSender(handler);
        var id = Guid.NewGuid();

        var delivered = await sender.DeliverAsync(new GameEventWebhookDelivery(WebhookUrl, "secret", id, new BossKilledEvent(2, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7"))).ConfigureAwait(false);

        Assert.That(delivered, Is.True);
        var request = handler.Requests.Single();
        using var json = JsonDocument.Parse(request.Body);
        Assert.That(json.RootElement.GetProperty("id").GetGuid(), Is.EqualTo(id));
        Assert.That(json.RootElement.GetProperty("event").GetProperty("$type").GetString(), Is.EqualTo(nameof(BossKilledEvent)));
        Assert.That(json.RootElement.GetProperty("event").GetProperty("killerName").GetString(), Is.EqualTo("Hero"));
        var timestamp = long.Parse(request.Headers[GameEventWebhookSender.TimestampHeader]);
        Assert.That(request.Headers[GameEventWebhookSender.SignatureHeader], Is.EqualTo(GameEventWebhookSender.Sign("secret", timestamp, request.Body)));
    }

    /// <summary>
    /// Tests that transient failures are retried, and refusals and lasting failures are given up without an exception.
    /// </summary>
    [Test]
    public async Task FailuresAreRetriedThenGivenUpAsync()
    {
        var responses = new Queue<HttpStatusCode>([HttpStatusCode.ServiceUnavailable, HttpStatusCode.TooManyRequests, HttpStatusCode.OK]);
        var flaky = new RecordingHandler(_ => responses.Dequeue());
        Assert.That(await CreateSender(flaky).DeliverAsync(Delivery()).ConfigureAwait(false), Is.True);
        Assert.That(flaky.Requests, Has.Count.EqualTo(3));

        var refusing = new RecordingHandler(_ => HttpStatusCode.Unauthorized);
        Assert.That(await CreateSender(refusing).DeliverAsync(Delivery()).ConfigureAwait(false), Is.False);
        Assert.That(refusing.Requests, Has.Count.EqualTo(1));

        var unreachable = new RecordingHandler(_ => throw new HttpRequestException("down"));
        Assert.That(await CreateSender(unreachable).DeliverAsync(Delivery()).ConfigureAwait(false), Is.False);
        Assert.That(unreachable.Requests, Has.Count.EqualTo(GameEventWebhookSender.MaximumAttempts));
    }

    /// <summary>
    /// Tests that the queue keeps at most its capacity and drops the oldest events while the receiver hangs.
    /// </summary>
    [Test]
    public async Task QueueIsBoundedAndDropsTheOldestAsync()
    {
        using var release = new SemaphoreSlim(0);
        var handler = new RecordingHandler(_ =>
        {
            release.Wait();
            return HttpStatusCode.OK;
        });
        var sender = CreateSender(handler);

        sender.Enqueue(Delivery());
        await WaitUntilAsync(() => handler.Requests.Count == 1).ConfigureAwait(false);
        var ids = Enumerable.Range(0, GameEventWebhookSender.QueueCapacity + 5).Select(_ => Guid.NewGuid()).ToList();
        ids.ForEach(id => sender.Enqueue(Delivery(id)));

        Assert.That(sender.QueuedCount, Is.EqualTo(GameEventWebhookSender.QueueCapacity));
        Assert.That(sender.DroppedCount, Is.EqualTo(5));

        release.Release(GameEventWebhookSender.QueueCapacity + 1);
        await WaitUntilAsync(() => sender.QueuedCount == 0 && handler.Requests.Count == GameEventWebhookSender.QueueCapacity + 1).ConfigureAwait(false);
        var sentIds = handler.Requests.Skip(1).Select(r => JsonDocument.Parse(r.Body).RootElement.GetProperty("id").GetGuid());
        Assert.That(sentIds, Is.EqualTo(ids.Skip(5)), "the oldest five were dropped, the rest kept their order");
    }

    /// <summary>
    /// Tests that the plugin returns at once and never throws into the game code, whatever the receiver or the configuration does.
    /// </summary>
    [Test]
    public void PublishingNeverThrowsOrWaits()
    {
        using var hang = new SemaphoreSlim(0);
        var handler = new RecordingHandler(_ =>
        {
            hang.Wait(TimeSpan.FromSeconds(5));
            throw new HttpRequestException("down");
        });
        var plugIn = new GameEventWebhookPlugIn(CreateSender(handler), NullLogger.Instance)
        {
            Configuration = new GameEventWebhookConfiguration { Url = WebhookUrl.ToString(), Secret = "secret" },
        };
        var boss = new BossKilledEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7");
        var drop = new MonsterItemDroppedEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7", "Kris", 0, true, false);

        Assert.That(plugIn.GameEventPublishedAsync(boss).IsCompletedSuccessfully, Is.True);

        plugIn.Configuration.DropItemNames = null!;
        Assert.That(plugIn.GameEventPublishedAsync(drop).IsCompletedSuccessfully, Is.True, "a broken configuration is logged, not thrown");

        plugIn.Configuration = new GameEventWebhookConfiguration { Url = "not a url", Secret = "secret" };
        Assert.That(plugIn.GameEventPublishedAsync(boss).IsCompletedSuccessfully, Is.True);

        plugIn.Configuration = null;
        Assert.That(plugIn.GameEventPublishedAsync(boss).IsCompletedSuccessfully, Is.True);
        hang.Release(10);
    }

    /// <summary>
    /// Tests that nothing is queued without a secret, or with a URL which isn't HTTP(S).
    /// </summary>
    [Test]
    public async Task NothingIsSentWithoutSecretOrHttpUrlAsync()
    {
        var handler = new RecordingHandler(_ => HttpStatusCode.OK);
        var sender = CreateSender(handler);
        var plugIn = new GameEventWebhookPlugIn(sender, NullLogger.Instance);
        var boss = new BossKilledEvent(1, DateTime.UtcNow, "Hero", "Kundun", "Kalima 7");

        plugIn.Configuration = new GameEventWebhookConfiguration { Url = WebhookUrl.ToString() };
        await plugIn.GameEventPublishedAsync(boss).ConfigureAwait(false);
        plugIn.Configuration = new GameEventWebhookConfiguration { Url = "file:///c:/events.json", Secret = "secret" };
        await plugIn.GameEventPublishedAsync(boss).ConfigureAwait(false);
        plugIn.Configuration = new GameEventWebhookConfiguration { Url = WebhookUrl.ToString(), Secret = "secret" };
        await plugIn.GameEventPublishedAsync(boss).ConfigureAwait(false);

        await WaitUntilAsync(() => handler.Requests.Count == 1).ConfigureAwait(false);
        Assert.That(sender.QueuedCount, Is.Zero);
        Assert.That(handler.Requests.Single().Uri, Is.EqualTo(WebhookUrl));
    }

    /// <summary>
    /// Tests that the listener passes the game events to the active plugins of the plugin point.
    /// </summary>
    [Test]
    public async Task ListenerPassesEventsToThePlugInsAsync()
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var plugIn = new RecordingPlugIn();
        plugInManager.RegisterPlugInAtPlugInPoint<IGameEventPublishedPlugIn>(plugIn);
        var gameEvent = new GlobalNoticeEvent(1, DateTime.UtcNow, "GM", "Hello");

        await new GameEventPlugInListener(plugInManager).OnGameEventAsync(gameEvent).ConfigureAwait(false);
        await new GameEventPlugInListener(new PlugInManager(null, NullLoggerFactory.Instance, null, null)).OnGameEventAsync(gameEvent).ConfigureAwait(false);

        Assert.That(plugIn.Events, Is.EqualTo(new[] { gameEvent }));
    }

    private static GameEventWebhookSender CreateSender(HttpMessageHandler handler)
    {
        return new GameEventWebhookSender(NullLogger.Instance, new HttpClient(handler), firstRetryDelay: TimeSpan.Zero);
    }

    private static GameEventWebhookDelivery Delivery(Guid? id = null)
    {
        return new GameEventWebhookDelivery(WebhookUrl, "secret", id ?? Guid.NewGuid(), new GlobalNoticeEvent(1, DateTime.UtcNow, "GM", "Hello"));
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!condition())
        {
            await Task.Delay(10, timeout.Token).ConfigureAwait(false);
        }
    }

    private sealed record RecordedRequest(Uri? Uri, string Body, Dictionary<string, string> Headers);

    private sealed class RecordingHandler(Func<RecordedRequest, HttpStatusCode> respond) : HttpMessageHandler
    {
        private readonly List<RecordedRequest> _requests = new();

        public IReadOnlyList<RecordedRequest> Requests
        {
            get
            {
                lock (this._requests)
                {
                    return this._requests.ToList();
                }
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = await request.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var recorded = new RecordedRequest(request.RequestUri, body, request.Headers.ToDictionary(h => h.Key, h => h.Value.Single()));
            lock (this._requests)
            {
                this._requests.Add(recorded);
            }

            return new HttpResponseMessage(respond(recorded));
        }
    }

    [Guid("1A2B6C3E-162A-4078-864E-8192418FD195")]
    private sealed class RecordingPlugIn : IGameEventPublishedPlugIn
    {
        public List<GameEvent> Events { get; } = new();

        public ValueTask GameEventPublishedAsync(GameEvent gameEvent)
        {
            this.Events.Add(gameEvent);
            return ValueTask.CompletedTask;
        }
    }
}
