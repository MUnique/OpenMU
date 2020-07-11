// <copyright file="HelpCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The help command which shows the usage of a command.
    /// </summary>
    [Guid("EFE9399A-9A14-4B94-BBC1-20718584C4C2")]
    [PlugIn("Help command", "List all the commands.")]
    [ChatCommandHelp(Command, typeof(Arguments))]
    public class HelpCommand : IChatCommandPlugIn
    {
        private const string Command = "/help";

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                var arguments = command.ParseArguments<Arguments>();
                var commandName = arguments.CommandName;
                var commandPluginAttribute = player.GetAvailableChatCommands()
                    .FirstOrDefault(x => x.Command.Equals("/" + commandName, StringComparison.InvariantCultureIgnoreCase));
                if (commandPluginAttribute == null)
                {
                    player.ShowMessage($"The command '{commandName}' does not exists.");
                    return;
                }

                player.ShowMessage(commandPluginAttribute.Usage);
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

        private class Arguments
        {
            public string CommandName { get; set; }
        }
    }
}