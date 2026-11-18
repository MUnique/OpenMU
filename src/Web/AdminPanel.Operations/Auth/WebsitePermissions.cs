// <copyright file="WebsitePermissions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The functions of the website API, which an administrator selects for an API key with the
/// <see cref="MUnique.OpenMU.Persistence.AdminAuth.AdminRoles.WebManager"/> role.
/// </summary>
/// <remarks>
/// The names are operation names, so they are stored in the allowlist of the key
/// (<see cref="MUnique.OpenMU.Persistence.AdminAuth.ApiKey.AllowedToolGroups"/>) and checked like the MCP tools.
/// A key of a website starts without any function: each one has to be selected explicitly, so
/// a stolen key of a website which only shows the server status can't do anything else.
/// </remarks>
public static class WebsitePermissions
{
    /// <summary>Gets the function which shows whether the server is online and how many players are online.</summary>
    public static string Status => "website_status";

    /// <summary>
    /// Gets all functions of the website API.
    /// </summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Status,
    ];
}
