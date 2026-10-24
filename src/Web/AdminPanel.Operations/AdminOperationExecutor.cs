// <copyright file="AdminOperationExecutor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Enforces authorization and persists an audit attempt before any side effects.</summary>
/// <param name="caller">The current caller.</param>
/// <param name="audit">The durable audit store.</param>
/// <param name="settings">The write safety policy.</param>
/// <param name="logger">The operational logger.</param>
public sealed class AdminOperationExecutor(IAdminOperationContext caller, IAdminAuditRepository audit, AdminWriteSettings settings, ILogger<AdminOperationExecutor> logger)
{
    /// <summary>Checks an operation's role independently of its front end.</summary>
    /// <param name="role">The minimum role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authorized caller.</returns>
    public async ValueTask<AdminActor> RequireAsync(string role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var actor = await caller.GetActorAsync(cancellationToken).ConfigureAwait(false);
        if (!actor.Roles.SelectMany(AdminRoles.GetEffectiveRoles).Contains(role, StringComparer.Ordinal))
        {
            throw new AdminOperationException("The operation is not allowed for this caller.", forbidden: true);
        }

        return actor;
    }

    /// <summary>Runs an audited action. Arguments must be an explicit projection which excludes secrets.</summary>
    /// <typeparam name="T">The safe result type.</typeparam>
    /// <param name="role">The required role.</param>
    /// <param name="operation">The operation name.</param>
    /// <param name="arguments">An explicit, secret-free argument projection.</param>
    /// <param name="action">The action, including its validation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="succeeded">An optional result predicate for services which return a refusal status.</param>
    /// <returns>The action result.</returns>
    public async ValueTask<T> ExecuteAsync<T>(string role, string operation, object arguments, Func<AdminActor, ValueTask<T>> action, CancellationToken cancellationToken, Func<T, bool>? succeeded = null)
    {
        var actor = await this.RequireAsync(role, cancellationToken).ConfigureAwait(false);
        if (actor.Frontend == "mcp" && settings.ReadOnly)
        {
            throw new AdminOperationException("MCP writes are disabled. Enable AdminPanel:Mcp:ReadOnly=false explicitly.");
        }

        var entry = new AdminAuditEntry
        {
            Id = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            PrincipalId = actor.Id,
            PrincipalName = actor.Name,
            Frontend = actor.Frontend,
            Operation = operation,
            ArgumentsJson = AdminAuditSanitizer.Serialize(arguments),
        };
        if (entry.ArgumentsJson.Length > 16384)
        {
            throw new ArgumentException("The audit arguments exceed the size limit.", nameof(arguments));
        }

        try
        {
            await audit.AddAsync(entry, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "The admin audit attempt could not be persisted; no action was executed.");
            throw new AdminOperationException("Audit storage is unavailable; no action was executed. Check the admin database connection.");
        }

        T result;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            result = await action(actor).ConfigureAwait(false);
        }
        catch
        {
            await this.RecordOutcomeAsync(entry.Id, "Failed", "{}").ConfigureAwait(false);
            throw;
        }

        if (!await this.RecordOutcomeAsync(entry.Id, succeeded?.Invoke(result) == false ? "Refused" : "Succeeded", AdminAuditSanitizer.Serialize(result)).ConfigureAwait(false))
        {
            throw new AdminOperationException("The action may have completed, but its audit outcome could not be saved. Check the current state before retrying.");
        }

        return result;
    }

    private async ValueTask<bool> RecordOutcomeAsync(Guid id, string outcome, string resultJson)
    {
        try
        {
            // Completion must survive caller cancellation after the operation has changed state.
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await audit.CompleteAsync(id, outcome, resultJson.Length <= 16384 ? resultJson : "{\"truncated\":true}", timeout.Token).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not record outcome of admin action {AuditId}; its audit record remains pending.", id);
            return false;
        }
    }
}
