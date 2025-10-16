﻿// <copyright file="MoveChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles move commands.
/// </summary>
[Guid("4564AE2B-4819-4155-B5B2-FE2ED0CF7A7F")]
[PlugIn("Move chat command", "Handles the chat command '/move <target> <mapIdOrName?> <x?> <y?>'. Moves the character to the specified destination.")]
[ChatCommandHelp(Command, "Moves the character to the specified destination.", typeof(MoveChatCommandArgs), CharacterStatus.Normal)]
public class MoveChatCommandPlugIn : ChatCommandPlugInBase<MoveChatCommandArgs>
{
    private const string Command = "/move";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player sender, MoveChatCommandArgs arguments)
    {
        var senderIsGameMaster = sender.SelectedCharacter?.CharacterStatus == CharacterStatus.GameMaster;
        var isGameMasterWarpingCharacter = senderIsGameMaster && !string.IsNullOrWhiteSpace(arguments.MapIdOrName);

        if (isGameMasterWarpingCharacter)
        {
            var targetPlayer = this.GetPlayerByCharacterName(sender, arguments.Target!);
            var exitGate = await this.GetExitGateAsync(sender, arguments.MapIdOrName!, arguments.Coordinates).ConfigureAwait(false);
            await targetPlayer.WarpToAsync(exitGate).ConfigureAwait(false);

            if (!targetPlayer.Name.Equals(sender.Name))
            {
                var targetMessage = targetPlayer.GetLocalizedMessage("Chat_Move_PlayerMoved", "You have been moved by the game master.");
                await this.ShowMessageToAsync(targetPlayer, targetMessage).ConfigureAwait(false);
                var senderMessage = sender.GetLocalizedMessage(
                    "Chat_Move_TargetMoved",
                    "[{0}] {1} has been moved to {2} at {3}, {4}",
                    this.Key,
                    targetPlayer.Name,
                    exitGate!.Map!.Name,
                    targetPlayer.Position.X,
                    targetPlayer.Position.Y);
                await this.ShowMessageToAsync(sender, senderMessage).ConfigureAwait(false);
            }
        }
        else
        {
            var warpInfo = this.GetWarpInfo(sender, arguments.Target!);
            if (warpInfo != null)
            {
                await new WarpAction().WarpToAsync(sender, warpInfo).ConfigureAwait(false);
            }
        }
    }
}