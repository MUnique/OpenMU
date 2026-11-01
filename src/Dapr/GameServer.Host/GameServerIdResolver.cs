// <copyright file="GameServerIdResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Determines the id of the game server of this process.
/// </summary>
public static partial class GameServerIdResolver
{
    /// <summary>
    /// Determines the id of the game server of this process.
    /// </summary>
    /// <returns>The id of the game server.</returns>
    /// <remarks>
    /// It's taken from the environment variable <c>GS_ID</c>, when it's set.
    /// Otherwise, it's the ordinal at the end of the host name, e.g. 3 for the host name
    /// <c>gameserver-3</c>, which is the name of a pod of a Kubernetes StatefulSet.
    /// An offset can be added with the environment variable <c>GS_ID_OFFSET</c>, e.g. to
    /// run multiple StatefulSets for different ranges of game server ids.
    /// When neither is available, it's 0.
    /// </remarks>
    public static byte Determine()
    {
        return Determine(
            Environment.GetEnvironmentVariable("GS_ID"),
            Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.MachineName,
            Environment.GetEnvironmentVariable("GS_ID_OFFSET"));
    }

    /// <summary>
    /// Determines the id of the game server.
    /// </summary>
    /// <param name="gameServerId">The explicitly configured id of the game server.</param>
    /// <param name="hostName">The host name.</param>
    /// <param name="offset">The offset which is added to the ordinal of the host name.</param>
    /// <returns>The id of the game server.</returns>
    public static byte Determine(string? gameServerId, string? hostName, string? offset)
    {
        if (!string.IsNullOrWhiteSpace(gameServerId))
        {
            return byte.Parse(gameServerId, CultureInfo.InvariantCulture);
        }

        if (hostName is not null && OrdinalRegex().Match(hostName) is { Success: true } match)
        {
            var ordinal = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var offsetValue = string.IsNullOrWhiteSpace(offset) ? 0 : int.Parse(offset, CultureInfo.InvariantCulture);
            return checked((byte)(ordinal + offsetValue));
        }

        return 0;
    }

    [GeneratedRegex(@"-(\d+)$")]
    private static partial Regex OrdinalRegex();
}
