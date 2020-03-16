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
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Adapter which takes an <see cref="GameServer"/> and adapts it to a <see cref="IGameServerInfo"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Interfaces.IGameServerInfo" />
    internal class GameServerInfoAdapter : IGameServerInfo
    {
        private readonly GameServer gameServer;
        private readonly GameServerConfiguration configuration;
        private IList<IGameMapInfo> gameMapInfos;

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
                return this.gameMapInfos ??= this.gameServer.Context.Maps.Select(this.CreateMapAdapter).ToList<IGameMapInfo>();
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
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool UpdateMaps()
        {
            if (this.gameMapInfos == null)
            {
                return false;
            }

            bool mapsChanged = false;

            // Add new maps
            this.gameServer.Context.Maps
                .Where(map => this.gameMapInfos.All(m => m.MapNumber != map.MapId))
                .Select(this.CreateMapAdapter)
                .ForEach(map =>
                {
                    this.gameMapInfos.Add(map);
                    mapsChanged = true;
                });

            // Remove old maps
            this.gameMapInfos
                .Where(map => this.gameServer.Context.Maps.All(m => m.MapId != map.MapNumber))
                .ForEach(map =>
                {
                    this.gameMapInfos.Remove(map);
                    map.PropertyChanged -= this.OnMapPropertyChanged;
                    mapsChanged = true;
                });

            return mapsChanged;
        }

        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.Maps));
        }

        private GameMapInfoAdapter CreateMapAdapter(GameMap gameMap)
        {
            var mapAdapter = new GameMapInfoAdapter(gameMap, this.gameServer.Context.PlayerList.Where(p => p.CurrentMap == gameMap));
            mapAdapter.PropertyChanged += this.OnMapPropertyChanged;
            return mapAdapter;
        }

        private void OnGameServerPropertyChanged(object sender, PropertyChangedEventArgs e)
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
}
