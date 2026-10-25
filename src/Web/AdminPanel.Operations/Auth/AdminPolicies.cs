// <copyright file="AdminPolicies.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The authorization policies of the admin panel.
/// </summary>
internal static class AdminPolicies
{
    /// <summary>The API-key-only policy of the MCP endpoint.</summary>
    internal const string McpAccess = "OpenMU.McpAccess";

    /// <summary>
    /// Gets the policy which requires the viewer role.
    /// </summary>
    internal const string Viewer = "OpenMU.Viewer";

    /// <summary>
    /// Gets the  policy which requires the operator role.
    /// </summary>
    internal const string Operator = "OpenMU.Operator";

    /// <summary>
    /// Gets the policy which requires the administrator role.
    /// </summary>
    internal const string Administrator = "OpenMU.Administrator";

    /// <summary>
    /// The policy which requires the <see cref="MUnique.OpenMU.Persistence.AdminAuth.AdminRoles.CashShop"/> role, to grant cash shop coins.
    /// </summary>
    internal const string CashShop = "OpenMU.CashShop";

    /// <summary>Authenticated operational API callers; initial setup anonymity is not accepted.</summary>
    internal const string AuthenticatedOperator = "Admin.AuthenticatedOperator";

    /// <summary>Authenticated cash shop API callers.</summary>
    internal const string AuthenticatedCashShop = "Admin.AuthenticatedCashShop";
}
