// <copyright file="GameServerEndPointInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// The public end point of a game server for clients of a specific version.
/// </summary>
/// <param name="PublicEndPoint">The public end point, to which the clients connect.</param>
/// <param name="Season">The season of the client version.</param>
/// <param name="Episode">The episode of the client version.</param>
/// <param name="Language">The language of the client version.</param>
public record GameServerEndPointInfo(string PublicEndPoint, byte Season, byte Episode, byte Language);
