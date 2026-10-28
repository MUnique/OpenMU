// <copyright file="ItemExchangeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

/// <summary>
/// Provides the items as an item exchange file of the MuMain client, which its editor downloads.
/// </summary>
/// <param name="operations">The item exchange operations.</param>
[ApiController]
[Route("api/items")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = AdminPolicies.ApiKeyViewer)]
public sealed class ItemExchangeController(ItemExchangeOperations operations) : ControllerBase
{
    /// <summary>
    /// Gets the item exchange file of the stored game configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file.</returns>
    [HttpGet("exchange")]
    public async Task<IActionResult> GetExchangeFileAsync(CancellationToken cancellationToken)
    {
        var file = await operations.ExportAsync(cancellationToken).ConfigureAwait(false);
        return this.File(file, "application/json; charset=utf-8");
    }
}
