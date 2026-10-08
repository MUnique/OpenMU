// <copyright file="ApiKeyRole.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// The roles which can be given to an API key.
/// The names are stored as <see cref="ApiKey.Roles"/>.
/// </summary>
public enum ApiKeyRole
{
    /// <summary>
    /// The <see cref="AdminRoles.Viewer"/> role.
    /// </summary>
    Viewer,

    /// <summary>
    /// The <see cref="AdminRoles.Operator"/> role.
    /// </summary>
    Operator,

    /// <summary>
    /// The <see cref="AdminRoles.Administrator"/> role.
    /// </summary>
    Administrator,

    /// <summary>
    /// The <see cref="AdminRoles.CashShop"/> role, which only allows to grant cash shop coins.
    /// </summary>
    CashShop,
}
