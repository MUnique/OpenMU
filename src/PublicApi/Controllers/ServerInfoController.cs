// <copyright file="ServerInfoController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PublicApi.Models;

    /// <summary>
    /// Controller which provides data about the state of the <see cref="IConnectServer"/>s and its registered <see cref="IGameServer"/>s.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ServerInfoController : ControllerBase
    {
        private readonly IEnumerable<IConnectServer> connectServers;
        private readonly ICollection<IGameServer> gameServers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoController"/> class.
        /// </summary>
        /// <param name="connectServers">The connect servers.</param>
        /// <param name="gameServers">The game servers.</param>
        public ServerInfoController(IEnumerable<IConnectServer> connectServers, ICollection<IGameServer> gameServers)
        {
            this.connectServers = connectServers;
            this.gameServers = gameServers;
        }

        /// <summary>
        /// Gets the data about all <see cref="IConnectServer"/>s.
        /// </summary>
        /// <returns>The data about all <see cref="IConnectServer"/>s.</returns>
        [HttpGet]
        public IList<ConnectServerDto> Get()
        {
            return this.connectServers.Select(cs => ConnectServerDto.Create(cs, this.gameServers)).ToList();
        }
    }
}