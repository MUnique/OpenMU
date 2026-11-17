// <copyright file="TraceChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles trace commands.
/// </summary>
[Guid("F22C989B-A2A1-4991-B6C2-658337CC19CE")]
[PlugIn]
[Display(Name = nameof(PlugInResources.TraceChatCommandPlugIn_Name), Description = nameof(PlugInResources.TraceChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(TraceChatCommandArgs), CharacterStatus.GameMaster)]
public class TraceChatCommandPlugIn : ChatCommandPlugInBase<TraceChatCommandArgs>
{
    private const string Command = "/trace";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player player, TraceChatCommandArgs arguments)
    {
        var targetPlayer = await this.GetPlayerByCharacterNameAsync(player, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);
        var character = targetPlayer?.SelectedCharacter;

        if (character is null)
        {
            return;
        }

        var characterLocation = new ExitGate
        {
            Map = character.CurrentMap,
            X1 = character.PositionX,
            X2 = (byte)(character.PositionX + 2),
            Y1 = character.PositionY,
            Y2 = (byte)(character.PositionY + 2),
        };

        await player.WarpToAsync(characterLocation).ConfigureAwait(false);
    }
}