// <copyright file="CastleSiegeEconomyNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Synchronizes Castle Siege tax rates with game clients.
/// </summary>
internal static class CastleSiegeEconomyNotifier
{
    /// <summary>
    /// Sends the current percentage tax rates to a player.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="player">The player.</param>
    internal static async ValueTask SynchronizePlayerAsync(CastleSiegeContext context, Player player)
    {
        await SendTaxRateAsync(context, player, CastleSiegeTaxType.ChaosMachine).ConfigureAwait(false);
        await SendTaxRateAsync(context, player, CastleSiegeTaxType.Store).ConfigureAwait(false);
    }

    /// <summary>
    /// Broadcasts a percentage tax rate to all connected players.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="taxType">The tax type.</param>
    /// <param name="taxRate">The tax rate captured when the change was applied.</param>
    internal static async ValueTask BroadcastTaxRateAsync(
        CastleSiegeContext context,
        CastleSiegeTaxType taxType,
        byte taxRate)
    {
        if (taxType is not (CastleSiegeTaxType.ChaosMachine or CastleSiegeTaxType.Store))
        {
            return;
        }

        foreach (var player in await context.GameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.PlayerState.CurrentState == PlayerState.EnteredWorld)
            {
                await SendTaxRateAsync(player, taxType, taxRate).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Broadcasts all percentage tax rates to connected players.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    internal static async ValueTask BroadcastTaxRatesAsync(CastleSiegeContext context)
    {
        foreach (var player in await context.GameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.PlayerState.CurrentState == PlayerState.EnteredWorld)
            {
                await SynchronizePlayerAsync(context, player).ConfigureAwait(false);
            }
        }
    }

    private static ValueTask SendTaxRateAsync(
        CastleSiegeContext context,
        Player player,
        CastleSiegeTaxType taxType)
    {
        var taxRate = taxType switch
        {
            CastleSiegeTaxType.ChaosMachine => Math.Min((int)context.SiegeData.TaxChaos, CastleSiegeTaxProvider.MaximumPercentageTax),
            CastleSiegeTaxType.Store => Math.Min((int)context.SiegeData.TaxStore, CastleSiegeTaxProvider.MaximumPercentageTax),
            _ => 0,
        };
        return SendTaxRateAsync(player, taxType, checked((byte)taxRate));
    }

    private static ValueTask SendTaxRateAsync(
        Player player,
        CastleSiegeTaxType taxType,
        byte taxRate)
    {
        return player.InvokeViewPlugInAsync<ICastleSiegeTaxChangeResultPlugIn>(
            view => view.ShowTaxRateUpdateAsync(taxType, taxRate));
    }
}
