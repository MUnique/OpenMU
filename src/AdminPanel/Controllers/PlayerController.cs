// <copyright file="PlayerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Controller for the player list in the live map view.
    /// </summary>
    [Route("admin/[controller]/[action]")]
    public class PlayerController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PlayerController));

        private readonly IList<IManageableServer> servers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerController"/> class.
        /// </summary>
        /// <param name="servers">The servers.</param>
        public PlayerController(IList<IManageableServer> servers)
        {
            this.servers = servers;
        }

        /// <summary>
        /// Bans the specified player.
        /// </summary>
        /// <param name="serverId">The server identifier of the server where the player is playing on.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The success of the operation.</returns>
        [HttpGet("{serverId}/{playerName}")]
        public ActionResult<bool> Ban(int serverId, string playerName)
        {
            Log.Info($"requested ban for player {playerName}");
            var server = this.servers.OfType<IGameServer>().FirstOrDefault(s => s.Id == serverId);
            if (server == null)
            {
                Log.Warn($"game server not found: {serverId}");
                return false;
            }

            return server.BanPlayer(playerName);
        }

        /// <summary>
        /// Disconnects the specified player.
        /// </summary>
        /// <param name="serverId">The server identifier of the server where the player is playing on.</param>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The success of the operation.</returns>
        [HttpGet("{serverId}/{playerName}")]
        public ActionResult<bool> Disconnect(int serverId, string playerName)
        {
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
