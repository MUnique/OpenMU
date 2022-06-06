// <copyright file="GameServerHeartbeatArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a game server heartbeat.
/// </summary>
public record GameServerHeartbeatArguments(ServerInfo ServerInfo, string PublicEndPoint, TimeSpan UpTime)
{
    /// <summary>
    /// Gets or sets the up-time of the server.
    /// </summary>
    public TimeSpan UpTime { get; set; } = UpTime;
}