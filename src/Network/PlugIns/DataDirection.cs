// <copyright file="DataDirection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    /// <summary>
    /// The direction of the data flow.
    /// </summary>
    public enum DataDirection
    {
        /// <summary>
        /// The data is sent from the client to the server.
        /// </summary>
        ClientToServer,

        /// <summary>
        /// The data is sent from the server to the client.
        /// </summary>
        ServerToClient,
    }
}