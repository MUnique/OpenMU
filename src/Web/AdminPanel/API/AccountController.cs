// <copyright file="AccountController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// API controller which creates accounts, e.g. for the registration form of a website.
/// </summary>
/// <remarks>
/// Creating an account changes data, so it needs the operator role, like editing an account in
/// the admin panel does. The caller is meant to be a server side application, which keeps the API
/// key to itself and protects its form against abuse (a captcha, a rate limit per visitor): this
/// server only sees that application, not the visitors behind it.
/// </remarks>
[ApiController]
[Route("api/accounts")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.Operator)]
public class AccountController : ControllerBase
{
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The source of the game configuration.</param>
    /// <param name="logger">The logger.</param>
    public AccountController(
        IPersistenceContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> gameConfigurationSource,
        ILogger<AccountController> logger)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._gameConfigurationSource = gameConfigurationSource;
        this._logger = logger;
    }

    /// <summary>
    /// Registers a new account, which can log in right away.
    /// </summary>
    /// <param name="registration">The login name, password and e-mail address of the new account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>201</c> when the account was created, <c>400</c> with the invalid fields when the
    /// registration is invalid, and <c>409</c> when the login name is already taken.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountRegistration registration, CancellationToken cancellationToken)
    {
        var gameConfiguration = await this._gameConfigurationSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        using var context = this._persistenceContextProvider.CreateNewPlayerContext(gameConfiguration);
        if (await context.GetAccountByLoginNameAsync(registration.LoginName, cancellationToken).ConfigureAwait(false) is not null)
        {
            return this.Conflict();
        }

        var account = context.CreateNew<Account>();
        account.LoginName = registration.LoginName;
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registration.Password);
        account.EMail = registration.EMail ?? string.Empty;
        account.State = AccountState.Normal;
        account.RegistrationDate = DateTime.UtcNow;

        try
        {
            if (!await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        catch
        {
            // Another request may have registered the same name between the check above and this
            // save, in which case the unique login name rejects this one.
            if (await this.IsRegisteredAsync(registration.LoginName, gameConfiguration, cancellationToken).ConfigureAwait(false))
            {
                return this.Conflict();
            }

            throw;
        }

        this._logger.LogInformation("Account {LoginName} was registered by API client {Client}.", account.LoginName, this.User.Identity?.Name);
        return this.StatusCode(StatusCodes.Status201Created);
    }

    private async ValueTask<bool> IsRegisteredAsync(string loginName, GameConfiguration gameConfiguration, CancellationToken cancellationToken)
    {
        using var context = this._persistenceContextProvider.CreateNewPlayerContext(gameConfiguration);
        return await context.GetAccountByLoginNameAsync(loginName, cancellationToken).ConfigureAwait(false) is not null;
    }
}
