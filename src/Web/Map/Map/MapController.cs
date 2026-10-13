// <copyright file="MapController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.Map.ViewPlugIns;
using Nito.AsyncEx;

/// <summary>
/// Controller which contains the logic for the shown game map.
/// </summary>
public sealed class MapController : IMapController, IWorldObserver, ILocateable, IBucketMapObserver, ISupportIdUpdate
{
    /// <summary>
    /// The time to wait before resynchronizing after the browser is back.
    /// Blazor raises the connection-up event before the circuit can carry js interop calls again,
    /// so the first calls of an immediate resynchronization would be dropped.
    /// </summary>
    private static readonly TimeSpan ResyncDelay = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// The time which the disposal waits for a running resynchronization.
    /// </summary>
    private static readonly TimeSpan ResyncSettleTimeout = TimeSpan.FromSeconds(5);

    private readonly string _identifier;
    private readonly IObservableGameServer _gameServer;
    private readonly Guid _mapId;
    private readonly IJSRuntime _jsRuntime;
    private readonly JsInteropQueue _jsQueue;
    private readonly ObserverToWorldViewAdapter _adapterToWorldView;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly CircuitConnectionState _circuitState;
    private readonly AsyncLock _resyncLock = new();

    private int _disposeState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapController" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="mapIdentifier">The map identifier.</param>
    /// <param name="gameServer">The game server.</param>
    /// <param name="mapId">The map id.</param>
    /// <param name="circuitState">The connection state of the browser circuit.</param>
    public MapController(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string mapIdentifier, IObservableGameServer gameServer, Guid mapId, CircuitConnectionState circuitState)
    {
        this._jsRuntime = jsRuntime;
        this._circuitState = circuitState;

        this._identifier = mapIdentifier;
        this._gameServer = gameServer;
        this._mapId = mapId;
        this.Logger = loggerFactory.CreateLogger(this.GetType());

        // The view plugins call the browser through the queue, so the game logic never waits for the browser.
        this._jsQueue = new JsInteropQueue(jsRuntime, this.Logger, circuitState);
        this._jsQueue.ResynchronizationRequired += this.OnResynchronizationRequired;
        this._adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);

