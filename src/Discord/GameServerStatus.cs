// <copyright file="GameServerStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The status of a game server.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
/// <param name="IsOnline">A value indicating whether the server is online.</param>
/// <param name="CurrentPlayers">The number of connected players.</param>
/// <param name="MaximumPlayers">The maximum number of players.</param>
public sealed record GameServerStatus(int Id, string Name, bool IsOnline, int CurrentPlayers, int MaximumPlayers);
