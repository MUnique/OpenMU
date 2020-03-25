namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Help command
    /// </summary>
    [Guid("a5b0a3e5-bb2a-4287-821a-cd97714fe209")]
    [PlugIn("List command", "List all the commands.")]
    public class ListCommand : IChatCommandPlugIn
    {
        /// <inheritdoc />
        public string Key => "/list";

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public string Usage { get; } = "/list";

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var commands = player.GameContext.PlugInManager.GetKnownPlugInsOf<IChatCommandPlugIn>().Select(x => (IChatCommandPlugIn)Activator.CreateInstance(x));

            var stringBuilder = new StringBuilder();
            foreach (var commandUsage in commands.Select(x => x.Usage))
            {
                stringBuilder.AppendLine(commandUsage);
            }


            player.ShowMessage(stringBuilder.ToString());
        }
    }
}