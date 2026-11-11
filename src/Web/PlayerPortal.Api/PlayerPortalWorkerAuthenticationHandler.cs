// <copyright file="PlayerPortalWorkerAuthenticationHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

/// <summary>Authenticates only the trusted Cloudflare Worker service identity.</summary>
public sealed class PlayerPortalWorkerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly byte[] _expectedToken;

    /// <summary>Initializes a new instance of the <see cref="PlayerPortalWorkerAuthenticationHandler"/> class.</summary>
    /// <param name="options">The authentication scheme options monitor.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="credential">The configured Worker service credential.</param>
    public PlayerPortalWorkerAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        PlayerPortalWorkerCredential credential)
        : base(options, logger, encoder)
    {
        this._expectedToken = Encoding.UTF8.GetBytes(credential.Value);
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = this.Request.Headers[HeaderNames.Authorization];
        if (authorization.Count != 1 || !AuthenticationHeaderValue.TryParse(authorization[0], out var parsed)
            || !string.Equals(parsed.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrEmpty(parsed.Parameter))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var candidate = Encoding.UTF8.GetBytes(parsed.Parameter);
        if (candidate.Length != this._expectedToken.Length || !CryptographicOperations.FixedTimeEquals(candidate, this._expectedToken))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid service credentials."));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "cloudflare-worker")], this.Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name)));
    }

    /// <inheritdoc />
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        this.Response.StatusCode = StatusCodes.Status401Unauthorized;
        this.Response.Headers.Append(HeaderNames.WWWAuthenticate, "Bearer");
        return Task.CompletedTask;
    }
}
