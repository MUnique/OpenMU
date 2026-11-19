// <copyright file="WebsiteController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The API for the website of a server, e.g. to show its status, register accounts and show rankings.
/// </summary>
/// <remarks>
/// It needs an API key with the <see cref="MUnique.OpenMU.Persistence.AdminAuth.AdminRoles.WebManager"/> role,
/// and each function has to be selected for the key in the admin panel. The keys of the administration
/// can't use it: a website should never hold a key which can do more than the website needs.
/// The caller is meant to be the server side of the website, which keeps the key to itself.
/// </remarks>
/// <param name="operations">The website operations.</param>
/// <param name="loginThrottle">The limit of the failed logins, which tells how long a lockout lasts.</param>
[ApiController]
[Route("api/website")]
[Authorize(Policy = AdminPolicies.ApiKeyWebManager)]
public sealed class WebsiteController(WebsiteOperations operations, WebsiteLoginThrottle loginThrottle) : ControllerBase
{
    /// <summary>
    /// Gets the state of the game servers and the number of players.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The state of the game servers.</returns>
    [HttpGet("status")]
    public ValueTask<WebsiteServerStatus> GetStatusAsync(CancellationToken cancellationToken)
        => operations.StatusAsync(cancellationToken);

    /// <summary>
    /// Registers a new account, which can log in right away.
    /// </summary>
    /// <param name="registration">The login name, password and e-mail address of the new account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>201</c> when the account was created, <c>400</c> when the registration is invalid,
    /// and <c>409</c> when the login name is already taken.
    /// </returns>
    [HttpPost("accounts")]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountRegistration registration, CancellationToken cancellationToken)
    {
        var created = await operations.RegisterAsync(registration.LoginName, registration.Password, registration.EMail, cancellationToken).ConfigureAwait(false);
        return created ? this.StatusCode(StatusCodes.Status201Created) : this.Conflict();
    }

    /// <summary>
    /// Gets the best characters, ranked by resets, master level and level.
    /// </summary>
    /// <param name="count">The number of characters, up to 100.</param>
    /// <param name="characterClass">The number of a character class, to rank only its line; empty for all classes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ranked characters.</returns>
    [HttpGet("ranking")]
    public ValueTask<IReadOnlyList<WebsiteRankingEntry>> GetRankingAsync([FromQuery] int count = 10, [FromQuery(Name = "class")] byte? characterClass = null, CancellationToken cancellationToken = default)
        => operations.RankingAsync(count, characterClass, cancellationToken);

    /// <summary>
    /// Gets the leaderboard of a mini game type, like Blood Castle, Devil Square or Chaos Castle.
    /// </summary>
    /// <param name="type">The invariant name of the mini game type, e.g. <c>BloodCastle</c>; empty for the first one which has a leaderboard.</param>
    /// <param name="days">The number of past days of which finished games are counted; empty for all time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The leaderboard, and the mini game types which have one.</returns>
    [HttpGet("minigames/ranking")]
    public ValueTask<WebsiteMiniGameRanking> GetMiniGameRankingAsync([FromQuery] MiniGameType? type = null, [FromQuery] int? days = null, CancellationToken cancellationToken = default)
        => operations.MiniGameRankingAsync(type, days, cancellationToken);

    /// <summary>
    /// Gets the next starts of the scheduled events, like mini games, invasions and the happy hour, per game server.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The events per started game server.</returns>
    [HttpGet("events")]
    public ValueTask<IReadOnlyList<WebsiteServerEvents>> GetEventsAsync(CancellationToken cancellationToken)
        => operations.EventsAsync(cancellationToken);

    /// <summary>
    /// Gets the public profile of a character, with its equipped items, e.g. for an armory.
    /// </summary>
    /// <param name="name">The exact name of the character.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The profile; <c>404</c>, if the character doesn't exist.</returns>
    [HttpGet("characters/{name}")]
    public async Task<IActionResult> GetCharacterAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            return this.Ok(await operations.CharacterAsync(name, cancellationToken).ConfigureAwait(false));
        }
        catch (AdminResourceNotFoundException)
        {
            return this.NotFound();
        }
    }

    /// <summary>
    /// Checks the login name and password of a game account, for a login at the website.
    /// </summary>
    /// <param name="request">The login name and password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>200</c> with the account and its characters, when the login succeeded;
    /// <c>401</c>, when the login name or the password is wrong;
    /// <c>429</c> with the remaining time of the lockout in <c>Retry-After</c>, when the account had too many failed logins recently.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] WebsiteLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await operations.LoginAsync(request.LoginName, request.Password, cancellationToken).ConfigureAwait(false);
        switch (result.Status)
        {
            case WebsiteLoginStatus.Succeeded:
                return this.Ok(result.Account);
            case WebsiteLoginStatus.LockedOut:
                this.SetRetryAfter(request.LoginName);
                return this.StatusCode(StatusCodes.Status429TooManyRequests, new { status = result.Status.ToString() });
            default:
                return this.StatusCode(StatusCodes.Status401Unauthorized, new { status = result.Status.ToString() });
        }
    }

    /// <summary>
    /// Changes the password of a game account, after checking its current password.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="request">The current and the new password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>204</c>, when the password has been changed; <c>400</c>, when the new password is invalid;
    /// <c>401</c>, when the login name or the current password is wrong; <c>409</c>, when the account is online
    /// (<c>AccountOnline</c>) or one of its characters is offline leveling (<c>OfflineLeveling</c>);
    /// <c>429</c> with the remaining time of the lockout in <c>Retry-After</c>, when the account had too many failed logins recently;
    /// <c>503</c>, when a game server can't tell whether a character of the account is offline leveling.
    /// </returns>
    [HttpPut("accounts/{loginName}/password")]
    public async Task<IActionResult> ChangePasswordAsync(string loginName, [FromBody] WebsitePasswordChangeRequest request, CancellationToken cancellationToken)
    {
        var status = await operations.ChangePasswordAsync(loginName, request.CurrentPassword, request.NewPassword, cancellationToken).ConfigureAwait(false);
        switch (status)
        {
            case WebsitePasswordChangeStatus.Changed:
                return this.NoContent();
            case WebsitePasswordChangeStatus.AccountOnline:
            case WebsitePasswordChangeStatus.OfflineLeveling:
                return this.Conflict(new { status = status.ToString() });
            case WebsitePasswordChangeStatus.GameServerUnavailable:
                return this.StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = status.ToString() });
            case WebsitePasswordChangeStatus.LockedOut:
                this.SetRetryAfter(loginName);
                return this.StatusCode(StatusCodes.Status429TooManyRequests, new { status = status.ToString() });
            default:
                return this.StatusCode(StatusCodes.Status401Unauthorized, new { status = status.ToString() });
        }
    }

    /// <summary>
    /// Grants cash shop coins to an account, e.g. after a payment on the website.
    /// The game server applies them the next time the player opens the cash shop.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="request">The request, with a positive amount and the unique reference of the payment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>201</c> with the created grant; <c>200</c> with the existing grant, if the account already got a grant with
    /// the same reference; <c>400</c>, if the request is invalid; <c>404</c>, if the account doesn't exist;
    /// <c>409</c>, if another account already got a grant with the same reference, or the account got one with the same
    /// reference, but another amount or coin type.
    /// </returns>
    [HttpPost("accounts/{loginName}/cash-shop/grants")]
    public async Task<IActionResult> GrantCashShopCoinsAsync(string loginName, [FromBody] CashShopCoinGrantRequest request, CancellationToken cancellationToken)
    {
        if (request.CoinType is not { } coinType || request.Amount is not { } amount || request.Reference is not { } reference)
        {
            return this.BadRequest();
        }

        var (status, grant) = await operations.GrantCashShopCoinsAsync(loginName, coinType, amount, request.Reason, reference, cancellationToken).ConfigureAwait(false);
        return status switch
        {
            CashShopCoinGrantStatus.Created => this.StatusCode(StatusCodes.Status201Created, grant),
            CashShopCoinGrantStatus.AlreadyGranted => this.Ok(grant),
            CashShopCoinGrantStatus.AccountNotFound => this.NotFound(),
            _ => this.Conflict(),
        };
    }

    /// <summary>
    /// Sets the <c>Retry-After</c> header of a locked out account to the remaining time of its lockout.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    private void SetRetryAfter(string loginName)
    {
        // Rounded up, so that the website doesn't retry a moment before the lockout ends.
        var seconds = (int)Math.Ceiling(loginThrottle.GetRetryAfter(loginName, DateTime.UtcNow).TotalSeconds);
        this.Response.Headers.RetryAfter = Math.Max(1, seconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
