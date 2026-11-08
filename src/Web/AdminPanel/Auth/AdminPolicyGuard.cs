// <copyright file="AdminPolicyGuard.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

/// <summary>
/// Checks an authorization policy for the user of the current circuit, before a page writes
/// directly through its persistence context.
/// </summary>
/// <remarks>
/// The pages which write directly, without an admin operation, are already restricted by their
/// <see cref="AuthorizeAttribute"/>. This is the second line of defense, so that a component which
/// is reused on a less restricted page can't write anyway.
/// </remarks>
public class AdminPolicyGuard
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPolicyGuard"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    /// <param name="authorizationService">The authorization service.</param>
    public AdminPolicyGuard(AuthenticationStateProvider authenticationStateProvider, IAuthorizationService authorizationService)
    {
        this._authenticationStateProvider = authenticationStateProvider;
        this._authorizationService = authorizationService;
    }

    /// <summary>
    /// Determines whether the current user fulfills the specified policy.
    /// </summary>
    /// <param name="policyName">The name of the policy, see <see cref="AdminPolicies"/>.</param>
    /// <returns><c>true</c>, if the current user fulfills the policy; Otherwise, <c>false</c>.</returns>
    public async Task<bool> IsAuthorizedAsync(string policyName)
    {
        var state = await this._authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
        var result = await this._authorizationService.AuthorizeAsync(state.User, policyName).ConfigureAwait(false);
        return result.Succeeded;
    }

    /// <summary>
    /// Makes sure that the current user fulfills the specified policy.
    /// </summary>
    /// <param name="policyName">The name of the policy, see <see cref="AdminPolicies"/>.</param>
    /// <exception cref="UnauthorizedAccessException">The current user doesn't fulfill the policy.</exception>
    public async Task EnsureAuthorizedAsync(string policyName)
    {
        if (!await this.IsAuthorizedAsync(policyName).ConfigureAwait(false))
        {
            throw new UnauthorizedAccessException($"The current user doesn't fulfill the authorization policy '{policyName}'.");
        }
    }
}