        var viewPlugIns = new CustomPlugInContainer<IViewPlugIn>();
        var worldAccessor = $"{mapIdentifier}.world";
        viewPlugIns.RegisterPlugIn<IObjectGotKilledPlugIn>(new ObjectGotKilledPlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IObjectMovedPlugIn>(new ObjectMovedPlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IShowAnimationPlugIn>(new ShowAnimationPlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IObjectsOutOfScopePlugIn>(new ObjectsOutOfScopePlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token, this.Players, () => this.PlayersChanged?.Invoke(this, EventArgs.Empty)));
        viewPlugIns.RegisterPlugIn<INewPlayersInScopePlugIn>(new NewPlayersInScopePlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token, this.Players, () => this.PlayersChanged?.Invoke(this, EventArgs.Empty)));
        viewPlugIns.RegisterPlugIn<INewNpcsInScopePlugIn>(new NewNpcsInScopePlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IShowSkillAnimationPlugIn>(new ShowSkillAnimationPlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IShowAreaSkillAnimationPlugIn>(new ShowAreaSkillAnimationPlugIn(this._jsQueue, loggerFactory, worldAccessor, this._disposeCts.Token));

        this.ViewPlugIns = viewPlugIns;

        this._circuitState.ConnectionRestored += this.OnConnectionRestored;
    }

    /// <inheritdoc />
    public event EventHandler? PlayersChanged;

    /// <inheritdoc />
    public ILogger Logger { get; }

    /// <inheritdoc/>
    public int InfoRange => byte.MaxValue;

    /// <inheritdoc/>
    public GameMap? CurrentMap => null;

    /// <inheritdoc/>
    public Point Position { get; set; }

    /// <inheritdoc cref="ISupportIdUpdate" />
    public ushort Id { get; set; }

    /// <inheritdoc/>
    public IList<Bucket<ILocateable>> ObservingBuckets => this._adapterToWorldView.ObservingBuckets;

    /// <inheritdoc/>
    public ICustomPlugInContainer<IViewPlugIn> ViewPlugIns { get; }

    /// <inheritdoc />
    public ConcurrentDictionary<int, Player> Players { get; } = new();

    /// <inheritdoc/>
    public ValueTask LocateableAddedAsync(ILocateable item)
    {
        return this._adapterToWorldView.LocateableAddedAsync(item);
    }

    /// <inheritdoc/>
    public ValueTask LocateableRemovedAsync(ILocateable item)
    {
        return this._adapterToWorldView.LocateableRemovedAsync(item);
    }

    /// <inheritdoc/>
    public ValueTask LocateablesOutOfScopeAsync(IEnumerable<ILocateable> oldObjects)
    {
        return this._adapterToWorldView.LocateablesOutOfScopeAsync(oldObjects);
    }

    /// <inheritdoc/>
    public ValueTask NewLocateablesInScopeAsync(IEnumerable<ILocateable> newObjects)
    {
        return this._adapterToWorldView.NewLocateablesInScopeAsync(newObjects);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this._disposeState, 1) != 0)
        {
            return;
        }

        this._circuitState.ConnectionRestored -= this.OnConnectionRestored;
        this._jsQueue.ResynchronizationRequired -= this.OnResynchronizationRequired;

        IDisposable? resyncLock = null;
        try
        {
            await this._disposeCts.CancelAsync().ConfigureAwait(false);

            // A resynchronization between its unregistration and its registration would otherwise
            // put this disposed controller back on the game map, where nothing would remove it again.
            try
            {
                using var timeout = new CancellationTokenSource(ResyncSettleTimeout);
                resyncLock = await this._resyncLock.LockAsync(timeout.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                this.Logger.LogWarning("Timed out waiting for the resynchronization of map view '{MapIdentifier}' before disposing it.", this._identifier);
            }

            await this._gameServer.UnregisterMapObserverAsync(this._mapId, this.Id).ConfigureAwait(false);
            await this._adapterToWorldView.DisposeAsync().ConfigureAwait(false);
            await this._jsQueue.DisposeAsync().ConfigureAwait(false);

            try
            {
                await this._jsRuntime.InvokeVoidAsync("DisposeMap", this._identifier).ConfigureAwait(false);
            }
            catch (Exception e) when (e is JSDisconnectedException or OperationCanceledException or ObjectDisposedException)
            {
                // The browser is already gone, so there is nothing left to dispose on its side.
            }
        }
        finally
        {
            resyncLock?.Dispose();
            this._disposeCts.Dispose();
        }
    }

    /// <summary>
    /// Resynchronizes the map after the browser reconnected, because the calls were dropped while it was away.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnConnectionRestored(object? sender, EventArgs e)
    {
        // It must not run inline: the circuit doesn't carry js interop calls yet, and the
        // registration needs the observer lock as a writer.
        this.ScheduleResync("reconnected");
    }

    /// <summary>
    /// Resynchronizes the map after the browser fell so far behind that calls were dropped.
    /// It's raised by the pump of the queue, so it only schedules the resynchronization.
    /// </summary>
    private void OnResynchronizationRequired()
    {
        this.ScheduleResync("fell behind");
    }

    private void ScheduleResync(string reason)
    {
        if (Volatile.Read(ref this._disposeState) != 0)
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await this.ResyncAsync(reason).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, "Error when resynchronizing the map view '{MapIdentifier}' after the browser {Reason}.", this._identifier, reason);
            }
        });
    }

    private async ValueTask ResyncAsync(string reason)
    {
        CancellationToken disposeToken;
        IDisposable resyncLock;
        try
        {
            // Taken once, because the token source is disposed with this controller.
            disposeToken = this._disposeCts.Token;
            await Task.Delay(ResyncDelay, disposeToken).ConfigureAwait(false);

            // The browser can go down and up repeatedly, and the resynchronizations must not overlap.
            resyncLock = await this._resyncLock.LockAsync(disposeToken).ConfigureAwait(false);
        }
        catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException)
        {
            return;
        }

        using (resyncLock)
        {
            if (Volatile.Read(ref this._disposeState) != 0 || !this._circuitState.IsConnected)
            {
                // Disposed, or the browser is gone again. The next reconnect resynchronizes.
                return;
            }

            this.Logger.LogInformation("The browser of map view '{MapIdentifier}' {Reason}; resynchronizing the map.", this._identifier, reason);

            // Unregistering first, so that the adapter doesn't consider the objects as already
            // observed and skips them when they're added again.
            await this._gameServer.UnregisterMapObserverAsync(this._mapId, this.Id).ConfigureAwait(false);
            this._jsQueue.DiscardPending();

            // Objects which left the map while calls were dropped were never removed in the browser,
            // so it starts off empty and gets the current state with the registration.
            await this._jsQueue.InvokeVoidAsync($"{this._identifier}.world.clear", disposeToken).ConfigureAwait(false);

            if (Volatile.Read(ref this._disposeState) != 0)
            {
                return;
            }

            await this._gameServer.RegisterMapObserverAsync(this._mapId, this).ConfigureAwait(false);
        }
    }
}