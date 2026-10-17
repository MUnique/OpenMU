// <copyright file="GameServerHeartbeatArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a game server heartbeat.
/// </summary>
/// <param name="ServerInfo">The information about the game server.</param>
/// <param name="EndPoints">The public end points of the game server, one for each supported client version.</param>
/// <param name="UpTime">The up-time of the server.</param>
public record GameServerHeartbeatArguments(ServerInfo ServerInfo, IReadOnlyList<GameServerEndPointInfo> EndPoints, TimeSpan UpTime)
{
    /// <summary>
    /// Gets or sets the up-time of the server.
    /// </summary>
    public TimeSpan UpTime { get; set; } = UpTime;
}