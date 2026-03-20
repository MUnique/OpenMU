// <copyright file="AddAgilityStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add agility stat points.
/// </summary>
[Guid("43156A52-03EE-42C0-88BF-CA9665DC8E1E")]
[PlugIn]
[Display(Name = nameof(PlugInResources.AddAgilityStatChatCommandPlugIn_Name), Description = nameof(PlugInResources.AddAgilityStatChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, null, MinimumStatus)]
public class AddAgilityStatChatCommandPlugIn : AddStatChatCommandPlugIn, IDisabledByDefault
{
    private const string Command = "/addagi";

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