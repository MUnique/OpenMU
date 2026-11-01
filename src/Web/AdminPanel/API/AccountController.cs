// <copyright file="AccountController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

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
public class AccountController : ControllerBase
{
    private readonly MUnique.OpenMU.Web.AdminPanel.Operations.AccountWriteOperations _operations;

    /// <summary>Initializes a new instance of the <see cref="AccountController"/> class.</summary>
    /// <param name="operations">The shared, audited account operations.</param>
    public AccountController(MUnique.OpenMU.Web.AdminPanel.Operations.AccountWriteOperations operations) => this._operations = operations;

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
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Policy = AdminPolicies.AuthenticatedOperator)]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountRegistration registration, CancellationToken cancellationToken)
    {
        var result = await this._operations.CreateAsync(registration.LoginName, registration.Password, registration.EMail, cancellationToken).ConfigureAwait(false);
        return result["status"]?.GetValue<string>() == "created" ? this.StatusCode(StatusCodes.Status201Created) : this.Conflict();
    }

    /// <summary>Reads the saved account snapshot without credentials.</summary>
    /// <param name="loginName">The exact account login name.</param>
    /// <param name="operations">The shared read operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe snapshot, or 404 when the account does not exist.</returns>
    [HttpGet("{loginName}")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Roles = nameof(AdminRole.Viewer))]
    public async ValueTask<IActionResult> GetAsync(string loginName, [FromServices] AccountReadOperations operations, CancellationToken cancellationToken)
    {
        try
        {
            return this.Ok(await operations.GetAsync(loginName, cancellationToken).ConfigureAwait(false));
        }
        catch (AdminResourceNotFoundException error)
        {
            return this.NotFound(new { error = error.Message });
        }
    }

    /// <summary>Changes an offline account's state through the audited operations layer.</summary>
    /// <param name="loginName">The exact account login name.</param>
    /// <param name="change">The state and audit reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The applied state.</returns>
    [HttpPut("{loginName}/state")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = AdminPolicies.AuthenticatedOperator)]
    public ValueTask<JsonObject> SetStateAsync(string loginName, [FromBody] AccountStateChange change, CancellationToken cancellationToken)
        => this._operations.SetStateAsync(loginName, change.State ?? throw new AdminOperationException("An account state is required.", AdminOperationErrorKind.InvalidInput), change.Reason, cancellationToken);

    /// <summary>Resets an offline account's password without returning the secret.</summary>
    /// <param name="loginName">The exact account login name.</param>
    /// <param name="change">The password and audit reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe password-reset status.</returns>
    [HttpPut("{loginName}/password")]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = AdminPolicies.AuthenticatedOperator)]
    public ValueTask<JsonObject> ResetPasswordAsync(string loginName, [FromBody] AccountPasswordChange change, CancellationToken cancellationToken)
        => this._operations.ResetPasswordAsync(loginName, change.Password, change.Reason, cancellationToken);
}
