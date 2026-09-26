// <copyright file="RoleCaption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>Localizes role labels without changing stored authorization identifiers.</summary>
public static class RoleCaption
{
    /// <summary>Gets the display label for a stored role or comma-separated role list.</summary>
    /// <param name="roles">The stored role identifiers.</param>
    /// <returns>The localized labels, preserving unknown identifiers.</returns>
    public static string Get(string roles) => string.Join(", ", roles.Split(',').Select(role => role.Trim() switch
    {
        "Administrator" => Resources.RoleAdministrator,
        "Operator" => Resources.RoleOperator,
        "Viewer" => Resources.RoleViewer,
        var other => other,
    }));
}
