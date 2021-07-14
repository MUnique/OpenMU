// <copyright file="GuildDisconnectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles gm disconnect commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("F23262E6-0D7C-4B9C-8CD5-7E44AF4EE469")]
    [PlugIn("Guild disconnect chat command", "Handles the chat command '/guilddisconnect <guild>'. Disconnect the guild members.")]
    [ChatCommandHelp(Command, typeof(GuildDisconnectChatCommandArgs), CharacterStatus.GameMaster)]
    public class GuildDisconnectChatCommandPlugIn : ChatCommandPlugInBase<GuildDisconnectChatCommandArgs>
    {
        private const string Command = "/guilddisconnect";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, GuildDisconnectChatCommandArgs arguments)
        {
            var guildId = this.GetGuildIdByName(gameMaster, arguments.GuildName!);

            var guildPlayers = gameMaster.GameContext.PlayerList
                .Where(p => p.GuildStatus?.GuildId == guildId)
                .ToList();

            foreach (var targetPlayer in guildPlayers)
            {
                targetPlayer.Disconnect();

                if (!targetPlayer.Name.Equals(gameMaster.Name))
                {
                    this.ShowMessageTo(gameMaster, $"[{this.Key}] {targetPlayer.Name} has been disconnected.");
                }
            }
        }
    }
}
