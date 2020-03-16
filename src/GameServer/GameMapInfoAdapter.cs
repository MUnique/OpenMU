// <copyright file="GameMapInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace MUnique.OpenMU.GameServer
{
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
    internal class GameMapInfoAdapter : IGameMapInfo
    {
        private readonly GameMap map;

        private readonly IEnumerable<Player> players;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapInfoAdapter"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="players">The players.</param>
        public GameMapInfoAdapter(GameMap map, IEnumerable<Player> players)
        {
            this.map = map;
            this.players = players;
            this.map.ObjectAdded += this.OnMapObjectAddedOrRemoved;
            this.map.ObjectRemoved += this.OnMapObjectAddedOrRemoved;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public short MapNumber => this.map.Definition.Number;

        /// <inheritdoc/>
        public string MapName => this.map.Definition.Name;

        /// <inheritdoc/>
        public byte[] TerrainData => this.map.Definition.TerrainData;

        /// <inheritdoc/>
        public IList<IPlayerInfo> Players
        {
            get { return this.players.Select(p => new PlayerInfo(p) as IPlayerInfo).ToList(); }
        }

        /// <inheritdoc/>
        public int PlayerCount => this.players.Count();

        private void OnMapObjectAddedOrRemoved(object sender, GameMapEventArgs e)
        {
            if (e.Object is Player)
            {
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
                    if (remotePlayer?.Connection != null)
                    {
                        return remotePlayer.Connection.ToString();
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

            public string AccountName => this.player.Account.LoginName;

            public byte LocationX => this.player.Position.X;

            public byte LocationY => this.player.Position.Y;
        }
    }
}
