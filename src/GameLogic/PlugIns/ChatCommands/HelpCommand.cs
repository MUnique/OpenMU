namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The help command which shows the usage of a command.
    /// </summary>
    [Guid("a5b0a3e5-bb2a-4287-821a-cd97714fe209")]
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
                    player.ShowMessage($"The {commandName} does not exists.");
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