// <copyright file="AdminAccessRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// The requirement to access the admin panel, optionally with a specific role.
/// </summary>
/// <param name="RequiredRole">The role which is required; <c>null</c>, if any authenticated user is allowed.</param>
public record AdminAccessRequirement(string? RequiredRole = null) : IAuthorizationRequirement;

/// <summary>
/// Handles the <see cref="AdminAccessRequirement"/>.
/// </summary>
/// <remarks>
/// As long as no user exists at all, the panel has to stay reachable: it's the tool which creates
/// the database and therefore the first user. That initial setup mode ends as soon as the first
/// user exists, or immediately when a bootstrap user is configured.
/// </remarks>
public class AdminAccessRequirementHandler : AuthorizationHandler<AdminAccessRequirement>
{
    private readonly AdminUserAvailabilityService _userAvailability;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAccessRequirementHandler"/> class.
    /// </summary>
    /// <param name="userAvailability">The service which knows whether any user exists.</param>
    public AdminAccessRequirementHandler(AdminUserAvailabilityService userAvailability)
    {
        this._userAvailability = userAvailability;
    }

    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAccessRequirement requirement)
    {
        if (!await this._userAvailability.AnyUserExistsAsync().ConfigureAwait(false))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.Identity?.IsAuthenticated is not true)
        {
            return;
        }

        if (requirement.RequiredRole is null || context.User.IsInRole(requirement.RequiredRole))
        {
            context.Succeed(requirement);
        }
    }
}
