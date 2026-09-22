// <copyright file="CastleSiegeHuntZoneToggleAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Changes public access to the Land of Trials.
/// </summary>
public sealed class CastleSiegeHuntZoneToggleAction
{
    /// <summary>
    /// Tries to change public Land of Trials access.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="isPublic">Whether public entry should be enabled.</param>
    /// <returns><see langword="true"/> when the setting was changed.</returns>
    public async ValueTask<bool> SetPublicAccessAsync(
        Player player,
        CastleSiegeContext? context,
        bool isPublic)
    {
        var result = CastleSiegeRequestResult.Failed;
        if (context is { Configuration.Enabled: true })
        {
            var ownerGuildId = await CastleSiegeTaxProvider.GetPersistentGuildMasterIdAsync(player).ConfigureAwait(false);
            await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (context.CurrentState != CastleSiegeState.Start)
                {
                    if (context.SiegeData.IsOccupied
                        && ownerGuildId is not null
                        && ownerGuildId == context.SiegeData.OwnerGuildId)
                    {
                        context.SiegeData.IsHuntZoneEnabled = isPublic;
                        await context.SaveOwnerAsync().ConfigureAwait(false);
                        result = CastleSiegeRequestResult.Success;
                    }
                    else
                    {
                        result = CastleSiegeRequestResult.NotAuthorized;
                    }
                }
            }
            finally
            {
                context.ExecutionLock.Release();
            }
        }

        var currentSetting = result == CastleSiegeRequestResult.Success
            ? isPublic
            : context?.SiegeData.IsHuntZoneEnabled ?? false;
        await player.InvokeViewPlugInAsync<ICastleSiegeHuntZoneResultPlugIn>(
                view => view.ShowEntranceSettingResultAsync(
                    result,
                    currentSetting))
            .ConfigureAwait(false);

        return result == CastleSiegeRequestResult.Success;
    }
}
