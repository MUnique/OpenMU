// <copyright file="PKClearChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles pk clear commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("EB97A8F6-F6BD-460A-BCBE-253BF679361A")]
    [PlugIn("PK clear chat command", "Handles the chat command '/pkclear <char>'. Clears a character murders.")]
    [ChatCommandHelp(Command, typeof(PKClearChatCommandArgs), CharacterStatus.GameMaster)]
    public class PKClearChatCommandPlugIn : ChatCommandPlugInBase<PKClearChatCommandArgs>
    {
        private const string Command = "/pkclear";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, PKClearChatCommandArgs arguments)
        {
            var targetPlayer = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);

            targetPlayer.SelectedCharacter!.State = HeroState.Normal;
            targetPlayer.SelectedCharacter!.StateRemainingSeconds = 0;
            targetPlayer.SelectedCharacter!.PlayerKillCount = 0;
            targetPlayer.ForEachWorldObserver(o => o.ViewPlugIns.GetPlugIn<IUpdateCharacterHeroStatePlugIn>()?.UpdateCharacterHeroState(targetPlayer), true);

            if (!targetPlayer.Name.Equals(gameMaster.Name))
            {
                this.ShowMessageTo(targetPlayer, $"Your murders have been cleaned by the game master.");
            }

            this.ShowMessageTo(gameMaster, $"[{this.Key}] {targetPlayer.Name} murders have been cleaned.");
        }
    }
}
