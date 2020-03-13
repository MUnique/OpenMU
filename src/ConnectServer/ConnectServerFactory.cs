// <copyright file="ConnectServerFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network.PlugIns;

    /// <summary>
    /// The connect server factory.
    /// </summary>
    public static class ConnectServerFactory
    {
        /// <summary>
        /// Creates a new connect server instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="configurationId">The configuration identifier.</param>
        /// <returns>
        /// The new connect server instance.
        /// </returns>
        public static OpenMU.Interfaces.IConnectServer CreateConnectServer(IConnectServerSettings settings, ClientVersion clientVersion, Guid configurationId)
        {
            return new ConnectServer(settings, clientVersion, configurationId);
        }
    }
}
