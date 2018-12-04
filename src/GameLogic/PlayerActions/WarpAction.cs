// <copyright file="WarpAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

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
        public void WarpTo(Player player, WarpInfo warpInfo)
        {
            if (warpInfo.LevelRequirement <= player.Attributes[Stats.Level]
                && player.TryRemoveMoney(warpInfo.Costs))
            {
                player.WarpTo(warpInfo.Gate);
            }
            else
            {
                // no further action required.
            }
        }
    }
}
