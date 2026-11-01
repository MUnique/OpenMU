// <copyright file="ShowShopCurrenciesAfterEnterWorldPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Shows the items and account coins which can be used for prices in player shops to the player when it enters
/// the game, so that the client offers them when the player sets the prices of its shop.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowShopCurrenciesAfterEnterWorldPlugIn_Name), Description = nameof(PlugInResources.ShowShopCurrenciesAfterEnterWorldPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C9DF78E0-AC93-42A0-AD0C-2A83165A6522")]
public class ShowShopCurrenciesAfterEnterWorldPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld)
        {
            return;
        }

        await player.ShowAllowedPricesAsync().ConfigureAwait(false);
    }
}
