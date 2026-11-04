// <copyright file="ApiKeyClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;

/// <summary>
/// An external application which is allowed to use the public API.
/// </summary>
/// <param name="Id">The persistent identifier of the API key.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Roles">The effective roles of the client.</param>
public record ApiKeyClient(Guid Id, string Name, IReadOnlyList<string> Roles)
{
    /// <summary>Gets the optional MCP tool allowlist in addition to the key's role.</summary>
    public string AllowedToolGroups { get; init; } = string.Empty;

    /// <summary>
    /// Creates the claims of this client.
    /// </summary>
    /// <returns>The claims.</returns>
    public IEnumerable<Claim> CreateClaims()
    {
        yield return new Claim(ApiKeyAuthenticationDefaults.ClientIdClaimType, this.Id.ToString());
        yield return new Claim(ClaimTypes.Name, this.Name);
        yield return new Claim(AdminToolGroups.ClaimType, this.AllowedToolGroups);
        yield return new Claim(ApiKeyAuthenticationDefaults.ClientNameClaimType, this.Name);
        foreach (var role in this.Roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }
    }
}
