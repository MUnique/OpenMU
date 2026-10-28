// <copyright file="AdminUserExistsRequirementHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Handles the <see cref="AdminUserExistsRequirement"/>.
/// </summary>
/// <param name="userAvailability">The service which knows whether the admin panel has a user.</param>
public sealed class AdminUserExistsRequirementHandler(AdminUserAvailabilityService userAvailability) : AuthorizationHandler<AdminUserExistsRequirement>
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminUserExistsRequirement requirement)
    {
        if (await userAvailability.AnyUserExistsAsync().ConfigureAwait(false))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "The admin panel has no user yet."));
        }
    }
}
