// <copyright file="GMoveChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.GameMaster
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles gm move commands.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("9163C3EA-6722-4E55-A109-20C163C05266")]
    [PlugIn("GMove chat command", "Handles the chat command '/gmove <guildName> <map> <x?> <y?>'. Move the character from a guild to a specified map and coordinates.")]
    [ChatCommandHelp(Command, typeof(GMoveChatCommandArgs), CharacterStatus.GameMaster)]
    public class GMoveChatCommandPlugIn : ChatCommandPlugInBase<GMoveChatCommandArgs>
    {
        private const string Command = "/gmove";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, GMoveChatCommandArgs arguments)
        {
            var guildId = this.GetGuildIdByName(gameMaster, arguments.GuildName!);

            var guildPlayers = gameMaster.GameContext.PlayerList
                .Where(p => p.GuildStatus?.GuildId == guildId)
                .ToList();

            var exitGate = this.GetExitGate(gameMaster, arguments.Map!, arguments.Coordinates);

            foreach (var targetPlayer in guildPlayers)
            {
                targetPlayer.WarpTo(exitGate);

                if (!targetPlayer.Name.Equals(gameMaster.Name))
                {
                    this.ShowMessageTo(targetPlayer, "You have been moved by the game master.");
                    this.ShowMessageTo(gameMaster, $"[{this.Key}] {targetPlayer.Name} has been moved to {exitGate!.Map!.Name} at {targetPlayer.Position.X}, {targetPlayer.Position.Y}");
                }
            }
        }
    }
}
