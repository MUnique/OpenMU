// <copyright file="InMemoryAdminAuditRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>A synchronized audit store for operation and transport tests.</summary>
public sealed class InMemoryAdminAuditRepository : IAdminAuditRepository
{
    private readonly List<AdminAuditEntry> _entries = [];

    /// <summary>Gets or sets a value indicating whether audit writes should fail.</summary>
    public bool RejectWrites { get; set; }

    /// <summary>Gets or sets a value indicating whether outcome updates should fail.</summary>
    public bool RejectCompletion { get; set; }

    /// <inheritdoc />
    public ValueTask<AdminAuditEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (this._entries)
        {
            return ValueTask.FromResult(this._entries.FirstOrDefault(e => e.Id == id));
        }
    }

    /// <inheritdoc />
    public ValueTask AddAsync(AdminAuditEntry entry, CancellationToken cancellationToken = default)
    {
        if (this.RejectWrites)
        {
            throw new InvalidOperationException("Audit unavailable");
        }

        lock (this._entries)
        {
            this._entries.Add(entry);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask CompleteAsync(Guid id, string outcome, string resultJson, CancellationToken cancellationToken = default)
    {
        if (this.RejectCompletion)
        {
            throw new InvalidOperationException("Audit unavailable");
        }

        lock (this._entries)
        {
            var entry = this._entries.Single(e => e.Id == id);
            entry.Outcome = outcome;
            entry.ResultJson = resultJson;
            entry.CompletedAt = DateTime.UtcNow;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<(IReadOnlyList<AdminAuditEntry> Items, int Total)> ReadAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        lock (this._entries)
        {
            return ValueTask.FromResult<(IReadOnlyList<AdminAuditEntry>, int)>((this._entries.OrderByDescending(e => e.StartedAt).Skip(offset).Take(limit).ToArray(), this._entries.Count));
        }
    }
}
