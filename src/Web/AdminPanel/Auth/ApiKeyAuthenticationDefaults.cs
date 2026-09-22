// <copyright file="ApiKeyAuthenticationDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Authentication.Cookies;

/// <summary>
/// Constants of the API key authentication.
/// </summary>
public static class ApiKeyAuthenticationDefaults
{
    /// <summary>
    /// Gets the name of the authentication scheme.
    /// </summary>
    internal const string AuthenticationScheme = "OpenMU.ApiKey";

    /// <summary>
    /// Gets the request header which carries the API key.
    /// </summary>
    public static string HeaderName => "X-Api-Key";

    /// <summary>
    /// Gets the scheme of the <c>Authorization</c> header which carries the API key as an alternative
    /// to the <see cref="HeaderName"/> header.
    /// </summary>
    public static string AuthorizationHeaderScheme => "Bearer";

    /// <summary>
    /// Gets the claim type which holds the configured name of the API client.
    /// </summary>
    public static string ClientNameClaimType => "openmu:api-client";

    /// <summary>
    /// Gets the path prefix of the public API. Requests below it get a status code instead of a
    /// redirect to the login page when they are not authenticated.
    /// </summary>
    public static string ApiPathPrefix => "/api";

    /// <summary>
    /// Gets the authentication schemes which are accepted by the public API: an API key for external
    /// applications, and the cookie of the admin panel, so a logged in user can use it as well.
    /// </summary>
    internal const string ApiSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + AuthenticationScheme;

    /// <summary>
    /// Gets the minimum length of an API key.
    /// </summary>
    /// <remarks>
    /// The key is a bearer credential which is sent with every request, so it has to have enough
    /// entropy to make guessing it pointless. 32 characters are what a base64 encoded 24 byte
    /// random value takes.
    /// </remarks>
    public static int MinimumKeyLength => 32;
}
