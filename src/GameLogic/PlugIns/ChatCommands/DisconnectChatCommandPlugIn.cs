// <copyright file="DisconnectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles disconnect commands.
/// </summary>
[Guid("B5E0F108-9E55-48F6-A7A8-220BFAEF2F3E")]
[PlugIn("Disconnect chat command", "Handles the chat command '/disconnect <char>'. Disconnects a player from the game server.")]
[ChatCommandHelp(Command, "Disconnects a player from the game server.", typeof(DisconnectChatCommandArgs), CharacterStatus.GameMaster)]
public class DisconnectChatCommandPlugIn : ChatCommandPlugInBase<DisconnectChatCommandArgs>
{
    private const string Command = "/disconnect";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, DisconnectChatCommandArgs arguments)
    {
        var player = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);
        await player.DisconnectAsync().ConfigureAwait(false);

        if (!player.Name.Equals(gameMaster.Name))
        {
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] {player.Name} has been disconnected.").ConfigureAwait(false);
        }
    }
}