// <copyright file="GameMapInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Adapter which takes an <see cref="GameMap"/> and adapts it to a <see cref="IGameMapInfo"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IGameMapInfo" />
    internal sealed class GameMapInfoAdapter : IGameMapInfo, IDisposable
    {
        private readonly GameMap map;
        private readonly ConcurrentDictionary<Player, IPlayerInfo> players = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapInfoAdapter"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="gameContext">The game context.</param>
        public GameMapInfoAdapter(GameMap map, IGameContext gameContext)
        {
            this.map = map;
            gameContext.ForEachPlayer(p =>
            {
                if (p.CurrentMap == this.map)
                {
                    this.players.TryAdd(p, new PlayerInfo(p));
                }
            });

            this.map.ObjectAdded += this.OnMapObjectAdded;
            this.map.ObjectRemoved += this.OnMapObjectRemoved;

        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public short MapNumber => this.map.Definition.Number;

        /// <inheritdoc/>
        public string MapName => this.map.Definition.Name;

        /// <inheritdoc/>
        public byte[]? TerrainData => this.map.Definition.TerrainData;

        /// <inheritdoc/>
        public IList<IPlayerInfo> Players => this.players.Values.ToList();

        /// <inheritdoc/>
        public int PlayerCount => this.players.Count();

        /// <inheritdoc />
        public void Dispose()
        {
            this.map.ObjectAdded -= this.OnMapObjectAdded;
            this.map.ObjectRemoved -= this.OnMapObjectRemoved;
            this.PropertyChanged = null;
            this.players.Clear();
        }

        private void OnMapObjectAdded(object? sender, (GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Player player)
            {
                this.players.TryAdd(player, new PlayerInfo(player));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Players)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlayerCount)));
            }
        }

        private void OnMapObjectRemoved(object? sender, (GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Player player)
            {
                this.players.TryRemove(player, out _);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Players)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PlayerCount)));
            }
        }

        private class PlayerInfo : IPlayerInfo
        {
            private readonly Player player;

            public PlayerInfo(Player player)
            {
                this.player = player;
            }

            public string HostAdress
            {
                get
                {
                    var remotePlayer = this.player as RemotePlayer;
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
                    var character = this.player.SelectedCharacter;
                    if (character != null)
                    {
                        return character.Name;
                    }

                    return "N/A";
                }
            }

            public string AccountName => this.player.Account?.LoginName ?? "N/A";

            public byte LocationX => this.player.Position.X;

            public byte LocationY => this.player.Position.Y;
        }
    }
}
