// <copyright file="WarpAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to warp to another place.
/// </summary>
public class WarpAction
{
    /// <summary>
    /// Warps the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="warpInfo">The warp information.</param>
    public async ValueTask WarpToAsync(Player player, WarpInfo warpInfo)
    {
        if (!warpInfo.IsWarpable && player.SelectedCharacter?.CharacterStatus != CharacterStatus.GameMaster)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.WarpOnlyForGameMasters)).ConfigureAwait(false);
            return;
        }

        if (!CheckRequirements(player, warpInfo, out var errorMessage))
        {
            await player.ShowBlueMessageAsync(errorMessage).ConfigureAwait(false);
            return;
        }

        // Reserve the warp cost before entry plug-ins can charge additional fees.
        if (!player.TryRemoveMoney(warpInfo.Costs))
        {
            await player.ShowBlueMessageAsync($"You need {warpInfo.Costs} in order to warp").ConfigureAwait(false);
            return;
        }

        var admitted = false;
        try
        {
            if (player.GameContext.PlugInManager.GetPlugInPoint<IWarpGateEnteringPlugIn>() is { } plugInPoint)
            {
                var eventArgs = new CancelEventArgs();
                await plugInPoint.WarpGateEnteringAsync(player, warpInfo.Gate!, eventArgs).ConfigureAwait(false);
                if (eventArgs.Cancel)
                {
                    return;
                }
            }

            admitted = true;
        }
        finally
        {
            if (!admitted)
            {
                // Restored unconditionally, because TryAddMoney refuses it at the money limit.
                // It never exceeds the limit, as it was part of the money before.
                player.Money = checked(player.Money + warpInfo.Costs);
            }
        }

        await player.WarpToAsync(warpInfo.Gate!).ConfigureAwait(false);
    }

    private static bool CheckRequirements(Player player, WarpInfo warpInfo, [MaybeNullWhen(true)] out string errorMessage)
    {
        errorMessage = null;

        if (!warpInfo.IsLevelReachedBy(player))
        {
            errorMessage = $"You need to be level {warpInfo.GetLevelRequirementFor(player)} in order to warp";
            return false;
        }

        if (warpInfo.Gate?.Map is null)
        {
            errorMessage = "The warp target is not initialized";
            return false;
        }

        if (warpInfo.Gate.Map.TryGetRequirementError(player, out var message))
        {
            errorMessage = message;
            return false;
        }

        if (warpInfo.GetMissingRequirement(player) is { } missing)
        {
            errorMessage = player.GetLocalizedMessage(nameof(PlayerMessage.MissingWarpRequirement), missing.GetDisplayText(player));
            return false;
        }

        return true;
    }
}
