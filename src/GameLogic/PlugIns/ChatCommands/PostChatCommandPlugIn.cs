// <copyright file="PostChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles move commands.
    /// </summary>
    [Guid("ED2523C1-F66D-4B53-814E-D2FC0C1F46C0")]
    [PlugIn("Post chat command", "Handles the chat command '/post message'. Sends a global message to all players of the game.")]
    public class PostChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string CommandKey = "/post";

        /// <inheritdoc />
        public string Key => CommandKey;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var regex = new Regex(Regex.Escape(CommandKey));
            var message = regex.Replace(command, string.Empty, 1)?.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            player.GameContext.SendGlobalMessage(message, Interfaces.MessageType.BlueNormal);
        }
    }
}
