// <copyright file="CastleSiegeTaxInfoAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Shows the current Castle Siege economy state.
/// </summary>
public sealed class CastleSiegeTaxInfoAction
{
    private readonly CastleSiegeTaxProvider _taxProvider = new();

    /// <summary>
    /// Shows the tax configuration and the treasury balance visible to the player.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    public async ValueTask ShowAsync(Player player, CastleSiegeContext? context)
    {
        var result = CastleSiegeRequestResult.Failed;
        var data = context is { Configuration.Enabled: true, SiegeData.IsOccupied: true }
            ? context.SiegeData
            : null;
        if (data is not null)
        {
            result = await this._taxProvider.IsOwnerGuildMasterAsync(player, context).ConfigureAwait(false)
                ? CastleSiegeRequestResult.Success
                : CastleSiegeRequestResult.NotAuthorized;
        }

        // Percentage rates are public protocol state; only the treasury balance is restricted to the owner guild master.
        await player.InvokeViewPlugInAsync<ICastleSiegeTaxInfoPlugIn>(
                view => view.ShowTaxInfoAsync(
                    result,
                    data?.TaxChaos ?? 0,
                    data?.TaxStore ?? 0,
                    result == CastleSiegeRequestResult.Success ? Math.Max(0, data!.TributeMoney) : 0))
            .ConfigureAwait(false);
    }
}
