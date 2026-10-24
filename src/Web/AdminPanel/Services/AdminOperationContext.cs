// <copyright file="AdminOperationContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Resolves HTTP API callers and the current Blazor circuit user.</summary>
/// <param name="http">The HTTP context accessor.</param>
/// <param name="authentication">The circuit authentication state.</param>
public sealed class AdminOperationContext(IHttpContextAccessor http, AuthenticationStateProvider authentication) : IAdminOperationContext
{
    /// <inheritdoc />
    public async ValueTask<AdminActor> GetActorAsync(CancellationToken cancellationToken)
    {
        var request = http.HttpContext;
        var isMcp = request?.Request.Path.StartsWithSegments("/mcp") == true;
        var isApi = request?.Request.Path.StartsWithSegments("/api") == true;
        var principal = isMcp || isApi ? request!.User : (await authentication.GetAuthenticationStateAsync().ConfigureAwait(false)).User;

        // API keys carry their own roles; do not merge them with a simultaneous browser cookie.
        if (principal.Identities.FirstOrDefault(i => i.HasClaim(c => c.Type == ApiKeyAuthenticationDefaults.ClientIdClaimType)) is { } keyIdentity)
        {
            principal = new ClaimsPrincipal(keyIdentity);
        }

        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new AdminOperationException("An authenticated administrator or API client is required.", forbidden: true);
        }

        if (isMcp && !principal.HasClaim(c => c.Type == ApiKeyAuthenticationDefaults.ClientIdClaimType))
        {
            throw new AdminOperationException("MCP requires an API key identity.", forbidden: true);
        }

        var id = principal.FindFirst(ApiKeyAuthenticationDefaults.ClientIdClaimType)?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id))
        {
            throw new AdminOperationException("The caller has no stable identity.", forbidden: true);
        }

        return new AdminActor(
            id,
            principal.Identity.Name ?? id,
            isMcp ? "mcp" : isApi ? "api" : "panel",
            principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray());
    }
}
