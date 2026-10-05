// <copyright file="GameServerAppId.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Provides the dapr app ids of the game servers.
/// </summary>
/// <remarks>
/// The app id of a game server is configured at its dapr sidecar, e.g. in the docker compose file.
/// It contains the game server id as it is, so the game server with id 0 has the app id "gameServer0".
/// </remarks>
public static class GameServerAppId
{
    /// <summary>
    /// Gets the dapr app id of the game server with the specified id.
    /// </summary>
    /// <param name="serverId">The game server identifier.</param>
    /// <returns>The dapr app id.</returns>
    public static string Of(int serverId) => $"gameServer{serverId}";

    /// <summary>
    /// Determines whether the specified identifier can be the identifier of a game server.
    /// </summary>
    /// <param name="serverId">The identifier.</param>
    /// <returns><c>true</c>, if the identifier can be the identifier of a game server; otherwise, <c>false</c>.</returns>
    public static bool IsValid(int serverId) => serverId is >= 0 and <= byte.MaxValue;
}
