namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Help command
    /// </summary>
    [Guid("a5b0a3e5-bb2a-4287-821a-cd97714fe209")]
    [PlugIn("Help command", "List all the commands.")]
    public class HelpCommand : IChatCommandPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        public HelpCommand()
        {
            this.Usage = CommandExtensions.CreateUsage<Arguments>(this.Key);
        }

        /// <inheritdoc />
        public string Key => "/help";

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public string Usage { get; }

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                var arguments = command.ParseArguments<Arguments>();
                var commandName = arguments.CommandName;
                var commands = player.GameContext.PlugInManager.GetKnownPlugInsOf<IChatCommandPlugIn>().Select(x => (IChatCommandPlugIn)Activator.CreateInstance(x));
                var commandPlugin = commands.FirstOrDefault(x => x.Key.Contains(commandName));
                if (commandPlugin == null)
                {
                    player.ShowMessage($"The {commandName} does not exists.");
                    return;
                }

                player.ShowMessage(commandPlugin.Usage);
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