// <copyright file="PlayerPortalWorkerCredential.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>The configured Worker-only API credential; it is distinct from all OpenMU admin keys.</summary>
public sealed record PlayerPortalWorkerCredential(string Value);
