// <copyright file="BunitAdminAuthorization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Sets the authorization of a bunit context up, for components which check the role of the user.
/// </summary>
internal static class BunitAdminAuthorization
{
    /// <summary>
    /// Signs an administrator in, who fulfills all policies of the admin panel, and registers the
    /// <see cref="AdminPolicyGuard"/>.
    /// </summary>
    /// <param name="context">The bunit context.</param>
    public static void AddAdministratorAuthorization(this BunitContext context)
    {
        var authorization = context.AddAuthorization();
        authorization.SetAuthorized("admin");
        authorization.SetRoles(AdminRoles.Viewer, AdminRoles.Operator, AdminRoles.Administrator, AdminRoles.CashShop);
        authorization.SetPolicies(AdminPolicies.Viewer, AdminPolicies.Operator, AdminPolicies.Administrator, AdminPolicies.CashShop);
        context.Services.AddScoped<AdminPolicyGuard>();
    }
}
