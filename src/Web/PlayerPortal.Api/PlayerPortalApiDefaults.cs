// <copyright file="PlayerPortalApiDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>Constants for the private player portal API.</summary>
public static class PlayerPortalApiDefaults
{
    /// <summary>The authentication scheme used only between the Cloudflare Worker and this API.</summary>
    public const string WorkerAuthenticationScheme = "PlayerPortalWorker";

    /// <summary>The authorization policy required by all player portal API routes.</summary>
    public const string WorkerAuthorizationPolicy = "PlayerPortalWorkerOnly";

    /// <summary>The header carrying the opaque player session from the trusted Worker.</summary>
    public const string SessionHeaderName = "X-Player-Session";

    /// <summary>The configuration key holding the distinct Worker service credential.</summary>
    public const string WorkerTokenConfigurationKey = "PlayerPortalApi:WorkerToken";
}
