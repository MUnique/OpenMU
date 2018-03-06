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
        private const byte INACCURACY = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpGateAction"/> class.
        /// </summary>
        public WarpGateAction()
        {
        }

        /// <summary>
        /// Enters the gate.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="enterGate">The enter gate.</param>
        public void EnterGate(Player player, EnterGate enterGate)
        {
            if (enterGate == null)
            {
                return;
            }

            if (player.SelectedCharacter == null)
            {
                return;
            }

            if (enterGate.LevelRequirement > player.Attributes[Stats.Level])
            {
                player.PlayerView.ShowMessage("Your level is too low to enter this map.", Interfaces.MessageType.BlueNormal);
                return;
            }

            if (player.CurrentMap.Definition.EnterGates.Contains(enterGate) && !(player.X >= enterGate.X1 - INACCURACY &&
                  player.X <= enterGate.X2 + INACCURACY &&
                  player.Y >= enterGate.Y1 - INACCURACY &&
                  player.Y <= enterGate.Y2 + INACCURACY))
            {
                return;
            }

            player.WarpTo(enterGate.TargetGate);
        }
    }
}
