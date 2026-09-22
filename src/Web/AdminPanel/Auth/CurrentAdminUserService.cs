// <copyright file="CurrentAdminUserService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Resolves the <see cref="AdminUser"/> which belongs to the currently authenticated principal.
/// </summary>
public class CurrentAdminUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly UserManager<AdminUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentAdminUserService"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    /// <param name="userManager">The user manager.</param>
    public CurrentAdminUserService(AuthenticationStateProvider authenticationStateProvider, UserManager<AdminUser> userManager)
    {
        this._authenticationStateProvider = authenticationStateProvider;
        this._userManager = userManager;
    }

    /// <summary>
    /// Gets the currently authenticated user.
    /// </summary>
    /// <returns>The currently authenticated user; <c>null</c>, if nobody is authenticated.</returns>
    public async Task<AdminUser?> GetCurrentUserAsync()
    {
        var state = await this._authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
        if (state.User.Identity?.IsAuthenticated is not true)
        {
            return null;
        }

        var userId = state.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null ? null : await this._userManager.FindByIdAsync(userId).ConfigureAwait(false);
    }
}
