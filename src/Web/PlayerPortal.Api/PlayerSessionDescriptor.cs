// <copyright file="PlayerSessionDescriptor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>The one-time session token returned to the trusted Worker after successful login.</summary>
public sealed record PlayerSessionDescriptor(string Token, DateTimeOffset ExpiresAt);
