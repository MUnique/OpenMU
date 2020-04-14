// <copyright file="ResetChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles reset command.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("90B35404-AADE-4F22-B5D2-4CD59B8BB4C8")]
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
            resetAction.ResetCharacter();
        }
    }
}
