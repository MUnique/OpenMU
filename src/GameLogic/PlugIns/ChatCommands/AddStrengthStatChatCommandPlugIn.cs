// <copyright file="AddStrengthStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add strength stat points.
/// </summary>
[Guid("21B15D95-BA2F-40A3-AB7D-8BD886FAEAE5")]
[PlugIn("Add strength chat command", "Adds the specified amount of strength points to the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of strength points to the character.", null, MinimumStatus)]
public class AddStrengthStatChatCommandPlugIn : AddStatChatCommandPlugIn, IDisabledByDefault
{
    private const string Command = "/addstr";

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