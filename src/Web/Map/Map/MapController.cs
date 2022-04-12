// <copyright file="MapController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.Map.ViewPlugIns;

/// <summary>
/// Controller which contains the logic for the shown game map.
/// </summary>
public sealed class MapController : IMapController, IWorldObserver, ILocateable, IBucketMapObserver, ISupportIdUpdate
{
    private readonly string _identifier;
    private readonly IObservableGameServer _gameServer;
    private readonly Guid _mapId;
    private readonly IJSRuntime _jsRuntime;
    private readonly ObserverToWorldViewAdapter _adapterToWorldView;
    private readonly CancellationTokenSource _disposeCts = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MapController" /> class.
    /// </summary>
    /// <param name="jsRuntime">The js runtime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="mapIdentifier">The map identifier.</param>
    /// <param name="gameServer">The game server.</param>
    /// <param name="mapId">The map id.</param>
    public MapController(IJSRuntime jsRuntime, ILoggerFactory loggerFactory, string mapIdentifier, IObservableGameServer gameServer, Guid mapId)
    {
        this._jsRuntime = jsRuntime;

        this._identifier = mapIdentifier;
        this._gameServer = gameServer;
        this._mapId = mapId;
        this._adapterToWorldView = new ObserverToWorldViewAdapter(this, byte.MaxValue);

        var viewPlugIns = new CustomPlugInContainer<IViewPlugIn>();
        var worldAccessor = $"{mapIdentifier}.world";
        viewPlugIns.RegisterPlugIn<IObjectGotKilledPlugIn>(new ObjectGotKilledPlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IObjectMovedPlugIn>(new ObjectMovedPlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IShowAnimationPlugIn>(new ShowAnimationPlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IObjectsOutOfScopePlugIn>(new ObjectsOutOfScopePlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
        viewPlugIns.RegisterPlugIn<INewPlayersInScopePlugIn>(new NewPlayersInScopePlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
        viewPlugIns.RegisterPlugIn<INewNpcsInScopePlugIn>(new NewNpcsInScopePlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token, this.Objects, () => this.ObjectsChanged?.Invoke(this, EventArgs.Empty)));
        viewPlugIns.RegisterPlugIn<IShowSkillAnimationPlugIn>(new ShowSkillAnimationPlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token));
        viewPlugIns.RegisterPlugIn<IShowAreaSkillAnimationPlugIn>(new ShowAreaSkillAnimationPlugIn(this._jsRuntime, loggerFactory, worldAccessor, this._disposeCts.Token));

        this.ViewPlugIns = viewPlugIns;
    }

    /// <inheritdoc />
    public event EventHandler? ObjectsChanged;

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
    public IDictionary<int, ILocateable> Objects { get; } = new Dictionary<int, ILocateable>();

    /// <inheritdoc/>
    public void LocateableAdded(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
    {
        this._adapterToWorldView.LocateableAdded(sender, eventArgs);
    }

    /// <inheritdoc/>
    public void LocateableRemoved(object? sender, BucketItemEventArgs<ILocateable> eventArgs)
    {
        this._adapterToWorldView.LocateableRemoved(sender, eventArgs);
    }

    /// <inheritdoc/>
    public void LocateablesOutOfScope(IEnumerable<ILocateable> oldObjects)
    {
        this._adapterToWorldView.LocateablesOutOfScope(oldObjects);
    }

    /// <inheritdoc/>
    public void NewLocateablesInScope(IEnumerable<ILocateable> newObjects)
    {
        this._adapterToWorldView.NewLocateablesInScope(newObjects);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._disposeCts.Cancel();
        this._gameServer.UnregisterMapObserver(this._mapId, this.Id);
        this._adapterToWorldView.Dispose();
        try
        {
            await this._jsRuntime.InvokeVoidAsync("DisposeMap", this._identifier);
        }
        finally
        {
            this._disposeCts.Dispose();
        }
    }
}