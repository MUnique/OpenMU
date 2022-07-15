// <copyright file="TeleportChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles teleport commands.
/// </summary>
[Guid("ABFE2440-E765-4F17-A588-BD9AE3799886")]
[PlugIn("Teleport chat command", "Handles the chat command '/teleport <x> <y>'. Teleports the game master to the specified coordinates.")]
[ChatCommandHelp(Command, "Teleports the game master to the specified coordinates.", typeof(CoordinatesCommandArgs), CharacterStatus.GameMaster)]
public class TeleportChatCommandPlugIn : ChatCommandPlugInBase<CoordinatesCommandArgs>
{
    private const string Command = "/teleport";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CoordinatesCommandArgs arguments)
    {
        await gameMaster.MoveAsync(arguments.Coordinates).ConfigureAwait(false);
    }
}