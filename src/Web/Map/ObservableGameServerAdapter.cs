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
public class ObservableGameServerAdapter : IObservableGameServer
{
    private readonly IGameServerContext _gameContext;
    private readonly IList<IGameMapInfo> _gameMapInfos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGameServerAdapter"/> class.
    /// </summary>
    /// <param name="gameContext">The game server context.</param>
    public ObservableGameServerAdapter(IGameServerContext gameContext)
    {
        this._gameContext = gameContext;
        this._gameMapInfos ??= this._gameContext.Maps.Select(this.CreateMapAdapter).ToList<IGameMapInfo>();
        this._gameContext.GameMapCreated += this.OnGameMapCreated;
        this._gameContext.GameMapRemoved += this.OnGameMapRemoved;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public int Id => this._gameContext.Id;

    /// <inheritdoc/>
    public IList<IGameMapInfo> Maps => this._gameMapInfos;

    /// <inheritdoc/>
    public void RegisterMapObserver(Guid mapId, ILocateable worldObserver)
    {
        var map = this._gameContext.Maps.FirstOrDefault(m => m.Id == mapId);
        if (map != null)
        {
            map.AddAsync(worldObserver);
        }
        else
        {
            var message = $"map with id {mapId} not found.";
            throw new ArgumentException(message);
        }
    }

    /// <inheritdoc/>
    public void UnregisterMapObserver(Guid mapId, ushort worldObserverId)
    {
        if (this._gameContext.Maps.FirstOrDefault(m => m.Id == mapId) is { } map
            && map.GetObject(worldObserverId) is { } observer)
        {
            map.RemoveAsync(observer);
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this._gameContext.ToString() ?? string.Empty;
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

    private void OnGameMapCreated(object? sender, GameMap gameMap)
    {
        if (this._gameMapInfos.FirstOrDefault(i => i.Id == gameMap.Id) is not null)
        {
            // we already know this map - should never happen.
            return;
        }

        var map = this.CreateMapAdapter(gameMap);
        this._gameMapInfos.Add(map);
        this.RaisePropertyChanged(nameof(this.Maps));
    }

    private void OnMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(this.Maps));
    }

    private GameMapInfoAdapter CreateMapAdapter(GameMap gameMap)
    {
        var mapAdapter = new GameMapInfoAdapter(gameMap, this._gameContext);
        mapAdapter.PropertyChanged += this.OnMapPropertyChanged;
        return mapAdapter;
    }
}