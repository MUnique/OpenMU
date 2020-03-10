// <copyright file="TeleportChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles teleport commands.
    /// </summary>
    /// <remarks>
    /// This should be deactivated by default or limited to game masters.
    /// </remarks>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("ABFE2440-E765-4F17-A588-BD9AE3799886")]
    [PlugIn("Teleport chat command", "Handles the chat command '/teleport x y'. Teleports the character to the specified coordinates.")]
    public class TeleportChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string CommandKey = "/teleport";

        /// <inheritdoc />
        public string Key => CommandKey;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var arguments = command.Split(' ');
            if (arguments.Length > 2 && byte.TryParse(arguments[1], out var x) && byte.TryParse(arguments[2], out var y))
            {
                player.Move(new Point(x, y));
            }
        }
    }
}
