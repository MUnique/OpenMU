// <copyright file="AdminPolicies.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The authorization policies of the admin panel.
/// </summary>
internal static class AdminPolicies
{
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
}
