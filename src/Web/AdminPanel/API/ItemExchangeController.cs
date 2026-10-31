// <copyright file="ItemExchangeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

/// <summary>
/// Provides the items as an item exchange file of the MuMain client, which its editor downloads.
/// </summary>
/// <param name="operations">The item exchange operations.</param>
[ApiController]
[Route("api/items")]
[Authorize(Policy = AdminPolicies.ApiKeyViewer)]
public sealed class ItemExchangeController(ItemExchangeOperations operations) : ControllerBase
{
    /// <summary>
    /// Lets one export run at a time.
    /// </summary>
    /// <remarks>
    /// An export reads the whole item configuration, and loads it again while the admin panel has
    /// unsaved changes. A client which asks again and again must not run many of them at once.
    /// It's a semaphore instead of an <c>AsyncLock</c>, because a second request doesn't wait for it.
    /// </remarks>
    internal static readonly SemaphoreSlim ExportGate = new(1, 1);

    /// <summary>
    /// Gets the item exchange file of the stored game configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file; <c>429</c> while another export is running.</returns>
    [HttpGet("exchange")]
    public async Task<IActionResult> GetExchangeFileAsync(CancellationToken cancellationToken)
    {
        if (!await ExportGate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            this.Response.Headers.RetryAfter = "1";
            return this.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        try
        {
            var file = await operations.ExportAsync(cancellationToken).ConfigureAwait(false);
            return this.File(file, "application/json; charset=utf-8");
        }
        finally
        {
            ExportGate.Release();
        }
    }
}
