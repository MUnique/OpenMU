// <copyright file="ShowWarpListAfterEnterWorldPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Shows the warp list of the server to the player when it enters the game, so that the warp window
/// of the client offers the warps of the server instead of the ones of its data files.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowWarpListAfterEnterWorldPlugIn_Name), Description = nameof(PlugInResources.ShowWarpListAfterEnterWorldPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("CC3C6192-B0E9-441F-8E7D-D432B562AB86")]
public class ShowWarpListAfterEnterWorldPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld)
        {
            return;
        }

        var warps = GetSelectableWarps(player.GameContext.Configuration.WarpList);
        await player.InvokeViewPlugInAsync<IShowWarpListPlugIn>(p => p.ShowWarpListAsync(warps)).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the warps which a player can select, ordered by their index.
    /// </summary>
    /// <param name="warpList">The configured warp list.</param>
    /// <returns>
    /// The warps which lead to a map and which players can use (<see cref="WarpInfo.IsWarpable"/>).
    /// Of warps with the same index, only the first one is included, because it's the one which a warp
    /// request with that index uses.
    /// </returns>
    internal static IReadOnlyCollection<WarpInfo> GetSelectableWarps(IEnumerable<WarpInfo> warpList)
    {
        return warpList
            .DistinctBy(warp => warp.Index)
            .Where(warp => warp.IsWarpable && warp.Gate?.Map is not null)
            .OrderBy(warp => warp.Index)
            .ToList();
    }
}
