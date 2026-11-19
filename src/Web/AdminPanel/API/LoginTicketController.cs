// <copyright file="LoginTicketController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.LoginTickets;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The public API to issue login tickets, e.g. for a website which signs the players in with Discord.
/// The website hands the token to the game client, which logs in with it instead of a password.
/// </summary>
/// <remarks>
/// The lifetime of the tickets can be configured with <c>AdminPanel:LoginTickets:Lifetime</c>, e.g. <c>00:01:30</c>.
/// </remarks>
[ApiController]
[Route("api/accounts/{loginName}/login-tickets")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.AuthenticatedLoginTicket)]
public class LoginTicketController : ControllerBase
{
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly LoginTicketService _ticketService;
    private readonly ILogger<LoginTicketController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginTicketController"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public LoginTicketController(IPersistenceContextProvider persistenceContextProvider, IConfiguration configuration, ILogger<LoginTicketController> logger)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._ticketService = new LoginTicketService(
            this.CreateContext,
            configuration.GetValue<TimeSpan?>("AdminPanel:LoginTickets:Lifetime"));
        this._logger = logger;
    }

    /// <summary>
    /// Issues a login ticket for an account. Its token is only returned here, it's not stored.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// 201 with the token and its expiration;
    /// 404, if the account doesn't exist;
    /// 429, if the account got too many tickets recently.
    /// </returns>
    [HttpPost]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> IssueAsync(string loginName, CancellationToken cancellationToken)
    {
        Guid? accountId;
        using (var context = this.CreateContext())
        {
            accountId = await context.GetAccountIdByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false);
        }

        if (accountId is not { } id)
        {
            return this.NotFound();
        }

        if (await this._ticketService.IssueAsync(id, cancellationToken).ConfigureAwait(false) is not { } ticket)
        {
            this._logger.LogWarning("API client {Client} requested too many login tickets for account {LoginName}.", this.User.Identity?.Name, loginName);
            return this.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        this._logger.LogInformation("API client {Client} issued a login ticket for account {LoginName}, valid until {ExpiresAt:O}.", this.User.Identity?.Name, loginName, ticket.ExpiresAt);
        return this.StatusCode(StatusCodes.Status201Created, ticket);
    }

    private IPlayerContext CreateContext() => this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration());
}
