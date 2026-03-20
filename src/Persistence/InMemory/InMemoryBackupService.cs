// <copyright file="InMemoryBackupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// A stub implementation of <see cref="IBackupService"/> for the in-memory persistence layer.
/// Backup creation is not meaningful for an in-memory store; restore is not supported.
/// </summary>
public class InMemoryBackupService : IBackupService
{
    /// <inheritdoc />
    public Task CreateBackupAsync(Stream outputStream, CancellationToken cancellationToken = default)
    {
        // In-memory persistence has no persistent data to back up.
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RestoreBackupAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Backup restore is not supported for in-memory persistence.");
    }
}
