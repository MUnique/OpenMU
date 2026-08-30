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
    /// Gets the name of the authentication cookie.
    /// </summary>
    public static string CookieName => "OpenMU.AdminPanel.Auth";

    /// <summary>
    /// Gets the path of the login page.
    /// </summary>
    public static string LoginPath => "/login";

    /// <summary>
    /// Gets the path of the page which shows that the user is missing a permission.
    /// </summary>
    public static string AccessDeniedPath => "/access-denied";

    /// <summary>
    /// Gets the path of the page at which a user manages its own second factor.
    /// </summary>
    public static string SecurityPath => "/account/security";

    /// <summary>
    /// Gets the endpoint which turns a one time sign in ticket into an authentication cookie.
    /// </summary>
    public static string SignInEndpointPath => "/auth/complete";

    /// <summary>
    /// Gets the endpoint which removes the authentication cookie.
    /// </summary>
    public static string SignOutEndpointPath => "/auth/logout";

    /// <summary>
    /// Gets the path of the javascript module which talks to the sign in and sign out endpoints.
    /// </summary>
    public static string AuthScriptPath => "./_content/MUnique.OpenMU.Web.AdminPanel/js/auth.js";

    /// <summary>
    /// Gets the claim type which holds the security stamp of the user, so sessions can be invalidated.
    /// </summary>
    public static string SecurityStampClaimType => "openmu:security-stamp";

    /// <summary>
    /// Gets the claim type which describes how the user authenticated itself.
    /// </summary>
    public static string AuthenticationMethodClaimType => "amr";

    /// <summary>
    /// Gets the value of the <see cref="AuthenticationMethodClaimType"/> when a second factor was used.
    /// </summary>
    public static string MultiFactorAuthenticationMethod => "mfa";

    /// <summary>
    /// Gets the value of the <see cref="AuthenticationMethodClaimType"/> when only a password was used.
    /// </summary>
    public static string PasswordAuthenticationMethod => "pwd";
}
