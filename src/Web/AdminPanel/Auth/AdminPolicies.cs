// <copyright file="AdminPolicies.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The authorization policies of the admin panel.
/// </summary>
public static class AdminPolicies
{
    /// <summary>
    /// The policy which requires the viewer role.
    /// </summary>
    public const string Viewer = "OpenMU.Viewer";

    /// <summary>
    /// The policy which requires the operator role.
    /// </summary>
    public const string Operator = "OpenMU.Operator";

    /// <summary>
    /// The policy which requires the administrator role.
    /// </summary>
    public const string Administrator = "OpenMU.Administrator";
}
