// <copyright file="BotEventMeetingPlanner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Plans where a bot waits for players after it announced an opened event (see
/// <see cref="BotEventMeeting"/>). Pure decision logic - exposed for unit tests.
/// </summary>
internal static class BotEventMeetingPlanner
{
    /// <summary>
    /// Plans the meeting point of the bot for the event type.
    /// </summary>
    /// <param name="bot">The bot which announces the event.</param>
    /// <param name="type">The type of the opened event.</param>
    /// <param name="untilUtc">When the event's entrance closes.</param>
    /// <returns>The meeting, or <c>null</c> when the bot has nowhere to wait.</returns>
    /// <remarks>
    /// Blood Castle and Devil Square are entered through an NPC (Messenger of Archangel, Charon), so
    /// the bot waits next to it - on a map it may warp to, preferring the one it is on. Chaos Castle
    /// is entered with the ticket alone and has no such place; the bot waits in the safezone of its
    /// class's home town instead, and the post names the town.
    /// </remarks>
    internal static BotEventMeeting? TryPlan(Player bot, MiniGameType type, DateTime untilUtc)
    {
        var entryNpcWindow = type switch
        {
            MiniGameType.BloodCastle => NpcWindow.BloodCastle,
            MiniGameType.DevilSquare => NpcWindow.DevilSquare,
            _ => (NpcWindow?)null,
        };

        if (entryNpcWindow is not { } window)
        {
            return type == MiniGameType.ChaosCastle
                   && bot.SelectedCharacter?.CharacterClass?.HomeMap is { } homeMap
                ? new BotEventMeeting(homeMap, null, untilUtc)
                : null;
        }

        var currentMapNumber = bot.CurrentMap?.Definition.Number;
        var entryNpc = bot.GameContext.Configuration.Maps
            .SelectMany(map => map.MonsterSpawns
                .Where(spawn => spawn.MonsterDefinition?.NpcWindow == window)
                .Select(spawn => (Map: map, Position: new Point(spawn.X1, spawn.Y1))))
            .Where(npc => npc.Map.Number == currentMapNumber || BotChatPlugIn.MayWarpTo(bot, npc.Map))
            .OrderByDescending(npc => npc.Map.Number == currentMapNumber)
            .FirstOrDefault();

        return entryNpc.Map is null ? null : new BotEventMeeting(entryNpc.Map, entryNpc.Position, untilUtc);
    }

    /// <summary>
    /// Finds the tile the bot stands on next to an NPC: the nearest walkable tile within the range,
    /// preferring the safezone. The NPC's own tile is usually blocked, and a route to a blocked tile
    /// does not exist.
    /// </summary>
    /// <param name="terrain">The terrain of the map.</param>
    /// <param name="npcPosition">The position of the NPC.</param>
    /// <param name="range">The maximum distance from the NPC.</param>
    /// <returns>The tile, or <c>null</c> when none is walkable.</returns>
    internal static Point? FindStandingPoint(GameMapTerrain terrain, Point npcPosition, int range)
    {
        Point? best = null;
        var bestScore = double.MaxValue;
        for (var x = npcPosition.X - range; x <= npcPosition.X + range; x++)
        {
            for (var y = npcPosition.Y - range; y <= npcPosition.Y + range; y++)
            {
                if (x is < 0 or > byte.MaxValue
                    || y is < 0 or > byte.MaxValue
                    || (x == npcPosition.X && y == npcPosition.Y)
                    || !terrain.WalkMap[x, y])
                {
                    continue;
                }

                var point = new Point((byte)x, (byte)y);

                // A tile outside the safezone only wins when the safezone offers none in range.
                var score = point.EuclideanDistanceTo(npcPosition) + (terrain.SafezoneMap[x, y] ? 0 : range * 2);
                if (score < bestScore)
                {
                    best = point;
                    bestScore = score;
                }
            }
        }

        return best;
    }
}
