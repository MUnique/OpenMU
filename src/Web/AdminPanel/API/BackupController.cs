// <copyright file="BackupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.API;

using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence;

/// <summary>
/// API controller to download and upload backup archives.
/// </summary>
[Route("admin/backup")]
public class BackupController : Controller
{
    private readonly IBackupService _backupService;
    private readonly IMigratableDatabaseContextProvider _migratableDatabaseContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackupController"/> class.
    /// </summary>
    /// <param name="backupService">The backup service.</param>
    /// <param name="migratableDatabaseContextProvider">The migratable database context provider.</param>
    public BackupController(IBackupService backupService, IMigratableDatabaseContextProvider migratableDatabaseContextProvider)
    {
        this._backupService = backupService;
        this._migratableDatabaseContextProvider = migratableDatabaseContextProvider;
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
    /// Restores the database from an uploaded backup archive.
    /// </summary>
    /// <param name="file">The backup zip archive to restore from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Ok on success.</returns>
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> UploadBackupAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return this.BadRequest("No backup file provided.");
        }

        using var update = await this._migratableDatabaseContextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
        await using var stream = file.OpenReadStream();
        await this._backupService.RestoreBackupAsync(stream, cancellationToken).ConfigureAwait(false);
        return this.Ok();
    }
}
