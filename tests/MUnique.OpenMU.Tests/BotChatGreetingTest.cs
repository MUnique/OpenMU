// <copyright file="BotChatGreetingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests which bot greets a player entering the game (<see cref="BotChatPlugIn.FindGreeter"/>): the
/// greeting must come from a character the player sees on the screen.
/// </summary>
[TestFixture]
public class BotChatGreetingTest
{
    private static readonly Point PlayerPosition = new(100, 100);

    /// <summary>
    /// The nearest bot on the player's map greets; a closer bot on another map doesn't count.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask NearestBotOnTheMapGreetsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8);
        var otherMap = new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8);
        var player = await CreateHumanAsync(gameContext, map).ConfigureAwait(false);
        var near = await CreateBotAsync(gameContext, map, new Point(103, 100)).ConfigureAwait(false);
        var farther = await CreateBotAsync(gameContext, map, new Point(106, 100)).ConfigureAwait(false);
        var elsewhere = await CreateBotAsync(gameContext, otherMap, new Point(101, 100)).ConfigureAwait(false);

        var greeter = BotChatPlugIn.FindGreeter([player, farther, elsewhere, near], player, map);

        Assert.That(greeter, Is.SameAs(near));
    }

    /// <summary>
    /// Without a bot in sight, nobody greets - a greeting from off the screen would give the bots away.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask NobodyGreetsFromOutOfSightAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8);
        var player = await CreateHumanAsync(gameContext, map).ConfigureAwait(false);
        var far = await CreateBotAsync(gameContext, map, new Point(130, 100)).ConfigureAwait(false);

        Assert.That(BotChatPlugIn.FindGreeter([player, far], player, map), Is.Null);
    }

    private static async ValueTask<Player> CreateHumanAsync(IGameContext gameContext, GameMap map)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.SetCurrentMapSilently(map);
        player.Position = PlayerPosition;
        return player;
    }

    private static async ValueTask<OfflinePlayer> CreateBotAsync(IGameContext gameContext, GameMap map, Point position)
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.IsAlive = true;
        bot.Account!.IsBot = true;
        bot.SetCurrentMapSilently(map);
        bot.Position = position;
        return bot;
    }

    /// <summary>
    /// The bots keep quiet near players who are busy talking: from the configured number of their
    /// messages within the window and the range - not for older or distant messages, nor on another map.
    /// </summary>
    [Test]
    public void NoticesPlayersBusyTalking()
    {
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8);
        var otherMap = new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8);
        var throttle = new BotChatThrottle();
        var start = new DateTime(2026, 11, 4, 20, 0, 0, DateTimeKind.Utc);
        var window = TimeSpan.FromSeconds(30);
        var here = new Point(100, 100);

        throttle.RecordHumanChat(map, here, start, window);
        throttle.RecordHumanChat(map, new Point(103, 100), start.AddSeconds(5), window);
        throttle.RecordHumanChat(otherMap, here, start.AddSeconds(6), window);
        throttle.RecordHumanChat(map, new Point(150, 150), start.AddSeconds(7), window);

        Assert.That(throttle.IsHumanChatBusy(map, here, 12, start.AddSeconds(10), window, 2), Is.True);
        Assert.That(throttle.IsHumanChatBusy(map, here, 12, start.AddSeconds(10), window, 3), Is.False);
        Assert.That(throttle.IsHumanChatBusy(map, here, 12, start.AddSeconds(32), window, 2), Is.False);
        Assert.That(throttle.IsHumanChatBusy(map, here, 12, start.AddSeconds(10), window, 0), Is.False);
    }
}
