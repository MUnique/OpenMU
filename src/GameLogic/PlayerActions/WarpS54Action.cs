// <copyright file="WarpS54Action.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Action to warp to another place.
    /// </summary>
    /// <remarks>
    /// This action is no longer in use since Season 6.
    /// </remarks>
    public class WarpS54Action
    {
        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpS54Action"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public WarpS54Action(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Warps the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="warpInfo">The warp information.</param>
        public void WarpTo(Player player, WarpInfo warpInfo)
        {
            if (warpInfo.LevelRequirement > player.Attributes[Stats.Level])
            {
                return; // todo: check if we need failed packet
            }

            if (player.TryRemoveMoney((int)warpInfo.Costs))
            {
                player.WarpTo(warpInfo.Gate);
            } // todo: else maybe send failed packet?
        }
    }
}
