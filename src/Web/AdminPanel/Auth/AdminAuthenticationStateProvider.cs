// <copyright file="AdminAuthenticationStateProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;

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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AdminAuthenticationStateProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    public AdminAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        ILogger<AdminAuthenticationStateProvider> logger)
        : base(loggerFactory)
    {
        this._scopeFactory = scopeFactory;
        this._logger = logger;
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
        var principal = authenticationState.User;
        if (principal.Identity?.IsAuthenticated is not true)
        {
            return false;
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var securityStamp = principal.FindFirstValue(AdminAuthenticationDefaults.SecurityStampClaimType);
        if (!Guid.TryParse(userId, out var id) || securityStamp is null)
        {
            return false;
        }

        try
        {
            await using var scope = this._scopeFactory.CreateAsyncScope();
            var bootstrapUserProvider = scope.ServiceProvider.GetRequiredService<BootstrapAdminUserProvider>();
            AdminUser? user;
            if (bootstrapUserProvider.User is { } bootstrapUser && bootstrapUser.Id == id)
            {
                user = bootstrapUser;
            }
            else
            {
                var repository = scope.ServiceProvider.GetRequiredService<IAdminUserRepository>();
                user = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            }

            return user is { IsDisabled: false }
                   && string.Equals(user.SecurityStamp, securityStamp, StringComparison.Ordinal);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "The authentication state of an admin panel user couldn't be revalidated.");

            // Don't kick the user out just because the database hiccuped.
            return true;
        }
    }
}
