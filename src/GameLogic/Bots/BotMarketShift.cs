// <copyright file="BotMarketShift.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Threading;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A shift of a bot on the market map: the bot goes there with the goods it collected in its personal
/// store, opens the store at its stall and sells until the shift is over (see <see cref="BotMerchantPlugIn"/>).
/// </summary>
/// <remarks>
/// Set by <see cref="BotMerchantPlugIn"/> and carried out by <see cref="BotNavigator"/>. The plugin reads
/// and updates it from its own timer and from the sales, so the changing values are volatile.
/// </remarks>
internal sealed class BotMarketShift
{
    private long _lastSaleTicks;
    private volatile bool _isStoreOpened;
    private volatile bool _isOpenRequested;
    private volatile bool _isEnding;
    private volatile bool _isSettled;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotMarketShift"/> class.
    /// </summary>
    /// <param name="map">The market map.</param>
    /// <param name="stall">The position at which the bot opens its store.</param>
    /// <param name="storeName">The name of the store.</param>
    /// <param name="startUtc">When the shift starts.</param>
    /// <param name="untilUtc">When the shift is over.</param>
    public BotMarketShift(GameMapDefinition map, Point stall, string storeName, DateTime startUtc, DateTime untilUtc)
    {
        this.Map = map;
        this.Stall = stall;
        this.StoreName = storeName;
        this.StartUtc = startUtc;
        this.UntilUtc = untilUtc;
        this._lastSaleTicks = startUtc.Ticks;
    }

    /// <summary>
    /// Gets the market map.
    /// </summary>
    public GameMapDefinition Map { get; }

    /// <summary>
    /// Gets the position at which the bot opens its store - apart from the stalls of the other merchants.
    /// </summary>
    public Point Stall { get; }

    /// <summary>
    /// Gets the name of the store.
    /// </summary>
    public string StoreName { get; }

    /// <summary>
    /// Gets when the shift started.
    /// </summary>
    public DateTime StartUtc { get; }

    /// <summary>
    /// Gets when the shift is over.
    /// </summary>
    public DateTime UntilUtc { get; }

    /// <summary>
    /// Gets or sets when the bot sold something the last time; the start of the shift before the first sale.
    /// </summary>
    public DateTime LastSaleUtc
    {
        get => new(Interlocked.Read(ref this._lastSaleTicks), DateTimeKind.Utc);
        set => Interlocked.Exchange(ref this._lastSaleTicks, value.Ticks);
    }

    /// <summary>
    /// Gets or sets the money of the bot when it opened its store: everything above is income, which is removed.
    /// </summary>
    public int MoneyAtOpen { get; set; }

    /// <summary>
    /// Gets or sets the items of the inventory when the bot opened its store: every other item which
    /// appears in it during the shift is income, e.g. jewels paid for a good, and is removed.
    /// </summary>
    public IReadOnlySet<Item> ItemsAtOpen { get; set; } = new HashSet<Item>();

    /// <summary>
    /// Gets or sets a value indicating whether the bot opened its store on this shift.
    /// </summary>
    public bool IsStoreOpened
    {
        get => this._isStoreOpened;
        set => this._isStoreOpened = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the bot already queued the opening of its store, which
    /// runs in its next tick.
    /// </summary>
    public bool IsOpenRequested
    {
        get => this._isOpenRequested;
        set => this._isOpenRequested = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the shift is ending: the store gets closed in the next
    /// tick of the bot, and the shift is removed afterward.
    /// </summary>
    public bool IsEnding
    {
        get => this._isEnding;
        set => this._isEnding = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the store of the shift is closed and its income removed.
    /// A removal of the income which was queued by a late sale does nothing afterward - the goods taken
    /// out of the store meanwhile aren't income (see <see cref="BotMerchandise.CloseShiftStoreAsync"/>).
    /// </summary>
    public bool IsSettled
    {
        get => this._isSettled;
        set => this._isSettled = value;
    }
}
