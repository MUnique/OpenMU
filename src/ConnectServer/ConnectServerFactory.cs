// <copyright file="ConnectServerFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// The connect server factory.
    /// </summary>
    public class ConnectServerFactory
    {
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServerFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConnectServerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates a new connect server instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="configurationId">The configuration identifier.</param>
        /// <returns>
        /// The new connect server instance.
        /// </returns>
        public OpenMU.Interfaces.IConnectServer CreateConnectServer(IConnectServerSettings settings, ClientVersion clientVersion, Guid configurationId)
        {
            return new ConnectServer(settings, clientVersion, configurationId, this.loggerFactory);
        }
    }
}
