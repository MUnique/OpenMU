// <copyright file="AdminPanelAuthOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The configuration of the admin panel authentication.
/// </summary>
public class AdminPanelAuthOptions
{
    /// <summary>
    /// Gets the name of the configuration section.
    /// </summary>
    public static string SectionName => "AdminPanel:Auth";

    /// <summary>
    /// Gets or sets a value indicating whether all users must set up a second factor before they can use the panel.
    /// </summary>
    public bool RequireTwoFactor { get; set; }

    /// <summary>
    /// Gets or sets the time after which an inactive session expires.
    /// </summary>
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(8);

    /// <summary>
    /// Gets or sets the number of failed login attempts after which a user is locked out.
    /// </summary>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the duration of a lockout.
    /// </summary>
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the bootstrap user which is available without a database.
    /// </summary>
    /// <remarks>
    /// The admin panel is the tool which creates the game database, so on a fresh installation
    /// there is no place to store a user yet. Configuring a bootstrap user closes the window in
    /// which the panel would be reachable without any authentication. It's also the way to get
    /// back in when the last stored user lost its second factor.
    /// </remarks>
    public BootstrapAdminUserOptions? BootstrapUser { get; set; }
}

/// <summary>
/// The configuration of the bootstrap user of the admin panel.
/// </summary>
public class BootstrapAdminUserOptions
{
    /// <summary>
    /// Gets or sets the login name.
    /// </summary>
    public string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password, in plain text.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base32 encoded TOTP secret of this user, if it should require a second factor.
    /// </summary>
    public string? AuthenticatorKey { get; set; }
}
