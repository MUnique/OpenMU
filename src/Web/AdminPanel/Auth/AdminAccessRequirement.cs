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
