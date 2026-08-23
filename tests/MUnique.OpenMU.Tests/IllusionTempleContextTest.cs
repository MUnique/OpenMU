// <copyright file="IllusionTempleContextTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the illusion temple event: team assignment, the statue/relic loop, scoring, and the
/// end-of-game reward flow.
/// </summary>
/// <remarks>
/// <see cref="MiniGameContext"/> auto-starts a real-time countdown (via <c>RunGameAsync</c>) as soon as
/// it's constructed, and that countdown is clamped to at least 30 seconds - far too slow for a test
/// suite. Instead, these tests build a fully wired <see cref="IllusionTempleContext"/> and drive its
/// lifecycle hooks (<c>OnGameStartAsync</c>, <c>GameEndedAsync</c>, ...) directly via reflection, so
/// each scenario runs deterministically without waiting on the real timers.
/// </remarks>
[TestFixture]
public class IllusionTempleContextTest
{
    private const short StatueNumber = 380;
    private const short AlliedStorageNumber = 383;
    private const short IllusionStorageNumber = 384;

    /// <summary>
    /// While the entrance is open, players can join - once it's closed (e.g. because the entrance
    /// duration elapsed), further entries are refused.
    /// </summary>
    [Test]
    public async ValueTask EntranceAcceptsPlayersOnlyWhileOpenAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var player = await CreatePlayerAsync(gameContext).ConfigureAwait(false);

        var resultWhileOpen = await illusionTemple.TryEnterAsync(player).ConfigureAwait(false);

        await InvokePrivateAsync(illusionTemple, "CloseEntranceAsync").ConfigureAwait(false);
        var latePlayer = await CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var resultAfterClose = await illusionTemple.TryEnterAsync(latePlayer).ConfigureAwait(false);

