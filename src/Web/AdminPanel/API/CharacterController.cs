// <copyright file="CharacterController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Reads saved character snapshots without exposing credentials.</summary>
/// <param name="operations">The shared read operations.</param>
[ApiController]
[Route("api/characters")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.ApiSchemes, Roles = nameof(AdminRole.Viewer))]
public sealed class CharacterController(AccountReadOperations operations) : ControllerBase
{
    /// <summary>Reads one character's saved state, which may lag a live session.</summary>
    /// <param name="name">The exact character name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe snapshot, or 404 when the character does not exist.</returns>
    [HttpGet("{name}")]
    public async ValueTask<IActionResult> GetAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            return this.Ok(await operations.GetCharacterAsync(name, cancellationToken).ConfigureAwait(false));
        }
        catch (AdminResourceNotFoundException error)
        {
            return this.NotFound(new { error = error.Message });
        }
    }
}
