// <copyright file="AddVitalityStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add vitality stat points.
/// </summary>
[Guid("370CE86C-E382-4E0F-93F4-AD75FA079129")]
[PlugIn("Add vitality chat command", "Adds the specified amount of vitality points to the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of vitality points to the character.", null, MinimumStatus)]
public class AddVitalityStatChatCommandPlugIn : AddStatChatCommandPlugIn, IDisabledByDefault
{
    private const string Command = "/addvit";

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