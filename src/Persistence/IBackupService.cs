// <copyright file="IBackupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.IO;
using System.Threading;

/// <summary>
/// Service which can create and restore backups of the configuration and account data.
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// Creates a backup of all configuration and account data and writes it to the given stream as a zip archive.
    /// </summary>
    /// <param name="outputStream">The output stream to write the backup zip archive to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateBackupAsync(Stream outputStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores all configuration and account data from the given backup zip archive stream.
    /// </summary>
    /// <remarks>
    /// Note: This does not recreate the database schema. The caller is responsible for
    /// recreating the database (e.g. via <see cref="IMigratableDatabaseContextProvider.ReCreateDatabaseAsync"/>)
    /// before calling this method.
    /// </remarks>
    /// <param name="inputStream">The backup zip archive stream to restore from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RestoreBackupAsync(Stream inputStream, CancellationToken cancellationToken = default);
}
