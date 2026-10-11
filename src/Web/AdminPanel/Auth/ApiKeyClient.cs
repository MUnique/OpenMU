// <copyright file="ApiKeyClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.Security.Claims;

/// <summary>
/// An external application which is allowed to use the public API.
/// </summary>
/// <param name="Name">The name of the client.</param>
/// <param name="Roles">The effective roles of the client.</param>
public record ApiKeyClient(string Name, IReadOnlyList<string> Roles)
{
    /// <summary>
    /// Creates the claims of this client.
    /// </summary>
    /// <returns>The claims.</returns>
    public IEnumerable<Claim> CreateClaims()
    {
        yield return new Claim(ClaimTypes.Name, this.Name);
        yield return new Claim(ApiKeyAuthenticationDefaults.ClientNameClaimType, this.Name);
        foreach (var role in this.Roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }
    }
}
