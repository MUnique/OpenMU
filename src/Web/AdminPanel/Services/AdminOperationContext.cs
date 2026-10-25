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
/// <param name="userAvailability">The same initial-setup gate used by panel authorization.</param>
/// <param name="circuit">The scoped panel circuit lifecycle.</param>
public sealed class AdminOperationContext(IHttpContextAccessor http, AuthenticationStateProvider authentication, AdminUserAvailabilityService userAvailability, AdminOperationCircuitState circuit) : IAdminOperationContext
{
    /// <inheritdoc />
    public async ValueTask<AdminActor> GetActorAsync(CancellationToken cancellationToken)
    {
        var request = http.HttpContext;
        var isMcp = request?.Request.Path.StartsWithSegments("/mcp") == true;
        var isApi = request?.Request.Path.StartsWithSegments("/api") == true;
        var isPanel = !isMcp && !isApi && circuit.IsOpen;
        if (!isMcp && !isApi && !isPanel)
        {
            throw new AdminOperationException("Admin operations require an open panel circuit or an API endpoint.", AdminOperationErrorKind.Forbidden);
        }

        var principal = isMcp || isApi ? request!.User : (await authentication.GetAuthenticationStateAsync().ConfigureAwait(false)).User;

        // API keys carry their own roles; do not merge them with a simultaneous browser cookie.
        if (principal.Identities.FirstOrDefault(i => i.HasClaim(c => c.Type == ApiKeyAuthenticationDefaults.ClientIdClaimType)) is { } keyIdentity)
        {
            principal = new ClaimsPrincipal(keyIdentity);
        }

        if (principal.Identity?.IsAuthenticated != true)
        {
            // Anonymous setup applies only to panel circuits, never to MCP or REST requests.
            if (isPanel && !await userAvailability.AnyUserExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                return new AdminActor("setup", "Initial setup", "panel", [MUnique.OpenMU.Persistence.AdminAuth.AdminRoles.Administrator]) { IsInitialSetup = true };
            }

            throw new AdminOperationException("An authenticated administrator or API client is required.", AdminOperationErrorKind.Forbidden);
        }

        if (isMcp && !principal.HasClaim(c => c.Type == ApiKeyAuthenticationDefaults.ClientIdClaimType))
        {
            throw new AdminOperationException("MCP requires an API key identity.", AdminOperationErrorKind.Forbidden);
        }

        var id = principal.FindFirst(ApiKeyAuthenticationDefaults.ClientIdClaimType)?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id))
        {
            throw new AdminOperationException("The caller has no stable identity.", AdminOperationErrorKind.Forbidden);
        }

        return new AdminActor(
            id,
            principal.Identity.Name ?? id,
            isMcp ? "mcp" : isApi ? "api" : "panel",
            principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray())
        {
            AllowedToolGroups = principal.FindFirst(AdminToolGroups.ClaimType)?.Value ?? string.Empty,
        };
    }
}
