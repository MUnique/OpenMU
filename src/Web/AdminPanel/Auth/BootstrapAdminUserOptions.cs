// <copyright file="BootstrapAdminUserOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

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
