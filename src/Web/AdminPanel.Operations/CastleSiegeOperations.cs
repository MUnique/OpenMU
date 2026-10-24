// <copyright file="CastleSiegeOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>Audited Castle Siege operations using the existing in-process administration service.</summary>
/// <param name="siege">The Castle Siege service.</param>
/// <param name="executor">The authorization and audit executor.</param>
public sealed class CastleSiegeOperations(CastleSiegeManagementService siege, AdminOperationExecutor executor)
{
    /// <summary>Reads live Castle Siege state, or explains why it is unavailable.</summary>
    /// <param name="serverId">The game server identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded state snapshot or deployment limitation.</returns>
    [AdminOperation(AdminRole.Viewer)]
    public async ValueTask<JsonObject> GetAsync(int serverId, CancellationToken cancellationToken)
    {
        await executor.RequireAsync(AdminRoles.Viewer, cancellationToken).ConfigureAwait(false);
        var result = await siege.GetSnapshotAsync(serverId).ConfigureAwait(false);
        var snapshot = result.Snapshot;
        return AdminReadResult.From(new
        {
            available = snapshot is not null,
            error = result.Error.ToString(),
            snapshot = snapshot is null ? null : new
            {
                state = snapshot.State.ToString(),
                snapshot.StateStartTimeUtc,
                snapshot.StateEndTimeUtc,
                snapshot.IsOccupied,
                snapshot.OwnerGuildId,
                snapshot.OwnerGuildName,
                snapshot.ChaosTax,
                snapshot.StoreTax,
                snapshot.HuntTax,
                snapshot.TributeMoney,
                registrations = snapshot.Registrations.Take(100),
                npcs = snapshot.Npcs.Take(100),
            },
        });
    }

    /// <summary>SetState through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="state">The state parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> SetStateAsync(int serverId, CastleSiegeState state, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_set_state",
            new { serverId, state, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.ForceStateAsync(serverId, state).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);

    /// <summary>SetOwner through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="guildName">The guildName parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> SetOwnerAsync(int serverId, string guildName, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_set_owner",
            new { serverId, guildName, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.SetOwnerAsync(serverId, guildName).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);

    /// <summary>ResetCycle through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> ResetCycleAsync(int serverId, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_reset_cycle",
            new { serverId, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.ResetCycleAsync(serverId).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);

    /// <summary>SetTaxes through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="chaosTax">The chaosTax parameter.</param>
    /// <param name="storeTax">The storeTax parameter.</param>
    /// <param name="huntTax">The huntTax parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> SetTaxesAsync(int serverId, byte chaosTax, byte storeTax, int huntTax, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_set_taxes",
            new { serverId, chaosTax, storeTax, huntTax, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.SetTaxesAsync(serverId, chaosTax, storeTax, huntTax).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);

    /// <summary>ClearTribute through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> ClearTributeAsync(int serverId, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_clear_tribute",
            new { serverId, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.ClearTributeAsync(serverId).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);

    /// <summary>RemoveRegistration through the existing Castle Siege administration rules.</summary>
    /// <param name="serverId">The serverId parameter.</param>
    /// <param name="guildId">The guildId parameter.</param>
    /// <param name="reason">The reason parameter.</param>
    /// <param name="cancellationToken">The cancellationToken parameter.</param>
    /// <returns>The operation status.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> RemoveRegistrationAsync(int serverId, Guid guildId, string reason, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "castle_siege_remove_registration",
            new { serverId, guildId, reason },
            async _ =>
            {
                AccountWriteOperations.ValidateReason(reason);
                var result = await siege.RemoveRegistrationAsync(serverId, guildId).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    throw new AdminOperationException("Castle Siege refused the action: " + result.Error);
                }

                return AdminReadResult.From(new { succeeded = true });
            },
            cancellationToken);
}
