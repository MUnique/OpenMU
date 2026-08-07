// <copyright file="CastleSiegeGateOperateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Opens and closes Castle Siege gates after validating the requesting player's side.
/// </summary>
public static class CastleSiegeGateOperateAction
{
    /// <summary>
    /// Tries to open the Castle Siege gate-operation interface.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="gateId">The target gate identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async ValueTask ShowInterfaceAsync(
        Player player,
        CastleSiegeContext? context,
        ushort gateId)
    {
        var result = await ValidateAsync(player, context, gateId).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                view => view.ShowGateInterfaceAsync(result, gateId))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Tries to open or close a Castle Siege gate.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="gateId">The target gate identifier.</param>
    /// <param name="open">Whether the gate should be opened.</param>
    /// <returns>The operation result.</returns>
    public static async ValueTask<CastleSiegeNpcOperationResult> OperateAsync(
        Player player,
        CastleSiegeContext? context,
        ushort gateId,
        bool open)
    {
        var (result, isOpen) = await OperateCoreAsync(player, context, gateId, open).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                view => view.ShowGateOperationResultAsync(result, isOpen, gateId))
            .ConfigureAwait(false);
        return result;
    }

    private static async ValueTask<(CastleSiegeNpcOperationResult Result, bool IsOpen)> OperateCoreAsync(
        Player player,
        CastleSiegeContext? context,
        ushort gateId,
        bool open)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (CastleSiegeNpcOperationResult.Failed, open);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!await IsAuthorizedAsync(player, context).ConfigureAwait(false))
            {
                return (CastleSiegeNpcOperationResult.NotAuthorized, open);
            }

            if (context.NpcController.FindGate(gateId) is not { IsAlive: true } gate)
            {
                return (CastleSiegeNpcOperationResult.Failed, open);
            }

            if (open)
            {
                await gate.OpenAsync().ConfigureAwait(false);
            }
            else
            {
                await gate.CloseAsync().ConfigureAwait(false);
            }

            return (CastleSiegeNpcOperationResult.Success, !gate.IsClosed);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private static async ValueTask<CastleSiegeNpcOperationResult> ValidateAsync(
        Player player,
        CastleSiegeContext? context,
        ushort gateId)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return CastleSiegeNpcOperationResult.Failed;
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!await IsAuthorizedAsync(player, context).ConfigureAwait(false))
            {
                return CastleSiegeNpcOperationResult.NotAuthorized;
            }

            return context.NpcController.FindGate(gateId) is { IsAlive: true }
                ? CastleSiegeNpcOperationResult.Success
                : CastleSiegeNpcOperationResult.Failed;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private static ValueTask<bool> IsAuthorizedAsync(Player player, CastleSiegeContext context)
    {
        return context.CurrentState == CastleSiegeState.Start
            ? ValueTask.FromResult(context.GetPlayerJoinSide(player) == CastleSiegeJoinSide.Defense)
            : CastleSiegeNpcAuthorization.IsOwnerAllianceMemberAsync(player, context);
    }
}
