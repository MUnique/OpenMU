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
[PlugIn("Track chat command", "Handles the chat command '/track <char>'. Moves the player to the game masters location.")]
[ChatCommandHelp(Command, "Moves the player to the game masters location.", typeof(TraceChatCommandArgs), CharacterStatus.GameMaster)]
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
        var player = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);

        if (gameMaster.SelectedCharacter != null)
        {
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
}