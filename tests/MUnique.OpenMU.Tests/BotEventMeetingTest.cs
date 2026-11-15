// <copyright file="BotEventMeetingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests where a bot waits for players after it announced an opened event
/// (<see cref="BotEventMeetingPlanner"/>).
/// </summary>
[TestFixture]
public class BotEventMeetingTest
{
    private static readonly Point Npc = new(100, 100);

    /// <summary>
    /// The NPC's own tile is usually blocked, and a route to a blocked tile does not exist - the bot
    /// stands on the nearest walkable tile next to it.
    /// </summary>
    [Test]
    public void StandsOnTheNearestWalkableTileNextToTheNpc()
    {
        var terrain = CreateBlockedTerrain();
        terrain.WalkMap[Npc.X, Npc.Y] = true;
        terrain.WalkMap[Npc.X + 2, Npc.Y] = true;
        terrain.WalkMap[Npc.X + 1, Npc.Y + 1] = true;

        var point = BotEventMeetingPlanner.FindStandingPoint(terrain, Npc, 3);

        Assert.That(point, Is.EqualTo(new Point((byte)(Npc.X + 1), (byte)(Npc.Y + 1))));
    }

    /// <summary>
    /// The meeting happens in town: a walkable tile in the safezone wins over a nearer one outside.
    /// </summary>
    [Test]
    public void PrefersTheSafezone()
    {
        var terrain = CreateBlockedTerrain();
        terrain.WalkMap[Npc.X + 1, Npc.Y] = true;
        terrain.WalkMap[Npc.X + 3, Npc.Y] = true;
        terrain.SafezoneMap[Npc.X + 3, Npc.Y] = true;

        var point = BotEventMeetingPlanner.FindStandingPoint(terrain, Npc, 3);

        Assert.That(point, Is.EqualTo(new Point((byte)(Npc.X + 3), Npc.Y)));
    }

    /// <summary>
    /// Without any walkable tile in range there is no standing point - the bot gives up the meeting.
    /// </summary>
    [Test]
    public void NoStandingPointWithoutWalkableTiles()
    {
        var terrain = CreateBlockedTerrain();

        Assert.That(BotEventMeetingPlanner.FindStandingPoint(terrain, Npc, 3), Is.Null);
    }

    /// <summary>
    /// Chaos Castle has no entry NPC: the bot waits in the safezone of its home town.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ChaosCastleMeetsInTheHomeTownAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var homeTown = new GameMapDefinition { Number = 3 };
        Mock.Get(bot.SelectedCharacter!.CharacterClass!).Setup(c => c.HomeMap).Returns(homeTown);
        var untilUtc = new DateTime(2026, 11, 4, 20, 5, 0, DateTimeKind.Utc);

        var meeting = BotEventMeetingPlanner.TryPlan(bot, MiniGameType.ChaosCastle, untilUtc);

        Assert.That(meeting, Is.EqualTo(new BotEventMeeting(homeTown, null, untilUtc)));
    }

    /// <summary>
    /// Creates a terrain on which no tile is walkable and none is in the safezone - without data a
    /// terrain is the walkable default map, which would hide what the test sets up.
    /// </summary>
    private static GameMapTerrain CreateBlockedTerrain()
    {
        var terrain = new GameMapTerrain((byte[]?)null);
        Array.Clear(terrain.WalkMap);
        Array.Clear(terrain.SafezoneMap);
        return terrain;
    }
}
