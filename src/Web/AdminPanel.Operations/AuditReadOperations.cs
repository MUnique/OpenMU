// <copyright file="AuditReadOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Reads administrative audit history for administrators.</summary>
/// <param name="executor">The authorization service.</param>
/// <param name="audit">The audit repository.</param>
public sealed class AuditReadOperations(AdminOperationExecutor executor, IAdminAuditRepository audit)
{
    /// <summary>Reads a bounded page of audit entries, newest first.</summary>
    /// <param name="offset">The number of entries to skip.</param>
    /// <param name="limit">The maximum number of entries.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The audit page.</returns>
    [AdminOperation(AdminRole.Administrator)]
    public async ValueTask<JsonObject> ListAsync(int offset, int limit, CancellationToken cancellationToken)
    {
        await executor.RequireAsync(AdminRoles.Administrator, cancellationToken).ConfigureAwait(false);
        AdminReadResult.ValidatePage(offset, limit);
        var (items, total) = await audit.ReadAsync(offset, limit, cancellationToken).ConfigureAwait(false);
        return AdminReadResult.Page(items, total, offset, limit);
    }
}
