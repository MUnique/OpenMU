// <copyright file="ObservableGameServerAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Adapter which takes an <see cref="IGameServerContext"/> and adapts it to a <see cref="IObservableGameServer"/>.
/// </summary>
public class ObservableGameServerAdapter : Disposable, IObservableGameServer
{
    private readonly IGameServerContext _gameContext;
    private readonly List<IGameMapInfo> _gameMapInfos = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGameServerAdapter"/> class.
    /// </summary>
    /// <param name="gameContext">The game server context.</param>
    public ObservableGameServerAdapter(IGameServerContext gameContext)
    {
        this._gameContext = gameContext;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public int Id => this._gameContext.Id;

    /// <inheritdoc/>
    public IList<IGameMapInfo> Maps => this._gameMapInfos;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        foreach (var map in await this._gameContext.GetMapsAsync().ConfigureAwait(false))
        {
            var mapAdapter = await this.CreateMapAdapterAsync(map).ConfigureAwait(false);
            this._gameMapInfos.Add(mapAdapter);
        }

        this._gameContext.GameMapCreated += this.OnGameMapCreated;
        this._gameContext.GameMapRemoved += this.OnGameMapRemoved;
    }

    /// <inheritdoc/>
    public async ValueTask RegisterMapObserverAsync(Guid mapId, ILocateable worldObserver)
    {
        var maps = await this._gameContext.GetMapsAsync().ConfigureAwait(false);
        var map = maps.FirstOrDefault(m => m.Id == mapId);
        if (map != null)
        {
            await map.AddAsync(worldObserver).ConfigureAwait(false);
        }
        else
        {
            var message = $"map with id {mapId} not found.";
            throw new ArgumentException(message);
        }
    }

    /// <inheritdoc/>
    public async ValueTask UnregisterMapObserverAsync(Guid mapId, ushort worldObserverId)
    {
        var maps = await this._gameContext.GetMapsAsync().ConfigureAwait(false);
        if (maps.FirstOrDefault(m => m.Id == mapId) is { } map
            && map.GetObject(worldObserverId) is { } observer)
        {
            await map.RemoveAsync(observer).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this._gameContext.ToString() ?? string.Empty;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        this._gameContext.GameMapCreated -= this.OnGameMapCreated;
        this._gameContext.GameMapRemoved -= this.OnGameMapRemoved;
    }

    /// <summary>
    /// Called when a property has been changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnGameMapRemoved(object? sender, GameMap gameMap)
    {
        if (this._gameMapInfos.FirstOrDefault(i => i.Id == gameMap.Id) is not { } map)
        {
            return;
        }

        this._gameMapInfos.Remove(map);
        map.PropertyChanged -= this.OnMapPropertyChanged;
        (map as IDisposable)?.Dispose();
        this.RaisePropertyChanged(nameof(this.Maps));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnGameMapCreated(object? sender, GameMap gameMap)
    {
        try
        {
            if (this._gameMapInfos.FirstOrDefault(i => i.Id == gameMap.Id) is not null)
            {
                // we already know this map - should never happen.
                return;
            }

            var map = await this.CreateMapAdapterAsync(gameMap).ConfigureAwait(false);
            this._gameMapInfos.Add(map);
            this.RaisePropertyChanged(nameof(this.Maps));
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    private void OnMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(this.Maps));
    }

    private async ValueTask<GameMapInfoAdapter> CreateMapAdapterAsync(GameMap gameMap)
    {
        var mapAdapter = new GameMapInfoAdapter(gameMap, this._gameContext);
        await mapAdapter.InitializeAsync().ConfigureAwait(false);
        mapAdapter.PropertyChanged += this.OnMapPropertyChanged;
        return mapAdapter;
    }
}