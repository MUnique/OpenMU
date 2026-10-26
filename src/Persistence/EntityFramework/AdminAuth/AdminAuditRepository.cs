// <copyright file="AdminAuditRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Stores audit history in the isolated admin schema.</summary>
/// <param name="users">The repository which owns admin schema migrations.</param>
public sealed class AdminAuditRepository(IAdminUserRepository users) : IAdminAuditRepository
{
    /// <inheritdoc />
    public async ValueTask AddAsync(AdminAuditEntry entry, CancellationToken cancellationToken = default)
    {
        await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false);
        await using var context = new AdminPanelContext();
        context.AdminAuditEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask CompleteAsync(Guid id, string outcome, string resultJson, CancellationToken cancellationToken = default)
    {
        await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false);
        await using var context = new AdminPanelContext();
        var entry = await context.AdminAuditEntries.SingleAsync(e => e.Id == id, cancellationToken).ConfigureAwait(false);
        entry.Outcome = outcome;
        entry.ResultJson = resultJson;
        entry.CompletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<AdminAuditEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false);
        await using var context = new AdminPanelContext();
        return await context.AdminAuditEntries.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<(IReadOnlyList<AdminAuditEntry> Items, int Total)> ReadAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        if (offset < 0 || limit is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        await this.EnsureStorageAsync(cancellationToken).ConfigureAwait(false);
        await using var context = new AdminPanelContext();
        var query = context.AdminAuditEntries.AsNoTracking();
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(e => e.StartedAt).ThenByDescending(e => e.Id).Skip(offset).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    private async ValueTask EnsureStorageAsync(CancellationToken cancellationToken)
    {
        if (!await users.EnsureStorageAsync(cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Audit storage is unavailable; no action was executed.");
        }
    }
}
