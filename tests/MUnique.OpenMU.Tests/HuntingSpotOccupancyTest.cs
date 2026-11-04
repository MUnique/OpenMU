// <copyright file="HuntingSpotOccupancyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests whether bots recognize the hunting spots and monsters of other, non-party bots as taken.
/// </summary>
[TestFixture]
public class HuntingSpotOccupancyTest
{
    /// <summary>
    /// A spot within the configured range counts as taken, one beyond it does not.
    /// </summary>
    [Test]
    public void SpotIsOccupiedWithinRange()
    {
        var spots = new[] { new HuntingSpot(new Point(100, 100), 0) };

        Assert.That(HuntingSpotOccupancy.IsSpotOccupied(new Point(100, 112), spots), Is.True);
        Assert.That(HuntingSpotOccupancy.IsSpotOccupied(new Point(100, 113), spots), Is.False);
    }

    /// <summary>
    /// A monster within a bot's attack range counts as claimed, one beyond it does not.
    /// </summary>
    [Test]
    public void MonsterIsClaimedWithinAttackRange()
    {
        var spots = new[] { new HuntingSpot(new Point(100, 100), 0) };

        Assert.That(HuntingSpotOccupancy.IsMonsterClaimed(new Point(100, 106), spots, 6), Is.True);
        Assert.That(HuntingSpotOccupancy.IsMonsterClaimed(new Point(100, 107), spots, 6), Is.False);
    }

    /// <summary>
    /// Bots of the same party are supposed to hunt together, so they never compete for a spot.
    /// </summary>
    [Test]
    public async ValueTask SamePartyBotsDoNotCompeteAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var first = await CreateBotAsync(gameContext, "First").ConfigureAwait(false);
        var second = await CreateBotAsync(gameContext, "Second").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(first).ConfigureAwait(false);
        await party.AddAsync(second).ConfigureAwait(false);

        Assert.That(HuntingSpotOccupancy.IsCompetingBot(first, second), Is.False);
    }

    /// <summary>
    /// Bots of different parties (or without a party) compete for hunting grounds.
    /// </summary>
    [Test]
    public async ValueTask OtherPartyBotsCompeteAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var first = await CreateBotAsync(gameContext, "First").ConfigureAwait(false);
        var second = await CreateBotAsync(gameContext, "Second").ConfigureAwait(false);

        Assert.That(HuntingSpotOccupancy.IsCompetingBot(first, second), Is.True);
    }

    /// <summary>
    /// A regular player is not a bot and therefore no competitor for a bot's hunting ground.
    /// </summary>
    [Test]
    public async ValueTask HumanPlayersDoNotCompeteAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var human = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await human.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        human.IsAlive = true;

        Assert.That(HuntingSpotOccupancy.IsCompetingBot(bot, human), Is.False);
    }

    private static async ValueTask<OfflinePlayer> CreateBotAsync(IGameContext gameContext, string name)
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.SelectedCharacter!.Name = name;
        bot.IsAlive = true;
        bot.Account!.IsBot = true;
        return bot;
    }
}
