// <copyright file="CastleSiegeTaxRateChangeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Changes one of the Castle Siege tax rates.
/// </summary>
public sealed class CastleSiegeTaxRateChangeAction
{
    /// <summary>
    /// Tries to change a Castle Siege tax rate.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="taxType">The tax type.</param>
    /// <param name="taxRate">The requested tax rate or fee.</param>
    /// <returns><see langword="true"/> when the rate was changed.</returns>
    public async ValueTask<bool> ChangeAsync(
        Player player,
        CastleSiegeContext? context,
        CastleSiegeTaxType taxType,
        uint taxRate)
    {
        var result = CastleSiegeRequestResult.Failed;
        byte? broadcastTaxRate = null;
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
                    if (context.CurrentState != CastleSiegeState.Start
                        && IsValid(taxType, taxRate))
                    {
                        switch (taxType)
                        {
                            case CastleSiegeTaxType.ChaosMachine:
                                context.SiegeData.TaxChaos = (byte)taxRate;
                                break;
                            case CastleSiegeTaxType.Store:
                                context.SiegeData.TaxStore = (byte)taxRate;
                                break;
                            case CastleSiegeTaxType.HuntZone:
                                context.SiegeData.TaxHunt = (int)taxRate;
                                break;
                            default:
                                throw new InvalidOperationException($"Unexpected validated tax type {taxType}.");
                        }

                        await context.SaveOwnerAsync().ConfigureAwait(false);
                        result = CastleSiegeRequestResult.Success;
                        if (taxType is CastleSiegeTaxType.ChaosMachine or CastleSiegeTaxType.Store)
                        {
                            broadcastTaxRate = checked((byte)taxRate);
                        }
                    }
                }
            }
            finally
            {
                context.ExecutionLock.Release();
            }
        }

        await player.InvokeViewPlugInAsync<ICastleSiegeTaxChangeResultPlugIn>(
                view => view.ShowTaxChangeResultAsync(result, taxType, taxRate))
            .ConfigureAwait(false);
        if (context is not null && broadcastTaxRate is { } rate)
        {
            await CastleSiegeEconomyNotifier.BroadcastTaxRateAsync(context, taxType, rate).ConfigureAwait(false);
        }

        return result == CastleSiegeRequestResult.Success;
    }

    private static bool IsValid(CastleSiegeTaxType taxType, uint taxRate)
    {
        return taxType switch
        {
            CastleSiegeTaxType.ChaosMachine or CastleSiegeTaxType.Store => taxRate <= CastleSiegeTaxProvider.MaximumPercentageTax,
            CastleSiegeTaxType.HuntZone => taxRate <= CastleSiegeTaxProvider.MaximumHuntTax,
            _ => false,
        };
    }
}
