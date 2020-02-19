// <copyright file="GameServerInfoAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Adapter which takes an <see cref="GameServer"/> and adapts it to a <see cref="IGameServerInfo"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerInfo" />
    internal class GameServerInfoAdapter : IGameServerInfo
    {
        private readonly GameServer gameServer;
        private readonly GameServerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerInfoAdapter"/> class.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        /// <param name="configuration">The configuration.</param>
        public GameServerInfoAdapter(GameServer gameServer, GameServerConfiguration configuration)
        {
            this.gameServer = gameServer;
            this.configuration = configuration;
            this.gameServer.PropertyChanged += this.OnGameServerPropertyChanged;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public byte Id => this.gameServer.Id;

        /// <inheritdoc/>
        public string Description => this.gameServer.Description;

        /// <inheritdoc/>
        public ServerState State => this.gameServer.ServerState;

        /// <inheritdoc/>
        public int OnlinePlayerCount => this.gameServer.Context.PlayerList.Count;

        /// <inheritdoc/>
        public int MaximumPlayers => this.configuration.MaximumPlayers;

        /// <inheritdoc/>
        public IList<IGameMapInfo> Maps
        {
            get
            {
                return this.gameServer.Context.Maps.Select(map =>
                        new GameMapInfoAdapter(
                            map,
                            this.gameServer.Context.PlayerList.Where(p => p.CurrentMap == map).ToList()))
                    .ToList<IGameMapInfo>();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.gameServer.ToString();
        }

        /// <summary>
        /// Called when a property has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnGameServerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IManageableServer.CurrentConnections):
                    this.OnPropertyChanged(nameof(this.OnlinePlayerCount));
                    break;
                case nameof(IManageableServer.ServerState):
                    this.OnPropertyChanged(nameof(this.State));
                    break;
                case "":
                case null:
                    this.OnPropertyChanged();
                    break;
                default:
                    // don't need to handle other events.
                    break;
            }
        }
    }
}
