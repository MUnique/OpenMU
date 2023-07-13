// <copyright file="ChatUnbanCharChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles chatunban command.
/// </summary>
[Guid("82E74664-7700-433B-9428-90C17CC71350")]
[PlugIn("Chat Ban Character command", "Handles the chat command '/chatunban <characterName>'. Unbans the account of a character from chatting.")]
[ChatCommandHelp(Command, "Unbans the account of a character from chatting", typeof(ChatUnbanCharChatCommandArgs), CharacterStatus.GameMaster)]
public class ChatUnbanCharChatCommandPlugIn : ChatCommandPlugInBase<ChatUnbanCharChatCommandArgs>
{
    private const string Command = "/chatunban";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, ChatUnbanCharChatCommandArgs arguments)
    {
        if (string.IsNullOrEmpty(arguments.CharacterName))
        {
            throw new ArgumentException("Character name is required.");
        }

        var player = gameMaster.GameContext.GetPlayerByCharacterName(arguments.CharacterName);
        if (player == null)
        {
            throw new ArgumentException($"character not found.");
        }

        await this.ChangeAccountChatBanUntilAsync(player, null);

        // Send unban notice to Game Master
        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] The chat ban for the account from {arguments.CharacterName} has been removed.").ConfigureAwait(false);

        // Send unban notice to character
        await this.ShowMessageToAsync(player, $"Your chat ban has been removed by a gamemaster.").ConfigureAwait(false);
    }
}