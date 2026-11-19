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

    /// <summary>Gets the function which registers new game accounts.</summary>
    public static string Register => "website_register";

    /// <summary>Gets the function which shows the ranking of the characters.</summary>
    public static string Ranking => "website_ranking";

    /// <summary>Gets the function which shows the leaderboards of the mini games, like Blood Castle, Devil Square and Chaos Castle.</summary>
    public static string MiniGameRanking => "website_minigame_ranking";

    /// <summary>Gets the function which shows the schedule of the events, like mini games, invasions and the happy hour.</summary>
    public static string Events => "website_events";

    /// <summary>Gets the function which shows the public profile of a character, with its equipped items.</summary>
    public static string Character => "website_character";

    /// <summary>Gets the function which checks the password of a game account and lists its characters, for a login on the website.</summary>
    public static string Login => "website_login";

    /// <summary>
    /// Gets all functions of the website API.
    /// </summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Status,
        Register,
        Ranking,
        MiniGameRanking,
        Events,
        Character,
        Login,
    ];
}
