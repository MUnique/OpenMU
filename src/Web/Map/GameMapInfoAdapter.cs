// <copyright file="GameMapInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.Collections.Concurrent;
using System.ComponentModel;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Adapter which takes an <see cref="GameMap"/> and adapts it to a <see cref="IGameMapInfo"/>.
/// </summary>
internal sealed class GameMapInfoAdapter : IGameMapInfo, IDisposable
{
    private readonly GameMap _map;
    private readonly IGameContext _gameContext;
    private readonly ConcurrentDictionary<Player, IPlayerInfo> _players = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapInfoAdapter"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="gameContext">The game context.</param>
    public GameMapInfoAdapter(GameMap map, IGameContext gameContext)
    {
        this._map = map;
        this._gameContext = gameContext;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public Guid Id => this._map.Id;

    /// <inheritdoc/>
    public short MapNumber => this._map.Definition.Number;

    /// <inheritdoc/>
    public string MapName => this._map.Definition.Name;

    /// <inheritdoc/>
    public byte[]? TerrainData => this._map.Definition.TerrainData;

    /// <inheritdoc/>
    public IList<IPlayerInfo> Players => this._players.Values.ToList();

    /// <inheritdoc/>
    public int PlayerCount => this._players.Count();

    /// <summary>
    /// Initializes the adapter.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        await this._gameContext.ForEachPlayerAsync(async p =>
        {
            if (p.CurrentMap == this._map)
            {
                this._players.TryAdd(p, new PlayerInfo(p));
            }
        }).ConfigureAwait(false);

        this._map.ObjectAdded += this.OnMapObjectAddedAsync;
        this._map.ObjectRemoved += this.OnMapObjectRemovedAsync;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._map.ObjectAdded -= this.OnMapObjectAddedAsync;
        this._map.ObjectRemoved -= this.OnMapObjectRemovedAsync;
        this.PropertyChanged = null;
        this._players.Clear();
    }

    private async ValueTask OnMapObjectAddedAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            this._players.TryAdd(player, new PlayerInfo(player));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Players)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlayerCount)));
        }
    }

    private async ValueTask OnMapObjectRemovedAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            this._players.TryRemove(player, out _);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Players)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlayerCount)));
        }
    }

    private class PlayerInfo : IPlayerInfo
    {
        private readonly Player _player;

        public PlayerInfo(Player player)
        {
            this._player = player;
        }

        public string CharacterName
        {
            get
            {
                var character = this._player.SelectedCharacter;
                if (character != null)
                {
                    return character.Name;
                }

                return "N/A";
            }
        }

        public string AccountName => this._player.Account?.LoginName ?? "N/A";

        public byte LocationX => this._player.Position.X;

        public byte LocationY => this._player.Position.Y;
    }
}