// <copyright file="WebsiteServerEvents.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The scheduled events of a game server, as a website shows them.
/// </summary>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="ServerName">The name of the game server.</param>
/// <param name="Events">The events, ordered by their next start.</param>
public sealed record WebsiteServerEvents(int ServerId, string ServerName, IReadOnlyList<WebsiteEvent> Events);
