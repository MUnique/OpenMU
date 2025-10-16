// <copyright file="HelpCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The help command which shows the usage of a command.
/// </summary>
[Guid("EFE9399A-9A14-4B94-BBC1-20718584C4C2")]
[PlugIn("Help command", "Maneja el comando /help <comando>. Muestra información del comando solicitado.")]
[ChatCommandHelp(Command, "Muestra información del comando solicitado.", typeof(Arguments))]
public class HelpCommand : IChatCommandPlugIn
{
    private const string Command = "/help";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            var arguments = command.ParseArguments<Arguments>();
            var commandName = arguments.CommandName;
            var commandPluginAttribute = player.GetAvailableChatCommands()
                .FirstOrDefault(x => x.Command.Equals("/" + commandName, StringComparison.InvariantCultureIgnoreCase));
            if (commandPluginAttribute is null)
            {
                var message = player.GetLocalizedMessage("Chat_Help_CommandNotFound", "Command '{0}' does not exist.", commandName);
                await player.ShowMessageAsync(message).ConfigureAwait(false);
                return;
            }

            await player.ShowMessageAsync(commandPluginAttribute.Usage).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            await player.ShowMessageAsync(e.Message).ConfigureAwait(false);
        }
    }

    private class Arguments
    {
        public string? CommandName { get; set; }
    }
}
