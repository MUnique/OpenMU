// <copyright file="PlayerModule.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using Nancy;

    /// <summary>
    /// Module to manage players which are currently online.
    /// </summary>
    public class PlayerModule : NancyModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PlayerModule));

        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerModule"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public PlayerModule(IList<IManageableServer> servers)
            : base("admin/player")
        {
            this.servers = servers;
            this.Get["disconnect/{serverId:int}/{playerName}"] = this.DisconnectPlayer;
            this.Get["ban/{serverId:int}/{playerName}"] = this.BanPlayer;
        }

        private object BanPlayer(dynamic parameters)
        {
            int serverId = parameters.serverId;
            string playerName = parameters.playerName;
            Log.Info($"requested ban for player {playerName}");
            var server = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == serverId);
            if (server == null)
            {
                Log.Warn($"game server not found: {serverId}");
                return false;
            }

            return server.BanPlayer(playerName);
        }

        private object DisconnectPlayer(dynamic parameters)
        {
            int serverId = parameters.serverId;
            string playerName = parameters.playerName;
            Log.Info($"requested disconnect for player {playerName}");
            var server = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == serverId);
            if (server == null)
            {
                Log.Warn($"game server not found: {serverId}");
                return false;
            }

            return server.DisconnectPlayer(playerName);
        }
    }
}