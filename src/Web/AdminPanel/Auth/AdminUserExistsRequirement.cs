// <copyright file="AdminUserExistsRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Requires that the admin panel has a user.
/// </summary>
/// <remarks>
/// Without a user, the admin panel is in its setup mode, in which <see cref="AdminAccessRequirement"/>
/// lets every request through, including the page which creates API keys.
/// An endpoint with this requirement refuses requests in that state.
/// </remarks>
public sealed record AdminUserExistsRequirement : IAuthorizationRequirement;
