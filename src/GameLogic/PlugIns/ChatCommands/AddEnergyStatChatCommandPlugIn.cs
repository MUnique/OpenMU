// <copyright file="AddEnergyStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add energy stat points.
/// </summary>
[Guid("A597B6E7-9395-4CF4-8439-A1D60134B63E")]
[PlugIn("Add energy chat command", "Adds the specified amount of energy points to the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of energy points to the character.", null, MinimumStatus)]
public class AddEnergyStatChatCommandPlugIn : AddStatChatCommandPlugIn, IDisabledByDefault
{
    private const string Command = "/addene";

    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public override async ValueTask HandleCommandAsync(Player player, string command)
    {
        command = command.Insert(4, " ");
        await base.HandleCommandAsync(player, command).ConfigureAwait(false);
    }
}