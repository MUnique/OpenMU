// <copyright file="AdminChangeExecution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Persistence;

/// <summary>Shares preview authorization, confirmation and durable write execution between operation services.</summary>
internal static class AdminChangeExecution
{
    /// <summary>Saves a configuration or offline edit without claiming certainty after a transport or commit failure.</summary>
    /// <param name="context">The owned persistence context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal static async ValueTask SaveAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            if (!await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new AdminOperationException("The change could not be saved.");
            }
        }
        catch (Exception exception) when (exception is PersistenceConflictException or ValidationException)
        {
            throw new AdminOperationException("The change was rejected without being saved. Check constraints and request a new preview.", AdminOperationErrorKind.Conflict, exception);
        }
        catch (Exception exception) when (exception is not AdminOperationException)
        {
            throw new AdminOperationException("The persistence outcome could not be confirmed. Check the current state before retrying; request a new preview if needed.", AdminOperationErrorKind.Uncertain, exception);
        }
    }

    /// <summary>Checks that previews and applies have the same write capabilities.</summary>
    /// <param name="executor">The audit and authorization executor.</param>
    /// <param name="settings">The write safety policy.</param>
    /// <param name="role">The minimum required role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal static async ValueTask<AdminActor> RequireAsync(AdminOperationExecutor executor, AdminWriteSettings settings, string role, CancellationToken cancellationToken)
    {
        var actor = await executor.RequireAsync(role, cancellationToken).ConfigureAwait(false);
        if (actor.Frontend == "mcp" && settings.ReadOnly)
        {
            throw new AdminOperationException("MCP writes are disabled. Enable AdminPanel:Mcp:ReadOnly=false explicitly.");
        }

        return actor;
    }

    /// <summary>Returns a preview or runs the already validated change under the audit executor.</summary>
    /// <param name="executor">The audit and authorization executor.</param>
    /// <param name="confirmations">The shared confirmation coordinator.</param>
    /// <param name="actor">The authorized caller.</param>
    /// <param name="role">The minimum required role.</param>
    /// <param name="operation">The invariant operation name.</param>
    /// <param name="arguments">The exact validated operation arguments.</param>
    /// <param name="change">The safe proposed diff and state fingerprints.</param>
    /// <param name="apply">Whether to apply the confirmed preview.</param>
    /// <param name="token">The single-use confirmation token.</param>
    /// <param name="save">The validated persistence action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal static async ValueTask<JsonObject> RunAsync(AdminOperationExecutor executor, AdminChangeConfirmation confirmations, AdminActor actor, string role, string operation, object arguments, JsonObject change, bool apply, string? token, Func<ValueTask> save, CancellationToken cancellationToken)
    {
        if (!apply)
        {
            return confirmations.Preview(actor, operation, arguments, change);
        }

        confirmations.Consume(actor, operation, arguments, change, token);
        return await executor.ExecuteAsync(
            role,
            operation,
            new { arguments, change },
            async _ =>
        {
            await save().ConfigureAwait(false);
            return new JsonObject { ["status"] = "applied", ["change"] = change.DeepClone() };
        },
            cancellationToken).ConfigureAwait(false);
    }
}
