// <copyright file="ConnectServerFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The connect server factory.
    /// </summary>
    public static class ConnectServerFactory
    {
        /// <summary>
        /// Creates a new connect server instance.
        /// </summary>
        /// <param name="stateObserver">The state observer.</param>
        /// <returns>
        /// The new connect server instance.
        /// </returns>
        public static OpenMU.Interfaces.IConnectServer CreateConnectServer(IServerStateObserver stateObserver)
        {
            var settings = new Settings();
            return new ConnectServer(settings, stateObserver);
        }
    }
}
