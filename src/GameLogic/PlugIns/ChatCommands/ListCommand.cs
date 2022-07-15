// <copyright file="ListCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A command which lists all available chat commands with their usage.
/// </summary>
[Guid("a5b0a3e5-bb2a-4287-821a-cd97714fe209")]
[PlugIn("List command", "Lists all the commands.")]
[ChatCommandHelp(Command, "Lists all the commands.", null)]
public class ListCommand : IChatCommandPlugIn
{
    private const string Command = "/list";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var commands = player.GetAvailableChatCommands();

        foreach (var commandUsage in commands.Select(x => x.Usage))
        {
            await player.ShowMessageAsync(commandUsage).ConfigureAwait(false);
        }
    }
}