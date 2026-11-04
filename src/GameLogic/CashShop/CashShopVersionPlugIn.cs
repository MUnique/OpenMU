// <copyright file="CashShopVersionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Shows the versions of the cash shop script and banner to the player when it enters the game.
/// </summary>
/// <remarks>
/// The client keeps its cash shop locked until it received the script version.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopVersionPlugIn_Name), Description = nameof(PlugInResources.CashShopVersionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F93615F5-7924-4CA3-833F-A9040991C4B2")]
public class CashShopVersionPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState == PlayerState.CharacterSelection)
        {
            player.IsCashShopOpen = false;
            return;
        }

        if (previousState != PlayerState.CharacterSelection
            || currentState != PlayerState.EnteredWorld
            || player.GameContext.Configuration.CashShopConfiguration is not { } configuration
            || CashShopFeaturePlugIn.GetSettings(player.GameContext) is null)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowVersionsAsync(configuration)).ConfigureAwait(false);
    }
}
