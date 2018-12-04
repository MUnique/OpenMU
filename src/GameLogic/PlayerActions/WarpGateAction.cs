// <copyright file="WarpGateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Action to warp to another place through a gate.
    /// </summary>
    public class WarpGateAction
    {
        /// <summary>
        /// Enters the gate.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="enterGate">The enter gate.</param>
        public void EnterGate(Player player, EnterGate enterGate)
        {
            if (this.IsWarpLegit(player, enterGate))
            {
                player.WarpTo(enterGate.TargetGate);
            }
            else
            {
                // TODO: We need to send the correct response.
                // Currently, we just send the regular map change message, but this causes that the
                // player doesn't see objects in its range anymore.
                player.PlayerView.WorldView.MapChange();
            }
        }

        private bool IsWarpLegit(Player player, EnterGate enterGate)
        {
            if (enterGate == null)
            {
                return false;
            }

            if (player.SelectedCharacter == null)
            {
                return false;
            }

            if (enterGate.LevelRequirement > player.Attributes[Stats.Level])
            {
                player.PlayerView.ShowMessage("Your level is too low to enter this map.", Interfaces.MessageType.BlueNormal);
                return false;
            }

            var currentPosition = player.IsWalking ? player.WalkTarget : player.Position;
            var inaccuracy = player.GameContext.Configuration.InfoRange;
            if (player.CurrentMap.Definition.EnterGates.Contains(enterGate)
                && !(currentPosition.X >= enterGate.X1 - inaccuracy
                && currentPosition.X <= enterGate.X2 + inaccuracy
                && currentPosition.Y >= enterGate.Y1 - inaccuracy
                && currentPosition.Y <= enterGate.Y2 + inaccuracy))
            {
                return false;
            }

            return true;
        }
    }
}
