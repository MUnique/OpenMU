// <copyright file="AdminAuthenticationStateProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;

/// <summary>
/// The authentication state provider of the admin panel.
/// </summary>
/// <remarks>
/// Besides the periodic revalidation, it allows to change the authentication state from within
/// the circuit. That's what makes the login work without a page reload: after the browser
/// exchanged its sign in ticket for a cookie, the new state is pushed into the running circuit
/// and every <see cref="AuthorizeView"/> re-renders in place.
/// </remarks>
public class AdminAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly AdminPrincipalValidator _principalValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="principalValidator">The validator of the authenticated principals.</param>
    public AdminAuthenticationStateProvider(ILoggerFactory loggerFactory, AdminPrincipalValidator principalValidator)
        : base(loggerFactory)
    {
        this._principalValidator = principalValidator;
    }

    /// <inheritdoc />
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(15);

    /// <summary>
    /// Applies the specified claims as the new authentication state of this circuit.
    /// </summary>
    /// <param name="claims">The claims of the now authenticated user.</param>
    public void NotifySignedIn(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
        this.SetAuthenticationState(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    /// <summary>
    /// Applies an anonymous authentication state to this circuit.
    /// </summary>
    public void NotifySignedOut()
    {
        this.SetAuthenticationState(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }

    /// <inheritdoc />
    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        return await this._principalValidator.IsValidAsync(authenticationState.User, cancellationToken).ConfigureAwait(false);
    }
}
