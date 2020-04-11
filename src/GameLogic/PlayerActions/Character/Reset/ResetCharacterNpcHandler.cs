// <copyright file="ResetCharacterNpcHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character.Reset
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    [Guid("08953BE6-DABF-49CC-A500-FDB9DC2C4D80")]
    [PlugIn(nameof(ResetCharacterNpcHandler), "Handle Reset Character NPC Request")]
    public class ResetCharacterNpcHandler : ICustomNpcTalkHandlerPlugin
    {
        /// <summary>
        /// Gets Reset Npc Number.
        /// </summary>
        public string Key => "371";

        /// <inheritdoc />
        public void HandleNpcTalk(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
        {
            eventArgs.HasBeenHandled = true;
            var resetAction = new ResetCharacterAction(player);
            try
            {
                resetAction.ResetCharacter();
            }
            catch (ResetCharacterActionException e)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(e.Message, MessageType.BlueNormal);
            }
        }
    }
}
