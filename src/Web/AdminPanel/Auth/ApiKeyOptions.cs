// <copyright file="ApiKeyOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The configuration of the API keys with which external applications authenticate themselves
/// at the public API of the server.
/// </summary>
/// <remarks>
/// The keys are configured, not stored in the database: the API has to work before the game
/// database exists, and an operator who can edit the configuration of the server can grant an
/// API key anyway. A key which is managed in the admin panel and stored as a hash would be the
/// next step, but it needs a schema of its own and a user interface.
/// </remarks>
public class ApiKeyOptions
{
    /// <summary>
    /// The name of the configuration section.
    /// </summary>
    public const string SectionName = "AdminPanel:Api";

    /// <summary>
    /// Gets or sets the configured API clients.
    /// </summary>
    public IList<ApiKeyEntry> Keys { get; set; } = new List<ApiKeyEntry>();
}

/// <summary>
/// One configured API client.
/// </summary>
public class ApiKeyEntry
{
    /// <summary>
    /// Gets or sets the name of the client, e.g. <c>launcher</c>. It's only used to tell the
    /// clients apart in the log, and to be able to revoke one of them.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key itself, in plain text.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the roles of this client as a comma separated list, e.g. <c>Viewer</c>.
    /// When it's not set, the client gets the least privileged role.
    /// </summary>
    public string? Roles { get; set; }
}
