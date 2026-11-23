// <copyright file="MiniGameZenDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the configuration of the zen of the mini games: the maps and monsters which drop no zen,
/// and the reward tiers of the Devil Square.
/// </summary>
[TestFixture]
internal class MiniGameZenDataTest
{
    private static readonly short[] Season6MapsWithoutZen = [9, 32, 45, 46, 47, 48, 49, 50];

    private static readonly short[] Season6MonstersWithoutZen = [361, 362, 363, 364, 504, 505, 506, 507, 508, 509, 510, 511, 523, 524, 525, 526, 527, 528, 529, 530, 531];

    /// <summary>
    /// Tests that a new season 6 database has no money drop on the maps of the Devil Square and the Illusion Temple,
    /// but on the other maps, e.g. the Blood Castle.
    /// </summary>
    [Test]
    public async Task NewSeason6DatabaseHasNoMoneyDropOnMapsWithoutZenAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        AssertMapsWithoutZen(gameConfiguration, Season6MapsWithoutZen);
        Assert.That(gameConfiguration.Maps.Where(map => map.Number == 11).SelectMany(map => map.DropItemGroups), Has.Some.Matches<DropItemGroup>(group => group.ItemType == SpecialItemType.Money));
    }

    /// <summary>
    /// Tests that a new version 0.95d database has no money drop on the maps of the Devil Square.
    /// </summary>
    [Test]
    public async Task NewVersion095dDatabaseHasNoMoneyDropOnDevilSquareAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new Version095d.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        AssertMapsWithoutZen(gameConfiguration, [9]);
        Assert.That(gameConfiguration.Maps.Where(map => map.Number == 0).SelectMany(map => map.DropItemGroups), Has.Some.Matches<DropItemGroup>(group => group.ItemType == SpecialItemType.Money));
    }

    /// <summary>
    /// Tests that exactly the monsters which are only rewarded by their event drop no zen in a new season 6 database.
    /// </summary>
    [Test]
    public async Task NewSeason6DatabaseHasMonstersWithoutZenAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        AssertMonstersWithoutZen(gameConfiguration);
    }

    /// <summary>
    /// Tests that the mandatory update removes the money drop of the maps without zen and marks the monsters
    /// without zen of an existing database, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task ZenDropUpdateRepairsExistingSeason6DatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;
        var moneyGroup = gameConfiguration.Maps.Single(map => map.Number == 11).DropItemGroups.First(group => group.ItemType == SpecialItemType.Money);
        foreach (var map in gameConfiguration.Maps.Where(map => Season6MapsWithoutZen.Contains(map.Number)))
        {
            map.DropItemGroups.Add(moneyGroup);
        }

        foreach (var monster in gameConfiguration.Monsters)
        {
            monster.DropsNoZen = false;
        }

        var update = new FixMiniGameZenDropsUpdatePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertMapsWithoutZen(gameConfiguration, Season6MapsWithoutZen);
        AssertMonstersWithoutZen(gameConfiguration);
        Assert.That(gameConfiguration.Maps.Single(map => map.Number == 11).DropItemGroups, Has.Member(moneyGroup));
    }

    /// <summary>
    /// Tests that every ranked player of a devil square of a new database gets exactly one experience and one money reward,
    /// of the tier which is defined by the rank and the number of ranked players.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasDevilSquareRewardTiersAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        var devilSquares = gameConfiguration.MiniGameDefinitions.Where(game => game.Type == MiniGameType.DevilSquare).ToList();
        Assert.That(devilSquares, Has.Count.EqualTo(7));
        foreach (var devilSquare in devilSquares)
        {
            Assert.That(devilSquare.Rewards, Has.Count.EqualTo(16));
            AssertRewardTiers(devilSquare, GetRankRewardAmounts(devilSquare));
        }
    }

    /// <summary>
    /// Tests that the optional update configures the rewards of an existing devil square by tiers,
    /// keeping the customized amounts of the ranks 1 to 4, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task RewardTiersUpdateKeepsCustomizedAmountsAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new Version095d.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;
        var devilSquare = gameConfiguration.MiniGameDefinitions.First(game => game.Type == MiniGameType.DevilSquare);
        RevertToRewardsByRank(devilSquare);
        var customized = devilSquare.Rewards.Single(reward => reward is { Rank: 4, RewardType: MiniGameRewardType.Money });
        customized.RewardAmount = 12345;
        var amounts = devilSquare.Rewards.ToDictionary(reward => (reward.Rank!.Value, reward.RewardType), reward => reward.RewardAmount);

        var update = new DevilSquareRewardTiersUpdatePlugIn095d();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(devilSquare.Rewards, Has.Count.EqualTo(16));
        AssertRewardTiers(devilSquare, amounts);
        Assert.That(amounts[(4, MiniGameRewardType.Money)], Is.EqualTo(12345));
    }

    /// <summary>
    /// The tier of a rank in the original game: up to six players by rank, with more players by rank percentage.
    /// </summary>
    private static int GetExpectedTier(int rank, int rankedPlayers)
    {
        if (rankedPlayers <= 6)
        {
            return Math.Min(rank, 4);
        }

        if (rank == 1)
        {
            return 1;
        }

        var percentage = rank * 100 / rankedPlayers;
        return percentage <= 30 ? 2 : percentage <= 50 ? 3 : 4;
    }

    private static void AssertRewardTiers(MiniGameDefinition devilSquare, IReadOnlyDictionary<(int Tier, MiniGameRewardType Type), int> amounts)
    {
        for (var rankedPlayers = 1; rankedPlayers <= devilSquare.MaximumPlayerCount; rankedPlayers++)
        {
            for (var rank = 1; rank <= rankedPlayers; rank++)
            {
                var tier = GetExpectedTier(rank, rankedPlayers);
                foreach (var type in new[] { MiniGameRewardType.Experience, MiniGameRewardType.Money })
                {
                    var applying = devilSquare.Rewards.Where(reward => reward.RewardType == type && reward.IsApplicableToRank(rank, rankedPlayers)).ToList();
                    Assert.That(applying, Has.Count.EqualTo(1), $"{devilSquare.Name}, {type}, rank {rank} of {rankedPlayers}");
                    Assert.That(applying[0].RewardAmount, Is.EqualTo(amounts[(tier, type)]), $"{devilSquare.Name}, {type}, rank {rank} of {rankedPlayers}");
                }
            }
        }
    }

    private static Dictionary<(int Tier, MiniGameRewardType Type), int> GetRankRewardAmounts(MiniGameDefinition devilSquare)
    {
        return devilSquare.Rewards
            .Where(reward => reward.MaximumRankedPlayers is not null)
            .ToDictionary(reward => (reward.Rank!.Value, reward.RewardType), reward => reward.RewardAmount);
    }

    /// <summary>
    /// Brings the rewards into the state before the tiers: one reward per rank 1 to 4 and type.
    /// </summary>
    private static void RevertToRewardsByRank(MiniGameDefinition devilSquare)
    {
        foreach (var reward in devilSquare.Rewards.Where(reward => reward.MinimumRankedPlayers is not null).ToList())
        {
            devilSquare.Rewards.Remove(reward);
        }

        foreach (var reward in devilSquare.Rewards)
        {
            reward.MaximumRank = null;
            reward.MaximumRankedPlayers = null;
        }

        Assert.That(devilSquare.Rewards, Has.Count.EqualTo(8));
    }

    private static void AssertMapsWithoutZen(GameConfiguration gameConfiguration, IReadOnlyCollection<short> mapNumbers)
    {
        var maps = gameConfiguration.Maps.Where(map => mapNumbers.Contains(map.Number)).ToList();
        Assert.That(maps.Select(map => map.Number).Distinct(), Is.EquivalentTo(mapNumbers));
        Assert.That(maps.SelectMany(map => map.DropItemGroups), Has.None.Matches<DropItemGroup>(group => group.ItemType == SpecialItemType.Money));
        Assert.That(maps.SelectMany(map => map.DropItemGroups), Is.Not.Empty, "The other default drop groups are kept.");
    }

    private static void AssertMonstersWithoutZen(GameConfiguration gameConfiguration)
    {
        var monstersWithoutZen = gameConfiguration.Monsters.Where(monster => monster.DropsNoZen).Select(monster => monster.Number);
        Assert.That(monstersWithoutZen, Is.EquivalentTo(Season6MonstersWithoutZen));
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(Func<IPersistenceContextProvider, DataInitializationBase> createDataInitialization)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await createDataInitialization(contextProvider).CreateInitialDataAsync(1, true).ConfigureAwait(false);
        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
