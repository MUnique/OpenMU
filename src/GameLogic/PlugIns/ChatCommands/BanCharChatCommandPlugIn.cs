// <copyright file="BanCharChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles banchar commands.
/// </summary>
[Guid("7AD1E5F4-4B07-4165-B9A4-188614F00F7C")]
[PlugIn("Ban Character command", "Handles the chat command '/banchar <char>'. Bans the account of a character from the game.")]
[ChatCommandHelp(Command, "Bans the account of a character from the game.", typeof(BanCharChatCommandArgs), CharacterStatus.GameMaster)]
public class BanCharChatCommandPlugIn : ChatCommandPlugInBase<BanCharChatCommandArgs>
{
    private const string Command = "/banchar";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, BanCharChatCommandArgs arguments)
    {
        await this.ChangeAccountStateByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty, AccountState.Banned).ConfigureAwait(false);

        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account from {arguments.CharacterName} has been banned.").ConfigureAwait(false);
    }
}