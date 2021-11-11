﻿// <copyright file="GameMapInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.Collections.Concurrent;
using System.ComponentModel;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Adapter which takes an <see cref="GameMap"/> and adapts it to a <see cref="IGameMapInfo"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Interfaces.IGameMapInfo" />
internal sealed class GameMapInfoAdapter : IGameMapInfo, IDisposable
{
    private readonly GameMap _map;
    private readonly ConcurrentDictionary<Player, IPlayerInfo> _players = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapInfoAdapter"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="gameContext">The game context.</param>
    public GameMapInfoAdapter(GameMap map, IGameContext gameContext)
    {
        this._map = map;
        gameContext.ForEachPlayer(p =>
        {
            if (p.CurrentMap == this._map)
            {
                this._players.TryAdd(p, new PlayerInfo(p));
            }
        });

        this._map.ObjectAdded += this.OnMapObjectAdded;
        this._map.ObjectRemoved += this.OnMapObjectRemoved;

    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

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

    /// <inheritdoc />
    public void Dispose()
    {
        this._map.ObjectAdded -= this.OnMapObjectAdded;
        this._map.ObjectRemoved -= this.OnMapObjectRemoved;
        this.PropertyChanged = null;
        this._players.Clear();
    }

    private void OnMapObjectAdded(object? sender, (GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            this._players.TryAdd(player, new PlayerInfo(player));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Players)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlayerCount)));
        }
    }

    private void OnMapObjectRemoved(object? sender, (GameMap Map, ILocateable Object) args)
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

        public string HostAdress
        {
            get
            {
                var remotePlayer = this._player as RemotePlayer;
                if (remotePlayer?.Connection?.ToString() is { } address)
                {
                    return address;
                }

                return "N/A";
            }
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