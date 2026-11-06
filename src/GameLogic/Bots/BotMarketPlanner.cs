// <copyright file="BotMarketPlanner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The decisions of a market shift (see <see cref="BotMarketShift"/>) which don't need a running game:
/// which bot may go to the market, where it puts its stall and when it leaves.
/// </summary>
internal static class BotMarketPlanner
{
    /// <summary>
    /// The lowest level at which the game allows opening a personal store (see the set price action).
    /// </summary>
    internal const int MinimumStoreLevel = 6;

    /// <summary>
    /// How far from the arrival point of the market a stall may be: the stalls stay together, where the
    /// players arrive, instead of spreading over the whole map.
    /// </summary>
    internal const int StallSearchRange = 18;

    /// <summary>
    /// The least distance between two stalls, so that the players can click each store.
    /// </summary>
    internal const double StallSpacing = 3;

    /// <summary>
    /// Why a market shift ends.
    /// </summary>
    internal enum EndReason
    {
        /// <summary>The shift goes on.</summary>
        None,

        /// <summary>The shift is over.</summary>
        TimeIsUp,

        /// <summary>The bot has fewer goods left than it needs to stay.</summary>
        SoldOut,

        /// <summary>The bot didn't sell anything for a long time.</summary>
        NoSales,

        /// <summary>The store was closed by the game, e.g. after its last item was sold.</summary>
        StoreClosed,

        /// <summary>A player took the bot into a party.</summary>
        JoinedParty,
    }

    /// <summary>
    /// Counts the goods in the personal store of the bot which have a price - only those can be sold.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <returns>The number of priced goods.</returns>
    internal static int CountPricedGoods(Player bot)
        => bot.ShopStorage?.Items.Count(item => item.GetStorePrice() is not null) ?? 0;

    /// <summary>
    /// Determines whether the bot may start a market shift now: it has enough priced goods, is free of
    /// errands and company, and its last shift is long enough ago.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="now">The current time.</param>
    /// <returns><c>true</c>, if the bot may go to the market.</returns>
    internal static bool MayStartShift(OfflinePlayer bot, BotMerchantConfiguration configuration, DateTime now)
    {
        return bot.MarketShift is null
               && now >= bot.NextMarketShiftUtc
               && bot.PlayerState.CurrentState == PlayerState.EnteredWorld
               && bot.IsAlive
               && bot.Level >= Math.Max(configuration.MinimumLevel, MinimumStoreLevel)
               && bot.Party is null
               && bot.CurrentMiniGame is null
               && bot.EventMeeting is null
               && !bot.IsOnShoppingTrip
               && !bot.IsOnBuffTrip
               && !bot.IsOnChaosMachineTrip
               && !bot.HasRevengeIntent
               && !(bot.ShopStorage?.StoreOpen ?? false)
               && CountPricedGoods(bot) >= configuration.MinimumGoodsToStart;
    }

    /// <summary>
    /// Draws the length of a shift between the configured minimum and maximum.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="nextInt">Returns a random number between the inclusive minimum and the exclusive maximum.</param>
    /// <returns>The length of the shift.</returns>
    internal static TimeSpan DrawShiftDuration(BotMerchantConfiguration configuration, Func<int, int, int> nextInt)
    {
        var minimum = (int)Math.Max(1, configuration.MinimumShiftDuration.TotalSeconds);
        var maximum = (int)Math.Max(minimum, configuration.MaximumShiftDuration.TotalSeconds);
        return TimeSpan.FromSeconds(nextInt(minimum, maximum + 1));
    }

    /// <summary>
    /// Finds a free stall: a walkable safezone tile near the arrival point which keeps its distance from
    /// the stalls of the other merchants.
    /// </summary>
    /// <param name="terrain">The terrain of the market map.</param>
    /// <param name="arrival">The center of the arrival area of the market.</param>
    /// <param name="takenStalls">The stalls of the other merchants.</param>
    /// <param name="nextInt">Returns a random number between the inclusive minimum and the exclusive maximum.</param>
    /// <returns>The stall; <c>null</c>, if the market is full.</returns>
    internal static Point? FindStall(GameMapTerrain terrain, Point arrival, IReadOnlyCollection<Point> takenStalls, Func<int, int, int> nextInt)
    {
        var candidates = new List<Point>();
        for (var x = arrival.X - StallSearchRange; x <= arrival.X + StallSearchRange; x++)
        {
            for (var y = arrival.Y - StallSearchRange; y <= arrival.Y + StallSearchRange; y++)
            {
                if (x is < 0 or > byte.MaxValue
                    || y is < 0 or > byte.MaxValue
                    || !terrain.WalkMap[x, y]
                    || !terrain.SafezoneMap[x, y])
                {
                    continue;
                }

                var point = new Point((byte)x, (byte)y);
                if (point.EuclideanDistanceTo(arrival) <= StallSearchRange
                    && takenStalls.All(stall => stall.EuclideanDistanceTo(point) >= StallSpacing))
                {
                    candidates.Add(point);
                }
            }
        }

        return candidates.Count == 0 ? null : candidates[nextInt(0, candidates.Count)];
    }

    /// <summary>
    /// Draws a store name which no other open store has, so that two stores next to each other can be
    /// told apart.
    /// </summary>
    /// <param name="storeNames">The configured store names.</param>
    /// <param name="takenNames">The names of the other open stores.</param>
    /// <param name="botName">The name of the bot, used when all store names are taken.</param>
    /// <param name="nextInt">Returns a random number between the inclusive minimum and the exclusive maximum.</param>
    /// <returns>The store name.</returns>
    internal static string DrawStoreName(IReadOnlyList<string> storeNames, IReadOnlyCollection<string> takenNames, string botName, Func<int, int, int> nextInt)
    {
        var free = storeNames.Where(n => !takenNames.Contains(n, StringComparer.OrdinalIgnoreCase)).ToList();
        return free.Count == 0 ? botName : free[nextInt(0, free.Count)];
    }

    /// <summary>
    /// Determines whether and why the shift of the bot ends now.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="shift">The shift.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The reason; <see cref="EndReason.None"/>, if the shift goes on.</returns>
    internal static EndReason GetEndReason(Player bot, BotMarketShift shift, BotMerchantConfiguration configuration, DateTime now)
    {
        if (now >= shift.UntilUtc)
        {
            return EndReason.TimeIsUp;
        }

        if (bot.Party is not null)
        {
            return EndReason.JoinedParty;
        }

        if (!shift.IsStoreOpened)
        {
            // Still on the way to the stall: only the time and a party end it.
            return EndReason.None;
        }

        if (!(bot.ShopStorage?.StoreOpen ?? false))
        {
            return EndReason.StoreClosed;
        }

        if (CountPricedGoods(bot) < configuration.MinimumGoodsToStay)
        {
            return EndReason.SoldOut;
        }

        if (configuration.LeaveWithoutSalesAfter > TimeSpan.Zero
            && now - shift.LastSaleUtc >= configuration.LeaveWithoutSalesAfter)
        {
            return EndReason.NoSales;
        }

        return EndReason.None;
    }
}
