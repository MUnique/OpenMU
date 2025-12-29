// <copyright file="ChangeMuHelperStateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MuHelper;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to change the MU Helper state.
/// </summary>
public class ChangeMuHelperStateAction
{
    /// <summary>
    /// Tries to change the state of the MU Helper to the specified status.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="status">status to be set.</param>
    public async ValueTask ChangeHelperStateAsync(Player player, MuHelperStatus status)
    {
        var configuration = player.GameContext.FeaturePlugIns.GetPlugIn<MuHelperFeaturePlugIn>()?.Configuration;

        if (configuration is null)
        {
            var message = player.GetLocalizedMessage("MuHelper_Message_Disabled", "MU Helper is disabled.");
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        switch (status)
        {
            case MuHelperStatus.Enabled:
                await player.MuHelper.TryStartAsync().ConfigureAwait(false);
                break;
            case MuHelperStatus.Disabled:
                await player.MuHelper.StopAsync().ConfigureAwait(false);
                break;
            default: // unknown
                var message = player.GetLocalizedMessage("MuHelper_Message_UnknownState", "MU Helper cannot handle state {0}.", status);
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
                break;
        }
    }
}
