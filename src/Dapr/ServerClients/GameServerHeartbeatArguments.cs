// <copyright file="GameServerHeartbeatArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.ServerClients;

public record GameServerHeartbeatArguments(ServerInfo ServerInfo, string PublicEndPoint, TimeSpan UpTime)
{
    public TimeSpan UpTime { get; set; } = UpTime;
}