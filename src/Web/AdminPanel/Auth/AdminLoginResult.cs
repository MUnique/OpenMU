// <copyright file="AdminLoginResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;

/// <summary>
/// The result of a login attempt.
/// </summary>
/// <param name="Status">The status.</param>
/// <param name="Ticket">The sign in ticket, in case the login succeeded.</param>
/// <param name="Claims">The claims of the authenticated user, in case the login succeeded.</param>
public record AdminLoginResult(AdminLoginStatus Status, string? Ticket = null, IReadOnlyList<Claim>? Claims = null);
