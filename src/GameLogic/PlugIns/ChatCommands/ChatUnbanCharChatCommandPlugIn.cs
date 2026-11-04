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
[PlugIn]
[Display(Name = nameof(PlugInResources.ChatUnbanCharChatCommandPlugIn_Name), Description = nameof(PlugInResources.ChatUnbanCharChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(ChatUnbanCharChatCommandArgs), CharacterStatus.GameMaster)]
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
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNameIsRequired)).ConfigureAwait(false);
            return;
        }

        var player = gameMaster.GameContext.GetPlayerByCharacterName(arguments.CharacterName);
        if (player == null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), arguments.CharacterName).ConfigureAwait(false);
            return;
        }

        if (!await this.ChangeAccountChatBanUntilAsync(player, null).ConfigureAwait(false))
        {
            return;
        }

        // Send unban notice to Game Master
        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ChatBanRemoved), this.Key, arguments.CharacterName).ConfigureAwait(false);

        // Send unban notice to character
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.YourChatBanRemovedByGameMaster)).ConfigureAwait(false);
    }
}