// <copyright file="CashShopController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The public API to show and grant the cash shop coins of accounts, e.g. for a payment provider.
/// </summary>
[ApiController]
[Route("api/accounts/{loginName}/cash-shop")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.CashShop)]
public class CashShopController : ControllerBase
{
    private readonly CashShopCoinService _coinService;
    private readonly ILogger<CashShopController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopController"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration source.</param>
    /// <param name="logger">The logger.</param>
    public CashShopController(
        IPersistenceContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> gameConfigurationSource,
        ILogger<CashShopController> logger)
    {
        this._coinService = new CashShopCoinService(persistenceContextProvider, gameConfigurationSource);
        this._logger = logger;
    }

    /// <summary>
    /// Gets the cash shop coins of an account, with its latest grants.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The coins of the account; 404, if the account doesn't exist.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync(string loginName, CancellationToken cancellationToken)
    {
        var summary = await this._coinService.GetSummaryAsync(loginName, cancellationToken).ConfigureAwait(false);
        return summary is null ? this.NotFound() : this.Ok(summary);
    }

    /// <summary>
    /// Grants cash shop coins to an account. The game server applies them the next time the player opens the cash shop.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// 201 with the created grant;
    /// 200 with the existing grant, if the account already got a grant with the same reference;
    /// 400, if the request is invalid;
    /// 404, if the account doesn't exist;
    /// 409, if another account already got a grant with the same reference.
    /// </returns>
    [HttpPost("grants")]
    public async Task<IActionResult> GrantAsync(string loginName, [FromBody] CashShopCoinGrantRequest request, CancellationToken cancellationToken)
    {
        if (request.Amount is null or 0)
        {
            this.ModelState.AddModelError(nameof(request.Amount), "The amount must not be zero.");
        }

        if (request.CoinType is not { } requestedCoinType || !Enum.IsDefined(requestedCoinType))
        {
            this.ModelState.AddModelError(nameof(request.CoinType), "The coin type is unknown.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(new ValidationProblemDetails(this.ModelState));
        }

        var amount = request.Amount!.Value;
        var coinType = request.CoinType!.Value;

        var clientName = this.User.Identity?.Name;
        var (status, grant) = await this._coinService
            .GrantAsync(loginName, coinType, amount, request.Reason, request.Reference, clientName, cancellationToken)
            .ConfigureAwait(false);
        switch (status)
        {
            case CashShopCoinGrantStatus.Created:
                this._logger.LogInformation("API client {Client} granted {Amount} {CoinType} to account {LoginName}, reference {Reference}.", clientName, amount, coinType, loginName, request.Reference);
                return this.StatusCode(StatusCodes.Status201Created, grant);
            case CashShopCoinGrantStatus.AlreadyGranted:
                return this.Ok(grant);
            case CashShopCoinGrantStatus.AccountNotFound:
                return this.NotFound();
            default:
                return this.Conflict();
        }
    }
}
