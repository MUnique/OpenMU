// <copyright file="HuntingSpotOccupancy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Decides whether a hunting spot or monster is already taken by another bot. Bots of the same party
/// hunt together by design, so they are never competitors of each other.
/// </summary>
internal static class HuntingSpotOccupancy
{
    /// <summary>
    /// Distance (tiles) within which a spot counts as the same hunting ground. Independent bots keep
    /// at least this much space between them, so they do not farm each other's monsters.
    /// </summary>
    internal const int SpotRange = 2 * BotMuHelperSettings.DefaultHuntingRange;

    /// <summary>
    /// Determines whether another bot outside the player's party is already hunting at, or heading to,
    /// the spot.
    /// </summary>
    /// <param name="spot">The spot to check.</param>
    /// <param name="competitorSpots">The spots claimed by competing bots.</param>
    /// <returns><c>true</c> if a competing bot occupies the spot; otherwise, <c>false</c>.</returns>
    internal static bool IsSpotOccupied(Point spot, IReadOnlyList<HuntingSpot> competitorSpots)
        => IsNear(spot, competitorSpots, SpotRange);

    /// <summary>
    /// Determines whether another bot outside the player's party is close enough to the monster to
    /// engage it.
    /// </summary>
    /// <param name="monsterPosition">The position of the monster.</param>
    /// <param name="competitorSpots">The spots claimed by competing bots.</param>
    /// <param name="attackRange">The range in which a bot can attack the monster.</param>
    /// <returns><c>true</c> if a competing bot can already attack the monster; otherwise, <c>false</c>.</returns>
    internal static bool IsMonsterClaimed(Point monsterPosition, IReadOnlyList<HuntingSpot> competitorSpots, int attackRange)
        => IsNear(monsterPosition, competitorSpots, attackRange);

    /// <summary>
    /// Gets the spots occupied by the bots on the map which are not in the player's party.
    /// </summary>
    /// <param name="self">The bot which asks.</param>
    /// <param name="candidatePlayers">The players to inspect.</param>
    /// <param name="mapNumber">The number of the map.</param>
    /// <returns>The spots occupied by competing bots.</returns>
    internal static IReadOnlyList<HuntingSpot> GetCompetitorSpots(Player self, IEnumerable<Player> candidatePlayers, short mapNumber)
    {
        var spots = new List<HuntingSpot>();
        foreach (var other in candidatePlayers)
        {
            if (IsCompetingBot(self, other))
            {
                AddOccupiedSpots(other, mapNumber, spots);
            }
        }

        return spots;
    }

    /// <summary>
    /// Determines whether the given player is a bot which competes with the asking player for hunting
    /// grounds. Party members are explicitly excluded: they are supposed to hunt together.
    /// </summary>
    /// <param name="self">The player which asks.</param>
    /// <param name="other">The other player.</param>
    /// <returns><c>true</c> if the other player is a competing bot; otherwise, <c>false</c>.</returns>
    internal static bool IsCompetingBot(Player self, Player other)
    {
        return !ReferenceEquals(self, other)
               && other.Account?.IsBot == true
               && other.IsActive()
               && !IsSameParty(self, other);
    }

    private static bool IsSameParty(Player self, Player other)
        => self.Party is { } party && ReferenceEquals(party, other.Party);

    private static void AddOccupiedSpots(Player bot, short mapNumber, List<HuntingSpot> spots)
    {
        if (bot is OfflinePlayer { HuntingSpot: { } claimed } && claimed.MapNumber == mapNumber)
        {
            spots.Add(claimed);
        }

        // The current position counts as well: a bot which chases a monster away from its claimed
        // ground is physically hunting there, and other bots should not engage the same monsters.
        if (bot.CurrentMap?.Definition.Number == mapNumber)
        {
            spots.Add(new HuntingSpot(bot.Position, mapNumber));
        }
    }

    private static bool IsNear(Point point, IReadOnlyList<HuntingSpot> spots, int range)
    {
        var rangeSquared = range * range;
        foreach (var spot in spots)
        {
            if (spot.Position.EuclideanDistanceSquaredTo(point) <= rangeSquared)
            {
                return true;
            }
        }

        return false;
    }
}
