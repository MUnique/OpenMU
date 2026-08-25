// <copyright file="BackupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence;

/// <summary>
/// API controller to download a backup archive.
/// The restore of a backup is done on the setup page, so that the database is re-created and
/// the admin panel is notified about the new data.
/// </summary>
[Route("admin/backup")]
public class BackupController : Controller
{
    private readonly IBackupService _backupService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupController"/> class.
    /// </summary>
    /// <param name="backupService">The backup service.</param>
    public BackupController(IBackupService backupService)
    {
        this._backupService = backupService;
    }

    /// <summary>
    /// Downloads a backup archive containing all configuration and account data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The backup zip archive as a file download.</returns>
    [HttpGet]
    public async Task<IActionResult> DownloadBackupAsync(CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await this._backupService.CreateBackupAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Position = 0;
        var fileName = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
        return this.File(stream, "application/zip", fileName);
    }
}
