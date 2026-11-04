// <copyright file="CastleSiegeNpcListAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Shows the state of the castle gates or Guardian Statues to the officers of the castle owner.
/// </summary>
public static class CastleSiegeNpcListAction
{
    /// <summary>
    /// Shows the defense structures of one type, if the player is an officer of the castle owner guild.
    /// Other players get an empty list, so the positions and health of the structures don't leak to attackers.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="monsterNumber">The gate or Guardian Statue monster number.</param>
    /// <returns>The shown defense structures.</returns>
    public static async ValueTask<IReadOnlyList<CastleSiegeNpcInfo>> ShowDefenseStructuresAsync(
        Player player,
        CastleSiegeContext? context,
        short monsterNumber)
    {
        IReadOnlyList<CastleSiegeNpcInfo> npcs = context is { Configuration.Enabled: true }
            && await CastleSiegeNpcAuthorization.IsOwnerOfficerAsync(player, context).ConfigureAwait(false)
            ? await context.NpcController.GetDefenseStructureSnapshotAsync(monsterNumber).ConfigureAwait(false)
            : [];
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcListPlugIn>(
                view => view.ShowNpcListAsync(npcs))
            .ConfigureAwait(false);
        return npcs;
    }
}
