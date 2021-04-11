// <copyright file="PKChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles pk commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("30B7EFF0-33EE-4136-BEB0-BE503B748DC6")]
    [PlugIn("PK chat command", "Handles the chat command '/pk <characterName>'. Increments the character murders.")]
    [ChatCommandHelp(Command, typeof(PKChatCommandArgs), CharacterStatus.GameMaster)]
    public class PKChatCommandPlugIn : ChatCommandPlugInBase<PKChatCommandArgs>
    {
        private const string Command = "/pk";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, PKChatCommandArgs arguments)
        {
            var targetPlayer = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);
            targetPlayer.AfterKilledPlayer();

            var message = string.Format(
                "The state of {0} has been changed to {1} with {2} murders for {3} minutes",
                targetPlayer.Name,
                targetPlayer.SelectedCharacter!.State,
                targetPlayer.SelectedCharacter!.PlayerKillCount,
                Math.Round(TimeSpan.FromSeconds(targetPlayer.SelectedCharacter!.StateRemainingSeconds).TotalMinutes));

            this.ShowMessageTo(gameMaster, $"[{this.Key}] {message}");
        }
    }
}
