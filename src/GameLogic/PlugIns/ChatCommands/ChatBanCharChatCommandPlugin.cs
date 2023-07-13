// <copyright file="ChatBanCharChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles chatban command.
/// </summary>
[Guid("287AE9A6-E434-4E52-A791-8AAD267A8E05")]
[PlugIn("Chat Ban Character command", "Handles the chat command '/chatban <characterName> <durationMinutes>'. Bans the account of a character from chatting for the specified minutes.")]
[ChatCommandHelp(Command, "Bans the account of a character from chatting for the specified minutes.", typeof(ChatBanCharChatCommandArgs), CharacterStatus.GameMaster)]
public class ChatBanCharChatCommandPlugIn : ChatCommandPlugInBase<ChatBanCharChatCommandArgs>
{
    private const string Command = "/chatban";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, ChatBanCharChatCommandArgs arguments)
    {
        if (string.IsNullOrEmpty(arguments.CharacterName))
        {
            throw new ArgumentException("Character name is required.");
        }

        if (arguments.DurationMinutes == 0)
        {
            throw new ArgumentException("Duration must be longer than 0 minutes.");
        }

        var player = gameMaster.GameContext.GetPlayerByCharacterName(arguments.CharacterName);
        if (player == null)
        {
            throw new ArgumentException($"character not found.");
        }

        await this.ChangeAccountChatBanUntilAsync(player, DateTime.UtcNow.AddMinutes(arguments.DurationMinutes));

        // Send ban notice to Game Master
        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account from {arguments.CharacterName} chat banned for {arguments.DurationMinutes} minutes").ConfigureAwait(false);

        // Send ban notice to character
        await this.ShowMessageToAsync(player, $"You are chat banned for {arguments.DurationMinutes} minutes").ConfigureAwait(false);
    }
}