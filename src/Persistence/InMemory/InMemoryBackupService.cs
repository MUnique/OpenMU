// <copyright file="InMemoryBackupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.IO;
using System.Threading;

/// <summary>
/// An implementation of <see cref="IBackupService"/> for the in-memory persistence layer.
/// Export is supported via the base <see cref="BackupService"/>; restore is not supported.
/// </summary>
public class InMemoryBackupService : BackupService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryBackupService"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    public InMemoryBackupService(IPersistenceContextProvider contextProvider)
        : base(contextProvider)
    {
    }

    /// <inheritdoc />
    public override Task RestoreBackupAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Backup restore is not supported for in-memory persistence.");
    }
}
