// <copyright file="ReturnUrlValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Checks the return url of the login page, so that the user can't be sent to a foreign site
/// or to a script url after a successful login.
/// </summary>
public static class ReturnUrlValidator
{
    /// <summary>
    /// Gets the return url, if it's a relative url which stays at the origin of the admin panel.
    /// </summary>
    /// <param name="returnUrl">The requested return url.</param>
    /// <param name="baseUri">The absolute base uri of the admin panel, e.g. <c>https://host/admin/</c>.</param>
    /// <returns>
    /// The <paramref name="returnUrl"/>, if it's safe; Otherwise, an empty string.
    /// </returns>
    /// <remarks>
    /// The return url may point outside of the path base of the admin panel, e.g. to Grafana behind
    /// the same reverse proxy (see <see cref="ReverseProxyAuthenticationService.GetLoginUrl"/>).
    /// That's why only the origin is checked, and not the path.
    /// </remarks>
    public static string GetSafeReturnUrl(string? returnUrl, string baseUri)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return string.Empty;
        }

        // Browsers strip leading whitespace and remove tabs and line breaks anywhere in an url,
        // so e.g. "java\tscript:" would become "javascript:". Backslashes are treated like slashes.
        if (returnUrl.Any(c => char.IsWhiteSpace(c) || char.IsControl(c) || c == '\\'))
        {
            return string.Empty;
        }

        // An absolute path or protocol relative url ("//host") could leave the admin panel.
        if (returnUrl.StartsWith('/'))
        {
            return string.Empty;
        }

        // A colon before the first slash, query or fragment makes it an url with a scheme,
        // e.g. "https:", "javascript:" or "data:".
        var endOfFirstSegment = returnUrl.IndexOfAny(['/', '?', '#']);
        var firstSegment = endOfFirstSegment < 0 ? returnUrl : returnUrl[..endOfFirstSegment];
        if (firstSegment.Contains(':'))
        {
            return string.Empty;
        }

        if (!Uri.TryCreate(baseUri, UriKind.Absolute, out var baseUrl)
            || !Uri.TryCreate(baseUrl, returnUrl, out var resolvedUrl)
            || Uri.Compare(baseUrl, resolvedUrl, UriComponents.SchemeAndServer, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) != 0)
        {
            return string.Empty;
        }

        return returnUrl;
    }
}
