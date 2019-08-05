// <copyright file="ICapturedConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System.ComponentModel;

    /// <summary>
    /// Interface for a captured connection.
    /// </summary>
    public interface ICapturedConnection
    {
        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the packets of the connection.
        /// </summary>
        BindingList<Packet> PacketList { get; }
    }
}