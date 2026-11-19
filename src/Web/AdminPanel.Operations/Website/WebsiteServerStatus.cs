// <copyright file="WebsiteServerStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The public state of the game servers, as a website shows it.
/// </summary>
/// <param name="Players">The number of players on all game servers.</param>
/// <param name="Servers">The game servers.</param>
public sealed record WebsiteServerStatus(int Players, IReadOnlyList<WebsiteGameServerStatus> Servers);
