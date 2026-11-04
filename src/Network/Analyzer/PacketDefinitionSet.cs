// <copyright file="PacketDefinitionSet.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// The set of packet definitions which is used to analyze the traffic of a connection.
/// Which one applies depends on the server the client is connected to.
/// </summary>
public enum PacketDefinitionSet
{
    /// <summary>
    /// The packets which are exchanged between game client and game server.
    /// </summary>
    GameServer,

    /// <summary>
    /// The packets which are exchanged between game client and connect server.
    /// </summary>
    ConnectServer,

    /// <summary>
    /// The packets which are exchanged between game client and chat server.
    /// </summary>
    ChatServer,
}
