// <copyright file="ResetChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character.Reset;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles reset command.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("ABFE2440-E765-4F17-A588-BD9AE3799886")]
    [PlugIn("Reset chat command", "Handles the chat command '/reset'.")]
    [ChatCommandHelp(Command, null)]
    public class ResetChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string Command = "/reset";

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var resetAction = new ResetCharacterAction(player);
            try
            {
                resetAction.ResetCharacter();
            }
            catch (ResetCharacterException e)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(e.Message, MessageType.BlueNormal);
            }
        }
    }
}
