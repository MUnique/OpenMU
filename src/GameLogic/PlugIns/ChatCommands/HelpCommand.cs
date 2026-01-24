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
[PlugIn]
[Display(Name = nameof(PlugInResources.HelpCommand_Name), Description = nameof(PlugInResources.HelpCommand_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Shows information about the requested command.", typeof(Arguments))]
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
            if (await command.TryParseArgumentsAsync<Arguments>(player).ConfigureAwait(false) is not { } arguments)
            {
                return;
            }

            var commandName = arguments.CommandName;
            var commandPluginAttribute = player.GetAvailableChatCommands()
                .FirstOrDefault(x => x.Command.Equals("/" + commandName, StringComparison.InvariantCultureIgnoreCase));
            if (commandPluginAttribute is null)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandDoesNotExist), commandName ?? string.Empty).ConfigureAwait(false);
                return;
            }

            await player.ShowBlueMessageAsync(commandPluginAttribute.Usage).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            // Should not happen, as we don't throw them anymore. But just in case...
            await player.ShowBlueMessageAsync(e.Message).ConfigureAwait(false);
        }
    }

    private class Arguments
    {
        public string? CommandName { get; set; }
    }
}