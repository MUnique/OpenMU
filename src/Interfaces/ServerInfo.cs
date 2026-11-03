// <copyright file="ServerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The state info about a server.
/// </summary>
public record ServerInfo(ushort Id, string Description, int CurrentConnections, int MaximumConnections)
{
    /// <summary>
    /// Gets or sets the count of current connections.
    /// </summary>
    /// <remarks>
    /// For a game server, these are the logged in players, which are limited by <see cref="MaximumConnections"/>.
    /// </remarks>
    public int CurrentConnections { get; set; } = CurrentConnections;
}