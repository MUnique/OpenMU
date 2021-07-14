// <copyright file="TeleportChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles teleport commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("ABFE2440-E765-4F17-A588-BD9AE3799886")]
    [PlugIn("Teleport chat command", "Handles the chat command '/teleport <x> <y>'. Teleports the game master to the specified coordinate.")]
    [ChatCommandHelp(Command, typeof(TeleportChatCommandArgs), CharacterStatus.GameMaster)]
    public class TeleportChatCommandPlugIn : ChatCommandPlugInBase<TeleportChatCommandArgs>
    {
        private const string Command = "/teleport";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc/>
        protected override void DoHandleCommand(Player gameMaster, TeleportChatCommandArgs arguments)
        {
            gameMaster.Move(arguments.Coordinates);
        }
    }
}
