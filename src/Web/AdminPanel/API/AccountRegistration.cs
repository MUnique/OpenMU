// <copyright file="AccountRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The data of a new account, as sent to <see cref="AccountController.RegisterAsync"/>.
/// </summary>
/// <remarks>
/// The limits match what the game client can send when logging in: the login packet holds the
/// login name in 10 and the password in 20 bytes. Only printable ASCII characters are accepted,
/// because each of them takes exactly one byte - a longer UTF-8 encoded name or password would be
/// accepted here, but could never be used to log in.
/// </remarks>
public class AccountRegistration
{
    /// <summary>
    /// The minimum length of a login name and a password, like when creating an account in the admin panel.
    /// </summary>
    internal const int MinimumLength = 3;

    /// <summary>
    /// The maximum length of a login name, given by the login packet and the database column.
    /// </summary>
    internal const int LoginNameMaximumLength = 10;

    /// <summary>
    /// The maximum length of a password, given by the login packet of the newer clients.
    /// </summary>
    internal const int PasswordMaximumLength = 20;

    /// <summary>
    /// Gets or sets the login name. It may contain printable ASCII characters except the space.
    /// </summary>
    [Required]
    [MinLength(MinimumLength)]
    [MaxLength(LoginNameMaximumLength)]
    [RegularExpression(@"^[\x21-\x7E]+$")]
    public string LoginName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password. It may contain printable ASCII characters, including the space.
    /// </summary>
    [Required]
    [MinLength(MinimumLength)]
    [MaxLength(PasswordMaximumLength)]
    [RegularExpression(@"^[\x20-\x7E]+$")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional e-mail address.
    /// </summary>
    [EmailAddress]
    public string? EMail { get; set; }
}
