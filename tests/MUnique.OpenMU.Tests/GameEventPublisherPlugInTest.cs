// <copyright file="GameEventPublisherPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.GameEvents;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="GameEventPublisherPlugIn"/> and the <see cref="GameEvent"/>s.
/// </summary>
[TestFixture]
public class GameEventPublisherPlugInTest
{
    private const byte ServerId = 3;

    private readonly List<GameServerContext> _gameContexts = new();

    private Mock<IEventPublisher> _eventPublisher = null!;

    /// <summary>
    /// Gets the game events to test the serialization with.
    /// </summary>
    public static IEnumerable<GameEvent> GameEvents { get; } = new GameEvent[]
    {
        new MiniGameEntranceOpenedEvent(1, DateTime.UtcNow, "BloodCastle", "Blood Castle", 1, DateTime.UtcNow.AddMinutes(5)),
        new MiniGameStartedEvent(1, DateTime.UtcNow, "ChaosCastle", "Chaos Castle", 2, 42),
        new MiniGameEndedEvent(1, DateTime.UtcNow, "ChaosCastle", "Chaos Castle", 2, "Winner", new[] { "Winner", "Other" }),
        new InvasionStartedEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Golden Invasion", new[] { "Lorencia", "Devias" }),
        new InvasionEndedEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Golden Invasion", new[] { "Lorencia" }),
        new CastleSiegeStateChangedEvent(1, DateTime.UtcNow, "Ready", "Start", DateTime.UtcNow.AddHours(2), Guid.NewGuid()),
        new MonsterItemDroppedEvent(1, DateTime.UtcNow, "Killer", "Kundun", "Kalima 7", "Sword of Destruction", 13, true, false),
        new GlobalNoticeEvent(1, DateTime.UtcNow, "GameMaster", "Hello"),
        new BossKilledEvent(1, DateTime.UtcNow, "Killer", "Kundun", "Kalima 7"),
        new CharacterLevelMilestoneEvent(1, DateTime.UtcNow, "Hero", "Blade Knight", 400, false),
        new AccountUnlinkedEvent(1, DateTime.UtcNow, "discord", "42"),
        new ChatMessageEvent(1, DateTime.UtcNow, GameChatChannel.Alliance, 7, "Hero", "Hello"),
        new AccountLoginBlockedEvent(1, DateTime.UtcNow, "hero"),
        new LetterReceivedEvent(1, DateTime.UtcNow, "Hero", "Elf", "Hello"),
        new PlayerEnteredGameEvent(1, DateTime.UtcNow, Guid.NewGuid(), "Hero"),
        new CastleSiegeStateChangedEvent(1, DateTime.UtcNow, "Notify", "Ready", DateTime.UtcNow.AddHours(1), null, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3)),
    };

    /// <summary>
    /// Sets up the event publisher mock.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._eventPublisher = new Mock<IEventPublisher>();
    }

    /// <summary>
    /// Disposes the created game contexts.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        foreach (var gameContext in this._gameContexts)
        {
            await gameContext.DisposeAsync().ConfigureAwait(false);
        }

        this._gameContexts.Clear();
    }

    /// <summary>
    /// Tests that a game event keeps its type and values when it's serialized as <see cref="GameEvent"/>, like it's published between processes.
    /// </summary>
    /// <param name="gameEvent">The game event.</param>
    [TestCaseSource(nameof(GameEvents))]
    public void EventIsSerializedWithItsType(GameEvent gameEvent)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(gameEvent, options);
        var deserialized = JsonSerializer.Deserialize<GameEvent>(json, options);

        Assert.That(deserialized, Is.InstanceOf(gameEvent.GetType()));
        Assert.That(JsonSerializer.Serialize(deserialized, options), Is.EqualTo(json));
    }

    /// <summary>
    /// Tests that the start of an invasion is published with the names of the maps.
    /// </summary>
    [Test]
    public async Task InvasionStartIsPublishedAsync()
    {
        var gameContext = new Mock<IGameServerContext>();
        gameContext.SetupGet(c => c.Id).Returns(ServerId);
        gameContext.SetupGet(c => c.EventPublisher).Returns(this._eventPublisher.Object);
        var plugIn = CreatePlugIn();
        var maps = new[] { new GameMapDefinition { Name = "Lorencia" }, new GameMapDefinition { Name = "Devias" } };

        await plugIn.InvasionStartedAsync(gameContext.Object, new GoldenInvasionPlugIn(), maps).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<InvasionStartedEvent>(e =>
                e.ServerId == ServerId
                && e.InvasionId == typeof(GoldenInvasionPlugIn).GUID
                && e.MapNames.SequenceEqual(new[] { "Lorencia", "Devias" }))),
            Times.Once);
    }

    /// <summary>
    /// Tests that nothing is published when the corresponding event type is disabled in the configuration.
    /// </summary>
    [Test]
    public async Task DisabledEventTypeIsNotPublishedAsync()
    {
        var gameContext = new Mock<IGameServerContext>();
        gameContext.SetupGet(c => c.EventPublisher).Returns(this._eventPublisher.Object);
        var plugIn = CreatePlugIn();
        plugIn.Configuration!.PublishInvasionEvents = false;

        await plugIn.InvasionStartedAsync(gameContext.Object, new GoldenInvasionPlugIn(), Array.Empty<GameMapDefinition>()).ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Never);
    }

    /// <summary>
    /// Tests that the global notice of a game master is published, without the prefix.
    /// </summary>
    [Test]
    public async Task GlobalNoticeOfGameMasterIsPublishedAsync()
    {
        var gameContext = this.CreateGameServerContext();
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IChatMessageSentPlugIn>(CreatePlugIn());
        var sender = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        sender.SelectedCharacter!.Name = "GameMaster";
        sender.SelectedCharacter.CharacterStatus = CharacterStatus.GameMaster;

        await new ChatMessageAction().ChatMessageAsync(sender, sender.Name, "!Hello", false).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<GlobalNoticeEvent>(e => e.ServerId == ServerId && e.SenderName == "GameMaster" && e.Message == "Hello")),
            Times.Once);
    }

    /// <summary>
    /// Tests that a guild chat message is published without its prefix, when it's enabled.
    /// </summary>
    [Test]
    public async Task GuildChatIsPublishedWhenEnabledAsync()
    {
        var gameContext = this.CreateGameServerContext();
        var plugIn = CreatePlugIn();
        plugIn.Configuration!.PublishChatMessages = true;
        var sender = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        sender.SelectedCharacter!.Name = "Hero";
        sender.GuildStatus = new GuildMemberStatus(7, GuildPosition.NormalMember);

        await plugIn.ChatMessageSentAsync(sender, "@Hello", ChatMessageType.Guild, null).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<ChatMessageEvent>(e => e.ServerId == ServerId && e.Channel == GameChatChannel.Guild && e.GuildId == 7 && e.Sender == "Hero" && e.Message == "Hello")),
            Times.Once);
    }

    /// <summary>
    /// Tests that chat messages aren't published by default, because the chat of the players is private.
    /// </summary>
    [Test]
    public async Task ChatIsNotPublishedByDefaultAsync()
    {
        var gameContext = this.CreateGameServerContext();
        var sender = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);

        await CreatePlugIn().ChatMessageSentAsync(sender, "Hello", ChatMessageType.World, null).ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Never);
    }

    /// <summary>
    /// Tests that the world chat command sends the message to all game servers, and that it's published for Discord.
    /// </summary>
    [Test]
    public async Task WorldChatCommandSendsAndPublishesAsync()
    {
        var gameContext = this.CreateGameServerContext();
        var plugIn = CreatePlugIn();
        plugIn.Configuration!.PublishChatMessages = true;
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IChatMessageSentPlugIn>(plugIn);
        var sender = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        sender.SelectedCharacter!.Name = "Hero";

        await new WorldChatChatCommandPlugIn().HandleCommandAsync(sender, "/world Hello there").ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.WorldChatMessageAsync("Hero", "Hello there"), Times.Once);
        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<ChatMessageEvent>(e => e.Channel == GameChatChannel.World && e.Sender == "Hero" && e.Message == "Hello there")),
            Times.Once);
    }

    /// <summary>
    /// Tests that the world chat command respects the chat ban.
    /// </summary>
    [Test]
    public async Task WorldChatCommandRespectsChatBanAsync()
    {
        var gameContext = this.CreateGameServerContext();
        var sender = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        sender.Account!.ChatBanUntil = DateTime.UtcNow.AddHours(1);

        await new WorldChatChatCommandPlugIn().HandleCommandAsync(sender, "/world Hello").ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.WorldChatMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Tests that an excellent item which got dropped by a monster is published.
    /// </summary>
    [Test]
    public async Task ExcellentDropIsPublishedAsync()
    {
        var (plugIn, monster, killer) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        var item = CreateItem(isExcellent: true);

        await plugIn.MonsterItemDroppedAsync(monster, killer, new DroppedItem(item, monster.Position, monster.CurrentMap, null, null)).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<MonsterItemDroppedEvent>(e => e.ServerId == ServerId && e.IsExcellent && e.ItemName == "Sword" && e.ItemLevel == 9)),
            Times.Once);
    }

    /// <summary>
    /// Tests that a normal item which got dropped by a monster isn't published.
    /// </summary>
    [Test]
    public async Task NormalDropIsNotPublishedAsync()
    {
        var (plugIn, monster, killer) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        var item = CreateItem(isExcellent: false);

        await plugIn.MonsterItemDroppedAsync(monster, killer, new DroppedItem(item, monster.Position, monster.CurrentMap, null, null)).ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Never);
    }

    /// <summary>
    /// Tests that an excellent item isn't published when the excellent drops are disabled in the configuration.
    /// </summary>
    [Test]
    public async Task ExcellentDropIsNotPublishedWhenDisabledAsync()
    {
        var (plugIn, monster, killer) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        plugIn.Configuration!.PublishExcellentItemDrops = false;
        var item = CreateItem(isExcellent: true);

        await plugIn.MonsterItemDroppedAsync(monster, killer, new DroppedItem(item, monster.Position, monster.CurrentMap, null, null)).ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Never);
    }

    /// <summary>
    /// Tests that the kill of a configured boss monster is published.
    /// </summary>
    [Test]
    public async Task BossKillIsPublishedAsync()
    {
        var (plugIn, monster, killer) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        plugIn.Configuration!.BossMonsterNumbers.Add(monster.Definition.Number);

        await plugIn.AttackableGotKilledAsync(monster, killer).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<BossKilledEvent>(e => e.ServerId == ServerId && e.MonsterName == "Kundun" && e.KillerName == killer.Name)),
            Times.Once);
    }

    /// <summary>
    /// Tests that the kill of a monster which isn't configured as boss isn't published.
    /// </summary>
    [Test]
    public async Task KillOfNormalMonsterIsNotPublishedAsync()
    {
        var (plugIn, monster, killer) = await this.CreateDropSetupAsync().ConfigureAwait(false);

        await plugIn.AttackableGotKilledAsync(monster, killer).ConfigureAwait(false);

        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Never);
    }

    /// <summary>
    /// Tests that reaching a configured level milestone is published, and other levels are not.
    /// </summary>
    [Test]
    public async Task LevelMilestoneIsPublishedAsync()
    {
        var (plugIn, _, player) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        plugIn.Configuration!.LevelMilestones = new List<int> { 10 };
        var published = new TaskCompletionSource<CharacterLevelMilestoneEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
        this._eventPublisher.Setup(p => p.GameEventAsync(It.IsAny<CharacterLevelMilestoneEvent>()))
            .Callback<GameEvent>(e => published.TrySetResult((CharacterLevelMilestoneEvent)e))
            .Returns(ValueTask.CompletedTask);

        player.Attributes![Stats.Level] = 9;
        plugIn.CharacterLeveledUp(player);
        player.Attributes[Stats.Level] = 10;
        plugIn.CharacterLeveledUp(player);

        var milestone = await published.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(milestone.Level, Is.EqualTo(10));
        Assert.That(milestone.IsMasterLevel, Is.False);
        this._eventPublisher.Verify(p => p.GameEventAsync(It.IsAny<GameEvent>()), Times.Once);
    }

    /// <summary>
    /// Tests that reaching a configured master level milestone is published.
    /// </summary>
    [Test]
    public async Task MasterLevelMilestoneIsPublishedAsync()
    {
        var (plugIn, _, player) = await this.CreateDropSetupAsync().ConfigureAwait(false);
        plugIn.Configuration!.MasterLevelMilestones = new List<int> { 5 };
        player.Attributes![Stats.MasterLevel] = 5;

        await plugIn.CharacterMasterLeveledUpAsync(player).ConfigureAwait(false);

        this._eventPublisher.Verify(
            p => p.GameEventAsync(It.Is<CharacterLevelMilestoneEvent>(e => e.Level == 5 && e.IsMasterLevel)),
            Times.Once);
    }

    private static GameEventPublisherPlugIn CreatePlugIn()
    {
        return new GameEventPublisherPlugIn { Configuration = new GameEventPublisherConfiguration() };
    }

    private static Item CreateItem(bool isExcellent)
    {
        var item = new BasicModel.Item
        {
            Definition = new BasicModel.ItemDefinition { Name = "Sword" },
            Level = 9,
        };

        if (isExcellent)
        {
            item.ItemOptions.Add(new BasicModel.ItemOptionLink
            {
                ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Excellent },
            });
        }

        return item;
    }

    private async ValueTask<(GameEventPublisherPlugIn PlugIn, Monster Monster, Player Killer)> CreateDropSetupAsync()
    {
        var gameContext = this.CreateGameServerContext();
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        // A unique id, because the attributes of monster definitions are cached by their id.
        var monsterDefinition = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), Designation = "Kundun" };
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };
        var monster = new Monster(
            spawnArea,
            monsterDefinition,
            map,
            NullDropGenerator.Instance,
            new Mock<INpcIntelligence>().Object,
            gameContext.PlugInManager,
            gameContext.PathFinderPool);
        return (CreatePlugIn(), monster, killer);
    }

    private GameServerContext CreateGameServerContext()
    {
        var gameConfiguration = PlayerTestHelper.CreateGameConfiguration();
        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                ServerID = ServerId,
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            this._eventPublisher.Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        // Nothing should run in the background, which could affect other tests.
        gameServerContext.StopPeriodicTasks();
        this._gameContexts.Add(gameServerContext);
        return gameServerContext;
    }
}
