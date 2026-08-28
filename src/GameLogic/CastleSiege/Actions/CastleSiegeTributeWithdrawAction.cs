// <copyright file="CastleSiegeTributeWithdrawAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Withdraws money from the Castle Siege treasury.
/// </summary>
public sealed class CastleSiegeTributeWithdrawAction
{
    /// <summary>
    /// Tries to withdraw tribute money into the castle owner guild master's inventory.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="amount">The requested amount.</param>
    /// <returns><see langword="true"/> when the money was withdrawn.</returns>
    public async ValueTask<bool> WithdrawAsync(Player player, CastleSiegeContext? context, long amount)
    {
        var result = CastleSiegeRequestResult.Failed;
        if (context is { Configuration.Enabled: true })
        {
            var ownerGuildId = await CastleSiegeTaxProvider.GetPersistentGuildMasterIdAsync(player).ConfigureAwait(false);
            await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!context.SiegeData.IsOccupied
                    || ownerGuildId is null
                    || ownerGuildId != context.SiegeData.OwnerGuildId)
                {
                    result = CastleSiegeRequestResult.NotAuthorized;
                }
                else
                {
                    if (amount is > 0 and <= int.MaxValue
                        && amount <= context.SiegeData.TributeMoney
                        && player.TryAddMoney((int)amount))
                    {
                        var previousTribute = context.SiegeData.TributeMoney;
                        context.SiegeData.TributeMoney -= amount;
                        try
                        {
                            await context.SaveOwnerAsync().ConfigureAwait(false);
                            result = CastleSiegeRequestResult.Success;
                        }
                        catch
                        {
                            context.SiegeData.TributeMoney = previousTribute;
                            _ = player.TryRemoveMoney((int)amount);
                            throw;
                        }
                    }
                }
            }
            finally
            {
                context.ExecutionLock.Release();
            }
        }

        if (result == CastleSiegeRequestResult.Success)
        {
            await player.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(view => view.UpdateMoneyAsync()).ConfigureAwait(false);
        }

        await player.InvokeViewPlugInAsync<ICastleSiegeTributeWithdrawResultPlugIn>(
                view => view.ShowTributeWithdrawResultAsync(
                    result,
                    result == CastleSiegeRequestResult.Success ? amount : 0))
            .ConfigureAwait(false);
        return result == CastleSiegeRequestResult.Success;
    }
}
