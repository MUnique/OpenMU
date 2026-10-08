// <copyright file="ReverseProxyAuthenticationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The answer to an authentication request of a reverse proxy.
/// </summary>
/// <param name="StatusCode">The http status code of the answer.</param>
/// <param name="UserName">The name of the authenticated user, if the request is authorized.</param>
/// <param name="Role">The most privileged role of the authenticated user, if the request is authorized.</param>
public record ReverseProxyAuthenticationResult(int StatusCode, string? UserName = null, string? Role = null);
