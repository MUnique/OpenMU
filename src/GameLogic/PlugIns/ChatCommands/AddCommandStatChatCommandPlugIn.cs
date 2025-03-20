// <copyright file="AddCommandStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add command stat points.
/// </summary>
[Guid("EFE421FB-BE79-4656-AF39-D22A105D1455")]
[PlugIn("Add command chat command", "Adds the specified amount of command points to the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of command points to the character.", null, MinimumStatus)]
public class AddCommandStatChatCommandPlugIn : AddStatChatCommandPlugIn, IDisabledByDefault
{
    private const string Command = "/addcmd";

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