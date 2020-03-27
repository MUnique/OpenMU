// <copyright file="ListCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Help command
    /// </summary>
    [Guid("a5b0a3e5-bb2a-4287-821a-cd97714fe209")]
    [PlugIn("List command", "Lists all the commands.")]
    [ChatCommandHelp(Command, null)]
    public class ListCommand : IChatCommandPlugIn
    {
        private const string Command = "/list";

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var commands = player.GetAvailableChatCommands();

            var stringBuilder = new StringBuilder();
            foreach (var commandUsage in commands.Select(x => x.Usage))
            {
                stringBuilder.AppendLine(commandUsage);
            }

            player.ShowMessage(stringBuilder.ToString());
        }
    }
}