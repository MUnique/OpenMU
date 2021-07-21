// <copyright file="GuildMoveChatCommandPlugIn.cs" company="MUnique">
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
    /// A chat command plugin which handles gm move commands.
    /// </summary>
    [Guid("9163C3EA-6722-4E55-A109-20C163C05266")]
    [PlugIn("Guild move chat command", "Handles the chat command '/guildmove <guild> <map> <x?> <y?>'. Move the character from a guild to a specified map and coordinates.")]
    [ChatCommandHelp(Command, typeof(GuildMoveChatCommandArgs), CharacterStatus.GameMaster)]
    public class GuildMoveChatCommandPlugIn : ChatCommandPlugInBase<GuildMoveChatCommandArgs>
    {
        private const string Command = "/guildmove";

        /// <inheritdoc />
        public override string Key => Command;

        /// <inheritdoc/>
        public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

        /// <inheritdoc />
        protected override void DoHandleCommand(Player gameMaster, GuildMoveChatCommandArgs arguments)
        {
            var guildId = this.GetGuildIdByName(gameMaster, arguments.GuildName!);

            var guildPlayers = gameMaster.GameContext.PlayerList
                .Where(p => p.GuildStatus?.GuildId == guildId)
                .ToList();

            var exitGate = this.GetExitGate(gameMaster, arguments.MapIdOrName!, arguments.Coordinates);

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
