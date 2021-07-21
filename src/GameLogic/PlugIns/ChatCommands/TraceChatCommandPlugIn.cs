// <copyright file="TraceChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles trace commands.
    /// </summary>
    [Guid("F22C989B-A2A1-4991-B6C2-658337CC19CE")]
    [PlugIn("Trace chat command", "Handles the chat command '/trace <char>'. Moves the game master to the character's location.")]
    [ChatCommandHelp(Command, typeof(TraceChatCommandArgs), CharacterStatus.GameMaster)]
    public class TraceChatCommandPlugIn : ChatCommandPlugInBase<TraceChatCommandArgs>
    {
        private const string Command = "/trace";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc/>
        protected override void DoHandleCommand(Player gameMaster, TraceChatCommandArgs arguments)
        {
            var player = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);
            var character = player.SelectedCharacter;

            if (character != null)
            {
                var characterLocation = new ExitGate
                {
                    Map = character.CurrentMap,
                    X1 = character.PositionX,
                    X2 = (byte)(character.PositionX + 2),
                    Y1 = character.PositionY,
                    Y2 = (byte)(character.PositionY + 2),
                };

                gameMaster.WarpTo(characterLocation);
            }
        }
    }
}