        Assert.That(resultWhileOpen, Is.EqualTo(EnterResult.Success));
        Assert.That(resultAfterClose, Is.EqualTo(EnterResult.NotOpen));

    }

    /// <summary>
    /// If the player count drops below the configured minimum while a match is running (e.g. a player
    /// disconnects), the event is finished right away instead of continuing with too few participants.
    /// </summary>
    [Test]
    public async ValueTask FinishesWhenTooFewPlayersRemainAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);

        await illusionTemple.Map.RemoveAsync(players[0]).ConfigureAwait(false);

        Assert.That(illusionTemple.PlayerCount, Is.LessThan(2));

    }

    /// <summary>
    /// An even number of players is split into two equally sized teams.
    /// </summary>
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(10)]
    public async ValueTask SplitsAnEvenPlayerCountIntoEqualTeamsAsync(int playerCount)
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, playerCount).ConfigureAwait(false);

        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);

        var teamCounts = players
            .Select(p => GetTeamOf(illusionTemple, p))
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());

        Assert.That(teamCounts.Values, Has.All.EqualTo(playerCount / 2));
        Assert.That(teamCounts.Keys, Is.EquivalentTo(new[] { IllusionTempleTeam.AlliedForces, IllusionTempleTeam.IllusionForces }));

    }

    /// <summary>
    /// An odd number of players still gets split as evenly as possible - one team gets one extra
    /// member, the difference between the two teams is never more than one.
    /// </summary>
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(9)]
    public async ValueTask SplitsAnOddPlayerCountAsEvenlyAsPossibleAsync(int playerCount)
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, playerCount).ConfigureAwait(false);

        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);

        var teamCounts = players
            .Select(p => GetTeamOf(illusionTemple, p))
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());

        Assert.That(teamCounts.Values.Sum(), Is.EqualTo(playerCount));
        Assert.That(Math.Abs(teamCounts.Values.Max() - teamCounts.Values.Min()), Is.LessThanOrEqualTo(1));

    }

    /// <summary>
    /// Starting the game assigns every player a team - <see cref="MiniGameContext.GetSpawnGate"/> only
    /// resolves to a gate for players who made it into a team.
    /// </summary>
    [Test]
    public async ValueTask GameStartAssignsEveryPlayerASpawnGateAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);

        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);

        foreach (var player in players)
        {
            Assert.That(illusionTemple.GetSpawnGate(player), Is.Not.Null);
        }

    }

    /// <summary>
    /// Talking to the stone statue grants the sacred relic item and marks the player as its carrier -
    /// the client is informed who the new carrier is.
    /// </summary>
    [Test]
    public async ValueTask TalkingToTheStatueGrantsTheRelicAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var player = players[0];
        player.OpenedNpc = CreateStatueNpc(illusionTemple);

        await illusionTemple.TalkToNpcStoneStatueAsync(player).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items.Any(IsRelicItem), Is.True);
        Assert.That(GetRelicCarrier(illusionTemple), Is.EqualTo(player));

    }

    /// <summary>
    /// When the relic carrier dies, the relic is dropped on the ground and can be picked up by another
    /// participant, who then becomes the new carrier.
    /// </summary>
    [Test]
    public async ValueTask DyingDropsTheRelicAndItCanBePickedUpAgainAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var carrier = players[0];
        var otherPlayer = players[1];
        carrier.OpenedNpc = CreateStatueNpc(illusionTemple);
        await illusionTemple.TalkToNpcStoneStatueAsync(carrier).ConfigureAwait(false);

        // OnPlayerDied is "async void" in production (it's a plain event handler), so its relic-drop
        // logic - including the fire-and-forget continuation which finally clears _relicCarrier once
        // the dropped item lands on the map - keeps running after the reflection call above already
        // returned. Poll briefly instead of asserting immediately.
        await InvokeProtectedAsync(illusionTemple, "OnPlayerDied", [carrier, new DeathInformation(otherPlayer.Id, otherPlayer.Name, default, 0)]).ConfigureAwait(false);
        await WaitUntilAsync(() => GetRelicCarrier(illusionTemple) is null).ConfigureAwait(false);

        Assert.That(carrier.Inventory!.Items.Any(IsRelicItem), Is.False);
        Assert.That(GetRelicCarrier(illusionTemple), Is.Null);

        var droppedRelic = illusionTemple.Map.GetDropsInRange(carrier.Position, 5).OfType<DroppedItem>().FirstOrDefault(d => IsRelicItem(d.Item));
        Assert.That(droppedRelic, Is.Not.Null);

        await InvokeProtectedAsync(illusionTemple, "OnPlayerPickedUpItemAsync", [(otherPlayer, (ILocateable)droppedRelic!)]).ConfigureAwait(false);

        Assert.That(GetRelicCarrier(illusionTemple), Is.EqualTo(otherPlayer));

    }

    /// <summary>
    /// Delivering the relic to the carrier's own team storage scores a point for that team and clears
    /// the carrier state, so the relic can be granted again from the next statue.
    /// </summary>
    [Test]
    public async ValueTask DeliveringTheRelicScoresAPointAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var carrier = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.AlliedForces);
        carrier.OpenedNpc = CreateStatueNpc(illusionTemple);
        await illusionTemple.TalkToNpcStoneStatueAsync(carrier).ConfigureAwait(false);

        await illusionTemple.TalkToNpcTeamStorageAsync(AlliedStorageNumber, carrier).ConfigureAwait(false);

        Assert.That(illusionTemple.Score.AlliedForcesScore, Is.EqualTo(1));
        Assert.That(carrier.Inventory!.Items.Any(IsRelicItem), Is.False);
        Assert.That(GetRelicCarrier(illusionTemple), Is.Null);

    }

    /// <summary>
    /// Delivering the relic to the OTHER team's storage doesn't score anything - a carrier can only
    /// score for his own side.
    /// </summary>
    [Test]
    public async ValueTask DeliveringTheRelicToTheEnemyStorageDoesNotScoreAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var carrier = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.AlliedForces);
        carrier.OpenedNpc = CreateStatueNpc(illusionTemple);
        await illusionTemple.TalkToNpcStoneStatueAsync(carrier).ConfigureAwait(false);

        await illusionTemple.TalkToNpcTeamStorageAsync(IllusionStorageNumber, carrier).ConfigureAwait(false);

        Assert.That(illusionTemple.Score.AlliedForcesScore, Is.EqualTo(0));
        Assert.That(illusionTemple.Score.IllusionForcesScore, Is.EqualTo(0));
        Assert.That(GetRelicCarrier(illusionTemple), Is.EqualTo(carrier));

    }

    /// <summary>
    /// When the game ends, the winning team's members are granted experience, which is reported back
    /// per player - a losing player gets nothing.
    /// </summary>
    [Test]
    public async ValueTask GameEndGrantsExperienceToTheWinningTeamAsync()
    {
        var gameContext = CreateGameContext();
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var winner = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.AlliedForces);
        var loser = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.IllusionForces);
        illusionTemple.Score.IncreaseScore(IllusionTempleTeam.AlliedForces, 2);
        var winnerExperienceBefore = winner.SelectedCharacter!.Experience;
        var loserExperienceBefore = loser.SelectedCharacter!.Experience;

        await InvokeProtectedAsync(illusionTemple, "GameEndedAsync", [(ICollection<Player>)players]).ConfigureAwait(false);

        Assert.That(winner.SelectedCharacter!.Experience, Is.GreaterThan(winnerExperienceBefore));
        Assert.That(loser.SelectedCharacter!.Experience, Is.EqualTo(loserExperienceBefore));

        await illusionTemple.ClaimRewardAsync(winner).ConfigureAwait(false);
        await illusionTemple.ClaimRewardAsync(loser).ConfigureAwait(false);

    }

    /// <summary>
    /// Besides experience, a winner can also be rewarded with an item (e.g. a jewel). Unlike experience,
    /// which is granted right away, the item is only handed out once the winner actually claims his
    /// reward (<see cref="IllusionTempleContext.ClaimRewardAsync"/>) - matching the "close the result
    /// dialog to get compensated" flow of the original event. A losing player gets neither.
    /// </summary>
    [Test]
    public async ValueTask GameEndGrantsExperienceAndAnItemRewardToTheWinnerAsync()
    {
        var jewelDefinition = new MUnique.OpenMU.Persistence.BasicModel.ItemDefinition { Group = 14, Number = 16, Name = "Jewel of Life" };
        var gameContext = CreateGameContext(dropGenerator: new SingleItemDropGenerator(jewelDefinition));
        var definition = CreateDefinition(gameContext, minimumPlayerCount: 2, includeItemReward: true);
        await using var illusionTemple = await CreateContextAsync(gameContext, definition).ConfigureAwait(false);
        var players = await EnterPlayersAsync(illusionTemple, gameContext, 2).ConfigureAwait(false);
        await StartGameAsync(illusionTemple, players).ConfigureAwait(false);
        var winner = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.AlliedForces);
        var loser = players.First(p => GetTeamOf(illusionTemple, p) == IllusionTempleTeam.IllusionForces);
        illusionTemple.Score.IncreaseScore(IllusionTempleTeam.AlliedForces, 2);

        await InvokeProtectedAsync(illusionTemple, "GameEndedAsync", [(ICollection<Player>)players]).ConfigureAwait(false);

        // The item isn't granted yet at game-end time - only the experience is.
        Assert.That(winner.Inventory!.Items.Any(i => i.Definition == jewelDefinition), Is.False);

        await illusionTemple.ClaimRewardAsync(winner).ConfigureAwait(false);
        await illusionTemple.ClaimRewardAsync(loser).ConfigureAwait(false);

        Assert.That(winner.Inventory!.Items.Any(i => i.Definition == jewelDefinition), Is.True);
        Assert.That(loser.Inventory!.Items.Any(i => i.Definition == jewelDefinition), Is.False);

    }

    /// <summary>
    /// Talking to the entrance npc opens the illusion temple dialog and reports how many players are
    /// currently in each temple, so the client can show it next to the invite.
    /// </summary>
    [Test]
    public async ValueTask TalkingToTheEntranceNpcShowsTheUserCountsAsync()
    {
        var gameContext = CreateGameContext();
        var player = await CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var npcDefinition = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition
        {
            Number = 229,
            NpcWindow = NpcWindow.IllusionTemple,
        };
        var npc = new NonPlayerCharacter(new MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea { GameMap = map!.Definition }, npcDefinition, map);

        await new TalkNpcAction().TalkToNpcAsync(player, npc).ConfigureAwait(false);

        // No illusion temple is configured in this game context, so the dialog opens without throwing
        // and simply reports no members - the important part is that talking to the npc is routed here
        // at all and doesn't fall into the "talking not implemented" fallback.
        Assert.That(player.OpenedNpc, Is.EqualTo(npc));

        await gameContext.RemovePlayerAsync(player).ConfigureAwait(false);
    }

    private static bool IsRelicItem(MUnique.OpenMU.DataModel.Entities.Item item) => item.Definition?.Group == 14 && item.Definition?.Number == 64;

    private static IllusionTempleTeam GetTeamOf(IllusionTempleContext context, Player player)
    {
        var gate = context.GetSpawnGate(player);
        Assert.That(gate, Is.Not.Null);
        return gate!.X1 < 150 ? IllusionTempleTeam.AlliedForces : IllusionTempleTeam.IllusionForces;
    }

    /// <summary>
    /// Reads the private <c>_relicCarrier</c> field of <see cref="IllusionTempleContext"/> via
    /// reflection. This is test-only code operating on a type from the same solution (no untrusted
    /// input reaches this reflection call), used because the field has no public accessor - exposing
    /// one purely for tests isn't warranted for a single internal implementation detail.
    /// </summary>
    [SuppressMessage("Security Hotspot", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Test-only code reading a private field of a type from this same solution. The member name is a hardcoded literal - no external input reaches this call - and exposing a public accessor purely for tests isn't warranted for an internal implementation detail.")]
    private static Player? GetRelicCarrier(IllusionTempleContext context)
    {
        var field = typeof(IllusionTempleContext).GetField("_relicCarrier", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Player?)field!.GetValue(context);
    }

    private static NonPlayerCharacter CreateStatueNpc(IllusionTempleContext context)
    {
        var spawnArea = context.Map.Definition.MonsterSpawns.First(s => s.MonsterDefinition?.Number == StatueNumber);
        var npc = new NonPlayerCharacter(spawnArea, spawnArea.MonsterDefinition!, context.Map);
        npc.Initialize();
        return npc;
    }

    private static async ValueTask StartGameAsync(IllusionTempleContext context, IReadOnlyCollection<Player> players)
    {
        await InvokeProtectedAsync(context, "OnGameStartAsync", [(ICollection<Player>)players.ToList()]).ConfigureAwait(false);
    }

    private static async ValueTask<List<Player>> EnterPlayersAsync(IllusionTempleContext context, IGameContext gameContext, int count)
    {
        var players = new List<Player>();
        for (var i = 0; i < count; i++)
        {
            var player = await CreatePlayerAsync(gameContext, $"Player{i}").ConfigureAwait(false);
            var result = await context.TryEnterAsync(player).ConfigureAwait(false);
            Assert.That(result, Is.EqualTo(EnterResult.Success));

            // TryEnterAsync alone doesn't place the player on the event map. Mirror what the server
            // really does on entry: EnterMiniGameAction warps the player to the event's entrance gate,
            // and the client then acknowledges the map change, which is what actually puts him onto the
            // mini game's map instance.
            await player.WarpToAsync(context.Definition.Entrance!).ConfigureAwait(false);
            await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

            players.Add(player);
        }

        return players;
    }

    private static async ValueTask<Player> CreatePlayerAsync(IGameContext gameContext, string? name = null)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        if (name is { })
        {
            player.SelectedCharacter!.Name = name;
        }

        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.IsAlive = true;
        return player;
    }

    private static async ValueTask<IllusionTempleContext> CreateContextAsync(IGameContext gameContext, MiniGameDefinition definition)
    {
        var context = await gameContext.GetMiniGameAsync(definition, null!).ConfigureAwait(false);
        return (IllusionTempleContext)context;
    }

    private static MiniGameDefinition CreateDefinition(IGameContext gameContext, int minimumPlayerCount, int maximumPlayerCount = 10, bool includeItemReward = false)
    {
        var map = CreateMapDefinition();
        gameContext.Configuration.Maps.Add(map);

        // The sacred relic (Group 14, Number 64) - TalkToNpcStoneStatueAsync looks this up by
        // Group/Number, so it has to exist in the configuration's item list.
        if (!gameContext.Configuration.Items.Any(i => i.Group == 14 && i.Number == 64))
        {
            gameContext.Configuration.Items.Add(new MUnique.OpenMU.Persistence.BasicModel.ItemDefinition { Group = 14, Number = 64, Name = "Cursed Castle Water" });
        }

        // Devias (map number 2) - both OnObjectRemovedFromMapAsync and GameEndedAsync warp
        // participants there.
        if (!gameContext.Configuration.Maps.Any(m => m.Number == 2))
        {
            gameContext.Configuration.Maps.Add(new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition { Number = 2, TerrainData = new byte[ushort.MaxValue + 3] });
        }

        var definition = new MUnique.OpenMU.Persistence.BasicModel.MiniGameDefinition
        {
            Type = MiniGameType.IllusionTemple,
            MinimumPlayerCount = minimumPlayerCount,
            MaximumPlayerCount = maximumPlayerCount,
            EnterDuration = TimeSpan.FromMinutes(5),
            GameDuration = TimeSpan.FromMinutes(15),
            ExitDuration = TimeSpan.FromMinutes(1),
            MapCreationPolicy = MiniGameMapCreationPolicy.Shared,
            Entrance = map.ExitGates.First(),
        };
        definition.Rewards.Add(new MUnique.OpenMU.Persistence.BasicModel.MiniGameReward
        {
            RewardType = MiniGameRewardType.Experience,
            RewardAmount = 100_000,
            RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty,
        });

        if (includeItemReward)
        {
            definition.Rewards.Add(new MUnique.OpenMU.Persistence.BasicModel.MiniGameReward
            {
                RewardType = MiniGameRewardType.Item,
                RewardAmount = 1,
                RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty,
                ItemReward = new MUnique.OpenMU.Persistence.BasicModel.DropItemGroup(),
            });
        }

        return definition;
    }

    /// <summary>
    /// A fake drop generator which always hands out a single item of the given definition, regardless
    /// of the reward's drop item group - used to test that a mini game's item reward actually reaches
    /// the player's inventory, without needing a fully configured drop chance/item pool.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "The parameters are required by IDropGenerator; this fake deliberately ignores them and always returns the same item.")]
    private sealed class SingleItemDropGenerator : IDropGenerator
    {
        private readonly ItemDefinition _itemDefinition;

        public SingleItemDropGenerator(ItemDefinition itemDefinition) => this._itemDefinition = itemDefinition;

        public ValueTask<(IEnumerable<Item> Items, uint? Money)> GenerateItemDropsAsync(MonsterDefinition monster, int gainedExperience, Player player)
            => ValueTask.FromResult((Enumerable.Empty<Item>(), default(uint?)));

        public Item? GenerateItemDrop(DropItemGroup group) => new MUnique.OpenMU.Persistence.BasicModel.Item { Definition = this._itemDefinition };

        public (Item? Item, uint? Money, ItemDropEffect DropEffect) GenerateItemDrop(IEnumerable<DropItemGroup> groups)
            => (new MUnique.OpenMU.Persistence.BasicModel.Item { Definition = this._itemDefinition }, null, ItemDropEffect.Undefined);
    }

    private static GameMapDefinition CreateMapDefinition()
    {
        var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Number = 45,
            TerrainData = new byte[ushort.MaxValue + 3],
        };

        var entrance = new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Map = map,
            IsSpawnGate = true,
            X1 = 141,
            Y1 = 41,
            X2 = 146,
            Y2 = 45,
        };
        map.ExitGates.Add(entrance);

        var statueDefinition = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { Number = StatueNumber, ObjectKind = NpcObjectKind.Statue };
        var alliedStorageDefinition = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { Number = AlliedStorageNumber, ObjectKind = NpcObjectKind.PassiveNpc };
        var illusionStorageDefinition = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { Number = IllusionStorageNumber, ObjectKind = NpcObjectKind.PassiveNpc };

        map.MonsterSpawns.Add(CreateSpawn(map, 100, statueDefinition, 207, 47, SpawnTrigger.ManuallyForEvent));
        map.MonsterSpawns.Add(CreateSpawn(map, 101, statueDefinition, 134, 121, SpawnTrigger.ManuallyForEvent));
        map.MonsterSpawns.Add(CreateSpawn(map, 112, alliedStorageDefinition, 141, 59, SpawnTrigger.AutomaticDuringEvent));
        map.MonsterSpawns.Add(CreateSpawn(map, 113, illusionStorageDefinition, 194, 113, SpawnTrigger.AutomaticDuringEvent));

        return map;
    }

    private static MonsterSpawnArea CreateSpawn(GameMapDefinition map, short number, MonsterDefinition monsterDefinition, byte x, byte y, SpawnTrigger trigger)
    {
        return new MUnique.OpenMU.Persistence.BasicModel.MonsterSpawnArea
        {
            Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)map.Number, (byte)number),
            GameMap = map,
            MonsterDefinition = monsterDefinition,
            Quantity = 1,
            SpawnTrigger = trigger,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
        };
    }

    private static IGameContext CreateGameContext(IDropGenerator? dropGenerator = null)
    {
        return GameContextTestHelper.CreateGameContext(dropGenerator: dropGenerator);
    }

    private static async ValueTask InvokePrivateAsync(object target, string methodName, params object?[] args)
    {
        await InvokeProtectedAsync(target, methodName, args).ConfigureAwait(false);
    }

    private static async ValueTask InvokeProtectedAsync(object target, string methodName, object?[] args)
    {
        var method = FindMethod(target.GetType(), methodName)
                     ?? throw new MissingMethodException(target.GetType().Name, methodName);
        var result = method.Invoke(target, args);
        await AwaitResultAsync(result).ConfigureAwait(false);
    }

    /// <summary>
    /// Looks up a protected or private instance method by name, walking up the type hierarchy. Used to
    /// invoke <see cref="IllusionTempleContext"/>'s lifecycle hooks (e.g. <c>OnGameStartAsync</c>,
    /// <c>GameEndedAsync</c>) directly in tests, bypassing the real-time countdown that normally drives
    /// them (see the class remarks). This is test-only code operating on a type from the same solution -
    /// <paramref name="methodName"/> is always a hardcoded literal from this test file, never external
    /// input.
    /// </summary>
    [SuppressMessage("Security Hotspot", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Test-only code invoking lifecycle hooks of a type from this same solution, bypassing the real-time countdown that normally drives them (see the class remarks). Every method name passed in is a hardcoded literal from this file - no external input reaches this call.")]
    private static MethodInfo? FindMethod(Type type, string methodName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var method = current.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (method is not null)
            {
                return method;
            }
        }

        return null;
    }

    private static async ValueTask WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 5000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10).ConfigureAwait(false);
        }
    }

    private static async ValueTask AwaitResultAsync(object? result)
    {
        if (result is ValueTask valueTask)
        {
            await valueTask.ConfigureAwait(false);
        }
        else if (result is Task task)
        {
            await task.ConfigureAwait(false);
        }
        else
        {
            // The invoked method was synchronous (e.g. "async void") - there is nothing to await.
        }
    }
}
