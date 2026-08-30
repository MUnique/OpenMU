// <copyright file="ApiKeyAuthenticationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

/// <summary>
/// Authenticates a request by the API key in its <see cref="ApiKeyAuthenticationDefaults.HeaderName"/>
/// header, or in its <c>Authorization</c> header with the <c>Bearer</c> scheme.
/// </summary>
/// <remarks>
/// A request without a key is not a failure, but simply not authenticated by this scheme, so the
/// cookie of the admin panel still gets its chance to authenticate the same request.
/// </remarks>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApiKeyRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The url encoder.</param>
    /// <param name="registry">The registry of the configured API keys.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApiKeyRegistry registry)
        : base(options, logger, encoder)
    {
        this._registry = registry;
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!this.TryGetPresentedKey(out var presentedKey))
        {
            return AuthenticateResult.NoResult();
        }

        var client = await this._registry.FindAsync(presentedKey, this.Context.RequestAborted).ConfigureAwait(false);
        if (client is null)
        {
            // The key itself is never logged: it's a credential, and the log is readable in the panel.
            this.Logger.LogWarning(
                "Rejected an API request from {RemoteIpAddress} because its API key is unknown.",
                this.Context.Connection.RemoteIpAddress);
            return AuthenticateResult.Fail("The presented API key is unknown.");
        }

        var identity = new ClaimsIdentity(
            client.CreateClaims(),
            ApiKeyAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    /// <inheritdoc />
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        this.Response.StatusCode = StatusCodes.Status401Unauthorized;
        this.Response.Headers.Append(HeaderNames.WWWAuthenticate, ApiKeyAuthenticationDefaults.AuthorizationHeaderScheme);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        this.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    private bool TryGetPresentedKey(out string presentedKey)
    {
        presentedKey = string.Empty;
        if (this.Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var apiKeyHeader)
            && apiKeyHeader.Count > 0
            && !string.IsNullOrWhiteSpace(apiKeyHeader[0]))
        {
            presentedKey = apiKeyHeader[0]!.Trim();
            return true;
        }

        if (this.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader)
            && authorizationHeader.Count > 0
            && authorizationHeader[0] is { } authorization
            && authorization.StartsWith(ApiKeyAuthenticationDefaults.AuthorizationHeaderScheme + " ", StringComparison.OrdinalIgnoreCase))
        {
            presentedKey = authorization[(ApiKeyAuthenticationDefaults.AuthorizationHeaderScheme.Length + 1)..].Trim();
            return presentedKey.Length > 0;
        }

        return false;
    }
}
