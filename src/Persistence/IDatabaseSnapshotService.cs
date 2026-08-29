// <copyright file="IDatabaseSnapshotService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.IO;
using System.Threading;

/// <summary>
/// Service which creates and restores a snapshot of the whole database.
/// </summary>
/// <remarks>
/// In contrast to the <see cref="IBackupService"/>, a snapshot is created with the means of the
/// database system itself. It's a lot faster and contains all data, but it can only be restored
/// into a database with the same schema. Use the <see cref="IBackupService"/> to transfer data
/// between different versions of the server.
/// </remarks>
public interface IDatabaseSnapshotService
{
    /// <summary>
    /// Creates a snapshot of the database and writes it to the given stream as a zip archive.
    /// </summary>
    /// <param name="outputStream">The output stream to write the snapshot to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CreateSnapshotAsync(Stream outputStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the given stream contains a snapshot which can be restored into the current database.
    /// It's meant to be called before the database is re-created, so that selecting a wrong file doesn't cause a data loss.
    /// </summary>
    /// <param name="inputStream">The stream which should be checked. Its position is restored afterwards.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The reason why it can't be restored; <c>null</c>, if it can be restored.</returns>
    ValueTask<string?> GetRestoreBlockingReasonAsync(Stream inputStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores the data of the given snapshot into the database.
    /// </summary>
    /// <remarks>
    /// This does not create the database schema. The caller is responsible for
    /// re-creating an empty database before calling this method.
    /// </remarks>
    /// <param name="inputStream">The snapshot zip archive stream to restore from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RestoreSnapshotAsync(Stream inputStream, CancellationToken cancellationToken = default);
}
