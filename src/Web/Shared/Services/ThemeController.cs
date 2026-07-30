// <copyright file="ThemeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// A controller that writes the UI theme preference to a cookie and redirects to the specified uri.
/// </summary>
[Route("[controller]/[action]")]
public class ThemeController : Controller
{
    /// <summary>
    /// Gets the name of the cookie that stores the selected theme.
    /// </summary>
    public static string CookieName { get; } = "OpenMU.Theme";

    /// <summary>
    /// Gets the default theme used when no cookie is present.
    /// </summary>
    public static string DefaultTheme { get; } = "light";

    /// <summary>
    /// Normalizes the supplied theme value to a known token.
    /// </summary>
    /// <param name="theme">The raw value from the request.</param>
    /// <returns>Either dark or light.</returns>
    public static string NormalizeTheme(string? theme)
    {
        return string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase) ? "dark" : "light";
    }

    /// <summary>
    /// Sets the UI theme by writing the theme cookie and redirects to the specified URI.
    /// </summary>
    /// <param name="theme">The theme to set, e.g. dark or light.</param>
    /// <param name="redirectUri">The URI to redirect to after the cookie has been set. Must be a local URL; otherwise the user is sent to the application root.</param>
    /// <returns>A local redirect to <paramref name="redirectUri"/>, or to &quot;/&quot; if the URI is missing or non-local.</returns>
    public IActionResult Set(string? theme, string? redirectUri)
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

        // LocalRedirect throws InvalidOperationException (-> 500) on empty / non-local URIs.
        // Fall back to the app root so a malformed request never crashes the response.
        if (string.IsNullOrEmpty(redirectUri) || !this.Url.IsLocalUrl(redirectUri))
        {
            return this.LocalRedirect("/");
        }

        return this.LocalRedirect(redirectUri);
    }
}
