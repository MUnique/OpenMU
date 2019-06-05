// <copyright file="WarpAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
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
        public void WarpTo(Player player, WarpInfo warpInfo)
        {
            if (this.CheckRequirements(player, warpInfo, out string errorMessage))
            {
                player.WarpTo(warpInfo.Gate);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(errorMessage, MessageType.BlueNormal);
            }
        }

        private bool CheckRequirements(Player player, WarpInfo warpInfo, out string errorMessage)
        {
            errorMessage = null;
            if (!this.CheckLevelRequirement(player, warpInfo))
            {
                errorMessage = $"You need to be level {warpInfo.LevelRequirement} in order to warp";
                return false;
            }

            if (!this.CheckWarpRequirements(player, warpInfo, out string message))
            {
                errorMessage = message;
                return false;
            }

            // Money check should be last to avoid geting zen when other checks failed
            if (!this.CheckMoneyRequirement(player, warpInfo))
            {
                errorMessage = $"You need {warpInfo.Costs} in order to warp";
                return false;
            }

            return true;
        }

        private bool CheckMoneyRequirement(Player player, WarpInfo warpInfo)
        {
            if (player.TryRemoveMoney(warpInfo.Costs))
            {
                return true;
            }

            return false;
        }

        private bool CheckLevelRequirement(Player player, WarpInfo warpInfo)
        {
            if (warpInfo.LevelRequirement <= player.Attributes[Stats.Level])
            {
                return true;
            }

            return false;
        }

        private bool CheckWarpRequirements(Player player, WarpInfo warpInfo, out string errorMessage)
        {
            errorMessage = null;

            if (warpInfo.Gate.Map.MapRequirements != null && warpInfo.Gate.Map.MapRequirements.Any())
            {
                foreach (var requirement in warpInfo.Gate.Map.MapRequirements)
                {
                    var floatDiff = player.Attributes[requirement.Attribute] - requirement.MinimumValue;
                    if (Math.Abs(floatDiff) > 0.01)
                    {
                        errorMessage = $"{requirement.Attribute.Designation} {requirement.Attribute.Description}";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
