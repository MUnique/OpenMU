// <copyright file="WebsiteGameServerStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The public state of a game server.
/// </summary>
/// <param name="Id">The identifier of the game server.</param>
/// <param name="Name">The name of the game server, as the players see it in the server list.</param>
/// <param name="IsOnline">A value indicating whether the game server is started.</param>
/// <param name="Players">The number of connected players.</param>
/// <param name="MaximumPlayers">The maximum number of players.</param>
public sealed record WebsiteGameServerStatus(int Id, string Name, bool IsOnline, int Players, int MaximumPlayers);
