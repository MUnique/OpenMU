// <copyright file="IAdminAuditRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

using System.Threading;

/// <summary>Stores audit attempts before execution and records their outcomes separately.</summary>
public interface IAdminAuditRepository
{
    /// <summary>Persists an attempt. Failure must prevent the action from running.</summary>
    /// <param name="entry">The sanitized attempt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask AddAsync(AdminAuditEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Records the outcome of a previously persisted attempt.</summary>
    /// <param name="id">The attempt identifier.</param>
    /// <param name="outcome">The safe outcome.</param>
    /// <param name="resultJson">The sanitized result projection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask CompleteAsync(Guid id, string outcome, string resultJson, CancellationToken cancellationToken = default);

    /// <summary>Reads one durable audit record by identifier.</summary>
    /// <param name="id">The audit identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The record, or null when absent.</returns>
    ValueTask<AdminAuditEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Reads a bounded page ordered newest first.</summary>
    /// <param name="offset">The number of records to skip.</param>
    /// <param name="limit">The maximum number of records.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The records and matching total.</returns>
    ValueTask<(IReadOnlyList<AdminAuditEntry> Items, int Total)> ReadAsync(int offset, int limit, CancellationToken cancellationToken = default);
}
