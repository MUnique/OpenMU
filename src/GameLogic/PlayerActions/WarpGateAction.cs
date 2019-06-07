// <copyright file="WarpGateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Action to warp to another place through a gate.
    /// </summary>
    public class WarpGateAction
    {
        /// <summary>
        /// Enters the gate.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="gate">The enter gate.</param>
        public void EnterGate(Player player, EnterGate gate)
        {
            if (this.IsWarpLegit(player, gate))
            {
                player.WarpTo(gate.TargetGate);
            }
            else
            {
                // TODO: We need to send the correct response.
                // Currently, we just send the regular map change message, but this causes that the
                // player doesn't see objects in its range anymore.
                player.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()?.MapChange();
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
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Your level is too low to enter this map.", Interfaces.MessageType.BlueNormal);
                return false;
            }

            if (enterGate.TargetGate.Map.TryGetRequirementError(player, out var errorMessage))
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(errorMessage, Interfaces.MessageType.BlueNormal);
                return false;
            }

            var currentPosition = player.IsWalking ? player.WalkTarget : player.Position;
            var inaccuracy = player.GameContext.Configuration.InfoRange;
            if (player.CurrentMap.Definition.EnterGates.Contains(enterGate)
                && !(this.IsXInRange(currentPosition, enterGate, inaccuracy)
                && this.IsYInRange(currentPosition, enterGate, inaccuracy)))
            {
                return false;
            }

            return true;
        }

        private bool IsXInRange(Point currentPosition, Gate gate, byte inaccuracy) => currentPosition.X >= gate.X1 - inaccuracy
                                                                                      && currentPosition.X <= gate.X2 + inaccuracy;

        private bool IsYInRange(Point currentPosition, Gate gate, byte inaccuracy) => currentPosition.Y >= gate.Y1 - inaccuracy
                                                                                      && currentPosition.Y <= gate.Y2 + inaccuracy;
    }
}
