// <copyright file="ResetCharacterNpcPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    [Guid("08953BE6-DABF-49CC-A500-FDB9DC2C4D80")]
    [PlugIn(nameof(ResetCharacterNpcPlugin), "Handle Reset Character NPC Request")]
    public class ResetCharacterNpcPlugin : IPlayerTalkToNpcPlugIn
    {
        /// <summary>
        /// Gets the reset NPC number of 'Leo the Helper'.
        /// </summary>
        public static short ResetNpcNumber => 371;

        /// <inheritdoc />
        public void PlayerTalksToNpc(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
        {
            if (!ResetCharacterAction.IsEnabled)
            {
                return;
            }

            if (npc.Definition.Number != ResetNpcNumber)
            {
                return;
            }

            eventArgs.HasBeenHandled = true;
            var resetAction = new ResetCharacterAction(player, npc);
            resetAction.ResetCharacter();
        }
    }
}
