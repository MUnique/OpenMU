// <copyright file="AdminPrincipalValidator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Checks if the user of an authenticated admin panel principal still exists, isn't disabled
/// and wasn't changed in a security relevant way since the principal was issued.
/// </summary>
/// <remarks>
/// A cookie stays valid until it expires, so this check is what ends a session early,
/// e.g. after a user got disabled or changed its password.
/// </remarks>
public class AdminPrincipalValidator
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AdminPrincipalValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPrincipalValidator"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    public AdminPrincipalValidator(IServiceScopeFactory scopeFactory, ILogger<AdminPrincipalValidator> logger)
    {
        this._scopeFactory = scopeFactory;
        this._logger = logger;
    }

    /// <summary>
    /// Determines whether the principal is authenticated and its user is still valid.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if the principal is still valid; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> IsValidAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
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
