// <copyright file="AdminRoutes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// Centralized route templates of the admin panel. Pages and link components
/// should build their hrefs through these helpers instead of duplicating route strings.
/// </summary>
public static class AdminRoutes
{
    /// <summary>
    /// Gets the route of the guild detail page.
    /// </summary>
    /// <param name="guildId">The persistent identifier of the guild.</param>
    /// <returns>The route of the guild detail page.</returns>
    public static string Guild(Guid guildId) => $"guild/{guildId}";

    /// <summary>
    /// Gets the route of the alliance page.
    /// </summary>
    /// <param name="guildId">The persistent identifier of the alliance master guild.</param>
    /// <returns>The route of the alliance page.</returns>
    public static string Alliance(Guid guildId) => $"alliance/{guildId}";
}
