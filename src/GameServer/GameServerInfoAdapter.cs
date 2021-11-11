// <copyright file="GameServerInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Adapter which takes an <see cref="GameServer"/> and adapts it to a <see cref="IGameServerInfo"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerInfo" />
internal class GameServerInfoAdapter : IGameServerInfo
{
    private readonly GameServer _gameServer;
    private readonly GameServerConfiguration _configuration;
    private IList<IGameMapInfo>? _gameMapInfos;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerInfoAdapter"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    /// <param name="configuration">The configuration.</param>
    public GameServerInfoAdapter(GameServer gameServer, GameServerConfiguration configuration)
    {
        this._gameServer = gameServer;
        this._configuration = configuration;
        this._gameServer.PropertyChanged += this.OnGameServerPropertyChanged;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public byte Id => this._gameServer.Id;

    /// <inheritdoc/>
    public string Description => this._gameServer.Description;

    /// <inheritdoc/>
    public ServerState State => this._gameServer.ServerState;

    /// <inheritdoc/>
    public int OnlinePlayerCount => this._gameServer.Context.PlayerCount;

    /// <inheritdoc/>
    public int MaximumPlayers => this._configuration.MaximumPlayers;

    /// <inheritdoc/>
    public IList<IGameMapInfo> Maps => this._gameMapInfos ??= this._gameServer.Context.Maps.Select(this.CreateMapAdapter).ToList<IGameMapInfo>();

    /// <inheritdoc/>
    public override string ToString()
    {
        return this._gameServer.ToString();
    }

    /// <summary>
    /// Called when a property has been changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool UpdateMaps()
    {
        if (this._gameMapInfos is null)
        {
            return false;
        }

        bool mapsChanged = false;

        // Add new maps
        this._gameServer.Context.Maps
            .Where(map => this._gameMapInfos.All(m => m.MapNumber != map.MapId))
            .Select(this.CreateMapAdapter)
            .ForEach(map =>
            {
                this._gameMapInfos!.Add(map);
                mapsChanged = true;
            });

        // Remove old maps
        this._gameMapInfos
            .Where(map => this._gameServer.Context.Maps.All(m => m.MapId != map.MapNumber))
            .ForEach(map =>
            {
                this._gameMapInfos.Remove(map);
                map.PropertyChanged -= this.OnMapPropertyChanged;
                (map as IDisposable)?.Dispose();
                mapsChanged = true;
            });

        return mapsChanged;
    }

    private void OnMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(this.Maps));
    }

    private GameMapInfoAdapter CreateMapAdapter(GameMap gameMap)
    {
        var mapAdapter = new GameMapInfoAdapter(gameMap, this._gameServer.Context);
        mapAdapter.PropertyChanged += this.OnMapPropertyChanged;
        return mapAdapter;
    }

    private void OnGameServerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IManageableServer.CurrentConnections):
                this.RaisePropertyChanged(nameof(this.OnlinePlayerCount));
                break;
            case nameof(IManageableServer.ServerState):
                this.RaisePropertyChanged(nameof(this.State));
                break;
            case nameof(GameServer.Context):
                if (this.UpdateMaps())
                {
                    this.RaisePropertyChanged(nameof(this.Maps));
                }

                break;
            case "":
            case null:
                this.RaisePropertyChanged();
                break;
            default:
                // don't need to handle other events.
                break;
        }
    }
}