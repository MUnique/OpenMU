// <copyright file="TeleportChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportChatCommandPlugIn"/> class.
        /// </summary>
        public TeleportChatCommandPlugIn()
        {
            this.Usage = CommandExtensions.CreateUsage<Arguments>(this.Key);
        }

        /// <inheritdoc />
        public string Key => "/teleport";

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc/>
        public string Usage { get; }

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                var arguments = command.ParseArguments<Arguments>();
                player.Move(arguments.Point);
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

        /// <summary>
        /// Arguments
        /// </summary>
        private class Arguments : ArgumentsBase
        {
            [CommandsAttributes.Argument("x")]
            public byte X { get; set; }

            [CommandsAttributes.Argument("y")]
            public byte Y { get; set; }

            public Point Point => new Point(this.X, this.Y);
        }
    }
}
