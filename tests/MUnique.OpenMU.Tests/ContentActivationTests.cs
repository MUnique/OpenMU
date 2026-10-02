// <copyright file="ContentActivationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.Pathfinding;
using Character = MUnique.OpenMU.DataModel.Entities.Character;
using CharacterClass = MUnique.OpenMU.Persistence.BasicModel.CharacterClass;
using Item = MUnique.OpenMU.Persistence.BasicModel.Item;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemStorage = MUnique.OpenMU.Persistence.BasicModel.ItemStorage;
using MonsterDefinition = MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition;
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
}
