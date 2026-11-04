// <copyright file="PlayerPortalController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Private player account API. Every route requires the trusted Worker service identity.</summary>
[ApiController]
[Route("player-portal/v1")]
[Authorize(Policy = PlayerPortalApiDefaults.WorkerAuthorizationPolicy)]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PlayerPortalController(PlayerPortalService players, PlayerSessionStore sessions) : ControllerBase
{
    /// <summary>Creates a native OpenMU account without revealing whether the requested name was taken.</summary>
    /// <param name="registration">The game-compatible account registration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost("accounts")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountRegistration registration, CancellationToken cancellationToken)
    {
        var result = await players.RegisterAsync(registration, cancellationToken).ConfigureAwait(false);
        return result == AccountRegistrationStatus.Invalid
            ? this.BadRequest(new { error = "invalid_registration" })
            : this.Accepted(new { status = "accepted" });
    }

    /// <summary>Verifies native OpenMU credentials and issues a one-time opaque portal session token.</summary>
    /// <param name="request">The game-compatible login credentials.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost("sessions")]
    [ProducesResponseType(typeof(PlayerSessionDescriptor), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> LoginAsync([FromBody] PlayerLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await players.LoginAsync(request, cancellationToken).ConfigureAwait(false);
        return result.Status switch
        {
            PlayerLoginStatus.Authenticated => this.StatusCode(StatusCodes.Status201Created, result.Session),
            PlayerLoginStatus.RateLimited => this.TooManyRequests(new { error = "try_again_later" }, result.RetryAfter),
            PlayerLoginStatus.SessionCapacityReached => this.StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "temporarily_unavailable" }),
            _ => this.Unauthorized(new { error = "invalid_credentials" }),
        };
    }

    /// <summary>Returns the account and characters associated with the presented session only.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet("me")]
    [ProducesResponseType(typeof(PlayerAccountSnapshot), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOwnAccountAsync(CancellationToken cancellationToken)
    {
        var token = GetSessionToken(this.Request);
        if (!sessions.TryGetAccountId(token, out var accountId))
        {
            return this.Unauthorized(new { error = "invalid_session" });
        }

        var account = await players.GetOwnAccountAsync(accountId, cancellationToken).ConfigureAwait(false);
        if (account is null)
        {
            sessions.Revoke(token);
            return this.Unauthorized(new { error = "invalid_session" });
        }

        return this.Ok(account);
    }

    /// <summary>Revokes the presented session. The operation is idempotent to simplify Worker cookie clearing.</summary>
    [HttpDelete("sessions/current")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        sessions.Revoke(GetSessionToken(this.Request));
        return this.NoContent();
    }

    private static string? GetSessionToken(HttpRequest request)
        => request.Headers.TryGetValue(PlayerPortalApiDefaults.SessionHeaderName, out var values) && values.Count == 1
            ? values[0]
            : null;

    private ObjectResult TooManyRequests(object value, TimeSpan retryAfter)
    {
        this.Response.Headers.RetryAfter = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return this.StatusCode(StatusCodes.Status429TooManyRequests, value);
    }
}
