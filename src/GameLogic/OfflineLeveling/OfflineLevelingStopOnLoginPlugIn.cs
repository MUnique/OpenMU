// <copyright file="OfflineLevelingStopOnLoginPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Listens for the <see cref="PlayerState.Authenticated"/> transition and stops any active
/// offline leveling session for that account so the real player can take over normally.
/// </summary>
[PlugIn]
[Display(
    Name = nameof(PlugInResources.OfflineLevelingStopOnLoginPlugIn_Name),
    Description = nameof(PlugInResources.OfflineLevelingStopOnLoginPlugIn_Description),
    ResourceType = typeof(PlugInResources))]
[Guid("3E7A4D2C-81B5-4F6A-9C3D-0A2E5B8F1D9C")]
public sealed class OfflineLevelingStopOnLoginPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        // Ignore it to avoid immediately stopping the session we just started.
        if (currentState != PlayerState.Authenticated || player is OfflineLevelingPlayer)
        {
            return;
        }

        var loginName = player.Account?.LoginName;
        if (loginName is null)
        {
            return;
        }

        var manager = player.GameContext.OfflineLevelingManager;
        if (!manager.IsActive(loginName))
        {
            return;
        }

        player.Logger.LogInformation("Account {LoginName} authenticated, stopping offline leveling.", loginName);

        await manager.StopAsync(loginName).ConfigureAwait(false);
    }
}