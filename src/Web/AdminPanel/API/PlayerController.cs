// <copyright file="PlayerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Routes audited player operations through the owning game server.</summary>
/// <param name="operations">The shared player operations.</param>
[ApiController]
[Route("api/players")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = AdminPolicies.AuthenticatedOperator)]
public sealed class PlayerController(PlayerWriteOperations operations) : ControllerBase
{
    /// <summary>Disconnects an account's live session.</summary>
    /// <param name="loginName">The exact account login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether a session was disconnected.</returns>
    [HttpPost("{loginName}/disconnect")]
    public ValueTask<JsonObject> DisconnectAsync(string loginName, CancellationToken cancellationToken)
        => operations.DisconnectAsync(loginName, cancellationToken);
}
