// <copyright file="CastleSiegeNpcBuyAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Re-purchases destroyed Castle Siege gates and Guardian Statues.
/// </summary>
public static class CastleSiegeNpcBuyAction
{
    /// <summary>
    /// Tries to re-purchase a destroyed defense structure.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="npcNumber">The target NPC number.</param>
    /// <param name="npcIndex">The target NPC instance identifier.</param>
    /// <returns>The operation result.</returns>
    public static async ValueTask<CastleSiegeNpcOperationResult> BuyAsync(
        Player player,
        CastleSiegeContext? context,
        uint npcNumber,
        uint npcIndex)
    {
        var result = await BuyCoreAsync(player, context, npcNumber, npcIndex).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                view => view.ShowBuyResultAsync(result, npcNumber, npcIndex))
            .ConfigureAwait(false);
        return result;
    }

    private static async ValueTask<CastleSiegeNpcOperationResult> BuyCoreAsync(
        Player player,
        CastleSiegeContext? context,
        uint npcNumber,
        uint npcIndex)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return CastleSiegeNpcOperationResult.Failed;
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState == CastleSiegeState.Start)
            {
                return CastleSiegeNpcOperationResult.Failed;
            }

            if (!await CastleSiegeNpcAuthorization.IsOwnerOfficerAsync(player, context).ConfigureAwait(false))
            {
                return CastleSiegeNpcOperationResult.NotAuthorized;
            }

            var runtime = context.NpcController.FindDefenseStructure(npcNumber, npcIndex);
            if (runtime is { IsAlive: true })
            {
                return CastleSiegeNpcOperationResult.RequirementNotMet;
            }

            if (runtime is not { PersistedState: { } state })
            {
                return CastleSiegeNpcOperationResult.Failed;
            }

            var (price, initialHealth) = runtime.Definition.MonsterDefinition?.Number switch
            {
                var number when number == CastleSiegeGate.MonsterNumber => (
                    context.Configuration.GateBuyPrice,
                    context.Configuration.GetUpgrades(CastleSiegeGate.MonsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(0) ?? 0),
                var number when number == CastleSiegeStatue.MonsterNumber => (
                    context.Configuration.StatueBuyPrice,
                    context.Configuration.GetUpgrades(CastleSiegeStatue.MonsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(0) ?? 0),
                _ => (0, 0),
            };
            if (price <= 0 || initialHealth <= 0)
            {
                return CastleSiegeNpcOperationResult.Failed;
            }

            if (!player.TryRemoveMoney(price))
            {
                return CastleSiegeNpcOperationResult.InsufficientMoney;
            }

            state.DefenseLevel = 0;
            state.LifeLevel = 0;
            state.RegenLevel = 0;
            state.CurrentHp = initialHealth;
            await context.NpcController.RespawnAsync(runtime).ConfigureAwait(false);
            if (runtime.SpawnedInstance is CastleSiegeGate gate)
            {
                await gate.CloseAsync().ConfigureAwait(false);
            }

            await context.SaveNpcStatesAsync().ConfigureAwait(false);
            return CastleSiegeNpcOperationResult.Success;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}
