// <copyright file="CastleSiegeManagementGameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Identifies a game server available to the Castle Siege management screen.
/// </summary>
/// <param name="Id">The game-server identifier.</param>
/// <param name="Description">The server description.</param>
public sealed record CastleSiegeManagementGameServer(int Id, string Description);
