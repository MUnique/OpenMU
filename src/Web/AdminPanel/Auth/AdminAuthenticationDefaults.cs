// <copyright file="AdminAuthenticationDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Constants of the admin panel authentication.
/// </summary>
public static class AdminAuthenticationDefaults
{
    /// <summary>
    /// The name of the authentication cookie.
    /// </summary>
    public const string CookieName = "OpenMU.AdminPanel.Auth";

    /// <summary>
    /// The path of the login page.
    /// </summary>
    public const string LoginPath = "/login";

    /// <summary>
    /// The path of the page which shows that the user is missing a permission.
    /// </summary>
    public const string AccessDeniedPath = "/access-denied";

    /// <summary>
    /// The path of the page at which a user manages its own second factor.
    /// </summary>
    public const string SecurityPath = "/account/security";

    /// <summary>
    /// The endpoint which turns a one time sign in ticket into an authentication cookie.
    /// </summary>
    public const string SignInEndpointPath = "/auth/complete";

    /// <summary>
    /// The endpoint which removes the authentication cookie.
    /// </summary>
    public const string SignOutEndpointPath = "/auth/logout";

    /// <summary>
    /// The path of the javascript module which talks to the sign in and sign out endpoints.
    /// </summary>
    public const string AuthScriptPath = "./_content/MUnique.OpenMU.Web.AdminPanel/js/auth.js";

    /// <summary>
    /// The claim type which holds the security stamp of the user, so sessions can be invalidated.
    /// </summary>
    public const string SecurityStampClaimType = "openmu:security-stamp";

    /// <summary>
    /// The claim type which describes how the user authenticated itself.
    /// </summary>
    public const string AuthenticationMethodClaimType = "amr";

    /// <summary>
    /// The value of the <see cref="AuthenticationMethodClaimType"/> when a second factor was used.
    /// </summary>
    public const string MultiFactorAuthenticationMethod = "mfa";

    /// <summary>
    /// The value of the <see cref="AuthenticationMethodClaimType"/> when only a password was used.
    /// </summary>
    public const string PasswordAuthenticationMethod = "pwd";
}
