// <copyright file="BackupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// API controller to download a backup archive or a database snapshot.
/// The restore of a backup is done on the setup page, so that the database is re-created and
/// the admin panel is notified about the new data.
/// </summary>
[Route("admin/backup")]
[Authorize(Policy = AdminPolicies.Administrator)]
public class BackupController : Controller
{
    private readonly IBackupService _backupService;
    private readonly IDatabaseSnapshotService? _snapshotService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupController"/> class.
    /// </summary>
    /// <param name="backupService">The backup service.</param>
    /// <param name="snapshotService">The snapshot service, if the used persistence supports it.</param>
    public BackupController(IBackupService backupService, IDatabaseSnapshotService? snapshotService = null)
    {
        this._backupService = backupService;
        this._snapshotService = snapshotService;
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

    /// <summary>
    /// Downloads a snapshot of the database, which can only be restored into a database with the same schema.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The snapshot zip archive as a file download.</returns>
    [HttpGet("snapshot")]
    public async Task<IActionResult> DownloadSnapshotAsync(CancellationToken cancellationToken)
    {
        if (this._snapshotService is not { } snapshotService)
        {
            return this.NotFound("The used persistence doesn't support database snapshots.");
        }

        var stream = new MemoryStream();
        await snapshotService.CreateSnapshotAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Position = 0;
        var fileName = $"snapshot_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
        return this.File(stream, "application/zip", fileName);
    }
}
