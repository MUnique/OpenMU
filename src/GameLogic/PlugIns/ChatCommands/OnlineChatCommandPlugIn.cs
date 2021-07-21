// <copyright file="OnlineChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles online commands.
    /// </summary>
    [Guid("6693ABA3-7B35-4800-815B-096F3420E998")]
    [PlugIn("Online chat command", "Handles the chat command '/online'. Gets the count of game masters and players online.")]
    [ChatCommandHelp(Command, typeof(EmptyChatCommandArgs), CharacterStatus.GameMaster)]
    public class OnlineChatCommandPlugIn : ChatCommandPlugInBase<EmptyChatCommandArgs>, IChatCommandPlugIn
    {
        private const string Command = "/online";

        /// <inheritdoc/>
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc/>
        protected override void DoHandleCommand(Player gameMasterPlayer, EmptyChatCommandArgs arguments)
        {
            var charactersOnline = gameMasterPlayer.GameContext.PlayerList
                .Where(x => x.SelectedCharacter != null)
                .Select(c => c.SelectedCharacter)
                .ToList();

            var totalCharactersOnline = charactersOnline.Where(x => x!.CharacterStatus == CharacterStatus.Normal).Count();
            var totalGMOnline = charactersOnline.Where(x => x!.CharacterStatus == CharacterStatus.GameMaster).Count();
            this.ShowMessageTo(gameMasterPlayer, $"[{this.Key}] {totalGMOnline} GM(s) and {totalCharactersOnline} player(s) online");
        }
    }
}
