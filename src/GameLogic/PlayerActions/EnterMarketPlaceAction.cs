// <copyright file="EnterMarketPlaceAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Action which warps a player between Lorencia and the Loren Market, triggered by the 'Warp'
/// button of the Market Union Member Julia window. It warps both ways, depending on which side
/// the player is currently on.
/// </summary>
public class EnterMarketPlaceAction
{
    /// <summary>
    /// The map number of the Loren Market.
    /// </summary>
    private const short LorenMarketMapNumber = 79;

    /// <summary>
    /// The map number of Lorencia, where the player is warped back to.
    /// </summary>
    private const short LorenciaMapNumber = 0;

    /// <summary>
    /// Warps the player between Lorencia and the Loren Market through Market Union Member Julia.
    /// </summary>
    /// <param name="player">The player who requested the warp.</param>
    public async ValueTask WarpToMarketPlaceAsync(Player player)
    {
        // Only allow the warp through the actual Julia window, so a crafted packet can't be used
        // as a free teleport from anywhere.
        if (player.OpenedNpc?.Definition.NpcWindow != NpcWindow.JuliaWarpMarketServer)
        {
            return;
        }

        // Julia warps both ways: from the Loren Market back to Lorencia, and from anywhere else
        // (her counterpart in Lorencia) into the Loren Market.
        var targetMapNumber = player.CurrentMap?.Definition.Number == LorenMarketMapNumber
            ? LorenciaMapNumber
            : LorenMarketMapNumber;

        var targetMap = await player.GameContext.GetMapAsync((ushort)targetMapNumber).ConfigureAwait(false);
        if (targetMap?.SafeZoneSpawnGate is not { } targetGate)
        {
            return;
        }

        player.OpenedNpc = null;
        await player.WarpToAsync(targetGate).ConfigureAwait(false);
    }
}
