// <copyright file="ThemeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// A controller which writes the UI theme preference to a cookie and redirects to the specified uri.
/// </summary>
[Route("[controller]/[action]")]
public class ThemeController : Controller
{
    /// <summary>
    /// The name of the cookie that stores the selected theme.
    /// </summary>
    public const string CookieName = "OpenMU.Theme";

    /// <summary>
    /// The default theme used when no cookie is present.
    /// </summary>
    public const string DefaultTheme = "light";

    /// <summary>
    /// Sets the UI theme by writing the theme cookie and redirects to the specified URI.
    /// </summary>
    /// <param name="theme">The theme to set, e.g. &quot;light&quot; or &quot;dark&quot;.</param>
    /// <param name="redirectUri">The URI to redirect to after the cookie has been set.</param>
    /// <returns>A local redirect to <paramref name="redirectUri"/>.</returns>
    public IActionResult Set(string? theme, string redirectUri)
    {
        var normalized = NormalizeTheme(theme);
        this.HttpContext.Response.Cookies.Append(
            CookieName,
            normalized,
            new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
            });

        return this.LocalRedirect(redirectUri);
    }

    /// <summary>
    /// Normalizes the supplied theme value to a known token.
    /// </summary>
    /// <param name="theme">The raw value from the request.</param>
    /// <returns>Either &quot;dark&quot; or &quot;light&quot;.</returns>
    public static string NormalizeTheme(string? theme)
    {
        return string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase) ? "dark" : "light";
    }
}
