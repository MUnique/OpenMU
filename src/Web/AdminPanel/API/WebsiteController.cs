// <copyright file="WebsiteController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;

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
[ApiController]
[Route("api/website")]
[Authorize(Policy = AdminPolicies.ApiKeyWebManager)]
public sealed class WebsiteController(WebsiteOperations operations) : ControllerBase
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
}
