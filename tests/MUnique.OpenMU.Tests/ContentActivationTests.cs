// <copyright file="ContentActivationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.Pathfinding;
using Character = MUnique.OpenMU.DataModel.Entities.Character;
using CharacterClass = MUnique.OpenMU.Persistence.BasicModel.CharacterClass;
using ExitGate = MUnique.OpenMU.Persistence.BasicModel.ExitGate;
using Item = MUnique.OpenMU.Persistence.BasicModel.Item;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemStorage = MUnique.OpenMU.Persistence.BasicModel.ItemStorage;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
using MiniGameDefinition = MUnique.OpenMU.Persistence.BasicModel.MiniGameDefinition;
using MonsterSpawnArea = MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea;
using QuestDefinition = MUnique.OpenMU.Persistence.BasicModel.QuestDefinition;
using QuestReward = MUnique.OpenMU.Persistence.BasicModel.QuestReward;
using QuestRewardType = MUnique.OpenMU.DataModel.Configuration.Quests.QuestRewardType;

/// <summary>
/// Tests that inactive content of the game configuration is not available in the game.
/// </summary>
[TestFixture]
public class ContentActivationTests
{
    /// <summary>
    /// Tests that an inactive map can't be entered, e.g. by a warp or a gate.
    /// </summary>
    [Test]
    public async ValueTask InactiveMapCanNotBeEnteredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var mapDefinition = player.SelectedCharacter!.CurrentMap!;

        Assert.That(mapDefinition.TryGetRequirementError(player, out _), Is.False);

        mapDefinition.IsActive = false;

