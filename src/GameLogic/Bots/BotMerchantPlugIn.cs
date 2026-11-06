// <copyright file="BotMerchantPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Lets the server-side bots sell their goods in personal stores on the market map (Loren Market): a bot
/// with enough priced goods in its store goes there for a shift, opens the store and goes back hunting
/// afterward (see <see cref="BotMarketShift"/>).
/// </summary>
/// <remarks>
/// Every bot may become a merchant now and then - there are no dedicated merchant bots. A shift ends
/// when its time is up, when the bot sold everything, after a long time without a sale and when
/// the bot logs out (the presence rotation of the bots goes on as usual). The limits of the
/// <see cref="BotMerchantConfiguration"/> apply per game server.
/// It's a feature plugin of the bots, listed in the admin panel next to them.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.BotMerchantPlugIn_Name), Description = nameof(PlugInResources.BotMerchantPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6E0B6C2F-8F7E-4C71-9C55-2B7C1E4A9D13")]
public class BotMerchantPlugIn
    : IFeaturePlugIn,
    IPeriodicTaskPlugIn,
    IItemSoldToOtherPlayerPlugIn,
    ISupportCustomConfiguration<BotMerchantConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    /// <summary>
    /// How often a game server sends new bots to the market. The shifts last half an hour and more, so
    /// there's no need to check every second like the timer of the periodic tasks does.
    /// </summary>
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    private readonly ConcurrentDictionary<IGameContext, DateTime> _nextCheckUtc = new();

    /// <summary>
    /// The number of market shifts each good spent in a store without a buyer, by the id of the item.
    /// In memory only: a restart of the server gives every good a new chance.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, int> _unsoldShifts = new();

    /// <inheritdoc />
    public BotMerchantConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new BotMerchantConfiguration();

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        if (this.Configuration is not { } configuration
            || now < this._nextCheckUtc.GetValueOrDefault(gameContext))
        {
            return;
        }

        this._nextCheckUtc[gameContext] = now + CheckInterval;
        await this.StartShiftsAsync(gameContext, configuration, now).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ForceStart() => this._nextCheckUtc.Clear();

    /// <inheritdoc />
    public void ItemSold(Player seller, Item item, Player buyer)
    {
        if (seller is OfflinePlayer { MarketShift: { } shift } bot)
        {
            shift.LastSaleUtc = DateTime.UtcNow;
            this._unsoldShifts.TryRemove(item.GetId(), out _);

            // The bot buys nothing, so its income is removed - in its own tick, which is the only
            // place allowed to change its inventory (see OfflinePlayer.PendingBotActions).
            bot.PendingBotActions.Enqueue(() => BotMerchandise.RemoveIncomeAsync(bot, shift));
        }
    }

    /// <summary>
    /// Counts one more market shift without a buyer for the good.
    /// </summary>
    /// <param name="item">The good.</param>
    /// <returns>The number of shifts the good spent in the store without a buyer.</returns>
    internal int CountUnsoldShift(Item item) => this._unsoldShifts.AddOrUpdate(item.GetId(), 1, (_, count) => count + 1);

    /// <summary>
    /// Determines whether the good spent too many shifts in a store without a buyer: it's sold to an NPC
    /// and doesn't go back into the store.
    /// </summary>
    /// <param name="item">The good.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns><c>true</c>, if the good is retired from the market.</returns>
    internal bool IsRetired(Item item, BotMerchantConfiguration configuration)
        => this._unsoldShifts.TryGetValue(item.GetId(), out var count) && count >= configuration.UnsoldShiftsUntilNpc;

    /// <summary>
    /// Sends bots with goods to the market until it has the configured number of merchants.
    /// </summary>
    private async ValueTask StartShiftsAsync(GameContext gameContext, BotMerchantConfiguration configuration, DateTime now)
    {
        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        var bots = players.OfType<OfflinePlayer>().Where(p => p.Account?.IsBot == true).ToList();
        var shifts = bots.Select(b => b.MarketShift).OfType<BotMarketShift>().ToList();
        var freePlaces = configuration.MaximumMerchants - shifts.Count;
        if (freePlaces <= 0)
        {
            return;
        }

        var candidates = bots.Where(b => BotMarketPlanner.MayStartShift(b, configuration, now)).ToList();
        if (candidates.Count == 0
            || gameContext.Configuration.Maps.FirstOrDefault(m => m.Number == configuration.MarketMapNumber) is not { } marketDefinition
            || marketDefinition.ExitGates.FirstOrDefault(g => g.IsSpawnGate) is not { } arrivalGate
            || await gameContext.GetMapAsync((ushort)marketDefinition.Number).ConfigureAwait(false) is not { } market)
        {
            return;
        }

        var arrival = new Point((byte)((arrivalGate.X1 + arrivalGate.X2) / 2), (byte)((arrivalGate.Y1 + arrivalGate.Y2) / 2));
        var stalls = shifts.Select(s => s.Stall).ToList();
        var storeNames = configuration.GetStoreNames();
        var takenNames = shifts.Select(s => s.StoreName).ToList();
        foreach (var bot in candidates.OrderBy(_ => Rand.NextInt(0, int.MaxValue)).Take(freePlaces))
        {
            if (BotMarketPlanner.FindStall(market.Terrain, arrival, stalls, Rand.NextInt) is not { } stall)
            {
                gameContext.LoggerFactory.CreateLogger<BotMerchantPlugIn>().LogDebug("The market {Map} has no free stall for more bots.", marketDefinition.Name);
                return;
            }

            stalls.Add(stall);
            var storeName = BotMarketPlanner.DrawStoreName(storeNames, takenNames, bot.Name, Rand.NextInt);
            takenNames.Add(storeName);
            var duration = BotMarketPlanner.DrawShiftDuration(configuration, Rand.NextInt);
            bot.MarketShift = new BotMarketShift(marketDefinition, stall, storeName, now, now + duration);
            bot.Logger.LogDebug("Bot '{Name}' goes to the market for {Duration} with {Count} goods.", bot.Name, duration, BotMarketPlanner.CountPricedGoods(bot));
        }
    }
}
