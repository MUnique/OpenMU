// <copyright file="ReverseProxyAuthenticationService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Answers the authentication requests of a reverse proxy, so that other web applications
/// behind the same proxy can rely on the admin panel login, e.g. Grafana in the distributed deployment.
/// </summary>
/// <remarks>
/// The proxy (e.g. nginx with <c>auth_request</c>) asks for every request of the other application,
/// passing the cookies of the original request. A successful answer contains the login name and the
/// most privileged role of the user in the headers <see cref="UserHeaderName"/> and <see cref="RoleHeaderName"/>.
/// It follows the access rule of the admin panel itself: in the initial setup mode, when no user exists yet,
/// the panel is reachable without a login - and so is everything which relies on this answer.
/// </remarks>
public class ReverseProxyAuthenticationService
{
    /// <summary>
    /// The maximum number of remembered validations, to keep the memory bounded.
    /// </summary>
    private const int MaximumCachedValidations = 1000;

    /// <summary>
    /// The time for which a successful validation of a user is remembered. The proxy asks for every
    /// single request, so without it, every request would hit the database.
    /// </summary>
    private static readonly TimeSpan ValidationCacheDuration = TimeSpan.FromMinutes(1);

    private readonly AdminUserAvailabilityService _userAvailability;
    private readonly AdminPrincipalValidator _principalValidator;
    private readonly ConcurrentDictionary<(string UserId, string SecurityStamp), DateTime> _validUntil = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseProxyAuthenticationService"/> class.
    /// </summary>
    /// <param name="userAvailability">The user availability service.</param>
    /// <param name="principalValidator">The validator of the authenticated principals.</param>
    public ReverseProxyAuthenticationService(AdminUserAvailabilityService userAvailability, AdminPrincipalValidator principalValidator)
    {
        this._userAvailability = userAvailability;
        this._principalValidator = principalValidator;
    }

    /// <summary>
    /// Gets the name of the response header which contains the name of the authenticated user.
    /// </summary>
    public static string UserHeaderName => "X-OpenMU-User";

    /// <summary>
    /// Gets the name of the response header which contains the most privileged role of the authenticated user.
    /// </summary>
    public static string RoleHeaderName => "X-OpenMU-Role";

    /// <summary>
    /// Gets the user name which is used in the initial setup mode, when no user exists yet.
    /// </summary>
    public static string SetupUserName => "setup";

    /// <summary>
    /// Authenticates the principal of a request of the reverse proxy.
    /// </summary>
    /// <param name="principal">The principal, authenticated by the cookie of the original request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The answer for the reverse proxy.</returns>
    public async ValueTask<ReverseProxyAuthenticationResult> AuthenticateAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (!await this._userAvailability.AnyUserExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return new ReverseProxyAuthenticationResult(StatusCodes.Status204NoContent, SetupUserName, AdminRoles.Administrator);
        }

        if (!await this.IsValidAsync(principal, cancellationToken).ConfigureAwait(false))
        {
            return new ReverseProxyAuthenticationResult(StatusCodes.Status401Unauthorized);
        }

        if (AdminRoles.All.LastOrDefault(principal.IsInRole) is not { } role)
        {
            return new ReverseProxyAuthenticationResult(StatusCodes.Status403Forbidden);
        }

        return new ReverseProxyAuthenticationResult(StatusCodes.Status204NoContent, GetUserName(principal), role);
    }

    /// <summary>
    /// Gets a user name which can be transmitted in a header. Header values are restricted to ASCII,
    /// so the identifier of the user is used for any other name.
    /// </summary>
    private static string GetUserName(ClaimsPrincipal principal)
    {
        var name = principal.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(name) && name.All(c => c is >= ' ' and <= '~'))
        {
            return name;
        }

        return principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private async ValueTask<bool> IsValidAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (principal.Identity?.IsAuthenticated is not true
            || principal.FindFirstValue(ClaimTypes.NameIdentifier) is not { } userId
            || principal.FindFirstValue(AdminAuthenticationDefaults.SecurityStampClaimType) is not { } securityStamp)
        {
            return false;
        }

        var key = (userId, securityStamp);
        var now = DateTime.UtcNow;
        if (this._validUntil.TryGetValue(key, out var validUntil) && now < validUntil)
        {
            return true;
        }

        if (!await this._principalValidator.IsValidAsync(principal, cancellationToken).ConfigureAwait(false))
        {
            this._validUntil.TryRemove(key, out _);
            return false;
        }

        if (this._validUntil.Count >= MaximumCachedValidations)
        {
            this._validUntil.Clear();
        }

        this._validUntil[key] = now.Add(ValidationCacheDuration);
        return true;
    }
}