        Assert.That(mapDefinition.TryGetRequirementError(player, out var errorMessage), Is.True);
        Assert.That(errorMessage, Is.Not.Null.And.Not.Empty);
    }

    /// <summary>
    /// Tests that the map initializer doesn't create instances of inactive maps.
    /// </summary>
    [Test]
    public void InactiveMapIsNotCreated()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var mapInitializer = new MapInitializer(gameContext.Configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);

        Assert.That(mapInitializer.CreateGameMap((ushort)mapDefinition.Number), Is.Not.Null);

        mapDefinition.IsActive = false;

        Assert.That(mapInitializer.CreateGameMap((ushort)mapDefinition.Number), Is.Null);
    }

    /// <summary>
    /// Tests that inactive monsters and NPCs are not spawned on a map.
    /// </summary>
    [Test]
    public async ValueTask InactiveMonsterIsNotSpawnedAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var activeNpc = new MonsterDefinition { Id = Guid.NewGuid(), Number = 1, ObjectKind = NpcObjectKind.PassiveNpc };
        var inactiveNpc = new MonsterDefinition { Id = Guid.NewGuid(), Number = 2, ObjectKind = NpcObjectKind.PassiveNpc, IsActive = false };
        mapDefinition.MonsterSpawns.Add(CreateSpawnArea(mapDefinition, activeNpc));
        mapDefinition.MonsterSpawns.Add(CreateSpawnArea(mapDefinition, inactiveNpc));

        var map = await gameContext.GetMapAsync((ushort)mapDefinition.Number).ConfigureAwait(false);
        var npcs = map!.GetNpcsInRange(new Point(100, 100), 10);

        Assert.That(npcs.Select(npc => npc.Definition), Is.EquivalentTo(new[] { activeNpc }));
    }

    /// <summary>
    /// Tests that a merchant store doesn't offer inactive items.
    /// </summary>
    [Test]
    public void MerchantStoreDoesNotOfferInactiveItems()
    {
        var activeItem = new Item { Definition = new ItemDefinition { Number = 1 } };
        var inactiveItem = new Item { Definition = new ItemDefinition { Number = 2, IsActive = false } };
        var store = new ItemStorage();
        store.Items.Add(activeItem);

        Assert.That(store.GetOfferedItems(), Is.SameAs(store.Items), "When all items are active, the items are offered as is.");

        store.Items.Add(inactiveItem);

        Assert.That(store.GetOfferedItems(), Is.EquivalentTo(new[] { activeItem }));
    }

    /// <summary>
    /// Tests that no character of an inactive character class can be created.
    /// </summary>
    /// <param name="isActive">If set to <c>true</c>, the character class is active.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask CharacterOfInactiveClassIsNotCreatedAsync(bool isActive)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        Mock.Get(player.Account!).Setup(a => a.Characters).Returns(new List<Character>());
        var characterClass = new CharacterClass
        {
            Number = 1,
            CanGetCreated = true,
            IsActive = isActive,
            HomeMap = player.GameContext.Configuration.Maps.Single(),
        };
        player.GameContext.Configuration.CharacterClasses.Add(characterClass);
        player.GameContext.Configuration.MaximumCharactersPerAccount = 5;

        await new CreateCharacterAction().CreateCharacterAsync(player, "Tester", characterClass.Number).ConfigureAwait(false);

        Assert.That(player.Account!.Characters.Any(c => c.CharacterClass == characterClass), Is.EqualTo(isActive));
    }

    /// <summary>
    /// Tests that a quest which evolves the character into the next generation class
    /// is blocked when that class is inactive, while other quests are not affected.
    /// </summary>
    /// <param name="isNextClassActive">If set to <c>true</c>, the next generation class is active.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask QuestWhichEvolvesIntoInactiveClassIsBlockedAsync(bool isNextClassActive)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var nextClass = new CharacterClass { Number = 2, IsActive = isNextClassActive };
        Mock.Get(player.SelectedCharacter!.CharacterClass!).Setup(c => c.NextGenerationClass).Returns(nextClass);
        var evolutionQuest = new QuestDefinition();
        evolutionQuest.Rewards.Add(new QuestReward { RewardType = QuestRewardType.CharacterEvolutionFirstToSecond });
        var otherQuest = new QuestDefinition();
        otherQuest.Rewards.Add(new QuestReward { RewardType = QuestRewardType.LevelUpPoints, Value = 10 });

        Assert.That(evolutionQuest.EvolvesIntoInactiveClass(player), Is.EqualTo(!isNextClassActive));
        Assert.That(otherQuest.EvolvesIntoInactiveClass(player), Is.False);
    }

    /// <summary>
    /// Tests that an inactive monster isn't spawned when a spawn is initialized directly,
    /// like the mini games do it, e.g. Chaos Castle and the spawns of change events.
    /// </summary>
    [Test]
    public async ValueTask InactiveMonsterIsNotSpawnedDirectlyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var map = await gameContext.GetMapAsync((ushort)mapDefinition.Number).ConfigureAwait(false);
        var mapInitializer = new MapInitializer(gameContext.Configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null)
        {
            PlugInManager = gameContext.PlugInManager,
            PathFinderPool = gameContext.PathFinderPool,
        };
        var inactiveNpc = new MonsterDefinition { Id = Guid.NewGuid(), Number = 2, ObjectKind = NpcObjectKind.PassiveNpc, IsActive = false };

        var npc = await mapInitializer.InitializeSpawnAsync(0, map!, CreateSpawnArea(mapDefinition, inactiveNpc)).ConfigureAwait(false);

        Assert.That(npc, Is.Null);
        Assert.That(map!.GetNpcsInRange(new Point(100, 100), 10), Is.Empty);
    }

    /// <summary>
    /// Tests that a mini game is only available when it's active, and its entrance map, too.
    /// </summary>
    [Test]
    public void MiniGameIsOnlyAvailableWithActiveEntranceMap()
    {
        var entranceMap = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition { Number = 11 };
        var miniGame = new MiniGameDefinition { Type = MiniGameType.BloodCastle, GameLevel = 1, Entrance = new ExitGate { Map = entranceMap } };

        Assert.That(miniGame.IsAvailable(), Is.True);

        entranceMap.IsActive = false;
        Assert.That(miniGame.IsAvailable(), Is.False, "The mini game is still active, but its map is not.");

        entranceMap.IsActive = true;
        miniGame.IsActive = false;
        Assert.That(miniGame.IsAvailable(), Is.False);
    }

    /// <summary>
    /// Tests that a mini game without an entrance map isn't available, because its map can't be created.
    /// </summary>
    [Test]
    public void MiniGameWithoutEntranceMapIsNotAvailable()
    {
        Assert.That(new MiniGameDefinition { Type = MiniGameType.BloodCastle }.IsAvailable(), Is.False, "Without entrance");
        Assert.That(new MiniGameDefinition { Type = MiniGameType.BloodCastle, Entrance = new ExitGate() }.IsAvailable(), Is.False, "Without entrance map");
    }

    /// <summary>
    /// Tests that the spawn map is the first active candidate with a spawn gate, or else the active map with the lowest number.
    /// </summary>
    [Test]
    public void ActiveSpawnMapSkipsInactiveMaps()
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        configuration.Maps.Clear();
        var lorencia = AddMap(configuration, 0, isActive: true);
        var inactive = AddMap(configuration, 1, isActive: false);
        var withoutGate = AddMap(configuration, 2, isActive: true, hasSpawnGate: false);
        var noria = AddMap(configuration, 3, isActive: true);

        Assert.That(configuration.GetActiveSpawnMap(inactive, withoutGate, noria), Is.SameAs(noria));
        Assert.That(configuration.GetActiveSpawnMap(inactive, withoutGate), Is.SameAs(lorencia), "Falls back to the active map with the lowest number.");

        lorencia.IsActive = false;
        noria.IsActive = false;
        Assert.That(configuration.GetActiveSpawnMap(inactive, withoutGate), Is.Null);
    }

    /// <summary>
    /// Tests that a player respawns on an active map, even if the current map and its safezone map are inactive.
    /// </summary>
    [Test]
    public async ValueTask RespawnTargetsAnActiveMapAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = player.GameContext.Configuration;
        var safezoneMap = AddMap(configuration, 2, isActive: false);
        var currentMap = AddMap(configuration, 1, isActive: false);
        currentMap.SafezoneMap = safezoneMap;
        var homeMap = AddMap(configuration, 3, isActive: true);
        Mock.Get(player.SelectedCharacter!.CharacterClass!).Setup(c => c.HomeMap).Returns(homeMap);
        await player.CurrentMap!.RemoveAsync(player).ConfigureAwait(false);
        player.SetCurrentMapSilently(null);
        player.SelectedCharacter.CurrentMap = currentMap;

        var gate = await player.GetSpawnGateOfCurrentMapAsync().ConfigureAwait(false);

        Assert.That(gate.Map, Is.SameAs(homeMap));
    }

    /// <summary>
    /// Tests that no character is created when the home map of its class is inactive,
    /// because the character would start on content which isn't available.
    /// </summary>
    [Test]
    public async ValueTask CharacterIsNotCreatedOnInactiveHomeMapAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        Mock.Get(player.Account!).Setup(a => a.Characters).Returns(new List<Character>());
        var characterClass = new CharacterClass
        {
            Number = 1,
            CanGetCreated = true,
            HomeMap = AddMap(player.GameContext.Configuration, 1, isActive: false),
        };
        player.GameContext.Configuration.CharacterClasses.Add(characterClass);
        player.GameContext.Configuration.MaximumCharactersPerAccount = 5;

        await new CreateCharacterAction().CreateCharacterAsync(player, "Tester", characterClass.Number).ConfigureAwait(false);

        Assert.That(player.Account!.Characters, Is.Empty);
    }

    /// <summary>
    /// Tests that the drops of Chaos Castle are distributed when fewer monsters spawn than there
    /// are drops, e.g. because most of its monsters are inactive. Before, it didn't terminate.
    /// </summary>
    [Test]
    public async Task ChaosCastleDropsWithFewerMonstersThanDropsAsync()
    {
        var (gameContext, chaosCastle) = CreateChaosCastle(isJewelActive: true);
        await using var disposable = chaosCastle;

        var dropGenerator = await Task.Run(() => new ChaosCastleDropGenerator(gameContext, chaosCastle, 2)).WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        Assert.That(await CountDropsAsync(dropGenerator, 2).ConfigureAwait(false), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the monsters of Chaos Castle don't drop inactive jewels, but the active ones.
    /// </summary>
    /// <param name="isJewelActive">If set to <c>true</c>, the jewels are active.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task ChaosCastleDropsOnlyActiveJewelsAsync(bool isJewelActive)
    {
        var (gameContext, chaosCastle) = CreateChaosCastle(isJewelActive);
        await using var disposable = chaosCastle;

        var dropGenerator = new ChaosCastleDropGenerator(gameContext, chaosCastle, 50);

        // Chaos Castle 7 drops two Jewels of Bless and three Jewels of Soul.
        Assert.That(await CountDropsAsync(dropGenerator, 50).ConfigureAwait(false), Is.EqualTo(isJewelActive ? 5 : 0));
    }

    /// <summary>
    /// Tests that the entrance to a quest map is rejected before the fee is charged, when the map got
    /// deactivated, even though an instance of the map still exists.
    /// </summary>
    [Test]
    public async ValueTask InactiveQuestMapIsNotEnteredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var questMap = AddMap(player.GameContext.Configuration, 1, isActive: true);
        _ = await player.GameContext.GetMapAsync((ushort)questMap.Number).ConfigureAwait(false);
        questMap.IsActive = false;
        var action = new TestEnterQuestMapAction(questMap.Number);
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { Number = TestEnterQuestMapAction.NpcNumber }, null!);
        Mock.Get(player.SelectedCharacter!).Setup(c => c.QuestStates).Returns(new List<DataModel.Entities.CharacterQuestState>
        {
            new Persistence.BasicModel.CharacterQuestState { Group = TestEnterQuestMapAction.QuestGroup, ActiveQuest = new QuestDefinition { Number = TestEnterQuestMapAction.QuestNumber } },
        });
        player.Money = 1000;

        await action.TryEnterQuestMapAsync(player).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1000), "The fee isn't charged.");
        Assert.That(player.SelectedCharacter!.CurrentMap, Is.Not.SameAs(questMap));
    }

    /// <summary>
    /// Tests that a warp to an inactive map is refused, even though an instance of the map still exists.
    /// </summary>
    [Test]
    public async ValueTask WarpToInactiveMapIsRefusedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var currentMap = player.CurrentMap;
        var targetMap = AddMap(player.GameContext.Configuration, 1, isActive: true);
        _ = await player.GameContext.GetMapAsync((ushort)targetMap.Number).ConfigureAwait(false);
        targetMap.IsActive = false;

        await player.WarpToAsync(targetMap.ExitGates.Single()).ConfigureAwait(false);

        Assert.That(player.CurrentMap, Is.SameAs(currentMap));
        Assert.That(player.SelectedCharacter!.CurrentMap, Is.Not.SameAs(targetMap));
    }

    private static (IGameContext GameContext, ChaosCastleContext ChaosCastle) CreateChaosCastle(bool isJewelActive)
    {
        var configuration = new Mock<GameConfiguration>();
        configuration.Setup(c => c.Items).Returns(new List<DataModel.Configuration.Items.ItemDefinition>
        {
            new ItemDefinition { Group = 14, Number = 13, IsActive = isJewelActive },
            new ItemDefinition { Group = 14, Number = 14, IsActive = isJewelActive },
        });
        var gameContext = new Mock<IGameContext>();
        gameContext.SetupGet(c => c.Configuration).Returns(configuration.Object);
        gameContext.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        gameContext.SetupGet(c => c.DropGenerator).Returns(NullDropGenerator.Instance);
        var mapInitializer = new Mock<IMapInitializer>();
        mapInitializer.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));
        var definition = new Mock<DataModel.Configuration.MiniGameDefinition>();
        definition.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definition.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definition.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition { Number = 18 } });
        definition.Object.Type = MiniGameType.ChaosCastle;
        definition.Object.GameLevel = 7; // Chaos Castle 7 drops five jewels.
        var chaosCastle = new ChaosCastleContext(new MiniGameMapKey(18, 7, string.Empty), definition.Object, gameContext.Object, mapInitializer.Object);
        return (gameContext.Object, chaosCastle);
    }

    private static async ValueTask<int> CountDropsAsync(ChaosCastleDropGenerator dropGenerator, int killedMonsters)
    {
        var drops = 0;
        for (var i = 0; i < killedMonsters; i++)
        {
            var (items, _) = await dropGenerator.GenerateItemDropsAsync(new MonsterDefinition(), 0, null!).ConfigureAwait(false);
            drops += items.Count();
        }

        return drops;
    }

    private static GameMapDefinition AddMap(GameConfiguration configuration, short number, bool isActive, bool hasSpawnGate = true)
    {
        var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition { Number = number, IsActive = isActive, TerrainData = new byte[ushort.MaxValue + 3] };
        if (hasSpawnGate)
        {
            map.ExitGates.Add(new ExitGate { Map = map, IsSpawnGate = true, X1 = 10, Y1 = 10, X2 = 12, Y2 = 12 });
        }

        configuration.Maps.Add(map);
        return map;
    }

    private static MonsterSpawnArea CreateSpawnArea(GameMapDefinition mapDefinition, DataModel.Configuration.MonsterDefinition monsterDefinition)
    {
        return new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = mapDefinition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.Automatic,
        };
    }

    /// <summary>
    /// An action which enters a quest map, like the ones of the original game.
    /// </summary>
    private sealed class TestEnterQuestMapAction : EnterQuestMapAction
    {
        /// <summary>
        /// The number of the NPC which lets the player enter the quest map.
        /// </summary>
        public const short NpcNumber = 1;

        /// <summary>
        /// The group of the quest which qualifies to enter.
        /// </summary>
        public const byte QuestGroup = 1;

        /// <summary>
        /// The number of the quest which qualifies to enter.
        /// </summary>
        public const short QuestNumber = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnterQuestMapAction"/> class.
        /// </summary>
        /// <param name="targetMapNumber">The number of the quest map.</param>
        public TestEnterQuestMapAction(short targetMapNumber)
            : base(NpcNumber, 100, targetMapNumber, QuestGroup, QuestNumber)
        {
        }
    }
}
