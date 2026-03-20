// <copyright file="TrackChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles track commands.
/// </summary>
[Guid("7F12326A-9B84-4A56-A013-8C485D7B2EF6")]
[PlugIn]
[Display(Name = nameof(PlugInResources.TrackChatCommandPlugIn_Name), Description = nameof(PlugInResources.TrackChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(TraceChatCommandArgs), CharacterStatus.GameMaster)]
public class TrackChatCommandPlugIn : ChatCommandPlugInBase<TraceChatCommandArgs>
{
    private const string Command = "/track";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, TraceChatCommandArgs arguments)
    {
        var player = await this.GetPlayerByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);

        if (gameMaster.SelectedCharacter is null || player is null)
        {
            return;
        }

        var gameMasterLocation = new ExitGate
        {
            Map = gameMaster.SelectedCharacter.CurrentMap,
            X1 = gameMaster.SelectedCharacter.PositionX,
            X2 = (byte)(gameMaster.SelectedCharacter.PositionX + 2),
            Y1 = gameMaster.SelectedCharacter.PositionY,
            Y2 = (byte)(gameMaster.SelectedCharacter.PositionY + 2),
        };

        await player.WarpToAsync(gameMasterLocation).ConfigureAwait(false);
    }
}